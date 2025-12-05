using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;


public class DataManager : MonoBehaviour
{
    // 单例
    public static DataManager Instance;
    public Image startButton;
    WXUserInfoButton wxUserInfoButton;
    [SerializeField] private BindablePlayerInfo _playerInfo=new BindablePlayerInfo();//全局的玩家信息
    private string userID=string.Empty;
    private string userFrom=string.Empty;

    public static event UnityAction<int> OnPassCountChanged;//通关次数变化
    public static event UnityAction OnDataLoaded;//数据加载完毕

    [SerializeField] private LoadingProgressChannelSO _loadingReward;//进入游戏时的加载数据

    /// <summary>
    /// 玩家全局信息
    /// </summary>
    public BindablePlayerInfo PlayerInfo { get => _playerInfo;}
    /// <summary>
    /// 玩家的来源
    /// </summary>
    public string UserFrom { get => userFrom; set => userFrom = value; }
    /// <summary>
    /// 玩家的UserID
    /// </summary>
    public string UserID { get => userID; set => userID = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 公共方法
    /// <summary>
    /// 初始化
    /// </summary>
    public void Start()
    {
        BattleManager.OnEndBattle += OnEndBattle;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.S))
        {
            MetaCurrencyManager.Instance.AddMetaCoin(RewardType.Diamond,500);
        }
    }

    /// <summary>
    /// 获取用户数据
    /// 先获取用户的openid，然后去数据库中拉取数据：
    ///1.若有数据，则表明不是新玩家，则直接拉取数据，并直接进入游戏
    ///2.若无数据，则表明是新玩家。则显示授权窗口->获取用户授权->创建用户数据->进入游戏
    /// </summary>
    public void InitOrLoadPlayerData()
    {
#if UNITY_EDITOR
        OnDataLoaded?.Invoke();
#else
        WeChatManager.Instance.InitSDK(() =>
        {
            userFrom = "weixin";
            _loadingReward.Raise(10,"登录中……");
            LoginToWeChat();
        });
#endif
    }

    //登录微信，拿到用户的临时code
    private void LoginToWeChat()
    {
        _loadingReward.Raise(15, "登录中……");
        WX.Login(new LoginOption
        {
            success = (res) =>
            {
                WeChatManager.UserCode = res.code;
                GetOpenID();
            },
            fail = (_) => TipManager.Instance.ShowConfirmTip("登录失败，请检查网络后重试。")
        });
    }

    //获取用户的openid
    private void GetOpenID()
    {
        _loadingReward.Raise(25, "获取玩家信息……");
        APIAccess.Instance.Code2Session(
            onSuccess: (res) =>
            {
                UserID = res.data["openid"].ToObject<string>();
                LoadOrCreatePlayerData(UserID);
            },
            onFail: (_) => TipManager.Instance.ShowConfirmTip("获取用户信息失败，请稍候重试。"),//这个一般是接口代码有误
            onError: (_) => TipManager.Instance.ShowConfirmTip("网络异常，请稍候重试。") // 网络层错误：弹出提示
        );
    }

    //最终加载或者创建玩家数据
    private int LoadOrCreatePlayerDataRetryCount = 0;//重试次数
    private void LoadOrCreatePlayerData(string userId)
    {
        if (LoadOrCreatePlayerDataRetryCount < 3)
        {
            _loadingReward.Raise(5, "玩家信息加载中……", true);
        }
        else
        {
            _loadingReward.Raise(45, "玩家信息加载中……", true);
        }
        
        APIAccess.Instance.GetData(userId,
            onSuccess: (res) =>//拿到玩家的数据,说明不是新玩家
            {
                //拿到玩家的数据,说明不是新玩家
                PlayerInfo parsed = res.data.ToObject<PlayerInfo>();
                //将数据转到_playerInfo变量中去
                _playerInfo.CopyFromPlayerInfo(parsed);
                //更新在线时间为当前时间
                APIAccess.Instance.UpdateLastOnline(userId);

                //判断用户是否给予过昵称和头像的授权
                WX.GetSetting(new GetSettingOption()
                {
                    success = (res) =>
                    {
                        _loadingReward.Raise(60, "玩家信息加载中……");
                        Debug.Log($"获取用户权限: {JsonConvert.SerializeObject(res.authSetting, Formatting.Indented)}");
                        //已经授权过,未授权过的话不需要做任何操作，所以没有else
                        if (res.authSetting.TryGetValue("scope.userInfo", out bool authorized) && authorized)
                        {
                            //获取用户的头像和昵称等信息
                            WX.GetUserInfo(new GetUserInfoOption()
                            {
                                success = (res) =>//成功获取用户信息，数据库中的昵称和头像不再使用默认值
                                {
                                    _loadingReward.Raise(80, "正在加载游戏……");
                                    //若昵称或者头像发生了变化，则同步用户的昵称和头像
                                    if (PlayerInfo.UserName.Value != res.userInfo.nickName ||
                                    PlayerInfo.AvatarUrl.Value != res.userInfo.avatarUrl)
                                    {
                                        PlayerInfo.UserName.Value = res.userInfo.nickName;
                                        PlayerInfo.AvatarUrl.Value = res.userInfo.avatarUrl;
                                        //最终都要保存用户的数据
                                        APIAccess.Instance.SaveData(onComplete: () =>
                                        {
                                            _loadingReward.Raise(95, "进入游戏……");
                                            //进入游戏
                                            OnDataLoaded?.Invoke();
                                        });
                                    }
                                    else//如果没有变化，那么直接进入游戏
                                    {
                                        _loadingReward.Raise(95, "进入游戏……");
                                        OnDataLoaded?.Invoke();
                                    }
                                },
                                fail = (res) =>//用户拒绝授权，就使用默认的昵称和头像，并且直接进入游戏
                                {
                                    Debug.LogError("获取用户信息失败：" + res.errMsg);
                                    _loadingReward.Raise(95, "进入游戏……");
                                    OnDataLoaded?.Invoke();
                                },
                            });
                        }
                        else//未获取到用户权限，直接进入游戏
                        {
                            _loadingReward.Raise(95, "进入游戏……");
                            OnDataLoaded?.Invoke();
                        }

                    },
                    //获取失败的话直接进入游戏
                    fail = (res) =>
                    {
                        _loadingReward.Raise(95, "进入游戏……");
                        OnDataLoaded?.Invoke();
                    }
                });

            },
            onFail: (res) =>
            {
                if (res.code == 1002)
                {
                    Debug.Log("GetData成功，但是需要创建新玩家");
                    _loadingReward.SetInvisible();
                    RequestUserInfoThenCreatePlayer();
                }
                    
                else
                {
                    Debug.Log("GetData失败" + res.message);
                    LoadOrCreatePlayerDataRetryCount++;
                    if (LoadOrCreatePlayerDataRetryCount < 3)//最多重试三次
                    {
                        //2秒钟后重试
                        StartCoroutine(DelayDoing(3, () => {
                            LoadOrCreatePlayerData(userId);
                        }));
                    }
                    else
                    {
                        TipManager.Instance.ShowConfirmTip("玩家信息获取失败，请点击重试", "重试", () =>
                        {
                            LoadOrCreatePlayerData(userId);
                        });
                    }
                }
            },
            onError: (_) =>
            {
                LoadOrCreatePlayerDataRetryCount++;
                if (LoadOrCreatePlayerDataRetryCount < 3)//最多重试三次
                {
                    //2秒钟后重试
                    StartCoroutine(DelayDoing(3, () => {
                        LoadOrCreatePlayerData(userId);
                    }));
                }
                else
                {
                    TipManager.Instance.ShowConfirmTip("玩家信息获取失败，请点击重试", "重试", () =>
                    {
                        LoadOrCreatePlayerData(userId);
                    });
                }
            },
            onComplete:()=> { Debug.Log("GetData结束"); });
        
    }

    //请求用户数据然后创建玩家
    private void RequestUserInfoThenCreatePlayer()
    {
        Rect rect = GetStartButtonRect();//获取开始按钮的位置和大小
        //创建按钮让用户点击以获取用户信息
        wxUserInfoButton = WX.CreateUserInfoButton((int)rect.x, Screen.height - (int)rect.y - (int)rect.height, (int)rect.width, (int)rect.height, "", true);
        //该按钮的点击事件
        wxUserInfoButton.OnTap((res) =>
        {
            Debug.Log($"res.errCode为{res.errCode}");//0代表允许授权，1代表拒绝授权
            wxUserInfoButton.Destroy();//无论有没有给予授权，都将按钮销毁
            startButton.gameObject.SetActive(false);//无论有没有给予授权，都将按钮隐藏

            if (res.errCode == 0)//用户允许授权
            {
                //获取用户的头像和昵称等信息
                WX.GetUserInfo(new GetUserInfoOption()
                {
                    success = (res) =>//成功获取用户信息，数据库中的昵称和头像不再使用默认值
                    {
                        //若昵称或者头像发生了变化，则同步用户的昵称和头像
                        if (PlayerInfo.UserName.Value != res.userInfo.nickName ||
                        PlayerInfo.AvatarUrl.Value != res.userInfo.avatarUrl)
                        {
                            PlayerInfo.UserName.Value = res.userInfo.nickName;
                            PlayerInfo.AvatarUrl.Value = res.userInfo.avatarUrl;
                        }
                    },
                    fail = (res) =>//用户拒绝授权，就使用默认的昵称和头像
                    {
                        Debug.LogError("获取用户信息失败：" + res.errMsg);
                        //TipManager.Instance.ShowTip("用户信息获取失败");
                    },
                    complete = (res) =>
                    {
                        Debug.Log("获取用户信息操作完成");
                        //最终都要保存用户的数据
                        APIAccess.Instance.SaveData(onComplete: () =>
                        {
                            _loadingReward.Raise(60, "玩家信息加载中……", true);
                            APIAccess.Instance.UpdateLastOnline(UserID);//更新在线时间为当前时间
                            _loadingReward.Raise(95, "进入游戏……");
                            //进入游戏
                            OnDataLoaded?.Invoke();
                        });

                    }
                });
            }
            else//用户拒绝授权也要保存用户的数据
            {
                //最终都要保存用户的数据
                APIAccess.Instance.SaveData(onComplete: () =>
                {
                    _loadingReward.Raise(60, "玩家信息加载中……", true);
                    APIAccess.Instance.UpdateLastOnline(UserID);//更新在线时间为当前时间
                    _loadingReward.Raise(95, "进入游戏……");
                    //进入游戏
                    OnDataLoaded?.Invoke();
                });
            }

        });
        
    }

    /// <summary>
    /// 保存 PlayerInfo
    /// </summary>
    public void SavePlayerInfo()
    {
        APIAccess.Instance.SaveData();
    }

    /// <summary>
    /// 加载 PlayerInfo（使用 Newtonsoft）
    /// </summary>
    public void LoadPlayerInfo(UnityAction onSuccess=null)
    {
        //这里要首先微信获取玩家的ID，玩家的来源，先假设一个ID
        APIAccess.Instance.GetData(UserID, 
            onSuccess:(res)=>{
                //直接用 JObject 内置方法转成 PlayerInfo
                PlayerInfo parsed = res.data.ToObject<PlayerInfo>();
                _playerInfo.CopyFromPlayerInfo(parsed);//从PlayerInfo转为BindablePlayerInfo
                APIAccess.Instance.UpdateLastOnline(UserID);
                onSuccess?.Invoke();
                _loadingReward.Raise(95, "进入游戏……");
                OnDataLoaded?.Invoke();
            },
            onFail: (res) =>
            {
                if (res.code == 1002)//没有该UserID，是新玩家，注册一个
                {
                    APIAccess.Instance.SaveData(
                        onSuccess: (res) =>
                        {
                            APIAccess.Instance.UpdateLastOnline(UserID);
                            _loadingReward.Raise(95, "进入游戏……");
                            OnDataLoaded?.Invoke();
                        });
                }
            },
            onError: (res) =>
            {

            });

    }
#endregion

    #region 私有方法
    //结束挑战
    private void OnEndBattle(bool success)
    {
        if(success)
        {
            _playerInfo.TotalPassCount.Value++;
            OnPassCountChanged?.Invoke(_playerInfo.TotalPassCount.Value);
            GameUIManager.UploadScore(_playerInfo.TotalPassCount.Value);//上传排行榜
        }
    }

    private Rect GetStartButtonRect()
    {
        var rectTransform = startButton.GetComponent<RectTransform>();
        // 获取 RectTransform 的四个角的世界坐标
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // 创建屏幕矩形
        var screenRect = new Rect(
                        worldCorners[0].x,
                        worldCorners[0].y,
                        worldCorners[2].x - worldCorners[0].x,
                        worldCorners[2].y - worldCorners[0].y);

        Debug.Log($"Screen Rect: {screenRect}");
        return screenRect;
    }

    // 协程实现延迟重试
    private IEnumerator DelayDoing(float delayTime,UnityAction callback)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        callback?.Invoke();
    }
    #endregion
}

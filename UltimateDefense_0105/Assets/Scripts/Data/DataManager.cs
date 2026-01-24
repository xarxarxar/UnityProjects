using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Text;


public class DataManager : MonoBehaviour
{
    // 单例
    public static DataManager Instance;
    public Image startButton;
    WXUserInfoButton wxUserInfoButton;
    [SerializeField] private PlayerInfo _playerInfo =new PlayerInfo();//全局的玩家信息
    private string userID = "wx12345678";// string.Empty;
    private string userFrom=string.Empty;

    public static event UnityAction<int> OnPassCountChanged;//通关次数变化
    public static event UnityAction OnDataLoaded;//数据加载完毕

    private DateTime lastSaveTime=DateTime.MinValue;
    private string version="v1";
    private FinalSavePlayerInfo localFinalPlayerInfp=null;//本地保存的finalplayer
    private FinalSavePlayerInfo cloudFinalPlayerInfp=null;//云端下载的finalplayer
    private FinalSavePlayerInfo newestFinalPlayerInfp=null;//最新的那个在使用的finalplayer

    public long secondsToNextDay = 99999;//距离下一天的秒数

    [SerializeField] private LoadingProgressChannelSO _loadingReward;//进入游戏时的加载数据
    public string FilePath;//玩家信息保存路径
    private bool _dataInitialized = false;

    //==========协程==========
    private Coroutine nextDayCoro = null;//等待下一天的协程

    private static bool _onHideRegistered = false;//是否注册微信小游戏生命周期事件


    /// <summary>
    /// 玩家全局信息
    /// </summary>
    public PlayerInfo PlayerInfo { get => _playerInfo;}
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
        FilePath= Path.Combine(Application.persistentDataPath, "player_info.dat");
        OnDataLoaded -= OnDataLoad;
        OnDataLoaded += OnDataLoad;
        BattleManager.OnEndBattle -= OnEndBattle;
        BattleManager.OnEndBattle += OnEndBattle;

        if (!_onHideRegistered)
        {
            _onHideRegistered = true;

            WX.OnHide((callBackRes) =>
            {
                Debug.Log("退出小游戏，触发保存文件");
                SavePlayerInfoCloud();
            });
        }
    }

    //玩家的数据读取完成之后
    private void OnDataLoad()
    {
        if (_dataInitialized) return;
        _dataInitialized = true;

        //转换成PlayerInfo
        SerializedPlayerInfo serializedPlayerInfo = newestFinalPlayerInfp.p;
        SerializedPlayerInfo.ChangeSerializedPlayerInfo(serializedPlayerInfo, _playerInfo);
        //不是同一天，需要重置每日任务之类的
        if (!IsSameBeijingDay(newestFinalPlayerInfp.s))
        {
            _playerInfo.DailyTask.ResetDailyTask();//重置每日任务
        }
        //获取距离下一天的秒数
        long nextDaySeconds = GetSecondsUntilNextBeijingMidnight();
        if (nextDayCoro != null)
        {
            StopCoroutine(nextDayCoro);
        }
        StartCoroutine(WaitNextDay(nextDaySeconds));//如果玩家在玩的过程中经过了00.00，则进行一次任务重置
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
        userID = "wx12345678";
        LoadOrCreatePlayerData(userID);
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
                UserID ="wx"+ res.data["openid"].ToObject<string>();
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
        localFinalPlayerInfp = LoadPlayerInfoLocal();//从本地加载信息

        //下载玩家数据
        APIAccess.Instance.DownloadPlayerInfo(userId,
            //API调用成功
            onSuccess: (finalPlayerInfo) =>
            {
                //==========更新最新的玩家数据==========
                cloudFinalPlayerInfp = finalPlayerInfo;//云端的信息

                if (localFinalPlayerInfp == null)//本地没有,说明玩家换了新设备
                {
                    newestFinalPlayerInfp = cloudFinalPlayerInfp;

                    Debug.Log($"将云端保存到本地，云端的钻石数量为{cloudFinalPlayerInfp.p.d}");
                    SavePlayerInfoLocal();//保存一份到本地
                }
                else//本地有存档，则判断哪个存档是更新的
                {
                    newestFinalPlayerInfp = GetLatestSave(cloudFinalPlayerInfp, localFinalPlayerInfp);//获取最新的那个
                    if(newestFinalPlayerInfp == cloudFinalPlayerInfp)//云端最新
                    {
                        Debug.Log($"两端都有，但云端是最新的");
                        SavePlayerInfoLocal();//保存一份到本地
                    }
                    else if(newestFinalPlayerInfp== localFinalPlayerInfp)
                    {
                        Debug.Log($"两端都有，但本地端是最新的");
                        SavePlayerInfoCloud();//上传一份到云端
                    }
                    //有了上面的判断，应该不会出现newestFinalPlayerInfp为null的情况，所以后面不用判断了
                }
                
#if UNITY_EDITOR
                _loadingReward.Raise(95, "进入游戏……");
                OnDataLoaded?.Invoke();
#else
                //==========判断用户是否给予过昵称和头像的授权==========
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
                                    if (PlayerInfo.UserName != res.userInfo.nickName ||
                                    PlayerInfo.AvatarUrl != res.userInfo.avatarUrl)
                                    {
                                        //SetUserName和SetAvatarUrl中会自动将改动保存到本地
                                        //先不要上传到云端，云端的文件是其他地方自动隔一段时间上传一次的
                                        PlayerInfo.SetUserName(res.userInfo.nickName);
                                        PlayerInfo.SetAvatarUrl(res.userInfo.avatarUrl);
                                        _loadingReward.Raise(95, "进入游戏……");
                                        //进入游戏
                                        OnDataLoaded?.Invoke();
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
#endif
            },
            onFail: (res) =>
            {
                Debug.Log($"错误信息为{res}");
                if (res == "3002")
                {
                    Debug.Log("云端没有玩家数据");
                    _loadingReward.SetInvisible();

                    if (localFinalPlayerInfp == null)//本地也没有，说明是新玩家
                    {
#if UNITY_EDITOR
                        _loadingReward.Raise(95, "进入游戏……");

#else
                        RequestUserInfoThenCreatePlayer();//创建请求微信信息的按钮，尝试获取玩家的信息
#endif
                        SavePlayerInfoLocal();//保存一份到本地
                    }
                    else//本地有存档，说明本地的没有及时上传
                    {
                        Debug.Log("本地有玩家数据");
                        newestFinalPlayerInfp = localFinalPlayerInfp;//最新的存档设为本地的存档
                    }
                    SavePlayerInfoCloud();//上传一份到云端
                    OnDataLoaded?.Invoke();
                }
                else
                {
                    Debug.Log("GetData失败" + res);
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
                        if (PlayerInfo.UserName != res.userInfo.nickName ||
                        PlayerInfo.AvatarUrl != res.userInfo.avatarUrl)
                        {
                            PlayerInfo.SetUserName(res.userInfo.nickName);
                            PlayerInfo.SetAvatarUrl(res.userInfo.avatarUrl);
                            
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
                        _loadingReward.Raise(60, "玩家信息加载中……", true);
                        _loadingReward.Raise(95, "进入游戏……");
                        //进入游戏
                        OnDataLoaded?.Invoke();

                    }
                });
            }
            else//用户拒绝授权也要保存用户的数据
            {
                _loadingReward.Raise(60, "玩家信息加载中……", true);
                _loadingReward.Raise(95, "进入游戏……");
                //进入游戏
                OnDataLoaded?.Invoke();
            }

        });
    }


    /// <summary>
    /// 更新newestFinalPlayerInfp
    /// </summary>
    public void UpdateNewestFinalPlayerInfp()
    {
        //Debug.Log($"更新newestFinalPlayerInfp之前,此时towerstatemap==null吗{_playerInfo.TowerStateMap==null},_playerInfo==null吗{_playerInfo == null}");
        newestFinalPlayerInfp.p= SerializedPlayerInfo.ChangePlayerInfo(_playerInfo);
    }

    /// <summary>
    /// 本地保存玩家信息
    /// </summary>
    public void SavePlayerInfoLocal()
    {
        if (newestFinalPlayerInfp == null)
        {
            newestFinalPlayerInfp=new FinalSavePlayerInfo();
        }
        
        newestFinalPlayerInfp.v=version;
        lastSaveTime= DateTime.UtcNow;
        newestFinalPlayerInfp.s =SerializedPlayerInfo.DateTimeToUnixSeconds(lastSaveTime);

        string SavePath = Path.Combine(Application.persistentDataPath, "player_info.dat");

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        //Debug.Log($"保存信息,d为{newestFinalPlayerInfp.p.d}");
        string json = JsonConvert.SerializeObject(newestFinalPlayerInfp, settings);

        // GZip 压缩
        byte[] compressedData = CompressJson(json);

        File.WriteAllBytes(SavePath, compressedData);
    }

    /// <summary>
    /// 从本地读取FinalSavePlayerInfo
    /// </summary>
    public FinalSavePlayerInfo LoadPlayerInfoLocal()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "player_info.dat");

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("存档文件不存在，返回 null");
            return null;
        }

        try
        {
            // 读取二进制
            byte[] compressedData = File.ReadAllBytes(savePath);

            if (!IsGZip(compressedData))
            {
                Debug.LogError("本地存档不是 GZip，视为损坏");
                return null;
            }

            //解压为 JSON
            string json = DecompressToString(compressedData);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            FinalSavePlayerInfo fin= JsonConvert.DeserializeObject<FinalSavePlayerInfo>(json, settings);
            Debug.Log($"本地的存档的d为{fin.p.d}");
            return fin;
        }
        catch (Exception e)
        {
            Debug.LogError($"读取玩家存档失败：{e}");
            return null;
        }
    }

    private bool IsGZip(byte[] data)
    {
        return data != null &&
               data.Length >= 2 &&
               data[0] == 0x1F &&
               data[1] == 0x8B;
    }

    /// <summary>
    /// 从云端加载玩家的信息
    /// </summary>
    /// <returns></returns>
    public void SavePlayerInfoCloud()
    {
        
        APIAccess.Instance.UploadPlayerInfo(userID);
    }

    /// <summary>
    /// 判断两个json内容是否一样
    /// </summary>
    /// <param name="jsonA"></param>
    /// <param name="jsonB"></param>
    /// <returns></returns>
    public static bool IsJsonEqual(string jsonA, string jsonB)
    {
        if (string.IsNullOrEmpty(jsonA) || string.IsNullOrEmpty(jsonB))
            return false;

        JToken tokenA = JToken.Parse(jsonA);
        JToken tokenB = JToken.Parse(jsonB);

        return JToken.DeepEquals(tokenA, tokenB);
    }

    /// <summary>
    /// 压缩string
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static byte[] CompressJson(string json)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(json);

        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                gzip.Write(inputBytes, 0, inputBytes.Length);
            }
            return output.ToArray();
        }
    }

    /// <summary>
    /// 解压string
    /// </summary>
    /// <param name="compressedData"></param>
    /// <returns></returns>
    public static string DecompressToString(byte[] compressedData)
    {
        using (var input = new MemoryStream(compressedData))
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        using (var output = new MemoryStream())
        {
            gzip.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
#endregion

    #region 私有方法
    //结束挑战
    private void OnEndBattle(bool success)
    {
        if(success)
        {
            _playerInfo.SetTotalPassCount(_playerInfo.TotalPassCount + 1);
            OnPassCountChanged?.Invoke(_playerInfo.TotalPassCount);
            GameUIManager.UploadScore(_playerInfo.TotalPassCount);//上传排行榜
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

    /// <summary>
    /// 比较两个存档，返回更新时间较新的那个
    /// </summary>
    private FinalSavePlayerInfo GetLatestSave(
        FinalSavePlayerInfo a,
        FinalSavePlayerInfo b
    )
    {
        if (a == null && b == null) return null;
        if (a == null) return b;
        if (b == null) return a;

        return a.s >= b.s ? a : b;
    }

    public static bool IsSameBeijingDay(long saveTimeUnixSeconds)
    {
        // 保存时间（UTC）
        DateTime saveUtc =
            DateTimeOffset.FromUnixTimeSeconds(saveTimeUnixSeconds).UtcDateTime;

        // 北京时间
        DateTime saveBeijing = saveUtc.AddHours(8);
        DateTime nowBeijing = DateTime.UtcNow.AddHours(8);

        return saveBeijing.Date == nowBeijing.Date;
    }


    /// <summary>
    /// 计算当前北京时间到明天 00:00:00 还剩多少秒
    /// </summary>
    public static long GetSecondsUntilNextBeijingMidnight()
    {
        // 当前北京时间
        DateTime nowBeijing = DateTime.UtcNow.AddHours(8);

        // 明天 00:00:00（北京时间）
        DateTime nextMidnightBeijing = nowBeijing.Date.AddDays(1);

        // 剩余秒数
        TimeSpan diff = nextMidnightBeijing - nowBeijing;

        return (long)diff.TotalSeconds;
    }


    //等到下一天，重置每日任务
    private IEnumerator WaitNextDay(long seconds)
    {

        yield return new WaitForSecondsRealtime(seconds);
        _playerInfo.DailyTask.ResetDailyTask();//重置
    }

    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WeChatWASM;


/// <summary>
/// 进入游戏
/// </summary>
public class GameEntrance : MonoBehaviour
{ 
    #region 静态变量
    public static GameEntrance instance;//单例
    /// <summary>
    /// 日期更新时执行
    /// </summary>
    public static event UnityAction OnGetingPlayerWechatInfo;//正在获取玩家微信授权
    public static event UnityAction OnGetPlayerWechatInfo;//拿到玩家微信授权
    public static event UnityAction OnDateUpdate;//日期更新事件
    public static event UnityAction OnPlayerInfoSync;//玩家信息同步完成
    public static event UnityAction<float> OnLoadSceneProgress;//场景加载进度
    public GameInfo PlayerInfo;//玩家信息

    #endregion

    #region 公共变量
    public DecalDatabase decalDatabase_meijia;
    public DecalDatabase decalDatabase_tiehua;
    public Image startButton;//开始界面伪装的开始按钮
    
    #endregion

    #region 私有变量
    private static DateTime todayDate;//今天日期
    private long currentTime;//当前时间戳，精确到秒
    private static int weekDay;//今天周几
    private DataManager dataManager=>DataManager.instance;//DataManager单例实例
    WXUserInfoButton wxUserInfoButton;
    #endregion

    #region 公共属性
    public static DateTime TodayDate { get => todayDate; }//今天日期
    public long CurrentTime { get => currentTime; }//当前时间戳
    /// <summary>
    /// 获取今日周几,周一到周日分别为0,1,2,3,4,5,6
    /// </summary>
    public static int WeekDay { get => weekDay; }//今天周几
  
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 如果你希望加载后仍保留它
        }
        else
        {
            Destroy(gameObject); // 避免重复
        }
    }

    private void Start()
    {
        LoadDataFromCloud();
    }

    //从云端加载数据
    private void LoadDataFromCloud()
    {
        PlayerInfo.Init();
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                WX.cloud.Init(new ICloudConfig()
                {
                    env = "cloud1-7gkr9w5v84fb9104", // 云环境 ID
                    traceUser = false
                });
                CreateUserInfoButtonBefore();//先创建获取用户信息按钮
                CreateUserInfoButton();
            }
        );

        //dataManager.GetCurrentTime(UpdateTime);//获取当前时间

    }
    
    //获取并更新当前日期
    private void UpdateTime(long timestamp)
    {
        currentTime = timestamp;//更新当前时间戳
        todayDate = dataManager.TimestampToDateTime(currentTime);//更新当前日期，这是服务器日期
        DateTime dt = DateTimeOffset.FromUnixTimeSeconds(CurrentTime).ToLocalTime().DateTime;//玩家当地的日期，涉及到时区
        DayOfWeek dayOfWeek = dt.DayOfWeek;//周几这个要看玩家当地的日期
        weekDay = ((int)dayOfWeek + 6) % 7;//获取今日周几
        int seconds = dataManager.SecondsUntilNextDay(currentTime);//离下一天还有多少秒
        GetPlayerInfoFromCloud();//获取玩家信息

        OnDateUpdate?.Invoke();//日期更新事件
        // 延迟执行下一次日期更新
        StartCoroutine(UpdateDate(seconds));
    }

    //从云端获取玩家数据
    private void GetPlayerInfoFromCloud()
    {
        //加载玩家数据
        dataManager.DownloadGameInfo((playerInfo) =>
        {
            //如果数据库没有玩家数据，创建并上传,有的话就直接同步
            if (playerInfo != null)
            {
                Debug.Log("玩家数据非空");
                PlayerInfo = playerInfo;
            }
            PlayerInfo.lastLoginDate = todayDate;
            dataManager.UploadGameInfo(PlayerInfo);
            OnPlayerInfoSync?.Invoke();//玩家信息同步完成
            LoadGameScene();//开始加载游戏场景
        });
    }

    //加载游戏主场景
    private void LoadGameScene()
    {
        StartCoroutine(LoadSceneAsync());
    }


    //更新日期,seconds为延时时长
    private IEnumerator UpdateDate(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        UpdateTime(currentTime+ seconds);
    }

    //加载游戏主场景的协程
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // 通知 UI 更新进度
            OnLoadSceneProgress?.Invoke(progress);
            if (asyncLoad.progress >= 0.9f)
            {
                // 场景准备好了，可以激活
                yield return new WaitForSeconds(1f); // 如果需要等待一秒
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }


    /// <summary>
    /// 将每日登录信息列表重新设置
    /// </summary>
    /// <param name="loginRewardInfos"></param>
    public void SetGameInfoLoginRewardInfos(List<LoginRewardInfo> loginRewardInfos)
    {
        //获取6个美甲
        List<DecalData> meijias = GetRandomSkins(isOwned: false, type: SkinType.NailArt, unlockMethod: UnlockMethod.DailyLogin, count: 6);
        //获取1个贴花
        List<DecalData> tiehua = GetRandomSkins(isOwned: false, type: SkinType.Tattoo, unlockMethod: UnlockMethod.DailyLogin, count: 1);
        // 添加美甲奖励（前6天）
        for (int i = 0; i < meijias.Count; i++)
        {
            LoginRewardInfo tmpLoginRewardInfo = new LoginRewardInfo();
            tmpLoginRewardInfo.skinId = meijias[i]?.id ?? ""; // 防止null
            tmpLoginRewardInfo.weekday = i;
            loginRewardInfos.Add(tmpLoginRewardInfo);
        }
        // 添加第7天的贴花奖励
        LoginRewardInfo lastLoginReward = new LoginRewardInfo();
        lastLoginReward.skinId = tiehua[0]?.id ?? ""; // 防止null
        lastLoginReward.weekday = 6;
        loginRewardInfos.Add(lastLoginReward);
    }

    /// <summary>
    /// 随机获取皮肤的decaldata
    /// </summary>
    /// <param name="skinType"></param>
    /// <returns></returns>
    public List<DecalData> GetRandomSkins(
    bool? isOwned = null,
    bool? isEquip = null,
    bool? isNewest = null,
    SkinType? type = null,
    UnlockMethod? unlockMethod = null,
    int count = 1
)
    {
        List<DecalData> sourceList = new List<DecalData>();

        // 收集数据源
        if (type == null)
        {
            if (decalDatabase_meijia?.decalList != null)
                sourceList.AddRange(decalDatabase_meijia.decalList);

            if (decalDatabase_tiehua?.decalList != null)
                sourceList.AddRange(decalDatabase_tiehua.decalList);
        }
        else if (type == SkinType.NailArt)
        {
            if (decalDatabase_meijia?.decalList != null)
                sourceList.AddRange(decalDatabase_meijia.decalList);
        }
        else
        {
            if (decalDatabase_tiehua?.decalList != null)
                sourceList.AddRange(decalDatabase_tiehua.decalList);
        }

        // 多条件过滤
        List<DecalData> filteredList = sourceList.FindAll(d =>
            (isOwned == null || d.isOwned == isOwned) &&
            (isEquip == null || d.isEquip == isEquip) &&
            (isNewest == null || d.isNewest == isNewest) &&
            (unlockMethod == null || d.unlockMethod == unlockMethod)
        );

        // 打乱顺序
        for (int i = filteredList.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (filteredList[i], filteredList[j]) = (filteredList[j], filteredList[i]);
        }

        // 构建固定长度的返回列表
        List<DecalData> result = new List<DecalData>();
        for (int i = 0; i < count; i++)
        {
            if (i < filteredList.Count)
            {
                result.Add(filteredList[i]);
            }
            else
            {
                result.Add(null); // 不足则补 null
            }
        }

        return result;
    }

    public void CreateUserInfoButton()
    {
        WX.GetSetting(new GetSettingOption()
        {
            success = (res) =>
            {
                Debug.Log($"获取Setting成功");
                //已经授权过
                if (res.authSetting.ContainsKey("scope.userInfo") && res.authSetting["scope.userInfo"] == true)
                {
                    startButton.gameObject.SetActive(false);
                    OnGetingPlayerWechatInfo?.Invoke();//已经获取过权限了
                    wxUserInfoButton.Hide();
                    Debug.Log($"已经获取过权限");
                    WX.GetUserInfo(new GetUserInfoOption()
                    {
                        success = (res) =>
                        {
                            OnGetPlayerWechatInfo?.Invoke();
                            Debug.Log($"获取用户信息成功:{res.userInfo.nickName}");
                            //ShowTipManager.instance.ShowLoading(true);
                            DataManager.instance.DownloadGameInfo((successAction) =>
                            {
                                if (res.userInfo.nickName != PlayerInfo.playerName
                            || res.userInfo.avatarUrl != PlayerInfo.avatarUrl)
                                {
                                    PlayerInfo.playerName = res.userInfo.nickName;
                                    PlayerInfo.avatarUrl = res.userInfo.avatarUrl;
                                }
                                DataManager.instance.UploadGameInfo(PlayerInfo);
                                //ShowTipManager.instance.ShowLoading(false);
                                //GameEntrance.instance.CloseEnterGamePanel();
                                
                                dataManager.GetCurrentTime(UpdateTime);//获取当前时间
                            });

                            //ShowTipManager.instance.ShowLoading(false);
                        },
                        fail = (res) =>
                        {
                            Debug.LogError("获取用户信息失败：" + res.errMsg);
                            //ShowTipManager.instance.ShowTip("获取用户信息失败");
                            //ShowTipManager.instance.ShowLoading(false);
                        },
                        complete = (res) =>
                        {
                            Debug.Log("获取用户信息操作完成");
                        }
                    });
                }
                else
                {
                    Debug.Log($"还未获取过权限");
                    wxUserInfoButton.Show();
                }
            },

            fail = (res) =>
            {
                Debug.Log($"获取Setting失败：{res.errMsg}");
            }
        });

    }


    public void CreateUserInfoButtonBefore()
    {
        Rect rect = GetStartButtonRect();
        wxUserInfoButton = WX.CreateUserInfoButton((int)rect.x, Screen.height - (int)rect.y - (int)rect.height, (int)rect.width, (int)rect.height, "", true);
        wxUserInfoButton.OnTap((res) =>
        {
            if (res.errCode == 0)
            {
                wxUserInfoButton.Hide();
                WX.GetUserInfo(new GetUserInfoOption()
                {
                    success = (res) =>
                    {
                        startButton.gameObject.SetActive(false);
                        OnGetPlayerWechatInfo?.Invoke();
                        Debug.Log($"获取用户信息成功:{res.userInfo.nickName}");
                        //ShowTipManager.instance.ShowLoading(true);
                        DataManager.instance.DownloadGameInfo((successAction) =>
                        {
                            if (res.userInfo.nickName != PlayerInfo.playerName
                        || res.userInfo.avatarUrl != PlayerInfo.avatarUrl)
                            {
                                PlayerInfo.playerName = res.userInfo.nickName;
                                PlayerInfo.avatarUrl = res.userInfo.avatarUrl;
                            }
                            DataManager.instance.UploadGameInfo(PlayerInfo);
                            //GameEntrance.instance.CloseEnterGamePanel();
                            
                            dataManager.GetCurrentTime(UpdateTime);//获取当前时间
                        });
                    },
                    fail = (res) =>
                    {
                        Debug.LogError("获取用户信息失败：" + res.errMsg);
                        TipManager.instance.ShowTip("获取信息失败");
                    },
                    complete = (res) =>
                    {
                        Debug.Log("获取用户信息操作完成");
                    }
                });
            }
            else
            {
                TipManager.instance.ShowTip("请先授权");
            }
        });
        wxUserInfoButton.Hide();

    }

    private Rect GetStartButtonRect()
    {
        var rectTransform = GameEntrance.instance.startButton.GetComponent<RectTransform>();
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
}

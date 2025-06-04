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
    public static event UnityAction OnDateUpdate;//日期更新事件
    public static event UnityAction OnPlayerInfoSync;//玩家信息同步完成
    public static event UnityAction<float> OnLoadSceneProgress;//场景加载进度
    public GameInfo PlayerInfo;//玩家信息

    #endregion

    #region 公共变量
    public DecalDatabase decalDatabase_meijia;
    public DecalDatabase decalDatabase_tiehua;
    #endregion

    #region 私有变量
    private static DateTime todayDate;//今天日期
    private long currentTime;//当前时间戳，精确到秒
    private static int weekDay;//今天周几
    private DataManager dataManager=>DataManager.instance;//DataManager单例实例
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
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                WX.cloud.Init(new ICloudConfig()
                {
                    env = "cloud1-7gkr9w5v84fb9104", // 云环境 ID
                    traceUser = false
                });
            }
        );

        dataManager.GetCurrentTime(UpdateTime);//获取当前时间

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
            //如果数据库没有玩家数据，创建并上传
            if (playerInfo == null)
            {
                Debug.Log("玩家数据为空");
                PlayerInfo.Init();
                dataManager.UploadGameInfo(PlayerInfo);
            }
            else
            {
                Debug.Log("玩家数据非空");
                PlayerInfo = playerInfo;
            }
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
        Debug.Log("执行到5");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;
        Debug.Log("执行到6");
        while (!asyncLoad.isDone)
        {
            Debug.Log("执行到61");
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("执行到62");
            // 通知 UI 更新进度
            OnLoadSceneProgress?.Invoke(progress);
            Debug.Log("执行到63");
            if (asyncLoad.progress >= 0.9f)
            {
                // 场景准备好了，可以激活
                yield return new WaitForSeconds(1f); // 如果需要等待一秒
                asyncLoad.allowSceneActivation = true;
            }
            Debug.Log("执行到64");
            yield return null;
        }
        Debug.Log("执行到7");
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
}

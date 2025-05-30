using Newtonsoft.Json;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool[][] weekdaysGesture = new bool[][] {
        new[] { false,true,false,false,false},
        new[] { false,true, true, false,false},
        new[] { false,true, true, true, false},
        new[] { false,true, true, true, true},
        new[] { true, true, true, true, true},
        new[] { true, false,false,false,true}
    };
    public Hand weekHand;
    public Text weekText;

    public Text coinText;
    public Text levelText;

    public Button startLevelButton;

    private DataManager dataManager=>DataManager.instance;
    private SkinManager skinManager => SkinManager.instance;
    public GameInfo gameInfo;//全局的游戏数据

    public long currentTime;//
    public int WeekDay;//今天周几
    private long SecondsUntilNextDay;
    public DateTime TodayDate;

    public static event UnityAction<int> OnDateUpdated;//日期更新

    private void Awake()
    {
        instance=this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }


    void GameStart()
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

                //StartCoroutine(GameFailCanvas.instance.LoadLua());
            }
        );
#if UNITY_EDITOR
        WeekDay = 3;
        TodayDate=DateTime.Now;
#else
        dataManager.GetCurrentTime(UpdateTime);
#endif
        DataManager.OnGetDatetime += LoadGameInfo;//获取日期完毕以后再开始加载玩家数据

        AudioManager.instance.PlayBGM("MenuBGM");
        
        startLevelButton.onClick.AddListener(() =>
        {
            Debug.Log($"当前关卡是{gameInfo.Level}");
            LevelControl.instance.GenerateLevel(gameInfo.Level);
            if( gameInfo.Level == 1)
            {
                LevelPlaying.instance.StartGuideLevel();
            }
            else
            {
                LevelPlaying.instance.StartLevel();
            }
        });
    }

    //加载玩家数据
    private void LoadGameInfo(DateTime dateTime)
    {
        TodayDate=dateTime;
        GetTodayDateWithWeekday();//显示周几
        //加载玩家数据
        dataManager.DownloadGameInfo((gameInfo) =>
        {
            //如果数据库没有玩家数据，创建并上传
            if (gameInfo == null)
            {
                Debug.Log("玩家数据为空");
                this.gameInfo.Init();
                dataManager.UploadGameInfo(this.gameInfo);
            }
            else
            {
                Debug.Log("玩家数据非空");
                this.gameInfo = gameInfo;
            }

            levelText.text = $"关卡{gameInfo.Level}";
            skinManager.Init();
            LoginReward.instance.Init();
        });
    }

    private void UpdateTime(long timestamp)
    {
        currentTime = timestamp;
        DateTime dt = DateTimeOffset.FromUnixTimeSeconds(currentTime).ToLocalTime().DateTime;
        DayOfWeek dayOfWeek = dt.DayOfWeek;
        WeekDay = ((int)dayOfWeek +6)%7;
        Debug.Log($"weekday is {WeekDay},dayOfWeek is {dayOfWeek}");
        int seconds = dataManager.SecondsUntilNextDay(currentTime);

        TodayDate = dataManager.TimestampToDateTime(currentTime);//更新今日时间
        OnDateUpdated?.Invoke(WeekDay);//日期更新
        // 延迟执行下一次 UpdateTime
        StartCoroutine(InvokeUpdateTimeAfterDelay(seconds));
    }

    private IEnumerator InvokeUpdateTimeAfterDelay(int delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        UpdateTime(currentTime+ delaySeconds);
    }


    public void GetTodayDateWithWeekday()
    {
        if (WeekDay == 6)
        {
            weekText.text = "明天又是周";
            weekHand.SetFingers(weekdaysGesture[0]);
        }
        else
        {
            weekText.text = "今天是周";
            weekHand.SetFingers(weekdaysGesture[WeekDay]);
        }

        int seconds = dataManager.SecondsUntilNextDay(currentTime);//到明天还有多久

        Invoke(nameof(GetTodayDateWithWeekday), seconds); // 延迟执行
    }


    /// <summary>
    /// 判断
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static bool IsMonday(long timestamp)
    {
        DateTime dt = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        return dt.DayOfWeek == DayOfWeek.Monday;
    }
}

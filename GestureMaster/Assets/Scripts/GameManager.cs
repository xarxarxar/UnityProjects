using Newtonsoft.Json;
using System;
using System.Collections;
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
    public Text levelText;

    public Button startLevelButton;

    private DataManager dataManager=>DataManager.instance;
    private SkinManager skinManager => SkinManager.instance;
    private GameInfo PlayerInfo=>GameEntrance.instance.PlayerInfo;
    private int WeekDay=> GameEntrance.WeekDay;

    private void Awake()
    {
        instance=this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStart();

    }

    void BindEvent()
    {
        GameEntrance.OnDateUpdate += GetTodayDateWithWeekday;//日期更新之后更新手势
    }

    void GameStart()
    {
        AudioManager.instance.PlayBGM("MenuBGM");
        
        startLevelButton.onClick.AddListener(() =>
        {
            Debug.Log($"当前关卡是{PlayerInfo.level}");
            LevelControl.instance.GenerateLevel(PlayerInfo.level);
            if(PlayerInfo.level == 1)
            {
                LevelPlaying.instance.StartGuideLevel();
            }
            else
            {
                LevelPlaying.instance.StartLevel();
            }
        });
        GetTodayDateWithWeekday();//显示周几
        levelText.text = $"关卡{PlayerInfo.level}";
        skinManager.Init();
        LoginReward.instance.Init();

        //WechatManager.EvaluateAndRecommend();
    }

    //设置首页周几的手势
    private void GetTodayDateWithWeekday()
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
    }
}

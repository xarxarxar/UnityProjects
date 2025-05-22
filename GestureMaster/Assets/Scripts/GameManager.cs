using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameInfo gameInfo;

    private void Awake()
    {
        instance=this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"gameInfo.Level is {gameInfo.Level}");
        }
    }

    void GameStart()
    {
        AudioManager.instance.PlayBGM("MenuBGM");
        GetTodayDateWithWeekday();

        gameInfo.coinCount = 500;
        coinText.text= gameInfo.coinCount.ToString();
        gameInfo.Level = 1;
        Debug.Log($"gameInfo.Level is {gameInfo.Level}");
        startLevelButton.onClick.AddListener(() =>
        {
            Debug.Log($"gameInfo.Level is {gameInfo.Level}");
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

    public void UpdateCoinText()
    {
        coinText.text = $"{gameInfo.coinCount}";
    }


    public void GetTodayDateWithWeekday()
    {
        DateTime now = DateTime.Now;
        
        DayOfWeek dayOfWeek = now.DayOfWeek;
        int weekIndex = (int)dayOfWeek;
        if (weekIndex == 7)
        {
            weekText.text = "明天又是周";
            weekHand.SetFingers(weekdaysGesture[0]);
        }
        else
        {
            weekText.text = "今天是周";
            weekHand.SetFingers(weekdaysGesture[weekIndex-1]);
        }

        int seconds = GetSecondsUntilTomorrow();//到明天还有多久
        Invoke(nameof(GetTodayDateWithWeekday), seconds); // 延迟执行
    }


    //离明天还有多少秒
    public int GetSecondsUntilTomorrow()
    {
        DateTime now = DateTime.Now;
        DateTime tomorrow = now.Date.AddDays(1); // 明天的0点
        TimeSpan timeLeft = tomorrow - now;
        return Mathf.FloorToInt((float)timeLeft.TotalSeconds); // 转为整数秒
    }

}

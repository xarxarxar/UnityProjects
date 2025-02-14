using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

// 在关卡控制器中调用
public class LevelController : MonoBehaviour
{
    private DeckManager deckManager;

    public LevelConfig levelConfig; 

    // 分数
    private int totalScore=0;//当前总分数
    public int TotalScore 
    { 
        get => totalScore; 
        set 
        { 
            if (totalScore != value) 
            { 
                totalScore = value;
                totalScoreText.text= totalScore.ToString();
            } 
        }
    }
    private int singleScore = 0;//单回合分数
    public int SingleScore 
    { 
        get => singleScore;
        set
        {
            if(singleScore != value)
            {
                singleScore = value;
                singleScoreText.text= value.ToString();
            }
        } 
    }
    public Text totalScoreText;//当前总分数显示text
    public Text singleScoreText;//单个回合的分数显示text

    // 回合
    private int currentRound=1;//当前回合
    public int CurrentRound
    {
        get => currentRound;
        set
        {
            currentRound = value;
            if (value <= deckManager.config.rounds)
            {
                roundText.text = $"{value}/{deckManager.config.rounds}";
            }
        }
    }
    public Text roundText;//回合数的text

    public static LevelController instance;

    

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        deckManager=DeckManager.instance;
        deckManager.roundOver +=(singleRoundScore)=>
        {
            OverRound(singleRoundScore);
        };

        deckManager.levelOver += (totalScore) =>
        {
            if(totalScore <deckManager.config.targetScore)
            {
                Debug.Log("本关卡失败");
            }
            else
            {
                Debug.Log("通关");
            }
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            deckManager.DrawCards(3, 1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            IncreaseHandLimit(20,10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayCards();
        }

    }

    public void StartLevel(int level)
    {
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = true;//打开游戏场景
        levelConfig = LevelConfigManager.instance.localDatabase.levels[level-1];
        deckManager.config= levelConfig;

        InitializeValue();
        InitializeText();
        deckManager.StartLevel();
    }

    /// <summary>
    /// 初始化数值
    /// </summary>
    void InitializeValue()
    {
        TotalScore = 0;//总分初始化为0
        SingleScore = 0;//单回合分数初始化为0
        CurrentRound = 1;
    }

    /// <summary>
    /// 初始化Text显示
    /// </summary>
    void InitializeText()
    {
        singleScoreText.text = "0";
        singleScoreText.text = SingleScore.ToString();
        roundText.text = $"{CurrentRound}/{deckManager.config.rounds}";
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    void StartRound()
    {
        CurrentRound++;
        // 每回合抽取3张字母牌，尝试抽1张特殊牌
        deckManager.DrawCards(3, 1);
    }

    /// <summary>
    /// 回合结束
    /// </summary>
    /// <param name="singleRoundScore">单回合分数</param>
    public void OverRound(int singleRoundScore)
    {
        SingleScore = singleRoundScore;//单回合分数
        TotalScore += SingleScore;//总分累加
        StartRound();
    }


    /// <summary>
    /// 出牌
    /// </summary>
    public void PlayCards()
    {
        deckManager.PlayCard();
        CacheText.ClearTextShow();
    }

    /// <summary>
    /// 抽卡
    /// </summary>
    public void DrawCards()
    {
        deckManager.DrawCards(3, 1);
    }

    /// <summary>
    /// 临时提升手牌上限（示例方法）
    /// </summary>
    /// <param name="normalBonus"></param>
    /// <param name="specialBonus"></param>
    public void IncreaseHandLimit(int normalBonus, int specialBonus)
    {
        // 这里可以修改DeckManager的内部状态
        deckManager.config.maxNormalCards = normalBonus;
        deckManager.config.maxSpecialCards = specialBonus;
    }
}

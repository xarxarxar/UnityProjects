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
                singleScoreText.text=value.ToString();
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
            if (value <= maxRounds)
            {
                roundText.text = $"{value}/{maxRounds}";
            }
        }
    }
    public Text roundText;//回合数的text

    public int maxRounds;//回合上限

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            deckManager.DrawDesignCard(SpecialEffectType.RemoveCard);
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

    public void EndLevel()
    {
        if (TotalScore >= levelConfig.targetScore)
        {
            ShowTipManager.instance.ShowTip("恭喜过关");
            //过关操作
        }
        else
        {
            ShowTipManager.instance.ShowTip("未过关");
            //未过关的操作
            GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = true;
            GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;//打开游戏场景
        }
    }

    /// <summary>
    /// 初始化数值
    /// </summary>
    void InitializeValue()
    {
        TotalScore = 0;//总分初始化为0
        SingleScore = 0;//单回合分数初始化为0
        maxRounds = levelConfig.rounds;
        CurrentRound = 1;
    }

    /// <summary>
    /// 初始化Text显示
    /// </summary>
    void InitializeText()
    {
        singleScoreText.text = "0";
        singleScoreText.text = SingleScore.ToString();
        roundText.text = $"{CurrentRound}/{maxRounds}";
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
        if(CurrentRound == maxRounds)
        {
            EndLevel();
        }
        else
        {
            StartRound();
        }
        
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
    /// 临时提升字母牌上限（示例方法）
    /// </summary>
    /// <param name="normalBonus"></param>
    public void IncreaseHandLimit(int count)
    {
        // 这里可以修改DeckManager的内部状态
        deckManager.maxNormalCards += count;
    }

    /// <summary>
    /// 临时提升缓存牌上限（示例方法）
    /// </summary>
    /// <param name="normalBonus"></param>
    public void IncreaseCacheLimit(int count)
    {
        // 这里可以修改DeckManager的内部状态
        deckManager.maxCacheCards += count;
    }

    /// <summary>
    /// 临时提升功能牌上限（示例方法）
    /// </summary>
    /// <param name="normalBonus"></param>
    public void IncreaseSpecialLimit(int count)
    {
        // 这里可以修改DeckManager的内部状态
        deckManager.maxSpecialCards += count;
    }
}

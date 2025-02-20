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
            }
        } 
    }
    public Text totalScoreText;//当前总分数显示text
    public Text targetScoreText;//当前关卡的目标分数text
    public Text targetRoundText;//当前关卡的总回合数text




    // 回合
    private int currentRound=1;//当前回合
    public int CurrentRound
    {
        get => currentRound;
        set
        {
            currentRound = value;
            if (value <= MaxRounds)
            {
                roundText.text = $"当前回合:{value}";
            }
        }
    }

    

    public Text roundText;//回合数的text

    private int maxRounds;//回合上限
    public int MaxRounds 
    { 
        get => maxRounds;
        set 
        {
            if (maxRounds != value)
            {
                maxRounds = value;
                targetRoundText.text = $"回合总数:{value}";
            }
        }
    }


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
            ShowTipManager.instance.ShowTip("恭喜过关", () =>
            {
                GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = true;
                GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;//打开游戏场景
                InfiniteLevelManager.instance.UnlockNextLevel();
            });
            //过关操作
            
        }
        else
        {
            ShowTipManager.instance.ShowTip("未过关",() =>
            {
                GameObject.Find("GameOverCanvas").GetComponent<Canvas>().enabled = true;
                GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;//打开游戏场景
            });
        }
    }

    /// <summary>
    /// 初始化数值
    /// </summary>
    void InitializeValue()
    {
        TotalScore = 0;//总分初始化为0
        SingleScore = 0;//单回合分数初始化为0
        MaxRounds = levelConfig.rounds;
        CurrentRound = 1;
    }

    /// <summary>
    /// 初始化Text显示
    /// </summary>
    void InitializeText()
    {
        targetScoreText.text = $"目标分数:{levelConfig.targetScore}";
        roundText.text = $"当前回合:{CurrentRound}";
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
        if(CurrentRound == MaxRounds)
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
        AudioManager.instance.PlaySoundEffect("PlayCard");
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

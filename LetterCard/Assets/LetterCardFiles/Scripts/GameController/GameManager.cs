using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 当游戏开始的时候，控制游戏的逻辑，并非整个游戏的逻辑，整个游戏的逻辑控制在GameEntrance里
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //局内数值
    private int currentRound;//当前回合数
    public int CurrentRound { get => currentRound; set { currentRound = value; roundText.text = $"{value}"; } }
    private int currentScore;//当前总分数
    public int CurrentScore { get => currentScore; set 
        { 
            currentScore = value;
            scoreText.text = value.ToString(); 
            CurrentInfo.Instance.currentScoreText.text=value.ToString();
        } }
    private uint nextScore;//下一目标分数
    public uint NextScore 
    { 
        get => nextScore; 
        set 
        { 
            nextScore = value;
            nextScoreText.text =$"{value}"; 
            CurrentInfo.Instance.nextScoreText.text=value.ToString();
        } }

    [SerializeField]public Text coinText;//总金币数

    public Text needCoinText;//抽卡需要的金币数
    private uint drawNeedCoin;//抽一次卡需要的金币数
    public uint DrawNeedCoin 
    { 
        get => drawNeedCoin;
        set
        {
            drawNeedCoin = value;
            if (drawNeedCoin > GameEntrance.instance.CoinCount)
            {
                needCoinText.color = new Color32(255, 34, 12, 255);//红色
            }
            else
            {
                needCoinText.color = new Color32(255, 255, 255, 255);//白色
            }
            needCoinText.text= $"×{value}";
        }
    }

    private uint minPlayCardCount=1;//最少出几张牌
    public uint MinPlayCardCount { get => minPlayCardCount; set => minPlayCardCount = value; }
    
    //字母手牌数量
    private int maxPlayCardCount=10;
    public int MaxPlayCardCount 
    { 
        get => maxPlayCardCount;
        set 
        {
            maxPlayCardCount = value;
            CurrentInfo.Instance.letterCardMaxText.text=value.ToString() ;
        }
    }
    
    //特殊牌手牌数量
    private int maxSpecialCaradCount=10;
    public int MaxSpecialCaradCount 
    { 
        get => maxSpecialCaradCount;
        set 
        { 
            maxSpecialCaradCount = value;
            CurrentInfo.Instance.specialCardMaxText.text=value.ToString() ;
        } 
    }

    //状态
    private int addScoreWhenDeleteScore = 0;//是否启用弃字生金的效果
    public int AddScoreWhenDeleteScore 
    { 
        get => addScoreWhenDeleteScore;
        set
        {
            addScoreWhenDeleteScore = value;
            CurrentInfo.Instance.addScoreWhenDeleteText.text=value.ToString() ;
        } 
    }
    private uint dropCardCount;//丢弃的牌的数量
    public uint DropCardCount 
    { 
        get => dropCardCount;
        set
        {
            dropCardCount = value;
            if( value % 3 == 0)
            {
                GameEntrance.instance.CoinCount += (uint)AddScoreWhenDeleteScore;
                if (AddScoreWhenDeleteScore != 0)
                {
                    ShowTipManager.instance.ShowTip($"弃字成金", AddScoreWhenDeleteScore);
                }
            }
        } 
    }

    //public bool useCanContinuousDraw=false;//是否启用连抽不止
    private float continuousProbability = 0.3f;//连抽的概率
    public float ContinuousProbability 
    { 
        get => continuousProbability;
        set 
        {
            continuousProbability = value;
            CurrentInfo.Instance.continousProbText.text = $"{value * 100}%";
        }
    }
    private int continuousCount = 0;//连抽的初始次数上限
    public int ContinuousCount 
    { 
        get => continuousCount;
        set 
        {
            continuousCount = value;
            CurrentInfo.Instance.continousCountText.text = value.ToString();
        }
    }

    private int extraScoreOnlyOneScore=0;//孤字成章额外加的分
    public int ExtraScoreOnlyOneScore 
    { 
        get => extraScoreOnlyOneScore;
        set
        {
            extraScoreOnlyOneScore = value;
            CurrentInfo.Instance.extraScoreOnlyOneText.text = value.ToString();
        } 
    }
   
    private int extraScoreRounOverScore=0;//字量结余额外加的分
    public int ExtraScoreRounOverScore 
    { 
        get => extraScoreRounOverScore; 
        set
        {
            extraScoreRounOverScore = value;
            CurrentInfo.Instance.extraScoreRounOverText.text = value.ToString();
        } 
    }

    

    private int canPlayZeroCardScore = 0;//空白书卷额外金币数
    public int CanPlayZeroCardScore 
    { 
        get => canPlayZeroCardScore;
        set
        {
            canPlayZeroCardScore = value;
            CurrentInfo.Instance.canPlayZeroCardText.text = value.ToString();
        }
    }




    //局内文本
    [SerializeField]private Text roundText;//显示回合数的Text
    [SerializeField]private Text scoreText;//显示当前分数的Text
    [SerializeField]private Text nextScoreText;//显示下一个目标分数的Text


    //游戏成功和游戏失败面板
    [SerializeField] private Canvas gameFailCanvas;//

    //特殊牌介绍面板
    public Transform specialIntroductionPanel;//
    public Text specialCardNameText;//特殊牌名称Text
    public Text specialCardDescriptionText;//特殊牌描述Text
    public UnityAction useSpecialCard;//使用特殊牌

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrentScore += 100;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameEntrance.instance.CoinCount += 100;
        }
#endif
    }


    /// <summary>
    /// 继续挑战
    /// </summary>
    public void ContinueChallenge()
    {

    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartChallenge()
    {
        DeckManager.instance.Init();//初始化DeckManager
        DeckManager.instance.ClearHandCards();//清空手牌
        MaxPlayCardCount = 10;
        MaxSpecialCaradCount = 10;
        CurrentRound = 0;
        CurrentScore = 0;
        NextScore = 100;
        DrawNeedCoin = 1;
        MinPlayCardCount = 1;
        DropCardCount = 0;

        ContinuousProbability = 0.3f;
        ContinuousCount = 0;
        ExtraScoreOnlyOneScore = 0;
        AddScoreWhenDeleteScore = 0;
        ExtraScoreRounOverScore = 0;
        CanPlayZeroCardScore = 0;
        StartRound();//开始回合
    }

    /// <summary>
    /// 挑战结束
    /// </summary>
    public void EndChallenge()
    {
        gameFailCanvas.enabled = true;
    }

    /// <summary>
    /// 抽卡
    /// </summary>
    public void DrawCards()
    {
        if (DeckManager.instance.IsDrawing) return;//如果正在抽牌

        if (GameEntrance.instance.CoinCount< DrawNeedCoin)
        {
            GetRewards.Instance.GetComponent<Canvas>().enabled = true;
            return;
        }
        //DeckManager.instance.IsDrawing = true;
        //ContinuousCount = 2;
        DeckManager.instance.DrawCard(ContinuousCount);//抽卡

        GameEntrance.instance.CoinCount-= DrawNeedCoin;
    }

    /// <summary>
    /// 出牌
    /// </summary>
    public void PlayCard()
    {
        AudioManager.instance.PlaySoundEffect("PlayCard");

        if (CacheText.instance.letterCards.Count < MinPlayCardCount)
        {
            ShowTipManager.instance.ShowTip("最少出一张牌");
            return;
        }

        ScoreCalculator.CalculateScore(CacheText.instance.letterCards);
        int normalScore = ScoreCalculator.normalScore;//基础分
        int extraScore = ScoreCalculator.extraScore;//额外分，如颜色相同，字母相同，组成单词
        int specialScore = ScoreCalculator.specialScore;//特殊分数
        int totalRoundScore= normalScore+ extraScore+specialScore;
        CurrentScore += totalRoundScore;//当前总分数
        GameEntrance.instance.CoinCount+= (uint)totalRoundScore;//当前总金币

        CacheText.instance.ClearCacheCard();
        EndRound();//回合结束
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    private void StartRound()
    {
        CurrentRound++;//回合数+1

        if (CurrentRound % 5 == 1) NextScore = 30 * (uint)CurrentRound;//每过5关设置一次目标分数

        DrawNeedCoin = (uint)(CurrentRound/5.0f)+1;//抽取一次所需要的金币数量就是当前的回合数

        DeckManager.instance.DrawLetterCard(2);//每回合开始抽两张卡牌
    }


    /// <summary>
    /// 结束当前回合
    /// </summary>
    private void EndRound()
    {
        if (CurrentRound % 5 == 0 && CurrentScore < NextScore)//每5关进行一次分数判定
        {
            EndChallenge();
        }
        else
        {
            StartRound(); 
        }
    }

    /// <summary>
    /// 添加金币按钮
    /// </summary>
    public void AddMoneyButton()
    {
        GetRewards.Instance.GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// 使用特殊牌
    /// </summary>
    public void UseSpecialCard()
    {
        useSpecialCard();
        CloseSpecialCardPanel();
    }

    /// <summary>
    /// 关闭当前面板按钮
    /// </summary>
    public void CloseSpecialCardPanel()
    {
        specialIntroductionPanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuart);
    }

    /// <summary>
    /// 打开当前状态面板
    /// </summary>
    public void OpenCurrentInfoPanel()
    {
        CurrentInfo.Instance.GetComponent<Canvas>().enabled = true;
        CurrentInfo.Instance.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuart);
    }
}

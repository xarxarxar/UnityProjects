using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
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
    public int CurrentScore 
    { 
        get => currentScore; 
        set 
        { 
            currentScore = value;
            scoreText.text = value.ToString(); 
            CurrentInfo.Instance.currentScoreText.text=value.ToString();

            AdaptSlider();
        }
    }
    private int nextScore;//下一目标分数
    public int NextScore 
    { 
        get => nextScore; 
        set 
        { 
            nextScore = value;
            nextScoreText.text = value.ToString();
            
            CurrentInfo.Instance.nextScoreText.text=value.ToString();

            AdaptSlider();
        } 
    }

    [SerializeField]public Text coinText;//总金币数

    public Text needCoinText;//抽卡需要的金币数
    private int drawNeedCoin;//抽一次卡需要的金币数
    public int DrawNeedCoin 
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

    private int minPlayCardCount=1;//最少出几张牌
    public int MinPlayCardCount { get => minPlayCardCount; set => minPlayCardCount = value; }
    
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
    private int dropCardCount;//丢弃的牌的数量
    public int DropCardCount 
    { 
        get => dropCardCount;
        set
        {
            dropCardCount = value;
            if( value % 3 == 0)
            {
                GameEntrance.instance.CoinCount += AddScoreWhenDeleteScore;
                if (AddScoreWhenDeleteScore != 0)
                {
                    ShowTipManager.instance.ShowTip($"弃字成金", AddScoreWhenDeleteScore);
                }
            }
        } 
    }

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


    private int tipWordCount;
    public int TipWordCount //单词提示的个数
    { 
        get => tipWordCount; 
        set
        {
            tipWordCount = value;
            tipWordCountText.text=value.ToString();
            tipWordButton.image.color = value ==0?Color.grey:new Color32(97,140,255,255);
        } 
    }

    private int doubleScoreRoundCount;//复活之后奖励的回合数
    public int DoubleScoreRoundCount { get => doubleScoreRoundCount; set => doubleScoreRoundCount = value; }

    //局内文本
    [SerializeField]private Text roundText;//显示回合数的Text
    [SerializeField]private Text scoreText;//显示当前分数的Text
    [SerializeField]private Slider scoreSlider;//显示当前分数的Text
    [SerializeField]private Text nextScoreText;//显示下一个目标分数的Text
    [SerializeField]private Text previousScoreText;//显示上一个目标分数的Text
    [SerializeField]private Text tipWordCountText;//显示单词提示的个数的Text


    //游戏成功和游戏失败面板
    [SerializeField] private Canvas gameFailCanvas;//

    //特殊牌介绍面板
    public Transform specialIntroductionPanel;//
    public Text specialCardNameText;//特殊牌名称Text
    public Text specialCardDescriptionText;//特殊牌描述Text
    public UnityAction useSpecialCard;//使用特殊牌
    public UnityAction sellSpecialCard;//出售特殊牌

    //删除字母牌提示
    public GameObject deleteTip;

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
        if (!director.playableGraph.IsValid())
            director.RebuildGraph();

        director.playableGraph.GetRootPlayable(0).SetSpeed(-2);
        director.time = director.duration - 0.001f;
        director.Play();

        // 启动协程监测倒放结束
        StartCoroutine(WaitForDirectorReverseEnd());
    }

    private IEnumerator WaitForDirectorReverseEnd()
    {
        // 等待直到 time <= 0 或 director 不再播放
        while (director.time > 0 && director.state == PlayState.Playing)
        {
            yield return null;
        }

        SendScore();
    }

    private void SendScore()
    {
        DOTween.To(() => CurrentScore,
            x => CurrentScore = x,
            NextScore,
            1.0f)
            .SetEase(Ease.Linear);

        ShowTipManager.instance.ShowTip("复活之后获取额外分数");

        //director.Stop(); // 可加可不加，已结束的话其实不影响
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartChallenge()
    {
        DeckManager.instance.Init();//初始化DeckManager
        DeckManager.instance.ClearHandCards();//清空手牌
        GameEntrance.instance.CoinCount = 10;//每局初始十个金币
        useTipWord = false;
        TipWordCount = 1;
        MaxPlayCardCount = 10;
        MaxSpecialCaradCount = 10;
        CurrentRound = 0;
        CurrentScore = 0;
        NextScore = 100;
        DrawNeedCoin = 1;
        MinPlayCardCount = 1;
        DropCardCount = 0;

        DoubleScoreRoundCount = 0;

        ContinuousProbability = 0.0f;
        ContinuousCount = 1;
        ExtraScoreOnlyOneScore = 0;
        AddScoreWhenDeleteScore = 0;
        ExtraScoreRounOverScore = 0;
        CanPlayZeroCardScore = 1;
        StartRound();//开始回合
    }


    [SerializeField] private PlayableDirector director;
    /// <summary>
    /// 挑战结束
    /// </summary>
    public void EndChallenge()
    {
        AudioManager.instance.PlaySoundEffect("Fail");
        //gameFailCanvas.enabled = true;
        director.Play();
        UploadPlayerInfo();
    }

    private void UploadPlayerInfo()
    {
        bool infoChanged = false;

        if (DataManager.instance.globalPlayerInfo.maxRound < currentRound)
        {
            DataManager.instance.globalPlayerInfo.maxRound = currentRound;
            infoChanged = true;
        }

        if (DataManager.instance.globalPlayerInfo.maxScore < currentScore)
        {
            DataManager.instance.globalPlayerInfo.maxScore = currentScore;
            infoChanged = true;
        }
        ///金币变为单局资源
        //if (GameEntrance.instance.CoinCount != DataManager.instance.globalPlayerInfo.coinCount)
        //{
        //    infoChanged = true;
        //    DataManager.instance.globalPlayerInfo.coinCount = (int)GameEntrance.instance.CoinCount;
        //}
        if (infoChanged)
        {
            DataManager.instance.UploadPlayerInfo();
        }
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
        AudioManager.instance.PlaySoundEffect("ClickButton");
        //DeckManager.instance.IsDrawing = true;
        //ContinuousCount = 2;

        if (!DeckManager.instance.CanDrawNormalCard())
        {
            ShowTipManager.instance.ShowTip("字母牌数达到上限，请及时出牌");
            DeckManager.instance.IsDrawing = false;
        }
        else
        {
            DeckManager.instance.DrawCard(ContinuousCount);//抽卡
            GameEntrance.instance.CoinCount -= DrawNeedCoin;
        }
    }

    /// <summary>
    /// 出牌
    /// </summary>
    public void PlayCard()
    {
        AudioManager.instance.PlaySoundEffect("GetScore");

        if (CacheText.instance.letterCards.Count < MinPlayCardCount)
        {
            ShowTipManager.instance.ShowTip("最少出一张牌");
            return;
        }

        ScoreCalculator.CalculateScore(CacheText.instance.letterCards);
        int normalScore = ScoreCalculator.normalScore;//基础分
        int extraScore = ScoreCalculator.extraScore;//额外分，如颜色相同，字母相同，组成单词
        int specialScore = ScoreCalculator.specialScore;//特殊分数
        ScoreCalculator.specialScore = 0;//特殊分数归0
        int totalRoundScore= normalScore+ extraScore+specialScore;

        if (DoubleScoreRoundCount > 0)
        {
            totalRoundScore *= 2;//奖励翻倍
            DoubleScoreRoundCount--;
            ShowTipManager.instance.ShowTip($"复活翻倍奖励,+{totalRoundScore/2}分,剩余{DoubleScoreRoundCount}回合");
        }

        CurrentScore += totalRoundScore;//当前总分数
        GameEntrance.instance.CoinCount+= totalRoundScore;//当前总金币

        CacheText.instance.ClearCacheCard();
        EndRound();//回合结束
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    private void StartRound()
    {
        UploadPlayerInfo();

        CurrentRound++;//回合数+1

        if(useTipWord)
        {
            useTipWord = false;
            TipWordCount--;
        }

        if (CurrentRound % 5 == 1) NextScore = TargetScore(CurrentRound);//每过5关设置一次目标分数

        DrawNeedCoin = (int)(CurrentRound/5.0f)+1;//抽取一次所需要的金币数量就是当前的回合数

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
        AudioManager.instance.PlaySoundEffect("ClickButton");
        GetRewards.Instance.GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// 使用特殊牌
    /// </summary>
    public void UseSpecialCard()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        useSpecialCard();
        CloseSpecialCardPanel();
    }

    /// <summary>
    /// 出售特殊牌
    /// </summary>
    public void SellSpecialCard()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        sellSpecialCard();
        CloseSpecialCardPanel();
    }

    /// <summary>
    /// 关闭当前面板按钮
    /// </summary>
    public void CloseSpecialCardPanel()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        specialIntroductionPanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuart);
    }

    /// <summary>
    /// 打开当前状态面板
    /// </summary>
    public void OpenCurrentInfoPanel()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        CurrentInfo.Instance.GetComponent<Canvas>().enabled = true;
        CurrentInfo.Instance.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuart);
    }

    /// <summary>
    /// 打开暂停面板
    /// </summary>
    public void PausePanel()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        CurrentInfo.Instance.GetComponent<Canvas>().enabled = true;
    }

    private int TargetScore(int round)
    {
        if (round<=0) return 0;
        int roundStep = (round-1) / 5 +1;
        return (int)(30*(1+(roundStep-1)*(roundStep-1)));
    }

    private void AdaptSlider()
    {
        if (CurrentRound == 0) return;
        int currentTargetScore = NextScore;
        int previousTargetScore = TargetScore(CurrentRound - 5);
        previousScoreText.text = previousTargetScore.ToString();

        float sliderValue= (float)(CurrentScore - previousTargetScore) / (currentTargetScore - previousTargetScore);
        scoreSlider.value = sliderValue;
        //Debug.Log($"CurrentScore - previousTargetScore is {CurrentScore - previousTargetScore},currentTargetScore - previousTargetScore is {currentTargetScore - previousTargetScore},sliderValue is {sliderValue}");
    }


    public GameObject questionPanel;//问号面板
    /// <summary>
    /// 打开问号面板
    /// </summary>
    public void OpenQuesPanel()
    {
        questionPanel.SetActive(true);
        if (tipTexts == null || tipTexts.Count == 0)
        {
            return;
        }
        tipText.text = tipTexts[Random.Range(0, tipTexts.Count)];
    }

    /// <summary>
    /// 关闭问号面板
    /// </summary>
    public void CloseQuesPanel()
    {
        questionPanel.SetActive(false);
    }

    private int imageGuidesIndex = 0;
    [SerializeField] private List<GameObject> imageGuides;//图文导航
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Text tipText;//问号面板里的提示文本
    [SerializeField] private List<string> tipTexts;//问号面板里的提示文本的数组



    /// <summary>
    /// 问号面板的下一个按钮
    /// </summary>
    public void NextGuide()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        imageGuidesIndex++;
        if (imageGuidesIndex >= imageGuides.Count - 1)
        {
            nextButton.interactable = false;
        }
        previousButton.interactable = true;

        imageGuides[imageGuidesIndex - 1].GetComponent<RectTransform>().DOAnchorPosX(-1000, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex - 1].GetComponent<RectTransform>().DOScale(0, 0.5f).SetEase(Ease.OutQuad);

        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 0);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(0, 0.01f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(1, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuad);

        if (tipTexts == null || tipTexts.Count == 0)
        {
            return;
        }
        tipText.text = tipTexts[Random.Range(0, tipTexts.Count)];
    }

    /// <summary>
    /// 问号面板的上一个按钮
    /// </summary>
    public void PreciousGuide()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        imageGuidesIndex--;
        if (imageGuidesIndex <= 0)
        {
            previousButton.interactable = false;
        }
        nextButton.interactable = true;

        imageGuides[imageGuidesIndex + 1].GetComponent<RectTransform>().DOAnchorPosX(1000, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex + 1].GetComponent<RectTransform>().DOScale(0, 0.5f).SetEase(Ease.OutQuad);

        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000, 0);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(0, 0.01f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(1, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuad);

        if (tipTexts == null || tipTexts.Count == 0)
        {
            return;
        }
        tipText.text = tipTexts[Random.Range(0, tipTexts.Count)];
    }


    [SerializeField] private Button tipWordButton;//显示单词提示的个数的Text
    [SerializeField] private GameObject getTipWordCanvas;//显示单词提示的个数的Text
    [SerializeField] private GameObject tipWordShowPanel;//显示单词提示
    private bool useTipWord = false;
    /// <summary>
    /// 提示按钮
    /// </summary>
    public void TipWordButton()
    {
        if (TipWordCount == 0)
        {
            getTipWordCanvas.SetActive(true);
        }
        else
        {
            List<char> availableLetters = DeckManager.instance.letterHandCards.OfType<LetterCard>()               // 安全转换为 LetterCard 类型
                                        .Select(card => card.Letter)        // 提取 Letter 属性
                                        .ToList();                          // 转为 List<char>
            List<string> matched = WordFinder.instance.FindAllWords(availableLetters, WordChecker.Instance.wordList.Words);

            if (matched.Count == 0)
            {
                ShowTipManager.instance.ShowTip("当前无法组成单词");
            }
            else
            {
                tipWordShowPanel.SetActive(true);
                useTipWord = true;
            }
        }
    }
}

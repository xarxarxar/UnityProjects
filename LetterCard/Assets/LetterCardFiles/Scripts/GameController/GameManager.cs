using DG.Tweening;
using UnityEngine;
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
    public int CurrentScore { get => currentScore; set { currentScore = value; scoreText.text = value.ToString(); } }
    private uint nextScore;//下一目标分数
    public uint NextScore { get => nextScore; set { nextScore = value; nextScoreText.text =$"目标分数:{value}"; } }


    private uint drawNeedCoin;//抽一次卡需要的金币数
    public uint DrawNeedCoin { get => drawNeedCoin; set => drawNeedCoin = value; }


    //局内文本
    [SerializeField]private Text roundText;//显示回合数的Text
    [SerializeField]private Text scoreText;//显示当前分数的Text
    [SerializeField]private Text nextScoreText;//显示下一个目标分数的Text

    //局内UI
    [SerializeField] private GameObject getCoinPanel;//获取金币的panel

    //游戏成功和游戏失败面板
    [SerializeField] private Canvas gameSuccessCanvas;//
    [SerializeField] private Canvas gameFailCanvas;//

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
#endif
    }

    public void RestartChallenge()
    {
        DeckManager.instance.Init();//初始化DeckManager
        DeckManager.instance.ClearHandCards();//清空手牌
        CurrentRound = 0;
        CurrentScore = 0;
        NextScore = 100;
        StartRound();//开始回合
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartChallenge()
    {
        DeckManager.instance.Init();//初始化DeckManager
        StartRound();//开始回合
    }

    /// <summary>
    /// 挑战结束
    /// </summary>
    public void EndChallenge()
    {
        ShowTipManager.instance.ShowTip("挑战失败");
        gameFailCanvas.enabled = true;

    }

    /// <summary>
    /// 抽卡
    /// </summary>
    public void DrawCards()
    {
        if(GameEntrance.CoinCount< DrawNeedCoin)
        {
            getCoinPanel.transform.DOScale(Vector3.one, 0.1f);//打开获取金币面板
            return;
        }
        DeckManager.instance.DrawLetterCard(1);
    }

    /// <summary>
    /// 出牌
    /// </summary>
    public void PlayCard()
    {
        AudioManager.instance.PlaySoundEffect("PlayCard");

        int normalScore = 0;//基础分
        int extraScore = 0;//额外分，如颜色相同，字母相同，组成单词
        int specialScore = 0;//特殊分数
        Debug.Log($"normalScore01 is{normalScore}");
        ScoreCalculator.CalculateScore(CacheText.instance.letterCards,ref normalScore,ref extraScore,ref specialScore);
        Debug.Log($"normalScore02 is{normalScore}");
        int totalRoundScore= normalScore+ extraScore+specialScore;

        

        CurrentScore += totalRoundScore;//当前总分数

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

        DrawNeedCoin = (uint)CurrentRound;//抽取一次所需要的金币数量就是当前的回合数

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



}

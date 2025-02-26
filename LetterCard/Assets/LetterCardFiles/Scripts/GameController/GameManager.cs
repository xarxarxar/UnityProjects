using System.Collections;
using System.Collections.Generic;
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
    public int CurrentRound { get => currentRound; set { currentRound = value; roundText.text = $"当前回合:{value}"; } }
    private int currentScore;//当前总分数
    public int CurrentScore { get => currentScore; set { currentScore = value; scoreText.text = value.ToString(); } }
    private int nextScore;//下一目标分数
    public int NextScore { get => nextScore; set { nextScore = value; nextScoreText.text =$"目标分数:{value}"; } }


    //局内文本
    [SerializeField]private Text roundText;//显示回合数的Text
    [SerializeField]private Text scoreText;//显示当前分数的Text
    [SerializeField]private Text nextScoreText;//显示下一个目标分数的Text


    private void Awake()
    {
        Instance = this;
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
    /// 抽卡
    /// </summary>
    public void DrawCards()
    {
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
        ScoreCalculator.CalculateScore(CacheText.instance.letterCards,ref normalScore,ref extraScore,ref specialScore);

        int totalRoundScore= normalScore+ extraScore+specialScore;

        //销毁暂存池中的所有物体
        foreach (LetterCard child in CacheText.instance.letterCards)
        {
            DeckManager.instance.cardPool.ReturnCard(child);
            DeckManager.instance.letterHandCards.Remove(child);//从手牌中移出
        }

        CurrentScore += totalRoundScore;//当前总分数

        CacheText.ClearTextShow();
        StartRound();
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    private void StartRound()
    {
        CurrentRound++;
    }


    /// <summary>
    /// 结束当前回合
    /// </summary>
    private void EndRound()
    {
        
    }

}

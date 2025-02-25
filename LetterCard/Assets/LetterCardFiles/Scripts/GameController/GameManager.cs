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
    public int CurrentRound { get => currentRound; set { currentRound = value; roundText.text = value.ToString(); } }
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
        InitText();//初始化文本
        InitValue();//初始化数值
        DeckManager.instance.StartLevel();
    }

    public void DrawCards()
    {
        DeckManager.instance.DrawCards(1,1);
    }

    //初始化文本
    private void InitText()
    {
        roundText.text = "1";
        scoreText.text = "0";
        nextScoreText.text = $"下一目标分数：{NextScore}";
    }

    //初始化数值
    private void InitValue()
    {
        CurrentRound = 1;
        CurrentScore = 0;
        NextScore = 0;
    }

}

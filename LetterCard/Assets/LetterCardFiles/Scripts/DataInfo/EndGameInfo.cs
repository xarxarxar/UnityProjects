using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameInfo
{
    //当前回合数
    public int currentRound;

    // 当前分数
    public int currentScore;

    // 当前字母牌手牌
    public List<LetterCard> currentLetterHandCard=new List<LetterCard>();

    // 当前功能牌手牌
    public List<FunctionCard> currentFunctionHandCard=new List<FunctionCard>();



    public EndGameInfo()
    {
        
    }

}

public class PlayerChallenge
{
    public PlayerChallenge()
    {

    }
}

using System;
using System.Collections.Generic;

[Serializable]
public class EndGameInfo
{
    /// <summary>
    /// 玩家剩余的挑战信息
    /// </summary>

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
        currentRound = 0;
        currentScore = 0;
    }

}

public class PlayerChallenge
{
    public int playerChallengeId;//玩家挑战的本局挑战ID

    public PlayerChallenge()
    {
        playerChallengeId = 489635;
    }
}

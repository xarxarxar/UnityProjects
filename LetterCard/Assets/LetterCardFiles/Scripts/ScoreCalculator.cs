using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class ScoreCalculator
{
    // 定义有效的字母组合列表，玩家出牌时如果能够组成这些字母组合，则会获得额外的分数奖励。
    // 这里是一些示例组合，你可以根据游戏规则扩展更多组合。
    private static List<string> validCombinations = new() {
        "hello", "world", "unity", "game","ab","bc" // 示例组合
    };

    // 字典：MissionType 到返回 int 的委托映射
    private static Dictionary<MissionType, System.Func<SpecialMission,List<LetterCard>, int>> missionActions = new Dictionary<MissionType, System.Func<SpecialMission, List<LetterCard>, int>>();

    /// <summary>
    /// 存在指定字母组合
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    int SpecificCombinationMission(SpecialMission mission,List<LetterCard> cards)
    {
        int score = 0;
        string sequence = BuildLetterSequence(cards);
        string targetString = mission.targetLetters;
        if (sequence.Contains(targetString))
        {
            score += targetString.Length;
        }
        return score;
    }


    // 计算玩家出牌后的总分数
    // 输入参数 playedCards 是玩家在当前回合打出的卡牌列表
    // 返回值是玩家在当前回合的得分
    public static  int CalculateScore(List<LetterCard> playedCards)
    {
        
        // 初始分数设为玩家出牌数量，即每张牌得1分
        int baseScore = playedCards.Count;

        // 构建一个由打出的字母卡牌组成的字母序列，用于后续检测是否有有效的字母组合
        string sequence = BuildLetterSequence( playedCards);
        int sequenceCount = sequence.Length;//该字符串的长度
        //如果在六级单词表里，那么获取额外分数
        int wordScore = WordScore(WordChecker.Instance.IsWordInList(sequence.ToLower()), sequence);

        // 遍历所有有效的字母组合，检查当前字母序列中是否包含这些组合
        foreach (var combo in validCombinations)
        {
            // 如果字母序列中包含该组合，则奖励额外分数
            if (sequence.Contains(combo))
            {
                // 每个有效组合的长度乘以2，作为奖励分数加到基本分数上
                baseScore += combo.Length * 2; // 示例奖励规则：每个组合的长度*2
            }
        }
        Debug.Log($"baseScore为{baseScore}，wordScore为{wordScore}");
        // 最后加上通过特殊任务（CheckSpecialMissions）获得的分数
        return baseScore + CheckSpecialMissions(LevelController.instance.levelConfig.specialMissions, playedCards)+ wordScore;
    }

    // 根据玩家出牌的卡牌，构建一个由字母组成的字符串序列
    // 例如，如果出牌是 "a", "b", "c"，则返回的字符串为 "abc"
    private static string BuildLetterSequence(List<LetterCard> cards)
    {
        // 初始化一个空字符串来存储字母序列
        StringBuilder sb = new StringBuilder();

        // 遍历 List 中的每个字母
        foreach (LetterCard card in cards)
        {
            sb.Append(card.Letter);  // 将每个 LetterCard 的字母追加到 StringBuilder
        }
        return sb.ToString();  // 返回拼接后的字符串
    }

    // 检查特殊任务的完成情况并返回额外的分数
    // 该方法可以根据任务的具体规则来调整，这里只是一个示例
    private static int CheckSpecialMissions(List<SpecialMission> specialMissions, List<LetterCard> playedCards)
    {
        int specialScore = 0;
       for(int i = 0; i < specialMissions.Count; i++)
        {
            specialScore += missionActions[specialMissions[i].missionType](specialMissions[i],playedCards);
        }

        // 返回根据特殊任务获得的奖励分数
        return specialScore;
    }

    /// <summary>
    /// 如果字母序列在单词表中，那么获取的额外分数
    /// </summary>
    /// <param name="isInWordList">是否在单词表中</param>
    /// <param name="sequence">字母序列</param>
    /// <returns></returns>
    private static int WordScore(bool isInWordList,string sequence)
    {
        if (isInWordList) Debug.Log($"{sequence}在单词表中");
        else Debug.Log($"{sequence}不在单词表中");
        return isInWordList ? sequence.Length*2 : 0;
    }
}


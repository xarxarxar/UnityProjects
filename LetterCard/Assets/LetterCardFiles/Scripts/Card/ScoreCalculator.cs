using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;


public class ScoreCalculator
{
    public static Dictionary<string, UnityAction> specialScoreDic = new Dictionary<string, UnityAction>();

    public static int normalScore;
    public static int extraScore;
    public static int specialScore;

    /// <summary>
    /// 存在指定字母组合
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    static int SpecificMissionScore(SpecialMission mission,List<LetterCard> cards)
    {
        int score = 0;
        string sequence = BuildLetterSequence(cards);
        switch (mission.missionType)
        {
            ///只要包含特殊字符串就可以
            case MissionType.SpecificCombination:
                string targetString = mission.targetLetters;
                if (sequence.Contains(targetString))
                {
                    score = targetString.Length;//额外奖励卡牌数量的分数
                }
                ShowTipManager.instance.ShowTip($"特定组合+{score}分");
                return score;
            ///TODO 所有字符必须是包含特定颜色
            case MissionType.SpecificColor:

                break;
            /// TODO
            case MissionType.MixLetterAndColor:

                break;
            /// TODO
            case MissionType.WordDictionary:
                break;
        }
        return score;
    }


    // 计算玩家出牌后的总分数
    // 输入参数 playedCards 是玩家在当前回合打出的卡牌列表
    // 返回值是玩家在当前回合的得分
    public static void CalculateScore(List<LetterCard> playedCards)
    {
        // 初始分数设为玩家出牌数量，即每张牌得1分
        normalScore = playedCards.Count;

        // 构建一个由打出的字母卡牌组成的字母序列，用于后续检测是否有有效的字母组合
        string sequence = BuildLetterSequence( playedCards);
        string colorSequence=BuildColorSequence( playedCards);

        ShowTipManager.instance.ShowTip($"基础+{normalScore}分");

        extraScore = GetRuleScore(sequence, colorSequence);
        //int specialScore = CheckSpecialMissions(LevelController.instance.levelConfig.specialMissions, playedCards);

        //计算specialScore
        foreach (UnityAction value in specialScoreDic.Values)
        {
            value();
        }
        
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

    // 根据玩家出牌的卡牌，构建一个由颜色字母组成的字符串序列
    // 例如，如果出牌是 "R", "G", "B"，则返回的字符串为 "RGB"
    private static string BuildColorSequence(List<LetterCard> cards)
    {
        // 初始化一个空字符串来存储字母序列
        StringBuilder sb = new StringBuilder();

        // 遍历 List 中的每个字母
        foreach (LetterCard card in cards)
        {
            sb.Append(card.Color);  // 将每个 LetterCard 的字母追加到 StringBuilder
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
            specialScore += SpecificMissionScore(specialMissions[i],playedCards);
        }

        // 返回根据特殊任务获得的奖励分数
        return specialScore;
    }


    /// <summary>
    /// 获取规则之内的分数，如同字符，同颜色，相邻字符，字符组成了单词等等
    /// </summary>
    /// <param name="sequence">字符序列</param>
    /// <param name="colorSequence">颜色序列</param>
    /// <returns></returns>
    private static int GetRuleScore(string sequence,string colorSequence)
    {
        int score = SameLetter(sequence)+ SameColor(colorSequence)+ SortByAlphabet(sequence)+ WordScore(sequence);
        return score;
    }

    /// <summary>
    /// 如果字母序列在单词表中，那么获取的额外分数，额外获取卡牌数量*2的分数,2025-04-18改成获取卡牌数量*3的分数
    /// </summary>
    /// <param name="sequence">字母序列</param>
    /// <returns></returns>
    private static int WordScore(string sequence)
    {
        bool isInWordList=WordChecker.Instance.IsWordInList(sequence.ToLower());
        if (isInWordList)
        {
            ShowTipManager.instance.ShowTip($"组成单词+{sequence.Length * 3}分");
            return sequence.Length * 3;
        }
        
        return 0;
    }

    /// <summary>
    /// 如果所有的字符都是相同字符，那么额外获取卡牌数量*1倍的分数，三张卡牌起步
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private static int SameLetter(string sequence)
    {
        if (sequence.Length < 3) return 0;
        sequence=sequence.ToLower();
        // 获取第一个字符
        char firstChar = sequence[0];

        // 从第二个字符开始检查是否与第一个字符相同
        for (int i = 1; i < sequence.Length; i++)
        {
            if (sequence[i] != firstChar)
            {
                return 0;  // 如果有不同的字符，返回 false
            }
        }

        ShowTipManager.instance.ShowTip($"相同字母+{sequence.Length}分");
        return sequence.Length;

    }

    /// <summary>
    /// 如果所有的卡牌都是相同颜色，额外获取卡牌数量*1倍的分数，三张卡牌起步，和SameLetter的逻辑一模一样
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private static int SameColor(string sequence)
    {
        if (sequence.Length < 3) return 0;
        // 获取第一个字符
        char firstChar = sequence[0];

        // 从第二个字符开始检查是否与第一个字符相同
        for (int i = 1; i < sequence.Length; i++)
        {
            if (sequence[i] != firstChar)
            {
                return 0;  // 如果有不同的字符，返回 false
            }
        }

        ShowTipManager.instance.ShowTip($"相同颜色+{sequence.Length}分");
        return sequence.Length;
    }

    /// <summary>
    /// 字母是按照字母表的顺序的，额外获取卡牌数量*1倍的分数，三张卡牌起步
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private static int SortByAlphabet(string sequence)
    {
        if(sequence.Length < 3) return 0;

        // 将字符串转换为小写，忽略大小写
        sequence = sequence.ToLower();

        // 遍历字符串，确保每个字符与前一个字符的差值为 1
        for (int i = 1; i < sequence.Length; i++)
        {
            if (sequence[i] != sequence[i - 1] + 1)  // 检查字符的 ASCII 值是否连续
            {
                return 0;
            }
        }
        ShowTipManager.instance.ShowTip($"顺序排列+{sequence.Length}分");
        return sequence.Length;
    }
}


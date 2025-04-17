using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 表示一个字母及其颜色
public class Letter
{
    public char Character;  // 字符，如 'A'、'B' 等
    public Color Color;     // Unity 的颜色类型
}

// 用于检查可以组成的最长单词
public class WordFinder : MonoBehaviour
{
    public static WordFinder instance;

    private void Awake()
    {
        instance = this;
    }
    public WordList sixWordList;
    void Start()
    {
        // 构建示例字母池
        List<char> availableLetters = new List<char> { 'h', 'e', 'l', 'l', 'o', 'w', 'r', 'd' };

        // 假设 sixWordList.Words 已经是 HashSet<string>
        HashSet<string> wordSet = sixWordList.Words;


        // 调用 FindAllWords
        List<string> matched = FindAllWords(availableLetters, wordSet);

        // 输出结果
        if (matched.Count > 0)
        {
            Debug.Log("可组成的单词： " + string.Join(", ", matched));
        }
        else
        {
            Debug.Log("没有可组成的单词");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // 构建一个示例字母池
            List<Letter> availableLetters = new List<Letter>
        {
            new Letter { Character = 'h', Color = Color.red },
            new Letter { Character = 'e', Color = Color.green },
            new Letter { Character = 'l', Color = Color.blue },
            new Letter { Character = 'l', Color = Color.yellow },
            new Letter { Character = 'o', Color = Color.red },
            new Letter { Character = 'w', Color = Color.red },
            new Letter { Character = 'r', Color = Color.red },
            new Letter { Character = 'l', Color = Color.red },
            new Letter { Character = 'd', Color = Color.red },
            new Letter { Character = 'a', Color = Color.red },
            new Letter { Character = 'b', Color = Color.black },
            new Letter { Character = 'u', Color = Color.red },
            new Letter { Character = 'n', Color = Color.gray },
            new Letter { Character = 'd', Color = Color.red },
            new Letter { Character = 'a', Color = Color.red },
            new Letter { Character = 'n', Color = Color.yellow },
            new Letter { Character = 'c', Color = Color.red },
        };

            // 构建词库（HashSet 更快，防止重复）
            HashSet<string> wordSet = sixWordList.Words;


            List<Letter> best = CheckHighestScoreWord(availableLetters, wordSet);

            // 4. 输出结果
            if (best != null)
            {
                string word = string.Concat(best.Select(l => l.Character));
                Debug.Log($"得分最高的单词：{word}");
            }
            else
            {
                Debug.Log("无法组成任何单词");
            }
        }
    }

    /// <summary>
    /// 在可用字母池中，找出能组成的单词，并根据规则打分，返回最高分的 Letter 列表
    /// </summary>
    /// <param name="availableLetters">可用的字母池（含颜色）</param>
    /// <param name="wordSet">单词词库（HashSet，自动去重，查找更快）</param>
    /// <returns>最高分单词对应的 Letter 列表；若无法组成任何单词则返回 null</returns>
    public List<Letter> CheckHighestScoreWord(List<Letter> availableLetters, HashSet<string> wordSet)
    {
        // 预处理：统计每个字符在可用字母池中的数量
        var availableCharCounts = availableLetters
            .GroupBy(l => l.Character)
            .ToDictionary(g => g.Key, g => g.Count());

        // 预处理：按颜色分组，统计每种颜色下每个字符的数量
        var colorGroups = availableLetters
            .GroupBy(l => l.Color)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(l => l.Character)
                      .ToDictionary(cc => cc.Key, cc => cc.Count())
            );

        Candidate best = null;

        // 遍历词库中所有单词
        foreach (string word in wordSet)
        {
            int len = word.Length;
            // 如果单词长度超过可用字母总数，则必然无法组成，跳过
            if (len > availableLetters.Count)
                continue;

            // 统计该单词所需的每个字符的数量
            var wordReq = word
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            // 必须能组成单词才能继续，否则跳过
            bool canForm = wordReq.All(kv =>
                availableCharCounts.ContainsKey(kv.Key) &&
                availableCharCounts[kv.Key] >= kv.Value);
            if (!canForm)
                continue;

            // 检查是否存在某种颜色可以同色完整组成该单词
            bool sameColor = false;
            Color matchingColor = default;
            foreach (var kv in colorGroups)
            {
                var charCountInColor = kv.Value;
                if (wordReq.All(r => charCountInColor.ContainsKey(r.Key) && charCountInColor[r.Key] >= r.Value))
                {
                    sameColor = true;
                    matchingColor = kv.Key;
                    break;
                }
            }

            // 按“同色优先、否则无色”策略从 availableLetters 中抽取具体 Letter 实例
            List<Letter> temp = new List<Letter>(availableLetters);
            var letters = new List<Letter>();
            foreach (char c in word)
            {
                Letter found;
                if (sameColor)
                {
                    // 如果可以同色组成，则必须从 matchingColor 组里抽
                    found = temp.FirstOrDefault(l => l.Character == c && l.Color == matchingColor);
                }
                else
                {
                    // 否则任意颜色都可
                    found = temp.FirstOrDefault(l => l.Character == c);
                }

                if (found == null)
                {
                    letters = null;
                    break;
                }
                letters.Add(found);
                temp.Remove(found);
            }
            if (letters == null || letters.Count != len)
                continue;  // 保险起见，若抽取失败则跳过

            // 计算得分：基础分 + 各种额外分
            int score = len;  // 基础分：单词长度 * 1

            // 只有长度 ≥ 3 时，额外规则才生效
            if (len >= 3)
            {
                // 同色加分：字母数量 * 1
                if (sameColor)
                    score += len;

                // 同字母加分：字母数量 * 1
                if (word.All(c => c == word[0]))
                    score += len;

                // 连续字母加分：字母数量 * 1
                var distinctSorted = word.Distinct().OrderBy(c => c).ToList();
                if (IsConsecutive(distinctSorted))
                    score += len;
            }

            // 记录最优：优先分数高，其次字母数多
            if (best == null
                || score > best.Score
                || (score == best.Score && len > best.Letters.Count))
            {
                best = new Candidate(letters, score);
            }
        }

        // 返回最高分对应的 Letter 列表，若 best 为 null 则直接返回 null
        return best?.Letters;
    }

    /// <summary>
    /// 判断字符列表在字母表上是否连续（如 A-B-C）
    /// </summary>
    private bool IsConsecutive(List<char> chars)
    {
        if (chars.Count < 2) return false;
        for (int i = 1; i < chars.Count; i++)
            if (chars[i] - chars[i - 1] != 1)
                return false;
        return true;
    }

    // 内部候选类型，用于记录一组 Letter 及其得分
    private class Candidate
    {
        public List<Letter> Letters { get; }
        public int Score { get; }
        public Candidate(List<Letter> letters, int score)
        {
            Letters = letters;
            Score = score;
        }
    }


    /// <summary>
    /// 找出所有能由 availableLetters 组成的单词（不消耗字母，可跨单词复用）
    /// </summary>
    /// <param name="availableLetters">可用的字母池（只包含字符，不含颜色）</param>
    /// <param name="wordSet">单词词库（HashSet，自动去重）</param>
    /// <returns>所有能组成的单词列表，若无则返回空列表</returns>
    public List<string> FindAllWords(List<char> availableLetters, HashSet<string> wordSet)
    {
        // 1. 统计每个可用字母的数量
        var availableCharCounts = availableLetters
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());

        var results = new List<string>();

        // 2. 遍历词库
        foreach (var word in wordSet)
        {
            // 2.1 统计该单词需要的字符及数量
            var wordReq = word
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            // 2.2 判断是否可以组成该单词
            bool canForm = wordReq.All(kv =>
                availableCharCounts.ContainsKey(kv.Key) &&
                availableCharCounts[kv.Key] >= kv.Value
            );

            // 2.3 可以组成则加入结果
            if (canForm)
                results.Add(word);
        }

        return results;
    }

}

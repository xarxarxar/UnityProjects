using System.Collections;
using System;
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
    /// 协程：找出所有能由 availableLetters 组成的单词（不消耗字母，可跨单词复用）
    /// 每帧检查一定数量的词，避免主线程卡顿
    /// </summary>
    public IEnumerator FindAllWordsCoroutine(
        List<char> availableLetters,
        HashSet<string> wordSet,
        Action<List<string>> onComplete,
        int wordsPerFrame = 500)
    {
        if (availableLetters == null || wordSet == null)
        {
            onComplete?.Invoke(new List<string>());
            yield break;
        }

        // 1. 统计每个可用字母的数量（非 LINQ）
        var availableCharCounts = new Dictionary<char, int>();
        foreach (char c in availableLetters)
        {
            if (!availableCharCounts.TryAdd(c, 1))
                availableCharCounts[c]++;
        }

        var results = new List<string>();
        int count = 0;

        foreach (var word in wordSet)
        {
            // 优化 1：长度比可用字母还长，跳过
            if (word.Length > availableLetters.Count)
                continue;

            // 优化 2：含有不可用字母，跳过
            if (word.Any(c => !availableCharCounts.ContainsKey(c)))
                continue;

            // 2.1 统计单词所需字符（非 LINQ）
            var wordReq = new Dictionary<char, int>();
            foreach (char c in word)
            {
                if (!wordReq.TryAdd(c, 1))
                    wordReq[c]++;
            }

            // 2.2 判断是否可以组成
            bool canForm = true;
            foreach (var kv in wordReq)
            {
                if (!availableCharCounts.TryGetValue(kv.Key, out int countAvailable) || countAvailable < kv.Value)
                {
                    canForm = false;
                    break;
                }
            }

            if (canForm)
                results.Add(word);

            count++;
            if (count % wordsPerFrame == 0)
                yield return null;
        }

        // 4. 回调结果
        onComplete?.Invoke(results);
    }

}

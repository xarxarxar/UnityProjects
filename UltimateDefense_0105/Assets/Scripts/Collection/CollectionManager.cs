using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : ManagerBase<CollectionManager>
{
    // 每月的字符集（1 月 ～ 12 月）
    public static readonly string Chars = "床前明月光疑是地上霜举头望明月低头思故乡";

    // index → Color32
    private Dictionary<int, Color32> indexColorMap = new Dictionary<int, Color32>();

    // 可用颜色（重复字符分配）
    private Color32[] colors =
    {
        new Color32(255, 170, 170, 255), // 柔红（Pastel Red）
        new Color32(180, 255, 180, 255), // 柔绿（Pastel Green）
        new Color32(170, 200, 255, 255), // 天空蓝（Soft Sky Blue）
        new Color32(255, 245, 170, 255), // 柔黄（Pastel Yellow）
        new Color32(220, 180, 255, 255), // 浅紫（Lavender）
    };

    private static readonly Color32 COLOR_WHITE = new Color32(255, 255, 255, 255);

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;
        Index = 3;
    }

    public override void Init()
    {
        // 统计字符出现的 index 列表
        Dictionary<char, List<int>> occurrences = new Dictionary<char, List<int>>();

        for (int i = 0; i < Chars.Length; i++)
        {
            char c = Chars[i];
            if (!occurrences.ContainsKey(c))
                occurrences[c] = new List<int>();
            occurrences[c].Add(i);
        }

        // 给每个 index 分配颜色
        // 给每个 index 分配颜色（第一次出现白色，第二次开始才使用颜色）
        foreach (var kv in occurrences)
        {
            List<int> idxList = kv.Value;

            for (int i = 0; i < idxList.Count; i++)
            {
                if (i == 0)
                {
                    // 第一次出现：永远是白色
                    indexColorMap[idxList[i]] = COLOR_WHITE;
                }
                else
                {
                    // 第二次及之后：使用柔和颜色
                    indexColorMap[idxList[i]] = colors[(i - 1) % colors.Length];
                }
            }
        }

        Enemy.OnEnemyDie += OnEnemyDie;
        BattleManager.OnEndBattle += OnEndBattle;
        Debug.Log($"indexColorMap is {indexColorMap}");
    }

    // 获取一个随机 index
    public static int GetRandomCharacter()
    {
        if (string.IsNullOrEmpty(Chars)) return -1;
        return Random.Range(0, Chars.Length);
    }

    /// <summary>
    /// 返回 index 对应的 Color32。
    /// </summary>
    public Color32 GetColorByIndex(int index)
    {
        if (indexColorMap.TryGetValue(index, out Color32 color))
            return color;

        return COLOR_WHITE;
    }

    private void OnEnemyDie(Enemy enemy)
    {
        int signleTotlaCollection = 0;//单局获得的收藏数
        foreach(var kv in BattleManager.Instance.CollectionSignleBattle)
        {
            signleTotlaCollection += kv.Value;
        }

        if (signleTotlaCollection >= 3) return;

        float probability = 0.001f;

        probability += DataManager.Instance.PlayerInfo.TotalPassCount * 0.001f;
        probability += WaveManager.Instance.CurrentRound * 0.0003f;
        probability = Mathf.Min(probability, 0.015f);
        probability += DataManager.Instance.PlayerInfo.BattleCountNoCollect * 0.0005f;

        if (Random.value < probability)
        {
            int idx = GetRandomCharacter();
            DataManager.Instance.PlayerInfo.SetBattleCountNoCollect(0);
            Vector3 screenStart = Camera.main.WorldToScreenPoint(enemy.transform.position);
            GameUIManager.Instance.PlayCollectionAnim(idx,screenStart);
            if (!BattleManager.Instance.CollectionSignleBattle.ContainsKey(idx))
            {
                BattleManager.Instance.CollectionSignleBattle[idx] = 0;
            }
            BattleManager.Instance.CollectionSignleBattle[idx]++;
            AudioManager.Instance.PlaySFX("升级");
            AudioManager.Instance.Vibrate("heavy");//重震动
            Debug.Log($"触发成功！得到字符 index = {idx}, 字符 = {Chars[idx]}");
        }
    }

    private void OnEndBattle(bool isSuccess)
    {
        foreach (var kv in BattleManager.Instance.CollectionSignleBattle)
        {
            int oldValue = 0;

            if (!DataManager.Instance.PlayerInfo.MonthlyCollection
                .TryGetValue(kv.Key, out oldValue))
            {
                oldValue = 0;
            }

            DataManager.Instance.PlayerInfo.SetMonthlyCollection(
                kv.Key,
                oldValue + kv.Value
            );
        }
    }
}

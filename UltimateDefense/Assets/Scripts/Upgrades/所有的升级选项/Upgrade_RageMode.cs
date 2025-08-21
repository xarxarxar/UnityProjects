using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 进入狂暴模式，攻速临时翻倍
/// </summary>
public class Upgrade_RageMode : UpgradeBase
{
    #region 私有字段
    private float duration;//持续的时长

    #endregion
    /// <summary>
    /// 时长
    /// </summary>
    public float Duration { get => duration; }
    #region 构造函数

    /// <summary>
    /// 构造新的升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_RageMode(float dur, int cost)
    {
        duration = dur;
        UpgradeID = $"RageMode{duration}";
        Cost = cost;
        Description = $"攻速变为当前300%，持续{duration}秒";
    }

    public static Upgrade_RageMode CreateDynamicUpgrade()
    {
        float[] durations = { 10, 15, 20 };
        float bonus = durations[Random.Range(0, durations.Length)];
        int cost = Mathf.RoundToInt(bonus * (3 + UpgradeManager.Instance.RefreshCount.Value * 1));
        return new Upgrade_RageMode(bonus, cost);
    }

    #endregion

    /// <summary>
    /// 应用本次升级效果：对当前场上所有塔的 BaseAttack 增加 _attackBonus，
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        TowerManager.Instance.SetTmpAtkRate(3.0f, duration);
    }
}

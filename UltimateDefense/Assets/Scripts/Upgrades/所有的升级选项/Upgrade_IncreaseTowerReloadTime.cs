using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// “增加所有炮塔攻击力”升级，购买后立即对全场所有塔生效
/// </summary>
public class Upgrade_IncreaseTowerReloadTime : UpgradeBase
{
    #region 私有字段

    private float _bonus;    // 本次升级带来的提升

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取此次带来的提升
    /// </summary>
    public float Bonus => _bonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“减少换弹时长”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseTowerReloadTime(float bonus, int cost)
    {
        _bonus = bonus;
        UpgradeID = $"IncreaseTowerReloadTime{_bonus}";
        Cost = cost;
        Description = $"炮塔换弹时长 -{bonus}秒";
    }

    public static Upgrade_IncreaseTowerReloadTime CreateDynamicUpgrade()
    {
        float[] values = { 0.1f, 0.15f, 0.2f };
        float bonus = values[Random.Range(0, values.Length)];
        int cost = Mathf.RoundToInt(bonus*(200 + UpgradeManager.Instance.RefreshCount.Value * 70));
        return new Upgrade_IncreaseTowerReloadTime(bonus, cost);
    }

    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return TowerManager.Instance.BonusReload.Value > 1.0f;
    }

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BonusReload.Value -= _bonus;
    }

    #endregion
}

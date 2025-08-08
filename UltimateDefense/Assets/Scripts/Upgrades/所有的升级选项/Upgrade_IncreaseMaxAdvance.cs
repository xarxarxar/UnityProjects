using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 增加银行贷款上限
/// </summary>
public class Upgrade_IncreaseMaxAdvance : UpgradeBase
{
    #region 私有字段

    private int _bonus;    // 本次升级带来的提升

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取此次带来的提升
    /// </summary>
    public int Bonus => _bonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“减少换弹时长”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseMaxAdvance(int bonus, int cost)
    {
        UpgradeID = "IncreaseMaxAdvance";
        _bonus = bonus;
        Cost = cost;
        Description = $"贷款上限+{_bonus}";
    }

    public static Upgrade_IncreaseMaxAdvance CreateDynamicUpgrade()
    {
        int[] bounses = { 100, 200, 300 };
        int bonus = bounses[Random.Range(0, bounses.Length)];
        int cost = bonus*2;
        return new Upgrade_IncreaseMaxAdvance(bonus, cost);
    }
    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return BankManager.Instance.MaxAdvance.Value < 5000;
    }

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        BankManager.Instance.MaxAdvance.Value += _bonus;
    }

    #endregion
}

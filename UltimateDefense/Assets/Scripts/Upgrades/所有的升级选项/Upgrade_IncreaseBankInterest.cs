using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 增加银行利息
/// </summary>
public class Upgrade_IncreaseBankInterest : UpgradeBase
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
    public Upgrade_IncreaseBankInterest(float bonus, int cost)
    {
        UpgradeID = "IncreaseTowerReloadTime";
        _bonus = bonus;
        Cost = cost;
        Description = $"存款利息 +{_bonus*100}%";
    }

    public static Upgrade_IncreaseBankInterest CreateDynamicUpgrade()
    {
        float bonus = 0.02f;
        int cost = 200;
        return new Upgrade_IncreaseBankInterest(bonus, cost);
    }

    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return BankManager.Instance.SaveInterest.Value < 1.3f;
    }

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        BankManager.Instance.SaveInterest.Value += _bonus;
    }

    #endregion
}

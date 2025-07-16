using UnityEngine;


/// <summary>
/// “增加所有炮塔攻击速度”升级，购买后立即对全场所有塔生效
/// </summary>
public class Upgrade_IncreaseTowerAttackRate : UpgradeBase
{
    #region 私有字段

    private float _attackRateBonus;    // 本次升级增加的攻击速度数值

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取此次增加的攻击力
    /// </summary>
    public float AttackRateBonus => _attackRateBonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“增加攻击速度”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要增加的攻击速度数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseTowerAttackRate(float attackRateBonus, int cost)
    {
        UpgradeID = "IncreaseTowerAttackRate";
        _attackRateBonus = attackRateBonus;
        Cost = cost;
        Description = $"每秒射击次数 +{_attackRateBonus}";
    }

    public static Upgrade_IncreaseTowerAttackRate CreateDynamicUpgrade()
    {
        float attackRateBonus =0.1f;
        int cost = Mathf.RoundToInt(attackRateBonus * 50);
        return new Upgrade_IncreaseTowerAttackRate(attackRateBonus, cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BonusAttackRate.Value += _attackRateBonus;
    }

    #endregion
}

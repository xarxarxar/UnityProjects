using UnityEngine;

/// <summary>
/// “增加所有炮塔攻击力”升级，购买后立即对全场所有塔生效
/// </summary>
public class Upgrade_IncreaseTowerAttack : UpgradeBase
{
    #region 私有字段

    private float _attackBonus;    // 本次升级增加的攻击力百分比

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取此次增加的攻击力
    /// </summary>
    public float AttackBonus => _attackBonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“增加攻击力”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseTowerAttack(float attackBonus, int cost)
    {
        _attackBonus = attackBonus;
        UpgradeID = $"IncreaseTowerAttack{_attackBonus}";
        Cost = cost;
        Description = $"炮塔伤害 +{_attackBonus*100}%";
    }

    public static Upgrade_IncreaseTowerAttack CreateDynamicUpgrade()
    {
        float[] values = { 0.2f, 0.4f, 0.6f };
        float bonus = values[Random.Range(0, values.Length)];
        int cost = Mathf.RoundToInt(bonus * (100 + UpgradeManager.Instance.RefreshCount.Value * 100));
        return new Upgrade_IncreaseTowerAttack(bonus, cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 应用本次升级效果：对当前场上所有塔的 BaseAttack 增加 _attackBonus，
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BonusAtk.Value += _attackBonus;
    }

    #endregion
}

using UnityEngine;
public class Upgrade_IncreaseCriticalMultiplier : UpgradeBase
{
    #region 私有字段

    private float _bonus;// 本次升级带来的提升

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取此次带来的提升
    /// </summary>
    public float Bonus => _bonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“增加暴击伤害”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseCriticalMultiplier(float bonus, int cost)
    {
        UpgradeID = "IncreaseCriticalMultiplier";
        _bonus = bonus;
        Cost = cost;
        Description = $"炮塔暴击伤害 +{_bonus * 100}%";
    }

    public static Upgrade_IncreaseCriticalMultiplier CreateDynamicUpgrade()
    {
        float[] values = { 0.1f, 0.15f, 0.2f };
        float bonus = values[Random.Range(0, values.Length)];
        int cost =Mathf.RoundToInt(bonus* 300) ;
        return new Upgrade_IncreaseCriticalMultiplier(bonus, cost);
    }

    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return TowerManager.Instance.BonusCritMult.Value < 5.0f;
    }

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BonusCritMult.Value += _bonus;
    }

    #endregion
}

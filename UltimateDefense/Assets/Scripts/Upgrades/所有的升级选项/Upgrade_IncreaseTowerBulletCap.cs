
using UnityEngine;

public class Upgrade_IncreaseTowerBulletCap : UpgradeBase
{
    #region 私有字段

    private int _bonus;// 本次升级带来的提升

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取此次带来的提升
    /// </summary>
    public int Bonus => _bonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“增加弹夹容量”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseTowerBulletCap(int bonus, int cost)
    {
        _bonus = bonus;
        UpgradeID = $"IncreaseTowerBulletCap{_bonus}";
        Cost = cost;
        Description = $"炮塔弹夹容量 +{bonus}";
    }

    public static Upgrade_IncreaseTowerBulletCap CreateDynamicUpgrade()
    {
        int[] values = { 2, 5, 8 };
        int bonus = values[Random.Range(0, values.Length)];
        int cost = Mathf.RoundToInt(bonus * (5 + UpgradeManager.Instance.RefreshCount.Value * 3));
        return new Upgrade_IncreaseTowerBulletCap(bonus, cost);
    }

    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return TowerManager.Instance.BonusCap.Value < 40f;
    }

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BonusCap.Value += _bonus;
    }

    #endregion
}

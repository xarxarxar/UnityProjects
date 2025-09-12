using UnityEngine;

/// <summary>
/// 增加元素子弹持续时长
/// </summary>
public class Upgrade_BulletKindDuration :  UpgradeBase
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
    /// 构造新的升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_BulletKindDuration(int bonus, int cost)
    {
        UpgradeID = $"Upgrade_BulletKindDuration{bonus}";
        _bonus = bonus;
        Cost = cost;
        Description = $"每次特殊子弹颗数+{_bonus}";
    }

    public static Upgrade_BulletKindDuration CreateDynamicUpgrade()
    {
        int[] values = { 1, 3, 5 };
        int bonus = values[Random.Range(0, values.Length)];
        int cost = Mathf.RoundToInt(bonus * (15 + UpgradeManager.Instance.RefreshCount.Value * 5));
        return new Upgrade_BulletKindDuration(bonus, cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BulletKindCount += _bonus;
    }

    #endregion
}

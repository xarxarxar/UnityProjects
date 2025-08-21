using UnityEngine;

/// <summary>
/// 增加元素子弹持续时长
/// </summary>
public class Upgrade_BulletKindDuration :  UpgradeBase
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
    /// 构造新的升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_BulletKindDuration(float bonus, int cost)
    {
        UpgradeID = $"Upgrade_BulletKindDuration{bonus}";
        _bonus = bonus;
        Cost = cost;
        Description = $"升级的子弹持续时长 +{_bonus}秒";
    }

    public static Upgrade_BulletKindDuration CreateDynamicUpgrade()
    {
        float[] values = { 1.0f, 2.0f, 3.0f };
        float bonus = values[Random.Range(0, values.Length)];
        int cost = Mathf.RoundToInt(bonus * (20 + UpgradeManager.Instance.RefreshCount.Value * 5));
        return new Upgrade_BulletKindDuration(bonus, cost);
    }

    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return TowerManager.Instance.BullletKindDuration < 30.0f;
    }

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.BullletKindDuration += _bonus;
    }

    #endregion
}

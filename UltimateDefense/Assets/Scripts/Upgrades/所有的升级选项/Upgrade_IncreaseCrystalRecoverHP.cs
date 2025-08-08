using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_IncreaseCrystalRecoverHP : UpgradeBase
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
    /// 构造新的“增加水晶每秒恢复血量”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseCrystalRecoverHP(int bonus, int cost)
    {
        UpgradeID = "IncreaseCrystalRecoverHP";
        _bonus = bonus;
        Cost = cost;
        Description = $"水晶血量每秒恢复 +{bonus}";
    }

    public static Upgrade_IncreaseCrystalRecoverHP CreateDynamicUpgrade()
    {
        int bonus = 1;
        int cost = 100;
        return new Upgrade_IncreaseCrystalRecoverHP(bonus, cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        //Crystal.Instance.RecoverHpPerSecond += _bonus;
    }

    #endregion
}

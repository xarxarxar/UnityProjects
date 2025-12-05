using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_IncreaseEnemyDieCoin : UpgradeBase
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
    public Upgrade_IncreaseEnemyDieCoin(int bonus, int cost)
    {
        _bonus = bonus;
        UpgradeID = $"IncreaseEnemyDieCoin{_bonus}";
        Cost = cost;
        Description = $"敌人掉落金币 +{bonus}";
    }

    public static Upgrade_IncreaseEnemyDieCoin CreateDynamicUpgrade()
    {
        int[] moneys = { 5, 7, 10 };
        int bonus = moneys[Random.Range(0, moneys.Length)];
        int cost = Mathf.RoundToInt(bonus * (3 + UpgradeManager.Instance.RefreshCount.Value * 2.8f));
        return new Upgrade_IncreaseEnemyDieCoin(bonus, cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        EnemyManager.Instance.EnemyDieCoin.Value += _bonus;
    }

    #endregion
}

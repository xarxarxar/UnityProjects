using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade_SummonCoinEnemy : UpgradeBase
{
    #region 构造函数

    /// <summary>
    /// 构造新的“增加攻击力”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_SummonCoinEnemy(int cost)
    {
        UpgradeID = $"SummonCoinEnemy";
        Cost = cost;
        Description = $"生成一个金币怪，消灭可获得大量金币";
    }

    public static Upgrade_SummonCoinEnemy CreateDynamicUpgrade()
    {
        int cost = 10;
        return new Upgrade_SummonCoinEnemy(cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 应用本次升级效果：对当前场上所有塔的 BaseAttack 增加 _attackBonus，
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        EnemyManager.Instance.SpawnEnemy(EnemyType.Coin, Random.Range(-5f, 5f), WaveManager.Instance.CurrentRound);
    }

    #endregion
}

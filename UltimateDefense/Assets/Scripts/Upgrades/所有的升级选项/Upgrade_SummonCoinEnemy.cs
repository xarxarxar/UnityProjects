using System.Collections;
using System.Collections.Generic;
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
        Description = $"召唤一只金币球,消灭后获得大量金币";
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
        // 通知 Manager 保存全局加成
        int currentRound = WaveManager.Instance.CurrentRound;
        int level = Mathf.Min(WaveManager.Instance.MaxRound, Random.Range(currentRound, currentRound + 3));
        EnemyManager.Instance.SpawnEnemy(EnemyType.Coin, Random.Range(-6f, 6f), level);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 立刻获得金币
/// </summary>
public class Upgrade_GetMoney : UpgradeBase
{
    #region 私有字段
    private int money;//金币的数量

    #endregion

    public int Money { get => money;}
    #region 构造函数

    /// <summary>
    /// 构造新的“增加攻击力”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_GetMoney(int count, int cost)
    {
        money = count;
        UpgradeID = $"GetMoney{money}";
        Cost = cost;
        Description = $"立刻获得{Money}金币";
    }

    public static Upgrade_GetMoney CreateDynamicUpgrade()
    {
        int[] times = { 10, 20, 30 };
        int bonus = times[Random.Range(0, times.Length)] * (UpgradeManager.Instance.FreshCost.Value+1);
        int cost = (UpgradeManager.Instance.FreshCost.Value + 1);
        return new Upgrade_GetMoney(bonus, cost);
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
        CurrencyManager.Instance.AddCoin(Money);
    }

    #endregion
}

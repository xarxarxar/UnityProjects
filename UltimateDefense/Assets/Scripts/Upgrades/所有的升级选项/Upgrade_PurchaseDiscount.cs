using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接下来的几次商品购买打折
/// </summary>
public class Upgrade_PurchaseDiscount : UpgradeBase
{

    #region 私有字段
    private float discount;//接下来几次打折的折扣

    #endregion


    #region 构造函数

    /// <summary>
    /// 构造新的“增加攻击力”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_PurchaseDiscount(float discount, int cost)
    {
        UpgradeID = "IncreaseTowerAttack";
        this.discount = discount;
        Cost = cost;
        Description = $"接下来3次刷新商品打{1- this.discount}折";
    }

    public static Upgrade_PurchaseDiscount CreateDynamicUpgrade()
    {
        float bonus = Random.Range(0.1f, 0.5f);
        int cost = 100;
        return new Upgrade_PurchaseDiscount(bonus, cost);
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
        //TowerManager.Instance.BonusAtk.Value += _attackBonus;
    }

    #endregion
}

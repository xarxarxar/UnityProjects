using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 立刻为城墙恢复一定的血量
/// </summary>
public class Upgrade_RecoverCrystal : UpgradeBase
{

    #region 私有字段
    private int hp;//恢复的血量

    #endregion

    public int HP { get => hp; }
    #region 构造函数

    /// <summary>
    /// 构造新的“恢复城墙血量”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_RecoverCrystal(int amount, int cost)
    {
        UpgradeID = "RecoverCrystal";
        hp = amount;
        Cost = cost;
        Description = $"恢复城墙{hp}点血量";
    }



    public static Upgrade_RecoverCrystal CreateDynamicUpgrade()
    {
        int[] hps = { 50, 100, 200 };
        int bonus = hps[Random.Range(0, hps.Length)];
        int cost = bonus*2;
        return new Upgrade_RecoverCrystal(bonus, cost);
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
        Crystal.Instance.Recover(hp);
    }

    #endregion
}

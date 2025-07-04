using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_SaveMoneyToBank : UpgradeBase
{
    #region 构造函数
    /// <summary>
    /// 构造新的“增加弹夹容量”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_SaveMoneyToBank()
    {
        UpgradeID = "SaveMoneyToBank";
        Cost = 100;
        Description = $"向银行存入{100}金币";
    }

    public static Upgrade_SaveMoneyToBank CreateDynamicUpgrade()
    {
        return new Upgrade_SaveMoneyToBank();
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        BankManager.Instance.SaveMoney(WaveManager.Instance.CurrentRound, 100);//存入100
    }

    #endregion
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机生成升级选项的工厂类，返回指定数量的可购买升级实例
/// </summary>
public static class UpgradeFactory
{
    #region 私有字段

    private static List<System.Func<UpgradeBase>> _allUpgradeTemplates;    // 所有可供随机抽取的升级模板列表

    #endregion

    #region 静态构造

    static UpgradeFactory()
    {
        // 初始化时预定义所有升级模板
        _allUpgradeTemplates = new List<System.Func<UpgradeBase>>()
        {
            Upgrade_IncreaseTowerAttack.CreateDynamicUpgrade,
            Upgrade_IncreaseTowerAttackRate.CreateDynamicUpgrade,
            Upgrade_IncreaseEnemyDieCoinProb.CreateDynamicUpgrade,
            Upgrade_IncreaseTowerReloadTime.CreateDynamicUpgrade,
            Upgrade_IncreaseTowerBulletCap.CreateDynamicUpgrade,
            Upgrade_IncreaseCriticalShotProb.CreateDynamicUpgrade,
            Upgrade_IncreaseCriticalMultiplier.CreateDynamicUpgrade,
            //Upgrade_SaveMoneyToBank.CreateDynamicUpgrade,
            //Upgrade_IncreaseBankInterest.CreateDynamicUpgrade,
            Upgrade_IncreaseEnemyDieCoin.CreateDynamicUpgrade,
            Upgrade_IncreaseCrystalMaxHP.CreateDynamicUpgrade,
            Upgrade_GetMoney.CreateDynamicUpgrade,
            Upgrade_RecoverCrystal.CreateDynamicUpgrade,
            Upgrade_SetCrystalInvincible.CreateDynamicUpgrade,
            Upgrade_SetEnemyHalf.CreateDynamicUpgrade,
            Upgrade_RageMode.CreateDynamicUpgrade,
            //Upgrade_DecreaseAdvanceInterest.CreateDynamicUpgrade,
            //Upgrade_IncreaseMaxAdvance.CreateDynamicUpgrade,
            Upgrade_ChangeBulletKind.CreateDynamicUpgrade,
            Upgrade_BulletKindDuration.CreateDynamicUpgrade,
            Upgrade_SummonCoinEnemy.CreateDynamicUpgrade,
            // 可在此继续扩充其他类型升级的模板，例如减速、银行利息等
        };
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 随机从模板池中返回 n 条克隆后的升级实例，若 n 超过池大小则返回全部
    /// </summary>
    /// <param name="n">需要抽取的升级数量</param>
    /// <returns>克隆后的升级实例列表</returns>
    public static List<UpgradeBase> GetRandomUpgrades(int n)
    {
        var result = new List<UpgradeBase>();

        for (int i = 0; i < n; i++)
        {
            // 多次尝试找到合法的升级（最多尝试10次防止死循环）
            for (int attempts = 0; attempts < 10; attempts++)
            {
                int randomIndex = Random.Range(0, _allUpgradeTemplates.Count);
                var upgrade = _allUpgradeTemplates[randomIndex].Invoke();

                if (upgrade.IsAvailable())
                {
                    result.Add(CloneUpgrade(upgrade));
                    break;
                }
            }
        }

        return result;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 按照模板类型返回一个新创建的升级实例
    /// </summary>
    /// <param name="template">升级模板</param>
    /// <returns>克隆出的新升级实例</returns>
    private static UpgradeBase CloneUpgrade(UpgradeBase template)
    {
        if (template is Upgrade_IncreaseTowerAttack atkT)
        {
            return new Upgrade_IncreaseTowerAttack(atkT.AttackBonus, atkT.Cost);
        }
        if (template is Upgrade_IncreaseTowerAttackRate atkrT)
        {
            return new Upgrade_IncreaseTowerAttackRate(atkrT.AttackRateBonus, atkrT.Cost);
        }
        if (template is Upgrade_IncreaseEnemyDieCoinProb edcp)
        {
            return new Upgrade_IncreaseEnemyDieCoinProb(edcp.EnemyDieCoinProbBonus, edcp.Cost);
        }
        if(template is Upgrade_IncreaseTowerReloadTime itrt)
        {
            return new Upgrade_IncreaseTowerReloadTime(itrt.Bonus, itrt.Cost);
        }
        if (template is Upgrade_IncreaseTowerBulletCap itbc)
        {
            return new Upgrade_IncreaseTowerBulletCap(itbc.Bonus, itbc.Cost);
        }
        if (template is Upgrade_IncreaseCriticalShotProb icsp)
        {
            return new Upgrade_IncreaseCriticalShotProb(icsp.Bonus, icsp.Cost);
        }
        if (template is Upgrade_IncreaseCriticalMultiplier icmp)
        {
            return new Upgrade_IncreaseCriticalMultiplier(icmp.Bonus, icmp.Cost);
        }
        //if (template is Upgrade_SaveMoneyToBank smtb)
        //{
        //    return new Upgrade_SaveMoneyToBank();
        //}
        //if (template is Upgrade_IncreaseBankInterest ibi)
        //{
        //    return new Upgrade_IncreaseBankInterest(ibi.Bonus, ibi.Cost);
        //}
        if (template is Upgrade_IncreaseEnemyDieCoin iedc)
        {
            return new Upgrade_IncreaseEnemyDieCoin(iedc.Bonus, iedc.Cost);
        }
        if (template is Upgrade_IncreaseCrystalMaxHP icmh)
        {
            return new Upgrade_IncreaseCrystalMaxHP(icmh.Bonus, icmh.Cost);
        }
        if (template is Upgrade_GetMoney ug)
        {
            return new Upgrade_GetMoney(ug.Money, ug.Cost);
        }
        if (template is Upgrade_RecoverCrystal urc)
        {
            return new Upgrade_RecoverCrystal(urc.HP, urc.Cost);
        }
        if (template is Upgrade_SetCrystalInvincible usci)
        {
            return new Upgrade_SetCrystalInvincible(usci.Duration, usci.Cost);
        }
        if (template is Upgrade_SetEnemyHalf useh)
        {
            return new Upgrade_SetEnemyHalf(useh.Count, useh.Cost);
        }
        if (template is Upgrade_RageMode urm)
        {
            return new Upgrade_RageMode(urm.Times, urm.Cost);
        }
        //if (template is Upgrade_DecreaseAdvanceInterest udai)
        //{
        //    return new Upgrade_DecreaseAdvanceInterest(udai.Bonus, udai.Cost);
        //}
        //if (template is Upgrade_IncreaseMaxAdvance uima)
        //{
        //    return new Upgrade_IncreaseMaxAdvance(uima.Bonus, uima.Cost);
        //}
        if (template is Upgrade_ChangeBulletKind ucbk)
        {
            return new Upgrade_ChangeBulletKind(ucbk.BulletKind, ucbk.Cost);
        }
        if (template is Upgrade_BulletKindDuration ubkd)
        {
            return new Upgrade_BulletKindDuration(ubkd.Bonus, ubkd.Cost);
        }
        if (template is Upgrade_SummonCoinEnemy usce)
        {
            return new Upgrade_SummonCoinEnemy(usce.Cost);
        }

        // 后续若添加其他 Upgrade 子类，在此继续扩充：
        // if (template is Upgrade_SlowEnemy slowT) return new Upgrade_SlowEnemy(slowT.SlowPercent, slowT.Cost);

        // 如果没有匹配，则返回 null（正常情况下模板池里都是可识别的）
        return null;
    }

    #endregion
}

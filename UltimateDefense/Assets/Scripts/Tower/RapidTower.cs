using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连发形态的炮塔
/// </summary>
public class RapidTower : BaseTower
{
   
    public override TowerType TowerType { get => TowerType.RapidFire; }

    public override int BaseDamage => 1;//1-4-7-10
    public override int BaseCap => 6;//6-8-10
    public override float BaseAtkRate => 1;
    public override float BaseReload => 3;
    public override float BaseCritProb => 0.1f;
    public override float BaseCritMult => 1.2f;

    public override string GetDescription()
    {
        return $"每次发射2颗子弹，初始伤害降低，换弹时间降低";
    }

    public override string GetUpgradeDescription()
    {
        // 判断是否为偶数等级且不为 0
        bool isEvenAndNotZero = TowerLevel != 0 && TowerLevel % 2 == 0;

        // 返回对应的描述文本
        return isEvenAndNotZero ? "弹夹容量+1" : "伤害+1";
    }

    protected override void CalculateAtkDamage()
    {
        //初始伤害为基础伤害的60%
        _bulletDamage.Value = Mathf.RoundToInt(BaseDamage * 0.6f * (1f + TowerManager.Instance.BonusAtk.Value));
    }
    protected override void CalculateBulletCap()
    {
        //初始容量为基础弹夹容量的120%
        _bulletCapacity.Value = Mathf.RoundToInt(BaseCap * 1.2f) + TowerManager.Instance.BonusCap.Value;
    }
    protected override void CalculateReloadTime()
    {
        //初始换弹时间为基础的80%
        _reloadTime.Value = BaseReload * 0.8f * (1f - TowerManager.Instance.BonusReload.Value);
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet1 = TowerManager.Instance.BulletPool.Get();

        float value1 = Random.value;
        if (value1 < CriticalProb.Value)
        {
            bullet1.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value));
        }
        else
        {
            bullet1.Init(_bulletInitPos.position, currentTarget, false, BulletDamage.Value);
        }
        JellySquash();
        CurrentBulletCount--;
        yield return TimerUtility.WaitForGameSeconds(0.1f);
        
        if (CurrentBulletCount == 0) 
        { 
            yield break;
        }

        Bullet bullet2 = TowerManager.Instance.BulletPool.Get();

        float value2 = Random.value;
        
        if (value2 < CriticalProb.Value)
        {
            bullet2.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value));
        }
        else
        {
            bullet2.Init(_bulletInitPos.position, currentTarget, false, BulletDamage.Value);
        }
        JellySquash();
        CurrentBulletCount--;
        yield return null;
    }
}

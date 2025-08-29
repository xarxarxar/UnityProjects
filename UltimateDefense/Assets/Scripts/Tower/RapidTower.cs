using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连发形态的炮塔
/// </summary>
public class RapidTower : BaseTower
{
   
    public override TowerType TowerType { get => TowerType.RapidFire; }

    private int bulletcount = 0;

    protected override void Init()
    {
        base.Init();
        bulletcount = 0;
    }

    protected override void CalculateAtkDamage()
    {
        //初始伤害
        _bulletDamage.Value = Mathf.RoundToInt(BaseDamage * (1f + TowerManager.Instance.BonusAtk.Value));
    }
    protected override void CalculateBulletCap()
    {
        //初始容量为基础弹夹容量的120%
        _bulletCapacity.Value = BaseCap + TowerManager.Instance.BonusCap.Value;
    }
    protected override void CalculateReloadTime()
    {
        //初始换弹时间为
        _reloadTime.Value = BaseReload * (1f - TowerManager.Instance.BonusReload.Value);
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
        bulletcount++;
        if (bulletcount >= 200)
        {
            Crystal.Instance.Recover(1);
            bulletcount = 0;
        }
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
        bulletcount++;
        if (bulletcount >= 200)
        {
            Crystal.Instance.Recover(1);
            bulletcount = 0;
        }
        yield return null;
    }

    
}

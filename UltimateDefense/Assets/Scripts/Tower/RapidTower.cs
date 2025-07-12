using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连发形态的炮塔
/// </summary>
public class RapidTower : BaseTower
{
    public override int BulletDamage => base.BulletDamage;
    public override int BulletCapacity => base.BulletCapacity;
    public override float AttackSpeed => base.AttackSpeed;
    public override float CriticalProb => base.CriticalProb;
    public override float ReloadTime => base.ReloadTime;

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet1 = TowerManager.Instance.BulletPool.Get();

        float value1 = Random.value;
        if (value1 < CriticalProb)
        {
            bullet1.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage * CriticalMult));
        }
        else
        {
            bullet1.Init(_bulletInitPos.position, currentTarget, false, BulletDamage);
        }
        CurrentBulletCount--;
        yield return TimerUtility.WaitForGameSeconds(0.2f);

        if (CurrentBulletCount == 0) 
        { 
            yield break;
        }

        Bullet bullet2 = TowerManager.Instance.BulletPool.Get();

        float value2 = Random.value;
        if (value2 < CriticalProb)
        {
            bullet2.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage * CriticalMult));
        }
        else
        {
            bullet2.Init(_bulletInitPos.position, currentTarget, false, BulletDamage);
        }
        CurrentBulletCount--;
        yield return null;
    }
}

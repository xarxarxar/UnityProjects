using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连发形态的炮塔
/// </summary>
public class RapidTower : Tower
{
    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        TrackBullet bullet1 = TowerManager.Instance.GetBullet<TrackBullet>();
        bullet1.OnGetFromPool();

        float value1 = Random.value;
        if (value1 < CriticalProb)
        {
            bullet1.Init(Mathf.RoundToInt(BaseDamage * CriticalMult),
                 true, _bulletInitPos.position, currentTarget);

        }
        else
        {
            bullet1.Init(BaseDamage, false, _bulletInitPos.position, currentTarget);
        }
        JellySquash();
        CurrentBulletCount--;
        yield return TimerUtility.WaitForGameSeconds(0.1f);
        
        if (CurrentBulletCount == 0) 
        { 
            yield break;
        }

        TrackBullet bullet2 = TowerManager.Instance.GetBullet<TrackBullet>();
        bullet2.OnGetFromPool();

        float value2 = Random.value;
        
        if (value2 < CriticalProb)
        {
            bullet2.Init(Mathf.RoundToInt(BaseDamage * CriticalMult),
                 true, _bulletInitPos.position, currentTarget);
        }
        else
        {
            bullet2.Init(BaseDamage, false, _bulletInitPos.position, currentTarget);
        }
        CurrentBulletCount--;
        yield return null;
    }

    
}

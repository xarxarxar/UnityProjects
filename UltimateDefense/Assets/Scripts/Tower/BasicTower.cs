using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础形态下的炮塔
/// </summary>
public class BasicTower : Tower
{

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        bullet.transform.position = _bulletInitPos.position;
        float value = Random.value;
        if (value < CriticalProb)
        {
            bullet.Init(Mathf.RoundToInt(BaseDamage * CriticalMult), 
                true);
        }
        else
        {
            bullet.Init(BaseDamage,false);
        }
        JellySquash();
        CurrentBulletCount--;
        yield return null;
    }

}

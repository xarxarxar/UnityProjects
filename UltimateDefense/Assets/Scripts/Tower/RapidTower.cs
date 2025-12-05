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
        Bullet bullet1 = TowerManager.Instance.BulletPool.Get();

        float value1 = Random.value;
        if (value1 < CriticalProb)
        {
           
               
        }
        else
        {
            
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
        
        if (value2 < CriticalProb)
        {
            
        }
        else
        {
            
        }
        //JellySquash();
        CurrentBulletCount--;
        yield return null;
    }

    
}

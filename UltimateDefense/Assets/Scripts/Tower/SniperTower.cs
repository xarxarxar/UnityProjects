using System.Collections;
using UnityEngine;

/// <summary>
/// 狙击形态的炮塔，狙击炮塔每五次必定暴击一次，暴击伤害也更高，但射击间隔明显变长
/// </summary>
public class SniperTower : Tower
{
    private int _shotCount;//射击的次数，狙击炮塔每五次必定暴击一次，暴击伤害也更高

    protected override void Init()
    {
        base.Init();
        _shotCount = 0;
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet = TowerManager.Instance.BulletPool.Get();
        int dam = 0;
        if (_shotCount >= 5)//必定暴击
        {
            dam = Mathf.RoundToInt(BaseDamage * CriticalMult);
            
            _shotCount = 0;
        }
        else//随机暴击
        {
            float value = Random.value;
            if (value < CriticalProb)
            {
                dam = Mathf.RoundToInt(BaseDamage * CriticalMult);
                
            }
            else
            {
                dam = BaseDamage;
                
            }
        }

        JellySquash();
        CurrentBulletCount--;
        _shotCount++;
        yield return null;
    }
}

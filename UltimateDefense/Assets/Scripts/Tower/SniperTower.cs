using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 狙击形态的炮塔，狙击炮塔每五次必定暴击一次，暴击伤害也更高，但射击间隔明显变长
/// </summary>
public class SniperTower : BaseTower
{
    public override int BulletDamage => base.BulletDamage;
    public override int BulletCapacity => base.BulletCapacity;
    public override float AttackSpeed => base.AttackSpeed;
    public override float CriticalProb => base.CriticalProb;
    public override float ReloadTime => base.ReloadTime;

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

        if (_shotCount >= 5)//必定暴击
        {
            bullet.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage * CriticalMult));
            _shotCount = 0;
        }
        else//随机暴击
        {
            float value = Random.value;
            if (value < CriticalProb)
            {
                bullet.Init(_bulletInitPos.position, currentTarget,
                    true, Mathf.RoundToInt(BulletDamage * CriticalMult));
            }
            else
            {
                bullet.Init(_bulletInitPos.position, currentTarget, false, BulletDamage);
            }
        }

        CurrentBulletCount--;
        _shotCount++;
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透形态的炮塔
/// </summary>
public class PierceTower : Tower
{
    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        ShootPierceBullet();
        JellySquash();
        CurrentBulletCount--;
        yield return null;
    }

    private void ShootPierceBullet()
    {
        PierceBullet bullet = TowerManager.Instance.GetBullet<PierceBullet>();

        Vector3 direction = currentTarget.transform.position - _bulletInitPos.position;
        float value = Random.value;
        if (value < CriticalProb)
        {
            bullet.Init(Mathf.RoundToInt(BaseDamage * CriticalMult),
                true, _bulletInitPos.position, direction);
        }
        else
        {
            bullet.Init(BaseDamage, false, _bulletInitPos.position, direction);
        }
    }
}

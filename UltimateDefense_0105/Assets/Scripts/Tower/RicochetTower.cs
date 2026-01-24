using System.Collections;
using UnityEngine;

/// <summary>
/// 弹射形态的炮塔
/// </summary>
public class RicochetTower : Tower
{
    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        ShootBounceBullet();
        JellySquash();
        CurrentBulletCount--;
        yield return null;
    }

    private void ShootBounceBullet()
    {
        BounceBullet bullet = TowerManager.Instance.GetBullet<BounceBullet>();

        float value = Random.value;
        if (value < CriticalProb)
        {
            bullet.Init(Mathf.RoundToInt(BaseDamage * CriticalMult),
                true, _bulletInitPos.position, currentTarget);
        }
        else
        {
            bullet.Init(BaseDamage, false, _bulletInitPos.position, currentTarget);
        }
    }
}

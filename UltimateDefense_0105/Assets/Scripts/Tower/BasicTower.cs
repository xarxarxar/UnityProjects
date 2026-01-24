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
        ShootTrackBullet();
        //ShootPierceBullet();
        //ShootBounceBullet();
        //ShootShellBullet();
        JellySquash();
        CurrentBulletCount--;
        yield return null;
    }


    private void ShootTrackBullet()
    {
        TrackBullet bullet = TowerManager.Instance.GetBullet<TrackBullet>();
        bullet.OnGetFromPool();

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

    private void ShootBounceBullet()
    {
        BounceBullet bullet = TowerManager.Instance.GetBullet<BounceBullet>();
        bullet.OnGetFromPool();

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

    private void ShootShellBullet()
    {
        ShellBullet bullet = TowerManager.Instance.GetBullet<ShellBullet>();
        bullet.OnGetFromPool();

        float value = Random.value;
        if (value < CriticalProb)
        {
            bullet.Init(Mathf.RoundToInt(BaseDamage * CriticalMult),
                true, _bulletInitPos.position, currentTarget.transform.position);
        }
        else
        {
            bullet.Init(BaseDamage, false, _bulletInitPos.position, currentTarget.transform.position);
        }
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// 狙击形态的炮塔，狙击炮塔每五次必定暴击一次，暴击伤害也更高，但射击间隔明显变长
/// </summary>
public class SniperTower : Tower
{

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        ShootTrackBullet();
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
}

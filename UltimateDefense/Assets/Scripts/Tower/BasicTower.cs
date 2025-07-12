using System.Collections;
using UnityEngine;
/// <summary>
/// 基础形态下的炮塔
/// </summary>
public class BasicTower : BaseTower
{
    public override int BulletDamage => base.BulletDamage;
    public override int BulletCapacity => base.BulletCapacity;
    public override float AttackSpeed => base.AttackSpeed;
    public override float CriticalProb => base.CriticalProb;
    public override float ReloadTime => base.ReloadTime;

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

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

        CurrentBulletCount--;
        yield return null;
    }
}

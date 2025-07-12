using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透形态的炮塔
/// </summary>
public class PierceTower : BaseTower
{
    public override int BulletDamage => base.BulletDamage;
    public override int BulletCapacity => base.BulletCapacity;
    public override float AttackSpeed => base.AttackSpeed;
    public override float CriticalProb => base.CriticalProb;
    public override float ReloadTime => base.ReloadTime;

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        // 计算发射方向（指向目标敌人）
        Vector3 direction = (currentTarget.transform.position - _bulletInitPos.position).normalized;

        // 创建并初始化子弹
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        // 暴击判定
        bool isCritical = Random.value < CriticalProb;
        int damage = isCritical ? Mathf.RoundToInt(BulletDamage * CriticalMult) : BulletDamage;

        // 初始化子弹（发射方向，直线穿透类型）
        bullet.Init(_bulletInitPos.position, direction, isCritical, damage, BulletType.MultiPenetrate);

        // 子弹数量减少
        CurrentBulletCount--;

        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹射形态的炮塔
/// </summary>
public class RicochetTower : BaseTower
{
    public override int BulletDamage => base.BulletDamage;
    public override int BulletCapacity => base.BulletCapacity;
    public override float AttackSpeed => base.AttackSpeed;
    public override float CriticalProb => base.CriticalProb;
    public override float ReloadTime => base.ReloadTime;

    private int _maxChain=3;//最大弹射的次数

    protected override void Init()
    {
        base.Init();
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        if (currentTarget == null) yield break;

        // 从对象池获取子弹
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        // 判断是否暴击
        bool isCritical = Random.value < CriticalProb;
        int damage = isCritical ? Mathf.RoundToInt(BulletDamage * CriticalMult) : BulletDamage;

        // 初始化为弹射子弹：最多弹射 3 次（即命中 4 个敌人）
        bullet.InitChainBullet(
            _bulletInitPos.position,
            currentTarget,
            isCritical,
            damage,
            maxChain: _maxChain,
            decayPercent: 0.3f // 每次弹射衰减30%
        );

        // 扣除子弹数
        CurrentBulletCount--;

        yield return null;
    }
}

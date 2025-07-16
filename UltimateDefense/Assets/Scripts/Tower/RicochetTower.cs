using System.Collections;
using UnityEngine;

/// <summary>
/// 弹射形态的炮塔
/// </summary>
public class RicochetTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Ricochet; }

    private int _maxChain=2;//最大弹射的次数

    public override int BaseDamage => 1;//1-4-7-10
    public override int BaseCap => 6;//6-8-10
    public override float BaseAtkRate => 1;
    public override float BaseReload => 3;
    public override float BaseCritProb => 0.1f;
    public override float BaseCritMult => 1.2f;

    public override string GetDescription()
    {
        return $"子弹命中敌人后可以额外弹射，当前额外弹射次数{_maxChain}，每次弹射伤害衰减70%";
    }

    public override string GetUpgradeDescription()
    {
        // 判断等级是否为5的倍数且不为 0
        bool isEvenAndNotZero = TowerLevel != 0 && TowerLevel % 5 == 0;

        // 返回对应的描述文本
        return isEvenAndNotZero ? "额外弹射次数+1" : "伤害衰减-10%";
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        if (currentTarget == null) yield break;

        // 从对象池获取子弹
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        // 判断是否暴击
        bool isCritical = Random.value < CriticalProb.Value;
        int damage = isCritical ? Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value) : BulletDamage.Value;

        // 初始化为弹射子弹：最多弹射 3 次（即命中 4 个敌人）
        bullet.InitChainBullet(
            _bulletInitPos.position,
            currentTarget,
            isCritical,
            damage,
            maxChain: _maxChain,
            decayPercent: 0.7f // 每次弹射衰减70%
        );

        // 扣除子弹数
        CurrentBulletCount--;

        yield return null;
    }
}

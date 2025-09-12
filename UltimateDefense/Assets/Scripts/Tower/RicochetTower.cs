using System.Collections;
using UnityEngine;

/// <summary>
/// 弹射形态的炮塔
/// </summary>
public class RicochetTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Ricochet; }

    private int _maxChain=2;//最大弹射的次数

    protected override void Init()
    {
        base.Init();
        EnemyManager.Instance.SignleEnemyDieCount.OnValueChanged += OnSignleEnemyDieCount;
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

        // 初始化为弹射子弹：最多弹射 2 次（即命中 3 个敌人）
        bullet.InitChainBullet(
            _bulletInitPos.position,
            currentTarget,
            isCritical,
            damage,
            maxChain: _maxChain,
            decayPercent: 0.3f // 每次弹射衰减30%
        );
        JellySquash();
        // 扣除子弹数
        CurrentBulletCount--;

        yield return null;
    }

    private void OnSignleEnemyDieCount(int count)
    {
        if (count % 3 == 0 && count != 0)
        {
            Crystal.Instance.Recover(1);
        }
    }
    protected override void EndBattle()
    {
        base.EndBattle();
        EnemyManager.Instance.SignleEnemyDieCount.OnValueChanged -= OnSignleEnemyDieCount;
    }

}

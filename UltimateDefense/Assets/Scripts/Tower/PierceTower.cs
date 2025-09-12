using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透形态的炮塔
/// </summary>
public class PierceTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Piercing; }

    private float totalDamage = 0;//单局总伤害

    protected override void Init()
    {
        base.Init();
        totalDamage = 0;
        Enemy.OnEnemyDamaged += OnEnemyDamaged;
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        // 计算发射方向（指向目标敌人）
        Vector3 direction = (currentTarget.transform.position - _bulletInitPos.position).normalized;

        // 创建并初始化子弹
        Bullet bullet = TowerManager.Instance.BulletPool.Get();
        // 暴击判定
        bool isCritical = Random.value < CriticalProb.Value;
        int damage = isCritical ? Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value) : BulletDamage.Value;

        // 初始化子弹（发射方向，直线穿透类型）
        bullet.Init(_bulletInitPos.position, direction, isCritical, damage, BulletType.MultiPenetrate);
       
        JellySquash();
        // 子弹数量减少
        CurrentBulletCount--;

        yield return null;
    }

    private void OnEnemyDamaged(Enemy enemy,bool isCri,int damage)
    {
        totalDamage += damage / 10.0f;
        if (totalDamage >= 50)//每造成50伤害，恢复一点生命
        {
            Crystal.Instance.Recover(Mathf.RoundToInt(totalDamage / 50));
            totalDamage = 0;
        }
    }

    protected override void EndBattle()
    {
        base.EndBattle();
        Enemy.OnEnemyDamaged -= OnEnemyDamaged;
    }
}

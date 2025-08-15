using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透形态的炮塔
/// </summary>
public class PierceTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Piercing; }

    public override int BaseDamage => 1;//1-4-7-10
    public override int BaseCap => 6;//6-8-10
    public override float BaseAtkRate => 1;
    public override float BaseReload => 3;
    public override float BaseCritProb => 0.1f;
    public override float BaseCritMult => 1.2f;

    public override string GetDescription()
    {
        return $"子弹可穿透敌人，初始伤害降低，初始弹夹容量降低";
    }

    public override string GetUpgradeDescription()
    {
        // 如果是偶数等级且不为0，提升弹夹容量
        if (TowerLevel % 2 == 0 && TowerLevel != 0)
        {
            return "弹夹容量+1";
        }

        // 其余情况（等级0 或 奇数等级），提升基础伤害
        return "基础伤害+1";
    }

    protected override void CalculateAtkDamage()
    {
        //初始伤害为基础伤害的80%,每升级一次伤害提示10%
        _bulletDamage.Value = Mathf.RoundToInt(BaseDamage * (0.8f + TowerLevel * 0.1f) * (1f + TowerManager.Instance.BonusAtk.Value));
    }
    protected override void CalculateBulletCap()
    {
        _bulletCapacity.Value = Mathf.RoundToInt(BaseCap * 0.8f) + TowerManager.Instance.BonusCap.Value;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础形态下的炮塔
/// </summary>
public class BasicTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Basic;}
    public override int BaseDamage => 1;//1-4-7-10
    public override int BaseCap => 6;//6-8-10
    public override float BaseAtkRate => 1;
    public override float BaseReload => 3;
    public override float BaseCritProb => 0.1f;
    public override float BaseCritMult => 1.2f;

    public override string GetDescription()
    {
        return $"每次发射单颗子弹,对敌人造成伤害";
    }

    public override string GetUpgradeDescription()
    {
        return $"基础伤害+1";
    }


    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        float value = Random.value;
        if (value < CriticalProb.Value)
        {
            bullet.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value));
        }
        else
        {
            bullet.Init(_bulletInitPos.position, currentTarget, false, BulletDamage.Value);
        }

        CurrentBulletCount--;
        yield return null;
    }

    
}

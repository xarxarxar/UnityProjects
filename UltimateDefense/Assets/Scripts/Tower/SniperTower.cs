using System.Collections;
using UnityEngine;

/// <summary>
/// 狙击形态的炮塔，狙击炮塔每五次必定暴击一次，暴击伤害也更高，但射击间隔明显变长
/// </summary>
public class SniperTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Sniper; }

    private int _shotCount;//射击的次数，狙击炮塔每五次必定暴击一次，暴击伤害也更高

    private int totalDamage = 0;//单局总伤害

    protected override void Init()
    {
        base.Init();
        _shotCount = 0;
        totalDamage = 0;
    }


    protected override void CalculateBulletCap()
    {
        //初始容量为基础弹夹容量的一半
        _bulletCapacity.Value = BaseCap + TowerManager.Instance.BonusCap.Value;
    }
    protected override void CalculateAtkRate()
    {
        //初始每秒攻击次数为基础攻击0.5倍
        _attackRate.Value = BaseAtkRate* (1f + TowerManager.Instance.BonusAttackRate.Value) * TowerManager.Instance.BonusTmpAttackRate.Value;
    }
    protected override void CalculateCriticalMult()
    {
        //初始暴击伤害倍率为基础暴击倍率的2倍
        _criticalMult.Value = BaseCritMult + TowerManager.Instance.BonusCritMult.Value;
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet = TowerManager.Instance.BulletPool.Get();
        int dam = 0;
        if (_shotCount >= 5)//必定暴击
        {
            dam = Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value);
            bullet.Init(_bulletInitPos.position, currentTarget,
                true, dam);
            _shotCount = 0;
        }
        else//随机暴击
        {
            float value = Random.value;
            if (value < CriticalProb.Value)
            {
                dam = Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value);
                bullet.Init(_bulletInitPos.position, currentTarget,
                    true, dam);
            }
            else
            {
                dam = BulletDamage.Value;
                bullet.Init(_bulletInitPos.position, currentTarget, false, dam);
            }
        }
        totalDamage += dam;
        if (totalDamage >= 100)//每造成100伤害，恢复一点生命
        {
            Crystal.Instance.Recover(totalDamage/100);
            totalDamage = 0;
        }
        JellySquash();
        CurrentBulletCount--;
        _shotCount++;
        yield return null;
    }
}

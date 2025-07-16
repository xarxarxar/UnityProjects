using System.Collections;
using UnityEngine;

/// <summary>
/// 狙击形态的炮塔，狙击炮塔每五次必定暴击一次，暴击伤害也更高，但射击间隔明显变长
/// </summary>
public class SniperTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Sniper; }

    private int _shotCount;//射击的次数，狙击炮塔每五次必定暴击一次，暴击伤害也更高

    public override int BaseDamage => 1;//1-4-7-10
    public override int BaseCap => 6;//6-8-10
    public override float BaseAtkRate => 1;
    public override float BaseReload => 3;
    public override float BaseCritProb => 0.1f;
    public override float BaseCritMult => 1.2f;

    protected override void Init()
    {
        base.Init();
        _shotCount = 0;
    }

    public override string GetDescription()
    {
        return $"攻击范围扩大到全屏，基础弹夹容量降低，基础攻速降低，基础暴击伤害倍率增大";
    }

    public override string GetUpgradeDescription()
    {
        // 判断等级是否为2的倍数且不为 0
        bool isEvenAndNotZero = TowerLevel != 0 && TowerLevel % 2 == 0;

        // 返回对应的描述文本
        return isEvenAndNotZero ? "弹夹容量+1" : "攻速+10%";
    }

    protected override void CalculateBulletCap()
    {
        //初始容量为基础弹夹容量的一半
        BulletCapacity.Value = Mathf.RoundToInt(BaseCap * 0.5f) + TowerManager.Instance.BonusCap.Value;
    }
    protected override void CalculateAtkRate()
    {
        //初始每秒攻击次数为基础攻击0.5倍
        AttackRate.Value = BaseAtkRate / 2.0f * (1f + TowerManager.Instance.BonusAttackRate.Value);
    }
    protected override void CalculateCriticalMult()
    {
        //初始暴击伤害倍率为基础暴击倍率的2倍
        CriticalMult.Value = BaseCritMult * 2 + TowerManager.Instance.BonusCritMult.Value;
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        if (_shotCount >= 5)//必定暴击
        {
            bullet.Init(_bulletInitPos.position, currentTarget,
                true, Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value));
            _shotCount = 0;
        }
        else//随机暴击
        {
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
        }

        CurrentBulletCount--;
        _shotCount++;
        yield return null;
    }
}

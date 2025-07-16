using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 散射形态的炮塔,初始暴击概率翻倍
/// </summary>
public class SpreadTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Spread; }

    public override int BaseDamage => 1;//1-4-7-10
    public override int BaseCap => 6;//6-8-10
    public override float BaseAtkRate => 1;
    public override float BaseReload => 3;
    public override float BaseCritProb => 0.1f;
    public override float BaseCritMult => 1.2f;

    public override string GetDescription()
    {
        return $"每次攻击呈扇形射出三颗子弹，弹夹不满三颗时全部射出，初始伤害降低，暴击概率翻倍";
    }

    public override string GetUpgradeDescription()
    {
        // 判断等级是否为2的倍数且不为 0
        bool isEvenAndNotZero = TowerLevel != 0 && TowerLevel % 2 == 0;

        // 返回对应的描述文本
        return isEvenAndNotZero ? "暴击概率+1%" : "伤害+10%";
    }

    protected override void CalculateAtkDamage()
    {
        //初始伤害为基础伤害的50%
        BulletDamage.Value = Mathf.RoundToInt(BaseDamage *0.5f* (1f + TowerManager.Instance.BonusAtk.Value));
    }
    protected override void CalculateCriticalProb()
    {
        //初始暴击概率为基础暴击概率的2倍
        CriticalProb.Value = BaseCritProb * 2.0f + TowerManager.Instance.BonusCritProb.Value;
    }

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        if (currentTarget == null || CurrentBulletCount <= 0)
            yield break;

        Vector3 firePos = _bulletInitPos.position;
        Vector3 dirToTarget = (currentTarget.transform.position - firePos).normalized;
        float baseAngle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;

        // 子弹数量决定射出角度
        List<float> angleOffsets = new List<float>();
        if (CurrentBulletCount >= 3)
            angleOffsets.AddRange(new[] { -10f, 0f, 10f });
        else if (CurrentBulletCount == 2)
            angleOffsets.AddRange(new[] { 0f, 10f });
        else if (CurrentBulletCount == 1)
            angleOffsets.Add(0f);

        foreach (float offset in angleOffsets)
        {
            float angle = baseAngle + offset;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 shootDir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            Bullet bullet = TowerManager.Instance.BulletPool.Get();

            bool isCrit = Random.value < CriticalProb.Value;
            int damage = isCrit ? Mathf.RoundToInt(BulletDamage.Value * CriticalMult.Value) : BulletDamage.Value;

            bullet.Init(firePos, shootDir, isCrit, damage,type:BulletType.SinglePenetrate);
        }

        // 消耗子弹
        CurrentBulletCount -= angleOffsets.Count;

        yield return null; // 如果需要连发间隔可改为 yield return TimerUtility.WaitForGameSeconds(x);
    }
}

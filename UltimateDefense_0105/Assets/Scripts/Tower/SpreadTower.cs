using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 散射形态的炮塔,初始暴击概率翻倍
/// </summary>
public class SpreadTower : Tower
{
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

            PierceBullet bullet = TowerManager.Instance.GetBullet<PierceBullet>();

            bool isCrit = Random.value < CriticalProb;
            //暴击恢复城墙血量
            if (isCrit)
            {
                Crystal.Instance.Recover(1);
            }
            int damage = isCrit ? Mathf.RoundToInt(BaseDamage * CriticalMult) : BaseDamage;
            bullet.Init(damage, isCrit, _bulletInitPos.position, shootDir,false);
        }
        JellySquash();
        // 消耗子弹
        CurrentBulletCount -= angleOffsets.Count;

        yield return null; // 如果需要连发间隔可改为 yield return TimerUtility.WaitForGameSeconds(x);
    }
}

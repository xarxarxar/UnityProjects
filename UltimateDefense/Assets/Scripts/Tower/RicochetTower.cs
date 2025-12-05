using System.Collections;
using UnityEngine;

/// <summary>
/// 弹射形态的炮塔
/// </summary>
public class RicochetTower : Tower
{
    private int _maxChain=2;//最大弹射的次数

    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        if (currentTarget == null) yield break;

        // 从对象池获取子弹
        Bullet bullet = TowerManager.Instance.BulletPool.Get();

        // 判断是否暴击
        bool isCritical = Random.value < CriticalProb;
        int damage = isCritical ? Mathf.RoundToInt(BaseDamage * CriticalMult) : BaseDamage;

        
        JellySquash();
        // 扣除子弹数
        CurrentBulletCount--;

        yield return null;
    }
}

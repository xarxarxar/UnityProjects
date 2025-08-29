using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 穿透形态的炮塔
/// </summary>
public class PierceTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Piercing; }

    private int coin = 0;

    protected override void Init()
    {
        base.Init();
        coin = 0;
        CurrencyManager.OnCoinChange += OnCoinChange;
    }

    protected override void CalculateAtkDamage()
    {
        //初始伤害为基础伤害的80%,每升级一次伤害提示10%
        _bulletDamage.Value = Mathf.RoundToInt(BaseDamage* (1f + TowerManager.Instance.BonusAtk.Value));
    }
    protected override void CalculateBulletCap()
    {
        _bulletCapacity.Value = BaseCap + TowerManager.Instance.BonusCap.Value;
    }


    //实现父类的DoAttack方法
    protected override IEnumerator DoAttack()
    {
        Debug.Log($"穿透形态攻击");
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    private void OnCoinChange(int count)
    {
        coin += count;
        if (coin >= 100)
        {
            Crystal.Instance.Recover(coin/100);
            coin = 0;
        }
    }

    protected override void EndBattle()
    {
        base.EndBattle();
        CurrencyManager.OnCoinChange -= OnCoinChange;
    }
}

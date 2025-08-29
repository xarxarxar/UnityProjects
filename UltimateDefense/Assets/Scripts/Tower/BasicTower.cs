using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础形态下的炮塔
/// </summary>
public class BasicTower : BaseTower
{
    public override TowerType TowerType { get => TowerType.Basic;}

    protected override void Init()
    {
        base.Init();
        WaveManager.OnWaveChanged += OnWaveChanged;
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
        JellySquash();
        CurrentBulletCount--;
        yield return null;
    }

    protected override void EndBattle()
    {
        base.EndBattle();
        WaveManager.OnWaveChanged -= OnWaveChanged;
    }

    private void OnWaveChanged(int value)
    {
        Crystal.Instance.Recover(Crystal.Instance.MaxHP.Value / 50);
    }
}

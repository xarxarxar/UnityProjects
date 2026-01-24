
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 进入狂暴模式，攻速临时翻倍
/// </summary>
public class Upgrade_RageMode : UpgradeBase
{
    #region 私有字段
    private float times;//赠送的子弹倍数

    #endregion
    /// <summary>
    /// 时长
    /// </summary>
    public float Times { get => times; }
    private static readonly object RageModeSource = new object();
    #region 构造函数

    /// <summary>
    /// 构造新的升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_RageMode(float times, int cost)
    {
        this.times = times;
        UpgradeID = $"RageMode{Times}";
        Cost = cost;
        //int givedBulletCount = Mathf.RoundToInt(TowerManager.Instance.CurrentTower.BulletCap* times);
        Description = $"临时赠送{times}倍弹夹容量，进入火力全开模式";
    }

    public static Upgrade_RageMode CreateDynamicUpgrade()
    {
        float[] times = { 2, 3, 4 };//赠送子弹是当前弹夹容量的多少倍
        float bonus = times[Random.Range(0, times.Length)];
        int cost = Mathf.RoundToInt(bonus * (7 + UpgradeManager.Instance.RefreshCount.Value * 3.5f));
        return new Upgrade_RageMode(bonus, cost);
    }
    #endregion

    /// <summary>
    /// 应用本次升级效果：对当前场上所有塔的 BaseAttack 增加 _attackBonus，
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        
        int givedBulletCount = Mathf.RoundToInt(TowerManager.Instance.CurrentTower.BulletCap * times);
        TowerManager.Instance.CurrentTower.GiveTmpBullet(givedBulletCount, () =>
        {
            TowerManager.Instance.AtkRateBuff.MultiChain.RemoveBuff(RageModeSource);
        });
        if (TowerManager.Instance.AtkRateBuff.MultiChain.HasBuff(RageModeSource)) return;
        TowerManager.Instance.AtkRateBuff.MultiChain.AddBuff(new BuffModifier
        {
            Value= 2.5f,
            Source= RageModeSource,
        });

    }
}

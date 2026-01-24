using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 设置水晶无敌状态
/// </summary>
public class Upgrade_SetCrystalInvincible : UpgradeBase
{
    #region 私有字段
    private float duration;//水晶无敌时长

    #endregion
    /// <summary>
    /// 水晶无敌时长
    /// </summary>
    public float Duration { get => duration; }
    #region 构造函数

    /// <summary>
    /// 构造新的“增加攻击力”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_SetCrystalInvincible(float dur, int cost)
    {
        duration = dur;
        UpgradeID = $"SetCrystalInvincible{duration}";
        Cost = cost;
        Description = $"水晶无敌{duration}秒,不可叠加";
    }

    public static Upgrade_SetCrystalInvincible CreateDynamicUpgrade()
    {
        float[] times = { 10, 15, 20 };
        float bonus = times[Random.Range(0, times.Length)];
        int cost = Mathf.RoundToInt(bonus * (3 + UpgradeManager.Instance.RefreshCount.Value * 1));
        return new Upgrade_SetCrystalInvincible(bonus, cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 应用本次升级效果：对当前场上所有塔的 BaseAttack 增加 _attackBonus，
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        Crystal.Instance.SetInvincible(Duration);
        
    }

    #endregion
}

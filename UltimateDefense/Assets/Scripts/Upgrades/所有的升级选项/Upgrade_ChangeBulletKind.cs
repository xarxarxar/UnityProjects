using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 修改子弹类型
/// </summary>
public class Upgrade_ChangeBulletKind : UpgradeBase
{
    BulletKind bulletKind;
    public BulletKind BulletKind { get => bulletKind; set => bulletKind = value; }


    #region 构造函数

    /// <summary>
    /// 构造新的“增加暴击伤害”升级实例
    /// </summary>
    /// <param name="attackRateBonus">要减少的的换弹时长数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_ChangeBulletKind(BulletKind kind,int cost)
    {
        bulletKind = kind;
        UpgradeID = $"ChangeBulletKind{kind}";
        Cost = cost;
        if (kind == BulletKind.Ice)
        {
            Description = $"特殊子弹：{TowerManager.Instance.BulletKindCount}颗干冰子弹，对敌人造成减速";
        }
        else if (kind == BulletKind.Fire)
        {
            Description = $"特殊子弹：{TowerManager.Instance.BulletKindCount}颗火焰子弹，对敌人造成持续伤害";
        }
        else if (kind == BulletKind.Electric)
        {
            Description = $"特殊子弹：{TowerManager.Instance.BulletKindCount}颗电击子弹，对敌人造成短暂眩晕";
        }

    }

   
    public static Upgrade_ChangeBulletKind CreateDynamicUpgrade()
    {
        // 随机获取一个 EnemyType 枚举值
        var types = Enum.GetValues(typeof(BulletKind)).Cast<BulletKind>()
                .Where(t => t != BulletKind.Normal) // 过滤掉 Normal
                .ToArray();

        BulletKind randomType = types[UnityEngine.Random.Range(0, types.Length)];
        int cost = Mathf.RoundToInt(80+ UpgradeManager.Instance.RefreshCount.Value * 5);
        return new Upgrade_ChangeBulletKind(randomType,cost);
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 并通知 TowerManager 记录全局加成，保证后续新塔继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        TowerManager.Instance.ChangeBullet(bulletKind);
    }

    #endregion
}

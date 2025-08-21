using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 随机选择几个敌人，将其当前生命值减半
/// </summary>
public class Upgrade_SetEnemyHalf : UpgradeBase
{
    #region 私有字段
    private int count;//选择的敌人个数

    #endregion
    /// <summary>
    /// 水晶无敌时长
    /// </summary>
    public int Count { get => count; }
    #region 构造函数

    /// <summary>
    /// 构造新的升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的攻击力数值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_SetEnemyHalf(int cou, int cost)
    {
        count = cou;
        UpgradeID = $"SetEnemyHalf{count}";
        Cost = cost;
        Description = $"随机选择{count}个敌人，生命值减半";
    }



    public static Upgrade_SetEnemyHalf CreateDynamicUpgrade()
    {
        int[] counts = { 10, 15, 20 };
        int bonus = counts[Random.Range(0, counts.Length)];
        int cost = Mathf.RoundToInt(bonus * (3 + UpgradeManager.Instance.RefreshCount.Value * 1));
        return new Upgrade_SetEnemyHalf(bonus, cost);
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
        if (EnemyManager.Instance.EnemyCurrentCount.Value <= 0) return;

        if(EnemyManager.Instance.EnemyCurrentCount.Value <= count)
        {
            for(int i = 0; i < EnemyManager.Instance.AllEnemies.Count; i++)
            {
                int damage = Mathf.Max(1, Mathf.RoundToInt(EnemyManager.Instance.AllEnemies[i].CurrentHP.Value / 2));
                EnemyManager.Instance.AllEnemies[i].TakeDamage(false, damage);
            }
        }
        else
        {
            // 敌人数量多于 count，随机选 count 个
            List<Enemy> shuffled = EnemyManager.Instance.AllEnemies.ToList(); // 转成 List 复制
            shuffled = shuffled.OrderBy(e => Random.value).Take(count).ToList();
            foreach (var enemy in shuffled)
            {
                int damage = Mathf.RoundToInt(enemy.CurrentHP.Value / 2f);
                enemy.TakeDamage(false, damage);
            }
        }
    }


    #endregion
}

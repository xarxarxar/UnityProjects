using UnityEngine;

/// <summary>
/// 增加敌人死亡获取金币的概率
/// </summary>
public class Upgrade_IncreaseEnemyDieCoinProb : UpgradeBase
{
    #region 私有字段

    private float _enemyDieCoinProbBonus;    // 本次升级增加的敌人死亡获取金币的概率值

    #endregion

    #region 公开属性

    /// <summary>
    /// 外部可读取本次升级增加的敌人死亡获取金币的概率值
    /// </summary>
    public float EnemyDieCoinProbBonus => _enemyDieCoinProbBonus;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造新的“增加敌人死亡获取金币的概率值”升级实例
    /// </summary>
    /// <param name="attackBonus">要增加的概率值</param>
    /// <param name="cost">消耗的金币数</param>
    public Upgrade_IncreaseEnemyDieCoinProb(float bonus, int cost)
    {
        _enemyDieCoinProbBonus = bonus;
        UpgradeID = $"IncreaseEnemyDieCoinProb{_enemyDieCoinProbBonus}";
        Cost = cost;
        Description = $"敌人掉落金币概率 +{_enemyDieCoinProbBonus * 100}%";
    }

    public static Upgrade_IncreaseEnemyDieCoinProb CreateDynamicUpgrade()
    {
        float[] values = { 0.02f, 0.05f, 0.1f };
        float bonus = values[Random.Range(0, values.Length)];
        int cost = Mathf.RoundToInt(bonus * (1000 + UpgradeManager.Instance.RefreshCount.Value * 300));
        return new Upgrade_IncreaseEnemyDieCoinProb(bonus, cost);
    }

    #endregion

    #region 公共方法
    public override bool IsAvailable()
    {
        return EnemyManager.Instance.EnemyDieCoinProb.Value <0.8f;
    }

    /// <summary>
    /// 应用本次升级效果
    /// 并通知 EnemyManager记录全局加成，保证后续继承
    /// </summary>
    public override void Apply()
    {
        // 通知 Manager 保存全局加成
        EnemyManager.Instance.EnemyDieCoinProb.Value += _enemyDieCoinProbBonus;
    }

    #endregion
}

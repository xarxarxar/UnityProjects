using System;
using System.Collections.Generic;
using UnityEngine;

// 1. 用来区分不同的成就
public enum AchievementType
{
    ContinuousVictory,   // 全胜通关
    TotalLogin,     // 累计登录
    ContinuousLogin,     // 连续登录
    EndlessModeLevel,    // 无尽模式达到多少关
    SameVictoryCount,    //相同手势获胜多少次
    OppositeVictoryCount,//相反手势获胜多少次
    RPSVictoryCount,     //石头剪刀布获胜多少次
    HalfVictoryCount,     //时间过半前获胜多少次
    PKbuttonVictoryCount,     //提前点击PK按钮获胜多少次
}

// 2. 定义“成就档位”
//    每个档位都有一个阈值和一个奖励描述（可扩展为具体奖励数据结构）
[Serializable]
public class AchievementTier
{
    public int threshold;          // 例如 1、7、30
    //public string rewardDesc;      // 档位解锁时给玩家的奖励说明
}

// 3. 每种成就的配置：哪些档位，是否已经解锁
[Serializable]
public class AchievementConfig
{
    public AchievementType type;
    public string smallDecsription;//小字进行描述
    public List<AchievementTier> tiers = new List<AchievementTier>();

    // 运行时状态
    [NonSerialized] public int currentValue = 0;             // 比如当前已登录天数
    [NonSerialized] public bool[] unlockedTiers;            // 每个 tiers[i] 是否已领取

    public void Initialize()
    {
        unlockedTiers = new bool[tiers.Count];
    }
}

// 4. 管理器：监听事件，更新进度、发放奖励
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("在 Inspector 里配置各个成就及其档位阈值和奖励")]
    public List<AchievementConfig> configs;


    private void Awake()
    {
        instance = this;
        foreach (var cfg in configs)
            cfg.Initialize();
    }

    private void Start()
    {
        // 如果需要持久化，加载 saved currentValue & unlockedTiers 数据
    }


    // 外部调用：记录一次全胜通关
    public void OnVictory()
    {
        UpdateAchievement(AchievementType.ContinuousVictory, 1);
    }

    // 记录登录
    public void OnLogin()
    {
        UpdateAchievement(AchievementType.ContinuousLogin, 1);
    }

    // 无尽模式到达新关
    public void OnEndlessLevel(int level)
    {
        // 这里直接将 level 赋值为进度
        SetAchievementValue(AchievementType.EndlessModeLevel, level);
    }

    // 通用方法：增加进度
    private void UpdateAchievement(AchievementType type, int delta)
    {
        var cfg = configs.Find(c => c.type == type);
        if (cfg == null) return;

        cfg.currentValue += delta;
        CheckTiers(cfg);
    }

    // 通用方法：直接设置进度（如无尽模式关数）
    private void SetAchievementValue(AchievementType type, int value)
    {
        var cfg = configs.Find(c => c.type == type);
        if (cfg == null) return;

        cfg.currentValue = Mathf.Max(cfg.currentValue, value);
        CheckTiers(cfg);
    }

    // 检查并发放所有新解锁的档位
    private void CheckTiers(AchievementConfig cfg)
    {
        for (int i = 0; i < cfg.tiers.Count; i++)
        {
            if (!cfg.unlockedTiers[i] && cfg.currentValue >= cfg.tiers[i].threshold)
            {
                cfg.unlockedTiers[i] = true;
                GrantReward(cfg.type, cfg.tiers[i]);
            }
        }
        // 如果所有 tiers 都已解锁，就是“彻底完成”了
        bool allDone = Array.TrueForAll(cfg.unlockedTiers, b => b);
        if (allDone)
        {
            Debug.Log($"成就 {cfg.type} 已彻底完成！");
        }
    }

    /// <summary>
    /// 判断成就是否全部完成（所有阶段都已解锁）
    /// </summary>
    private bool IsCompleted(AchievementConfig config)
    {
        foreach (bool unlocked in config.unlockedTiers)
        {
            if (!unlocked)
                return false;
        }
        return true;
    }

    // 发放奖励（这里只做示例打印，实际可以弹窗、加道具等）
    private void GrantReward(AchievementType type, AchievementTier tier)
    {
        //Debug.Log($"[{type}] 达到 {tier.threshold}，奖励：{tier.rewardDesc}");
        // TODO: 真实奖励逻辑（UI 提示、道具发放等）
    }

    private void OnApplicationQuit()
    {
        // 如果需要持久化， 在这里保存各 cfg.currentValue & cfg.unlockedTiers 数据
    }
}

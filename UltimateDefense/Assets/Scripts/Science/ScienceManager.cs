using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class ScienceManager : ManagerBase<ScienceManager>,IManager
{
    [SerializeField]
    private Bindable<int> _unlockIndex=new Bindable<int>();//玩家已解锁的科技index,0表示一个都未解锁
    private readonly ScienceNodeData[] baseSciences = new ScienceNodeData[]//基础科技
    {
        new ScienceNodeData { description = "炮塔攻击力+1", effectType = ScienceEffectType.IncreaseDamageFlat, value = 1,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "每秒攻击次数+0.1", effectType = ScienceEffectType.IncreaseAttackSpeedPct, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.1秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.2秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.3秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.4秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.5秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.6秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "换弹时间-0.7秒", effectType = ScienceEffectType.ReduceReloadTime, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        // 可以继续添加 6 个作为第 4~9 个
    };

    private readonly string[] skinNames = new string[]//特殊科技
    {
        "青铜火炮", "白银火炮", "黄金火炮", "白金火炮", "黑曜火炮",
        "星耀火炮", "王者火炮", "荣耀火炮", "传说火炮", "终极火炮"
    };
    /// <summary>
    /// 玩家已解锁的科技index
    /// </summary>
    public Bindable<int> UnlockIndex { get => _unlockIndex; set => _unlockIndex = value; }

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;//局外Manager
    }

    public override void Init()
    {
        _unlockIndex.Value=-1;//表示一个都没解锁
    }

    /// <summary>
    /// 应用科技点数
    /// </summary>
    public void ApplyScience()
    {
        for (int i = 0; i <= _unlockIndex.Value; i++)
        {
            ApplySignleScience(i);
        }
    }

    /// <summary>
    /// 解锁最新的一个科技点
    /// </summary>
    public void UnlockScience(int index)
    {
        if (index != _unlockIndex.Value + 1) return;
        _unlockIndex.Value++;
        DataManager.Instance.PlayerInfo.UnlockCount++;
    }

    /// <summary>
    /// 通过index获取sciencedata信息
    /// </summary>
    /// <returns></returns>
    public ScienceNodeData GetScienceDataByIndex(int index)
    {
        if(index<0) return null;
        ScienceNodeData scienceData = null;
        if (index % 10 != 9)
        {
            scienceData = baseSciences[index % 10];
        }
        else
        {
            // 特殊科技：创建一个新的 ScienceData 对象
            scienceData = new ScienceNodeData
            {
                //name = "测试皮肤",
                description = "测试皮肤描述",
                effectType = ScienceEffectType.UnlockSkin,
                value = 0,
                costType = RewardType.Crown,
                cost = 1,
                extraData = skinNames[(index / 10) % skinNames.Length] // 注意皮肤 index 对应关系
            };
        }
        return scienceData;
    }

    /// <summary>
    /// 根据index应用单个科技，index从1开始
    /// </summary>
    /// <param name="index"></param>
    private void ApplySignleScience(int index)
    {
        ScienceNodeData scienceData= GetScienceDataByIndex(index);
        switch(scienceData.effectType)
        {
            case ScienceEffectType.IncreaseDamageFlat:
                TowerManager.Instance.GlobalAttackBonus.Value +=Mathf.RoundToInt(scienceData.value);
                break;

            case ScienceEffectType.IncreaseAttackSpeedPct:
                TowerManager.Instance.GlobalAttackSpeedMultiplier.Value +=scienceData.value;
                break;

            case ScienceEffectType.ReduceReloadTime:
                TowerManager.Instance.GlobalReloadTime.Value -=scienceData.value;
                break;

            case ScienceEffectType.UnlockSkin:
                //SkinManager.Instance.UnlockSkin(data.extraData); // 传皮肤名
                break;
        }
    }

    
}

public class ScienceNodeData
{
    //public string name;
    public string description;
    public ScienceEffectType effectType;
    public float value;
    public string extraData; // 用于特殊科技，如皮肤名
    public RewardType costType;//花费的货币类型
    public int cost;//花费的数量

    public ScienceNodeData() { }

    public ScienceNodeData(ScienceNodeData other)
    {
        //name = other.name;
        description = other.description;
        effectType = other.effectType;
        value = other.value;
        extraData = other.extraData;
    }
}

public enum ScienceEffectType
{
    IncreaseDamageFlat,      // +1伤害
    IncreaseAttackSpeedPct,  // +1%攻速
    ReduceReloadTime,        // -0.1秒换弹
    UnlockSkin               // 解锁皮肤
}

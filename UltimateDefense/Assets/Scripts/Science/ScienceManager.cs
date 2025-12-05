using UnityEngine;

public class ScienceManager : ManagerBase<ScienceManager>
{
    public override string Description { get; } = "管理科技树，局外的Manager";
    [SerializeField]
    //private Bindable<int> _unlockIndex=new Bindable<int>();//玩家已解锁的科技index,0表示一个都未解锁
    private readonly ScienceNodeData[] baseSciences = new ScienceNodeData[]//基础科技
    {
        new ScienceNodeData { description = "城墙初始最大血量+100", effectType = ScienceEffectType.CrystalMaxHP,costType=RewardType.Diamond,cost=100 },
        new ScienceNodeData { description = "每次刷新所需金币-1", effectType = ScienceEffectType.FreshCoinCount,costType=RewardType.Diamond,cost=100 },
        new ScienceNodeData { description = "敌人初始掉落金币+1", effectType = ScienceEffectType.EnemyDieCount,costType=RewardType.Diamond,cost=100 },
        new ScienceNodeData { description = "对局初始金币+100", effectType = ScienceEffectType.DefaultCoinCount,costType=RewardType.Diamond,cost=100 },
        new ScienceNodeData { description = "刷新出的Buff相同时，固定折扣-5%", effectType = ScienceEffectType.Discount,costType=RewardType.Diamond,cost=100 },
        // 可以继续添加 6 个作为第 4~9 个
    };


    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;//局外Manager
    }

    public override void Init()
    {
        
    }

    /// <summary>
    /// 获取某个科技被升级了几次
    /// </summary>
    /// <param name="index"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetUpgradeCountByType(ScienceEffectType type)
    {
        int totalCount = baseSciences.Length;
        int unlockedCount = DataManager.Instance.PlayerInfo.UnlockCount.Value; // 已解锁数量（不包含当前 index）

        // 先找出此类型的顺序位置
        int typeIndex = -1;
        for (int i = 0; i < totalCount; i++)
        {
            if (baseSciences[i].effectType == type)
            {
                typeIndex = i;
                break;
            }
        }

        if (typeIndex == -1)
        {
            Debug.LogError($"未找到类型 {type} 对应的科技节点！");
            return 0;
        }

        // 计算完整循环次数 + 当前循环内是否轮到这个节点
        int fullCycles = unlockedCount / totalCount;
        int remainder = unlockedCount % totalCount;

        int upgradeCount = fullCycles + (typeIndex < remainder ? 1 : 0);
        return upgradeCount;
    }


    /// <summary>
    /// 解锁最新的一个科技点
    /// </summary>
    public void UnlockScience(int index)
    {
        if (index != DataManager.Instance.PlayerInfo.UnlockCount.Value) return;
        RewardType rewardType= GetScienceDataByIndex(index).costType;
        int count= GetScienceDataByIndex(index).cost;
        if (!MetaCurrencyManager.Instance.HasEnoughMoney(rewardType, count) )
        {
            if(rewardType==RewardType.Diamond)
            {
                TipManager.Instance.ShowTip("钻石不足");
            }
            else if(rewardType==RewardType.Crown)
            {
                TipManager.Instance.ShowTip("王冠不足");
            }
            return;
        }
        DataManager.Instance.PlayerInfo.UnlockCount.Value++;
        DataManager.Instance.SavePlayerInfo();
    }

    /// <summary>
    /// 通过index获取sciencedata信息
    /// </summary>
    /// <returns></returns>
    public ScienceNodeData GetScienceDataByIndex(int index)
    {
        if(index<0) return null;
        ScienceNodeData scienceData = null;

        scienceData = baseSciences[index % 5];
        return scienceData;
    }
}

public class ScienceNodeData
{
    //public string name;
    public string description;
    public ScienceEffectType effectType;
    public string extraData; // 用于特殊科技，如皮肤名
    public RewardType costType;//花费的货币类型
    public int cost;//花费的数量

    public ScienceNodeData() { }

    public ScienceNodeData(ScienceNodeData other)
    {
        //name = other.name;
        description = other.description;
        effectType = other.effectType;
        extraData = other.extraData;
    }
}

public enum ScienceEffectType
{
    CrystalMaxHP,      // +100城墙最大血量
    FreshCoinCount,  // -1每次刷新所需金币
    EnemyDieCount,        // +1敌人掉落金币
    DefaultCoinCount,               // +100初始金币
    Discount,               // -10%固定折扣
}

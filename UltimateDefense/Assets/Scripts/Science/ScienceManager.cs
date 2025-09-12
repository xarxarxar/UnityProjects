using UnityEngine;

public class ScienceManager : ManagerBase<ScienceManager>
{
    public override string Description { get; } = "管理科技树，局外的Manager";
    [SerializeField]
    //private Bindable<int> _unlockIndex=new Bindable<int>();//玩家已解锁的科技index,0表示一个都未解锁
    private readonly ScienceNodeData[] baseSciences = new ScienceNodeData[]//基础科技
    {
        new ScienceNodeData { description = "城墙初始最大血量+100", effectType = ScienceEffectType.CrystalMaxHP, value = 1,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "每次刷新所需金币-1", effectType = ScienceEffectType.FreshCoinCount, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "敌人初始掉落金币+1", effectType = ScienceEffectType.EnemyDieCount, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "对局初始金币+100", effectType = ScienceEffectType.DefaultCoinCount, value = 0.1f,costType=RewardType.Diamond,cost=1 },
        new ScienceNodeData { description = "刷新出的Buff相同时，固定折扣-10%", effectType = ScienceEffectType.Discount, value = 0.1f,costType=RewardType.Diamond,cost=1 },
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
    //public Bindable<int> UnlockIndex => DataManager.Instance.PlayerInfo.UnlockCount;

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;//局外Manager
    }

    public override void Init()
    {
        //DataManager.Instance.PlayerInfo.UnlockCount.Value=-1;//表示一个都没解锁
    }

    /// <summary>
    /// 应用科技点数
    /// </summary>
    //public void ApplyScience()
    //{
    //    for (int i = 0; i <= DataManager.Instance.PlayerInfo.UnlockCount.Value; i++)
    //    {
    //        ApplySignleScience(i);
    //    }
    //}

    /// <summary>
    /// 解锁最新的一个科技点
    /// </summary>
    public void UnlockScience(int index)
    {
        if (index != DataManager.Instance.PlayerInfo.UnlockCount.Value + 1) return;
        RewardType rewardType= GetScienceDataByIndex(index).costType;
        int count= GetScienceDataByIndex(index).cost;
        if (!MetaCurrencyManager.Instance.HasEnoughMoney(rewardType, count) )
        {
            if(rewardType==RewardType.Diamond)
            {
                GameUIManager.Instance.ShowQuickTip("钻石不足");
            }
            else if(rewardType==RewardType.Crown)
            {
                GameUIManager.Instance.ShowQuickTip("王冠不足");
            }
            return;
        }
        DataManager.Instance.PlayerInfo.UnlockCount.Value++;
        ApplySignleScience(DataManager.Instance.PlayerInfo.UnlockCount.Value);
        //DataManager.Instance.PlayerInfo.UnlockCount.Value++;
    }

    /// <summary>
    /// 通过index获取sciencedata信息
    /// </summary>
    /// <returns></returns>
    public ScienceNodeData GetScienceDataByIndex(int index)
    {
        if(index<0) return null;
        ScienceNodeData scienceData = null;
        //if (index % 10 != 9)
        //{
        //    scienceData = baseSciences[index % 10];
        //}
        //else
        //{
        //    // 特殊科技：创建一个新的 ScienceData 对象
        //    scienceData = new ScienceNodeData
        //    {
        //        //name = "测试皮肤",
        //        description = "测试皮肤描述",
        //        effectType = ScienceEffectType.UnlockSkin,
        //        value = 0,
        //        costType = RewardType.Crown,
        //        cost = 1,
        //        extraData = skinNames[(index / 10) % skinNames.Length] // 注意皮肤 index 对应关系
        //    };
        //}
        scienceData = baseSciences[index % 5];
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
            case ScienceEffectType.CrystalMaxHP:
                if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("CrystalMaxHp"))
                {
                    DataManager.Instance.PlayerInfo.Config["CrystalMaxHp"] = 1000;
                }
                DataManager.Instance.PlayerInfo.Config["CrystalMaxHp"] += 100;
                break;

            case ScienceEffectType.FreshCoinCount:
                break;

            case ScienceEffectType.EnemyDieCount:
                if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("EnemyDieCoin"))
                {
                    DataManager.Instance.PlayerInfo.Config["EnemyDieCoin"] = 10;
                }
                DataManager.Instance.PlayerInfo.Config["EnemyDieCoin"] += 1;
                break;

            case ScienceEffectType.DefaultCoinCount:
                if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("InitialCoin"))
                {
                    DataManager.Instance.PlayerInfo.Config["InitialCoin"] = 1000;
                }
                DataManager.Instance.PlayerInfo.Config["InitialCoin"] += 100;
                break;
            case ScienceEffectType.Discount:
                if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("SameDiscount"))
                {
                    DataManager.Instance.PlayerInfo.Config["SameDiscount"] = 0.5f;
                }
                DataManager.Instance.PlayerInfo.Config["SameDiscount"] -= 0.1f;
                break;
        }
        DataManager.Instance.SavePlayerInfo();
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
    CrystalMaxHP,      // +100城墙最大血量
    FreshCoinCount,  // -1每次刷新所需金币
    EnemyDieCount,        // +1敌人掉落金币
    DefaultCoinCount,               // +100初始金币
    Discount,               // -10%固定折扣
}

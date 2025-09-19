
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炮塔数据的管理
/// </summary>
public class TowerDataManager : ManagerBase<TowerDataManager>   
{
    [SerializeField] private TowerType _currentTowerData;
    public Dictionary<TowerType,string> TowerDiscription=new Dictionary<TowerType, string>();
    /// <summary>
    /// 当前对局使用的TowerType
    /// </summary>
    //DataManager.Instance.PlayerInfo.CurrentTowerType;

    public override void Init()
    {
        UpdateDescription();
    }



    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;

    }

    /// <summary>
    /// 获取塔的数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TowerData GetTowerData(TowerType type)
    {
        int towerLevel = DataManager.Instance.PlayerInfo.TowerDatas[type];
        return new TowerData
        {
            Level = towerLevel,
            BaseDamage = GetBaseDamage(type, towerLevel),
            BaseCap=GetBaseCap(type, towerLevel),
            BaseAtkRate=GetBaseAtkRate(type, towerLevel),
            BaseReload=GetBaseReload(type, towerLevel),
            BaseCritProb=GetBaseCritProb(type, towerLevel),
            BaseCritMult=GetBaseCritMult(type, towerLevel)
        };
            

    }

    /// <summary>
    /// 解锁炮塔
    /// </summary>
    public void UnlockTower(TowerType type)
    {
        if(!DataManager.Instance.PlayerInfo.TowerDatas.ContainsKey(type)) return;
        if (DataManager.Instance.PlayerInfo.TowerDatas[type]!=0) return;

        DataManager.Instance.PlayerInfo.TowerDatas[type]++;
        DataManager.Instance.SavePlayerInfo();
    }

    /// <summary>
    /// 升级炮塔
    /// </summary>
    /// <param name="type"></param>
    public void UpgradeTower(TowerType type)
    {
        if (!DataManager.Instance.PlayerInfo.TowerDatas.ContainsKey(type)) return;
        if (DataManager.Instance.PlayerInfo.TowerDatas[type] >=TowerData.MaxLevel) return;

        DataManager.Instance.PlayerInfo.TowerDatas[type]++;
        UpdateDescription();
        DataManager.Instance.SavePlayerInfo();
    }

    //更新炮塔描述
    private void UpdateDescription()
    {
        TowerDiscription[TowerType.Basic] = $"每次发射单颗子弹,对敌人造成<color=#F4C760>{GetTowerData(TowerType.Basic).BaseDamage / 10f}</color>点伤害\r\n\r\n被动：每回合恢复城墙最大生命值2%的血量";
        TowerDiscription[TowerType.Ricochet] = $"子弹对第一个敌人造成<color=#F4C760>{GetTowerData(TowerType.Ricochet).BaseDamage / 10f}</color>点伤害，额外弹射2个敌人,每次弹射伤害衰减30%\r\n\r\n被动：每消灭3个敌人为城墙恢复1点生命值";
        TowerDiscription[TowerType.Spread] = $"每次并排发射三颗子弹，每颗子弹伤害为<color=#F4C760>{GetTowerData(TowerType.Spread).BaseDamage / 10f}</color>，初始暴击率翻倍,初始暴击伤害降低\r\n被动：每次暴击为城墙恢复1点血";
        TowerDiscription[TowerType.Piercing] = $"子弹初始伤害为<color=#F4C760>{GetTowerData(TowerType.Piercing).BaseDamage / 10f}</color>，可以穿透敌人\r\n被动：每造成50点伤害恢复城墙1点生命";
        TowerDiscription[TowerType.RapidFire] = $"每次射出两颗子弹，每颗子弹伤害为<color=#F4C760>{GetTowerData(TowerType.RapidFire).BaseDamage / 10f}</color>，初始换弹时间减少\r\n\r\n被动：每发射200颗子弹恢复城墙1点生命值";
        //TowerDiscription[TowerType.Sniper] = $"初始射速降低，子弹初始伤害为<color=#F4C760>{GetTowerData(TowerType.Sniper).BaseDamage / 10f}</color>，炮塔范围变为全屏\r\n被动：每造成50点伤害恢复城墙1点生命";
    }

    /// <summary>
    /// 获取炮塔的基础伤害
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int GetBaseDamage(TowerType towerType,int level)
    {
        return towerType switch
        {
            TowerType.Basic => 10 + (level-1) * 5,//max为55
            TowerType.RapidFire => 5 + (level-1) * 3,//max为32
            TowerType.Ricochet => 7 + (level - 1) * 2,//max为25，衰减为30%
            TowerType.Spread => 4 + (level-1) * 2,//max为22
            TowerType.Sniper => 20 + (level - 1) * 6,//max为69
            TowerType.Piercing => 8 + (level-1) * 3,//max为35
            _ => 0
        };
    }

    /// <summary>
    /// 获取炮塔的基础弹夹容量
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int GetBaseCap(TowerType towerType, int level)
    {
        return towerType switch
        {
            TowerType.Basic => 20,// + (level - 1) * 1,
            TowerType.RapidFire => 40,// + (level - 1) * 1,
            TowerType.Ricochet => 20,// + (level - 1) * 4,
            TowerType.Spread => 30,// + (level - 1) * 4,
            TowerType.Sniper => 10,// + (level - 1) * 4,
            TowerType.Piercing => 15,// + (level - 1) * 4,
            _ => 0
        };
    }

    /// <summary>
    /// 获取炮塔的基础每秒攻击次数
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float GetBaseAtkRate(TowerType towerType, int level)
    {
        return towerType switch
        {
            TowerType.Basic => 1.2f,// + (level - 1) * 0.1f,
            TowerType.RapidFire => 1.2f,// + (level - 1) * 0.1f,
            TowerType.Ricochet => 1.2f,// + (level - 1) * 4,
            TowerType.Spread => 1.2f,// + (level - 1) * 4,
            TowerType.Sniper => 0.7f,// + (level - 1) * 4,
            TowerType.Piercing => 1.2f,// + (level - 1) * 4,
            _ => 0
        };
    }

    /// <summary>
    /// 获取炮塔的基础换弹时间
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float GetBaseReload(TowerType towerType, int level)
    {
        return towerType switch
        {
            TowerType.Basic => 3,// + (level - 1) * 0.1f,
            TowerType.RapidFire => 2.5f,// + (level - 1) * 0.1f,
            TowerType.Ricochet => 3,// + (level - 1) * 4,
            TowerType.Spread => 3,// + (level - 1) * 4,
            TowerType.Sniper => 3,// + (level - 1) * 4,
            TowerType.Piercing => 3,// + (level - 1) * 4,
            _ => 0
        };
    }

    /// <summary>
    /// 获取炮塔的基础暴击率
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float GetBaseCritProb(TowerType towerType, int level)
    {
        return towerType switch
        {
            TowerType.Basic => 0.1f,// + (level - 1) * 0.05f,
            TowerType.RapidFire => 0.1f,// + (level - 1) * 0.05f,
            TowerType.Ricochet => 0.1f,// + (level - 1) * 4,
            TowerType.Spread => 0.3f,// + (level - 1) * 4,
            TowerType.Sniper => 0.1f,// + (level - 1) * 4,
            TowerType.Piercing => 0.1f,// + (level - 1) * 4,
            _ => 0
        };
    }

    /// <summary>
    /// 获取炮塔的基础暴击伤害倍率
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float GetBaseCritMult(TowerType towerType, int level)
    {
        return towerType switch
        {
            TowerType.Basic => 1.5f,// + (level - 1) * 0.1f,
            TowerType.RapidFire => 1.5f,// + (level - 1) * 0.1f,
            TowerType.Ricochet => 1.5f,// + (level - 1) * 4,
            TowerType.Spread => 1.2f,// + (level - 1) * 4,
            TowerType.Sniper => 2.0f,// + (level - 1) * 4,
            TowerType.Piercing => 1.5f,// + (level - 1) * 4,
            _ => 0
        };
    }
}

[System.Serializable]
public class TowerData
{
    public int Level;
    public const int MaxLevel=10;

    //子类需重写值的逻辑，炮塔基础值，也就是未加Buff时候的值
    public int BaseDamage;    //子弹伤害
    public int BaseCap;       //子弹容量
    public float BaseAtkRate;  //每秒攻击次数
    public float BaseReload;  //换弹时长
    public float BaseCritProb; //暴击概率
    public float BaseCritMult; //暴击伤害倍率
}



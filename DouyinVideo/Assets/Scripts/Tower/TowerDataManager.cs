using SerializableDictionary.Scripts;
using UnityEngine;

/// <summary>
/// 炮塔数据的管理
/// </summary>
public class TowerDataManager : ManagerBase<TowerDataManager>,IManager
{
    [SerializeField] private TowerType _currentTowerData;
    /// <summary>
    /// 当前对局使用的TowerType
    /// </summary>
    public Bindable<TowerType> CurrentTowerType=>DataManager.Instance.PlayerInfo.CurrentTowerType;

    public override void Init()
    {
        
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
            TowerType.Basic => 1 + (level-1) * 1,
            TowerType.RapidFire => 6 + (level - 1) * 1,
            TowerType.Ricochet => 30 + (level - 1) * 4,
            TowerType.Spread => 30 + (level - 1) * 4,
            TowerType.Sniper => 30 + (level - 1) * 4,
            TowerType.Piercing => 30 + (level - 1) * 4,
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
            TowerType.Basic => 5 + (level - 1) * 1,
            TowerType.RapidFire => 20 + (level - 1) * 1,
            TowerType.Ricochet => 30 + (level - 1) * 4,
            TowerType.Spread => 30 + (level - 1) * 4,
            TowerType.Sniper => 30 + (level - 1) * 4,
            TowerType.Piercing => 30 + (level - 1) * 4,
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
            TowerType.Basic => 0.5f + (level - 1) * 0.1f,
            TowerType.RapidFire => 1.5f + (level - 1) * 0.1f,
            TowerType.Ricochet => 30 + (level - 1) * 4,
            TowerType.Spread => 30 + (level - 1) * 4,
            TowerType.Sniper => 30 + (level - 1) * 4,
            TowerType.Piercing => 30 + (level - 1) * 4,
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
            TowerType.Basic => 5 + (level - 1) * 0.1f,
            TowerType.RapidFire => 3.2f + (level - 1) * 0.1f,
            TowerType.Ricochet => 30 + (level - 1) * 4,
            TowerType.Spread => 30 + (level - 1) * 4,
            TowerType.Sniper => 30 + (level - 1) * 4,
            TowerType.Piercing => 30 + (level - 1) * 4,
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
            TowerType.Basic => 0.1f + (level - 1) * 0.05f,
            TowerType.RapidFire => 0.1f + (level - 1) * 0.05f,
            TowerType.Ricochet => 30 + (level - 1) * 4,
            TowerType.Spread => 30 + (level - 1) * 4,
            TowerType.Sniper => 30 + (level - 1) * 4,
            TowerType.Piercing => 30 + (level - 1) * 4,
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
            TowerType.Basic => 1.2f + (level - 1) * 0.1f,
            TowerType.RapidFire => 2.0f + (level - 1) * 0.1f,
            TowerType.Ricochet => 30 + (level - 1) * 4,
            TowerType.Spread => 30 + (level - 1) * 4,
            TowerType.Sniper => 30 + (level - 1) * 4,
            TowerType.Piercing => 30 + (level - 1) * 4,
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



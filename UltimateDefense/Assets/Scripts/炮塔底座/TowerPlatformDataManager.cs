using System.Collections;
using System.Collections.Generic;

public class TowerPlatformDataManager : ManagerBase<TowerPlatformDataManager>
{
    /// <summary>
    /// 所有的炮塔底座数据
    /// </summary>
    public List<TowerPlatformData> TowerPlatformDatas = new List<TowerPlatformData>();

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;
    }
    public override void Init()
    {
        EnsureTowerPlatformStates(TowerPlatformDatas);
    }

    /// <summary>
    /// 补齐玩家的TowerPlatformStateMap信息
    /// </summary>
    /// <param name="towerDatas"></param>
    public void EnsureTowerPlatformStates(IList<TowerPlatformData> towerDatas)
    {
        foreach (var tower in towerDatas)
        {
            int id = tower.ID;  // 你塔的唯一ID（你如果没有ID字段，我可以帮你加）

            // 玩家存档里没有这个塔 → 补一个默认状态
            if (!DataManager.Instance.PlayerInfo.TowerPlatformStateMap.ContainsKey(id))
            {
                DataManager.Instance.PlayerInfo.TowerPlatformStateMap[id] = new ItemState
                {
                    IsUnlocked = id == 0,
                    CurrentSkinID = 0,
                    OwnedSkins = new List<int>() { 0 } // 默认皮肤
                };
            }
        }
    }

    /// <summary>
    /// 获取某个TowerPlatformData在TowerPlatformDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerPlatformDataIndex(TowerPlatformData towerPlatformData)
    {
        return TowerPlatformDatas.FindIndex(t => t == towerPlatformData);
    }

    /// <summary>
    /// 获取某个ID的TowerPlatformData在TowerPlatformDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerPlatformDataIndex(int ID)
    {
        return TowerPlatformDatas.FindIndex(t => t.ID == ID);
    }


    /// <summary>
    /// 获取某个skin在某个TowerPlatformData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerPlatformData towerPlatformData, Skin skin)
    {
        return towerPlatformData.SkinData.skins.FindIndex(t => t == skin);
    }

    /// <summary>
    /// 获取某个ID的skinData在某个TowerPlatformData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerPlatformData towerPlatformData, int ID)
    {
        return towerPlatformData.SkinData.skins.FindIndex(t => t.ID == ID);
    }

    /// <summary>
    /// 通过index获取某个TowerPlatformData的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetSkin(TowerPlatformData towerPlatformData, int index)
    {
        return towerPlatformData.SkinData.skins[index];
    }

    /// <summary>
    /// 获取某个TowerPlatformData当前正在使用的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetCurrentSkin(TowerPlatformData towerPlatformData)
    {
        return towerPlatformData.SkinData.skins.Find(t => t.ID == DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformData.ID].CurrentSkinID);
    }

    /// <summary>
    /// 获取某个TowerPlatformData当前正在使用的skin的Index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSkinIndex(TowerPlatformData towerPlatformData)
    {
        return towerPlatformData.SkinData.skins.FindIndex(t => t.ID == DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformData.ID].CurrentSkinID);
    }

    /// <summary>
    /// 获取当前玩家使用的TowerPlatformData在TowerPlatformDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentTowerPlatformDataIndex()
    {
        int index = TowerPlatformDatas.FindIndex(t => t.ID == DataManager.Instance.PlayerInfo.CurrentTowerPlatformID.Value);
        return index;
    }

    /// <summary>
    /// 获取当前玩家使用的TowerDataPlatform
    /// </summary>
    /// <returns></returns>
    public TowerPlatformData GetCurrentTowerPlatformData()
    {
        TowerPlatformData towerPlatform = TowerPlatformDatas.Find(t => t.ID == DataManager.Instance.PlayerInfo.CurrentTowerPlatformID.Value);
        return towerPlatform;
    }

    /// <summary>
    /// 修改某个TowerPlatformData的当前skinID
    /// </summary>
    public void ChangeTowerPlatformCurrentSkinID(TowerPlatformData towerPlatformData, int skinID)
    {
        DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformData.ID].CurrentSkinID = skinID; // 保存选择
    }

    /// <summary>
    /// 判断某个TowerPlatformData是否有这个skin
    /// </summary>
    /// <returns></returns>
    public bool HasSkin(TowerPlatformData towerPlatformData, int ID)
    {
        if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(towerPlatformData.ID, out var state))
        {
            return state.OwnedSkins != null && state.OwnedSkins.Contains(ID);
        }
        return false;
    }

}

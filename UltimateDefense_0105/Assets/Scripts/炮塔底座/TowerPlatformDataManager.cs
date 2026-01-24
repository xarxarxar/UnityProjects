using System.Collections;
using System.Collections.Generic;

public class TowerPlatformDataManager : ManagerBase<TowerPlatformDataManager>
{
    /// <summary>
    /// 所有的炮塔底座数据
    /// </summary>
    public List<TowerPlatformData> TowerPlatformDatas = new List<TowerPlatformData>();

    private int CurrentTowerPlatformID => DataManager.Instance.PlayerInfo.CurrentTowerPlatformID;

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
            if (!DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(id, out var _itemState))
            {
                ItemState itemState = new ItemState();
                itemState.SetIsUnlocked(id == 0);
                itemState.SetCurrentSkinID(0);
                itemState.SetPieceCount(id == 0 ? -1 : 0);//-1代表无穷多个
                for (int i = 0; i < tower.SkinData.skins.Count; i++)
                {
                    SkinInfo skinInfo = new SkinInfo();
                    skinInfo.SetIsUnlocked(tower.SkinData.skins[i].ID == 0);
                    skinInfo.SetPieceCount(tower.SkinData.skins[i].ID == 0 ? -1 : 0);//-1代表无穷多个
                    itemState.SetSkins(tower.SkinData.skins[i].ID, skinInfo);
                }
                DataManager.Instance.PlayerInfo.SetTowerPlatformItemState(id, itemState);
            }
        }
    }

    /// <summary>
    /// 获取某个TowerPlatformData在TowerPlatformDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerPlatformDataIndex(TowerPlatformData towerPlatformData)
    {
        if (towerPlatformData == null)
            return -1;

        for (int i = 0; i < TowerPlatformDatas.Count; i++)
        {
            var data = TowerPlatformDatas[i];
            if (data == null)
                continue;

            if (data == towerPlatformData)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 获取某个ID的TowerPlatformData在TowerPlatformDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerPlatformDataIndex(int ID)
    {
        for (int i = 0; i < TowerPlatformDatas.Count; i++)
        {
            var data = TowerPlatformDatas[i];
            if (data == null)
                continue;

            if (data.ID == ID)
                return i;
        }

        return -1;
    }


    /// <summary>
    /// 获取某个skin在某个TowerPlatformData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerPlatformData towerPlatformData, Skin skin)
    {
        if (towerPlatformData == null ||
            towerPlatformData.SkinData == null ||
            towerPlatformData.SkinData.skins == null ||
            skin == null)
            return -1;

        var skins = towerPlatformData.SkinData.skins;
        for (int i = 0; i < skins.Count; i++)
        {
            var s = skins[i];
            if (s == null)
                continue;

            if (s == skin)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 获取某个ID的skinData在某个TowerPlatformData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerPlatformData towerPlatformData, int ID)
    {
        if (towerPlatformData == null ||
            towerPlatformData.SkinData == null ||
            towerPlatformData.SkinData.skins == null)
            return -1;

        var skins = towerPlatformData.SkinData.skins;
        for (int i = 0; i < skins.Count; i++)
        {
            var s = skins[i];
            if (s == null)
                continue;

            if (s.ID == ID)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 通过index获取某个TowerPlatformData的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetSkin(TowerPlatformData towerPlatformData, int index)
    {
        if (towerPlatformData == null ||
            towerPlatformData.SkinData == null ||
            towerPlatformData.SkinData.skins == null)
            return null;

        var skins = towerPlatformData.SkinData.skins;
        if (index < 0 || index >= skins.Count)
            return null;

        return skins[index];
    }

    /// <summary>
    /// 获取某个TowerPlatformData当前正在使用的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetCurrentSkin(TowerPlatformData towerPlatformData)
    {
        if (towerPlatformData == null ||
            towerPlatformData.SkinData == null ||
            towerPlatformData.SkinData.skins == null)
            return null;

        if (!DataManager.Instance.PlayerInfo.TowerPlatformStateMap
            .TryGetValue(towerPlatformData.ID, out var state))
            return null;

        int currentSkinId = state.CurrentSkinID;
        var skins = towerPlatformData.SkinData.skins;

        for (int i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            if (skin == null)
                continue;

            if (skin.ID == currentSkinId)
                return skin;
        }

        return null;
    }

    /// <summary>
    /// 获取某个TowerPlatformData当前正在使用的skin的Index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSkinIndex(TowerPlatformData towerPlatformData)
    {
        if (towerPlatformData == null ||
            towerPlatformData.SkinData == null ||
            towerPlatformData.SkinData.skins == null)
            return -1;

        if (!DataManager.Instance.PlayerInfo.TowerPlatformStateMap
            .TryGetValue(towerPlatformData.ID, out var state))
            return -1;

        int currentSkinId = state.CurrentSkinID;
        var skins = towerPlatformData.SkinData.skins;

        for (int i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            if (skin == null)
                continue;

            if (skin.ID == currentSkinId)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 获取当前玩家使用的TowerPlatformData在TowerPlatformDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentTowerPlatformDataIndex()
    {
        for (int i = 0; i < TowerPlatformDatas.Count; i++)
        {
            var data = TowerPlatformDatas[i];
            if (data == null)
                continue;

            if (data.ID == CurrentTowerPlatformID)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 获取当前玩家使用的TowerDataPlatform
    /// </summary>
    /// <returns></returns>
    public TowerPlatformData GetCurrentTowerPlatformData()
    {
        for (int i = 0; i < TowerPlatformDatas.Count; i++)
        {
            var data = TowerPlatformDatas[i];
            if (data == null)
                continue;

            if (data.ID == CurrentTowerPlatformID)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 修改某个TowerPlatformData的当前skinID
    /// </summary>
    public void ChangeTowerPlatformCurrentSkinID(TowerPlatformData towerPlatformData, int skinID)
    {
        DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformData.ID].SetCurrentSkinID(skinID); // 保存选择
    }

    /// <summary>
    /// 判断某个TowerPlatformData是否有这个skin
    /// </summary>
    /// <returns></returns>
    public bool HasSkin(TowerPlatformData towerPlatformData, int ID)
    {
        if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(towerPlatformData.ID, out var state)
            && state.AllSkins != null && state.AllSkins.TryGetValue(ID, out var skinState))
        {
            return skinState.IsUnlocked;
        }

        return false;
    }

}

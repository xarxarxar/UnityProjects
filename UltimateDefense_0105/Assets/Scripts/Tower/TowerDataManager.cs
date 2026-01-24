
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炮塔数据的管理
/// </summary>
public class TowerDataManager : ManagerBase<TowerDataManager>   
{
    /// <summary>
    /// 所有的炮塔数据
    /// </summary>
    public List<TowerData> TowerDatas = new List<TowerData>();


    public override void Init()
    {
        EnsureTowerStates(TowerDatas);
    }

    /// <summary>
    /// 补齐玩家的TowerStateMap信息
    /// </summary>
    /// <param name="towerDatas"></param>
    public void EnsureTowerStates(IList<TowerData> towerDatas)
    {
        foreach (var tower in towerDatas)
        {
            int id = tower.ID;  // 你塔的唯一ID（你如果没有ID字段，我可以帮你加）

            // 玩家存档里没有这个塔 → 补一个默认状态
            if (!DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(id, out var _itemState))
            {
                ItemState itemState = new ItemState();
                itemState.SetIsUnlocked(id == 0);
                itemState.SetCurrentSkinID(0);
                itemState.SetPieceCount(id==0?-1:0);//-1代表无穷多个
                for(int i = 0; i < tower.SkinData.skins.Count; i++)
                {
                    SkinInfo skinInfo=new SkinInfo();
                    skinInfo.SetIsUnlocked(tower.SkinData.skins[i].ID==0);
                    skinInfo.SetPieceCount(tower.SkinData.skins[i].ID == 0?-1:0);//-1代表无穷多个
                    itemState.SetSkins(tower.SkinData.skins[i].ID, skinInfo);
                }
                DataManager.Instance.PlayerInfo.SetTowerItemState(id, itemState);
            }
            else
            {
                for (int i = 0; i < tower.SkinData.skins.Count; i++)
                {
                    if (!_itemState.AllSkins.TryGetValue(tower.SkinData.skins[i].ID, out var skininfo))
                    {
                        SkinInfo skinInfo = new SkinInfo();
                        skinInfo.SetIsUnlocked(tower.SkinData.skins[i].ID == 0);
                        skinInfo.SetPieceCount(tower.SkinData.skins[i].ID == 0 ? -1 : 0);//-1代表无穷多个
                        _itemState.SetSkins(tower.SkinData.skins[i].ID, skinInfo);
                    }
                       
                }
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;
    }

    /// <summary>
    /// 获取某个TowerData在TowerDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerDataIndex(TowerData towerData)
    {
        if (towerData == null)
            return -1;

        for (int i = 0; i < TowerDatas.Count; i++)
        {
            var data = TowerDatas[i];
            if (data == null)
                continue;

            if (data == towerData)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 获取某个ID的TowerData在TowerDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerDataIndex(int ID)
    {
        for (int i = 0; i < TowerDatas.Count; i++)
        {
            var data = TowerDatas[i];
            if (data == null)
                continue;

            if (data.ID == ID)
                return i;
        }

        return -1;
    }


    /// <summary>
    /// 获取某个skin在某个TowerData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerData towerData, Skin skin)
    {
        if (towerData == null ||
            towerData.SkinData == null ||
            towerData.SkinData.skins == null ||
            skin == null)
            return -1;

        var skins = towerData.SkinData.skins;
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
    /// 获取某个ID的skinData在某个TowerData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerData towerData, int ID)
    {
        if (towerData == null ||
            towerData.SkinData == null ||
            towerData.SkinData.skins == null)
            return -1;

        var skins = towerData.SkinData.skins;
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
    /// 通过index获取某个towerdata的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetSkin(TowerData towerData, int index)
    {
        if (towerData == null ||
            towerData.SkinData == null ||
            towerData.SkinData.skins == null)
            return null;

        var skins = towerData.SkinData.skins;
        if (index < 0 || index >= skins.Count)
            return null;

        return skins[index];
    }

    /// <summary>
    /// 获取某个TowerData当前正在使用的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetCurrentSkin(TowerData towerData)
    {
        if (towerData == null ||
            towerData.SkinData == null ||
            towerData.SkinData.skins == null)
            return null;

        if (!DataManager.Instance.PlayerInfo.TowerStateMap
            .TryGetValue(towerData.ID, out var state))
            return null;

        int currentSkinId = state.CurrentSkinID;
        var skins = towerData.SkinData.skins;

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
    /// 获取某个ItemState当前正在使用的skin的Index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSkinIndex(SkinData skinData, ItemState itemState)
    {
        if (skinData == null ||
            skinData.skins == null ||
            itemState == null)
            return -1;

        int currentSkinId = itemState.CurrentSkinID;
        var skins = skinData.skins;

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
    /// 获取当前玩家使用的TowerData在TowerDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentTowerDataIndex()
    {
        int currentId = DataManager.Instance.PlayerInfo.CurrentTowerID;

        for (int i = 0; i < TowerDatas.Count; i++)
        {
            var data = TowerDatas[i];
            if (data == null)
                continue;

            if (data.ID == currentId)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 获取当前玩家使用的TowerData
    /// </summary>
    /// <returns></returns>
    public TowerData GetCurrentTowerData()
    {
        int currentId = DataManager.Instance.PlayerInfo.CurrentTowerID;

        for (int i = 0; i < TowerDatas.Count; i++)
        {
            var data = TowerDatas[i];
            if (data == null)
                continue;

            if (data.ID == currentId)
                return data;
        }

        return null;
    }

    /// <summary>
    /// 修改某个TowerData的当前skinID
    /// </summary>
    public void ChangeTowerCurrentSkinID(TowerData towerData,int skinID)
    {
        DataManager.Instance.PlayerInfo.TowerStateMap[towerData.ID].SetCurrentSkinID(skinID); // 保存选择
    }

    /// <summary>
    /// 判断某个ItemState是否有这个skin
    /// </summary>
    /// <returns></returns>
    public bool HasSkin(ItemState itemState,int ID)
    {
        return itemState.AllSkins != null && itemState.AllSkins.TryGetValue(ID,out var skinInfo) && skinInfo.IsUnlocked;
    }



    //更新炮塔描述
    private void UpdateDescription()
    {
        //TowerDiscription[TowerType.Basic] = $"每次发射单颗子弹,对敌人造成<color=#F4C760>{GetTowerData(TowerType.Basic).BaseDamage / 10f}</color>点伤害\r\n\r\n被动：每回合恢复城墙最大生命值2%的血量";
        //TowerDiscription[TowerType.Ricochet] = $"子弹对第一个敌人造成<color=#F4C760>{GetTowerData(TowerType.Ricochet).BaseDamage / 10f}</color>点伤害，额外弹射2个敌人,每次弹射伤害衰减30%\r\n\r\n被动：每消灭3个敌人为城墙恢复1点生命值";
        //TowerDiscription[TowerType.Spread] = $"每次并排发射三颗子弹，每颗子弹伤害为<color=#F4C760>{GetTowerData(TowerType.Spread).BaseDamage / 10f}</color>，初始暴击率翻倍,初始暴击伤害降低\r\n被动：每次暴击为城墙恢复1点血";
        //TowerDiscription[TowerType.Piercing] = $"子弹初始伤害为<color=#F4C760>{GetTowerData(TowerType.Piercing).BaseDamage / 10f}</color>，可以穿透敌人\r\n被动：每造成50点伤害恢复城墙1点生命";
        //TowerDiscription[TowerType.RapidFire] = $"每次射出两颗子弹，每颗子弹伤害为<color=#F4C760>{GetTowerData(TowerType.RapidFire).BaseDamage / 10f}</color>，初始换弹时间减少\r\n\r\n被动：每发射200颗子弹恢复城墙1点生命值";
        //TowerDiscription[TowerType.Sniper] = $"初始射速降低，子弹初始伤害为<color=#F4C760>{GetTowerData(TowerType.Sniper).BaseDamage / 10f}</color>，炮塔范围变为全屏\r\n被动：每造成50点伤害恢复城墙1点生命";
    }

}



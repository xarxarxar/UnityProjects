
using SuperScrollView;
using System;
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
            if (!DataManager.Instance.PlayerInfo.TowerStateMap.ContainsKey(id))
            {
                DataManager.Instance.PlayerInfo.TowerStateMap[id] = new ItemState
                {
                    IsUnlocked = id==0,
                    CurrentSkinID = 0,
                    OwnedSkins = new List<int>() { 0 } // 默认皮肤
                };
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
        return TowerDatas.FindIndex(t => t == towerData);
    }

    /// <summary>
    /// 获取某个ID的TowerData在TowerDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetTowerDataIndex(int ID)
    {
        return TowerDatas.FindIndex(t => t.ID == ID);
    }


    /// <summary>
    /// 获取某个skin在某个TowerData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerData towerData, Skin skin)
    {
        return towerData.SkinData.skins.FindIndex(t => t == skin);
    }

    /// <summary>
    /// 获取某个ID的skinData在某个TowerData中的SkinData的skins中的index
    /// </summary>
    /// <returns></returns>
    public int GetSkinIndex(TowerData towerData,int ID)
    {
        return towerData.SkinData.skins.FindIndex(t => t.ID == ID);
    }

    /// <summary>
    /// 通过index获取某个towerdata的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetSkin(TowerData towerData,int index)
    {
        return towerData.SkinData.skins[index];
    }

    /// <summary>
    /// 获取某个TowerData当前正在使用的skin
    /// </summary>
    /// <returns></returns>
    public Skin GetCurrentSkin(TowerData towerData)
    {
        return towerData.SkinData.skins.Find(t => t.ID == DataManager.Instance.PlayerInfo.TowerStateMap[towerData.ID].CurrentSkinID);
    }

    /// <summary>
    /// 获取某个ItemState当前正在使用的skin的Index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSkinIndex(SkinData skinData, ItemState itemState)
    {
        return skinData.skins.FindIndex(t => t.ID == itemState.CurrentSkinID);
    }

    /// <summary>
    /// 获取当前玩家使用的TowerData在TowerDatas中的index
    /// </summary>
    /// <returns></returns>
    public int GetCurrentTowerDataIndex()
    {
        int index = TowerDatas.FindIndex(t => t.ID == DataManager.Instance.PlayerInfo.CurrentTowerID.Value);
        return index;
    }

    /// <summary>
    /// 获取当前玩家使用的TowerData
    /// </summary>
    /// <returns></returns>
    public TowerData GetCurrentTowerData()
    {
        TowerData tower = TowerDatas.Find(t => t.ID == DataManager.Instance.PlayerInfo.CurrentTowerID.Value);
        return tower;
    }

    /// <summary>
    /// 修改某个TowerData的当前skinID
    /// </summary>
    public void ChangeTowerCurrentSkinID(TowerData towerData,int skinID)
    {
        DataManager.Instance.PlayerInfo.TowerStateMap[towerData.ID].CurrentSkinID = skinID; // 保存选择
    }

    /// <summary>
    /// 判断某个ItemState是否有这个skin
    /// </summary>
    /// <returns></returns>
    public bool HasSkin(ItemState itemState,int ID)
    {
        return itemState.OwnedSkins != null && itemState.OwnedSkins.Contains(ID);
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



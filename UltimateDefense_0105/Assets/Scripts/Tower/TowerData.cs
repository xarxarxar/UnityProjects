using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TowerData", menuName = "Game/Tower Data")]
public class TowerData : ScriptableObject
{
    [Tooltip("炮塔ID")]
    public int ID;

    [Tooltip("炮塔名称")]
    public string Name;

    [Tooltip("炮塔描述")]
    public string TowerDescription;

    [Tooltip("炮塔预制体")]
    public Tower TowerPrefab;

    [Tooltip("炮塔皮肤")]
    public SkinData SkinData;


    [Header("基础属性（未加Buff前）")]
    [Tooltip("子弹伤害")]
    public int BaseDamage;

    [Tooltip("子弹容量")]
    public int BaseCap;

    [Tooltip("每秒攻击次数")]
    public float BaseAtkRate;

    [Tooltip("换弹时间（秒）")]
    public float BaseReload;

    [Tooltip("暴击概率（0~1）")]
    [Range(0f, 1f)]
    public float BaseCritProb;

    [Tooltip("暴击伤害倍率")]
    public float BaseCritMult;
}


[Serializable]
public class ItemState
{
    public static event UnityAction OnStateChange;

    private bool _isUnlocked;
    private int _currentSkinId;
    private int _pieceCount;
    private Dictionary<int, SkinInfo> _allSkins = new Dictionary<int, SkinInfo>();

    public ItemState()
    {
        _allSkins = new Dictionary<int, SkinInfo>();
    }

    /// <summary>
    /// 是否已解锁
    /// </summary>
    public bool IsUnlocked=> _isUnlocked;
    /// <summary>
    /// 当前使用的皮肤ID
    /// </summary>
    public int CurrentSkinID => _currentSkinId;

    /// <summary>
    /// 该物品的碎片有几个
    /// </summary>
    public int PieceCount => _pieceCount;

    /// <summary>
    /// 该炮塔下的皮肤信息，第一个参数为皮肤的ID，第二个参数为皮肤的状态
    /// </summary>
    public Dictionary<int, SkinInfo> AllSkins => _allSkins;

    public void SetIsUnlocked(bool value, bool save = true)
    {
        if (value == _isUnlocked) return;
        _isUnlocked = value;
        if (save)
            OnStateChange?.Invoke();
    }

    public void SetCurrentSkinID(int value, bool save = true)
    {
        if (value == _currentSkinId) return;
        _currentSkinId = value;
        if (save)
            OnStateChange?.Invoke();
    }

    public void SetPieceCount(int value, bool save = true)
    {
        if (value == _pieceCount) return;
        _pieceCount = value;
        if (save)
            OnStateChange?.Invoke();
    }

    public void SetSkins(int skinID,SkinInfo skinInfo, bool save = true)
    {
        _allSkins[skinID]= skinInfo;
        if (save)
            OnStateChange?.Invoke();
    }

    /// <summary>
    /// 设置ItemState
    /// </summary>
    /// <param name="itemState"></param>
    public void SetItemState(ItemState itemState, bool save = true)
    {
        _isUnlocked=itemState.IsUnlocked;
        _currentSkinId=itemState.CurrentSkinID;
        _pieceCount=itemState.PieceCount;
        _allSkins.Clear();
        foreach(var kv in itemState.AllSkins)
        {
            _allSkins.Add(kv.Key, kv.Value);
        }
        if (save)
            OnStateChange?.Invoke();
    }
}
[System.Serializable]
public class SkinInfo
{
    public static event UnityAction OnSkinfoChange;
    private bool _isUnlocked;
    private int _pieceCount;

    public bool IsUnlocked=> _isUnlocked;//是否已拥有
    public int PieceCount=> _pieceCount; //皮肤碎片数量


    public void SetIsUnlocked(bool value, bool save = true)
    {
        if(value == _isUnlocked) return;
        _isUnlocked = value;
        if (save)
            OnSkinfoChange?.Invoke();
    }

    public void SetPieceCount(int value, bool save = true)
    {
        if (value == _pieceCount) return;
        _pieceCount = value;
        if (save)
            OnSkinfoChange?.Invoke();
    }

    /// <summary>
    /// 设置SkinInfo
    /// </summary>
    /// <param name="skinInfo"></param>
    public void SetSkinInfo(SkinInfo skinInfo, bool save = true)
    {
        _isUnlocked= skinInfo.IsUnlocked;
        _pieceCount= skinInfo.PieceCount;
        if(save)
            OnSkinfoChange?.Invoke();
    }
}
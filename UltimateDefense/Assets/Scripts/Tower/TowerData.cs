using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Game/Tower Data")]
public class TowerData : ScriptableObject
{
    [Tooltip("炮塔ID")]
    public int ID;

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
    /// <summary>
    /// 是否已解锁
    /// </summary>
    public bool IsUnlocked;
    /// <summary>
    /// 当前使用的皮肤ID
    /// </summary>
    public int CurrentSkinID;
    /// <summary>
    /// 已拥有的皮肤
    /// </summary>
    public List<int> OwnedSkins = new List<int>();
}
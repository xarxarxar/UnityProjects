using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TowerPlatformData", menuName = "Game/TowerPlatform Data")]
public class TowerPlatformData : ScriptableObject
{
    [Tooltip("ÅÚËşµ××ùID")]
    public int ID;

    [Tooltip("ÅÚËşµ××ùÃèÊö")]
    public string TowerDescription;

    [Tooltip("µ××ù¾ö¶¨µÄÉËº¦ÊôĞÔ")]
    public DamageEffect damageEffect;

    [Tooltip("ÅÚËşµ××ùÆ¤·ô")]
    public SkinData SkinData;
}

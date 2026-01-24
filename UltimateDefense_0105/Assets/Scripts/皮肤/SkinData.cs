using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData", menuName = "Game/Skin Data")]
public class SkinData : ScriptableObject
{
    public List<Skin> skins=new List<Skin>();
}

[System.Serializable]
public class Skin
{
    public int ID;
    //public int PieceCount;//Æ¤·ôËéÆ¬ÓÐ¼¸¸ö
    public string Name;
    public string description;
    public Sprite sprite;
}

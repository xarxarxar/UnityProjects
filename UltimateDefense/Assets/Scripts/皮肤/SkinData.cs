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
    public string name;
    public Sprite sprite;
}

using UnityEngine;

[System.Serializable]
public class BeastInfoItem
{
    public Team curTeam;
    public BeastId beastId;
    public int beastIndex;
    public string beastName;
    public Sprite icon;

    public int damage;
    public int damaged;
    public bool isDie;
    public bool showDamage;// or show damaged
}
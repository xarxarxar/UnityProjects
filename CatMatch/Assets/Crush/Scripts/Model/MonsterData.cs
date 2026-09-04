using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public BeastId beastId;
    public int level;
    [Range(1, 5)]
    public int star;
    public int laneIndex = -1;
}
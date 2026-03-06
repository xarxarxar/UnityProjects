using UnityEngine;

[System.Serializable]
public class FruitType
{
    public string name;
    public float defaultGrowTime = 5f;
    public int defaultMaxFruits = 3;
    public int buyPrice = 50; // 购买果树需要的金币
    public int defaultSoldPrice = 5;//单颗果实的售价
}
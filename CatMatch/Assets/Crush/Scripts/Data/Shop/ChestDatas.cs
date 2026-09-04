using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestData
{
    public ChestType chestType;
    public CurrencyType currencyType;
    public Sprite icon;

    public int amount;
    public string name;
    public string des;
    public int price;
    public int maxStock;
    public string days;
    public int quantity;
}

[CreateAssetMenu(fileName = "ChestDatas", menuName = "CrushDatas/ChestDatas")]
public class ChestDatas : ScriptableObject
{
    public List<ChestData> chestDatas;
}

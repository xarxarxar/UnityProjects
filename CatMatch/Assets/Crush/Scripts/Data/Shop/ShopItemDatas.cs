using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    public ShopItemType shopItemType;
    public CurrencyType currencyType;
    public Sprite icon;

    public int amount;
    public string name;
    public string des;
    public int price;
    public int maxStock;
}

[CreateAssetMenu(fileName = "ShopItemDatas", menuName = "CrushDatas/ShopItemDatas")]
public class ShopItemDatas : ScriptableObject
{
    public List<ShopItemData> shopItemDatas;
}

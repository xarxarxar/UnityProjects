using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestSprites
{
    public Sprite icon;
    public ChestType chestType;
}

[System.Serializable]
public class ShopItemSprites
{
    public Sprite icon;
    public ShopItemType shopItemType;
}

[CreateAssetMenu(fileName = "ShopSprites", menuName = "CrushDatas/ShopSprites")]
public class ShopSprites : MonoBehaviour
{
    public List<ChestSprites> chestSprites;
    public List<ShopItemSprites> shopItemSprites;
}

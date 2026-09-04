using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopItemViewType
{
    None,
    FortuneChest,
    Chest,
    Item
}

public class ShopDatas : MonoBehaviour
{
    public ChestDatas chestDatas;
    // public ShopSprites shopSprites;
    public ShopItemDatas shopItemDatas;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    private List<ItemContent> items;
    private int totalWeight = 0;

    public LootBox()
    {
        items = new List<ItemContent>();
        totalWeight=0;
    }

    public void AddItem(ItemContent item)
    {
        items.Add(item);
    }

    public void RemoveItem(ItemContent item)
    {
        items.Remove(item);
    }

    public ItemContent GetRandomItem()
    {
        totalWeight = 0;

        foreach (var item in items)
        {
            totalWeight += item.Weight;
        }

        int randomNumber = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var item in items)
        {
            currentWeight += item.Weight;
            if (randomNumber < currentWeight)
            {
                return item;
            }
        }

        return null; 
    }

    public int GetTotalWeight()
    {
        return totalWeight;
    }
}

[System.Serializable]
public class ItemContent
{
    public string Name="";
    public int Weight=0;
    public Sprite cardSprite = null;
}

public class GenerateCard
{
    //资源
    //物品
    //猫咪碎片

    public static List<ItemContent> catItems = new List<ItemContent>
    {
        
    };


}

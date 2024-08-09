using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LootBox 
{
    private List<ItemContent> items;
    private int totalWeight;

    public LootBox()
    {
        items = new List<ItemContent>();
        //totalWeight = 0;
    }

    public void AddItem(ItemContent item)
    {
        items.Add(item);
        //totalWeight += item.Weight;
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

        return null; // This line should never be reached
    }
}

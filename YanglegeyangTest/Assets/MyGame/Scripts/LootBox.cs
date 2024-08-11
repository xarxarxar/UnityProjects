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

public class ItemContent
{
    public string Name { get; set; }
    public int Weight { get; set; }

    public ItemContent(string name, int weight)
    {
        Name = name;
        Weight = weight;
    }
}

public class GenerateCard
{
    //◊ ‘¥
    //ŒÔ∆∑
    //√®ﬂ‰ÀÈ∆¨

    public static List<ItemContent> catItems = new List<ItemContent>
    {
        new ItemContent("√®ﬂ‰ÀÈ∆¨1",10),
        new ItemContent("√®ﬂ‰ÀÈ∆¨2",20),
        new ItemContent("√®ﬂ‰ÀÈ∆¨3",30),
        new ItemContent("√®ﬂ‰ÀÈ∆¨4",40),
        new ItemContent("√®ﬂ‰ÀÈ∆¨5",50),
        new ItemContent("√®ﬂ‰ÀÈ∆¨6",60),
        new ItemContent("√®ﬂ‰ÀÈ∆¨7",70),
        new ItemContent("√®ﬂ‰ÀÈ∆¨8",80),
        new ItemContent("√®ﬂ‰ÀÈ∆¨9",90),
        new ItemContent("√®ﬂ‰ÀÈ∆¨10",100),
    };


}

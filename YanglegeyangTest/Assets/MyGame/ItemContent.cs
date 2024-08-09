using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

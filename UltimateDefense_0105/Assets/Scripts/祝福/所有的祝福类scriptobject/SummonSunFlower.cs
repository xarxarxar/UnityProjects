using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonSunFlower", menuName = "Game/Bless/SummonSunFlower")]
public class SummonSunFlower : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new SummonSunFlowerInstance(this, rarity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SummonAWall", menuName = "Game/Bless/SummonAWall")]
public class SummonAWall : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new SummonAWallInstance(this, rarity);
    }
}

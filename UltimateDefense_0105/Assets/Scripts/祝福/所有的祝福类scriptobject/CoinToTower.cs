using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoinToTower", menuName = "Game/Bless/CoinToTower")]
public class CoinToTower : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new CoinToTowerInstance(this, rarity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalBloodSuckBless", menuName = "Game/Bless/CrystalBloodSuckBless")]
public class CrystalBloodSuckBless : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new CrystalBloodSuckInstance(this, rarity);
    }
}

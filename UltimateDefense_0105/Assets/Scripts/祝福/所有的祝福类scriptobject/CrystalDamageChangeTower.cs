using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalDamageChangeTower", menuName = "Game/Bless/CrystalDamageChangeTower")]
public class CrystalDamageChangeTower : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new CrystalDamageChangeTowerInstance(this, rarity);
    }
}

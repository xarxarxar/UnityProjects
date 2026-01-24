using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageToCrystalHp", menuName = "Game/Bless/DamageToCrystalHp")]
public class DamageToCrystalHp : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new DamageToCrystalHpInstance(this, rarity);
    }
}

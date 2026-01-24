using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyNinja", menuName = "Game/Bless/EnemyNinja")]
public class EnemyNinja : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new EnemyNinjaInstance(this, rarity);
    }
}

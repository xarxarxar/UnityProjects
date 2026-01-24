using UnityEngine;


[CreateAssetMenu(fileName = "DropCoinBless", menuName = "Game/Bless/DropCoinBless")]
public class DropCoinBless : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new DropCoinBlessInstance(this, rarity);
    }
}

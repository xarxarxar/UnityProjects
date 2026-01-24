using UnityEngine;

[CreateAssetMenu(fileName = "CriticalBless", menuName = "Game/Bless/CriticalBless")]
public class CriticalBless : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new CriticalBlessInstance(this, rarity);
    }
}

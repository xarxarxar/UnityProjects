using UnityEngine;

[CreateAssetMenu(fileName = "RageMode", menuName = "Game/Bless/RageMode")]
public class RageMode : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new RageModeInstance(this, rarity);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "BackHalfCoin", menuName = "Game/Bless/BackHalfCoin")]
public class BackHalfCoin : Bless
{
    public override BlessInstance CreateInstance(int rarity)
    {
        return new BackHalfCoinInstance(this, rarity);
    }
}

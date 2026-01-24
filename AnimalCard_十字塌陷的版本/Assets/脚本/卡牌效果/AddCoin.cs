using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCoin : SingleCard
{
    private float flyTime = 1.0f;
    public int coinCount;
    public int coinType = 0;//如果为0，那么PoolManager获取少量金币的那个Transform，如果为1，那么就获取大量金币的Transform

    public override void OnCardEffect()
    {
        Transform Coin;
        if (coinType == 0)
        {
            Coin = PoolManager.Instance.LittleCoinPool.Get();
        }
        else
        {
            Coin = PoolManager.Instance.MuchCoinPool.Get();
        }
        
        var effect = new AddCoinEffect(
            CurrentSlot,
            selfRole,
            flyTime,
            count: 1,
            showImage: Coin
        );

        CardEffectManager.instance.PlayEffect(effect);
    }
}

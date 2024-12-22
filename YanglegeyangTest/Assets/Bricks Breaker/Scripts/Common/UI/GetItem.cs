using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ItemGetType {
    Coin,
    Gem
}

public class GetItem : MonoBehaviour
{
    public GetItemObject _itemCoin;
    public GetItemObject _itemGem;

    public Transform posCoin;
    public Transform posGem;

    public void GetCoin(int value, Vector2 firstPos)
    {
        GameData.Coin += value;
        _itemCoin.GetItem(value, firstPos);
    }

    public void GetGem(int value, Vector2 firstPos)
    {
        GameData.Gem += value;
        _itemGem.GetItem(value, firstPos);
    }
}

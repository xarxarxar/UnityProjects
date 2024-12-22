using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GetItemObject : MonoBehaviour
{
    Vector2 first;
    Vector2 last;
    int count;

    public GameObject pItem;
    public ItemGetType itemGetType;

    private void Start()
    {
        if (PlayManager.Instance.GetSceneName() == Data.scene_home)
        {
            PoolManager.CreatePool(pItem, 20, false, 0);
        }
    }

    public void GetItem(int value, Vector2 f)
    {
        if (value <= 10)
        {
            count = 3;
        }
        else if (value > 10 && value <= 50)
        {
            count = 5;
        }
        else if (value > 50 && value <= 100)
        {
            count = 7;
        }
        else if (value > 100 && value <= 300)
        {
            count = 9;
        }
        else if (value > 300 && value <= 500)
        {
            count = 11;
        }
        else if (value > 500 && value <= 1000)
        {
            count = 13;
        }
        else
        {
            count = 20;
        }

        first = f;
        switch (itemGetType)
        {
            case ItemGetType.Coin:
                last = GetComponent<GetItem>().posCoin.position;
                SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_coin);
                break;
            case ItemGetType.Gem:
                last = GetComponent<GetItem>().posGem.position;
                SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_gem);
                break;
        }


        for (int i = 0; i < count; i++)
        {
            StartCoroutine(SpwanCoinCo());
        }

        transform.DOMove(transform.position, 0.15f).OnComplete(() =>
        {
            switch (itemGetType)
            {
                case ItemGetType.Coin:
                    PlayManager.Instance.commonUI._CoinGem.SetCoin(value);
                    break;
                case ItemGetType.Gem:
                    PlayManager.Instance.commonUI._CoinGem.SetGem(value);
                    break;
            }
        }).SetDelay(0.65f);
    }

    IEnumerator SpwanCoinCo()
    {
        float time = Random.Range(0, 0.15f);
        yield return new WaitForSeconds(time);
        Vector2 posf = Camera.main.WorldToScreenPoint(first);
        GameObject item = PoolManager.Spawn(pItem, posf, Quaternion.identity);

        item.transform.SetParent(transform, false);
        item.transform.position = first;
        item.transform.localScale = Vector3.one;

        Vector2 poss = Random.insideUnitCircle * 0.9f;
        item.transform.DOMove(first + poss, 0.2f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(0.15f + time);

        item.transform.DOMove(last, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            //GameObject fx = PoolManager.Spawn(CtrItemGet.instance.fxGet, last, Quaternion.identity);
            //fx.transform.SetParent(transform);
            //fx.transform.localScale = Vector3.one;
            //fx.transform.position = last;
            //PoolManager.Despawn(fx, 1f);
        });
        PoolManager.Despawn(item, 0.5f);

        yield return new WaitForSeconds(0.25f);

        switch (itemGetType)
        {
            case ItemGetType.Coin:
                break;
            case ItemGetType.Gem:
                break;
        }
    }
}

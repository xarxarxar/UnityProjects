using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 祝福商品
/// </summary>
public class BlessGoods : GoodsItem
{
    //UI
    public Image showImage;
    public Image circleImage;
    public Image glowImage;//光芒

    private Bless _bless;
    private int _rarity;

    public override void Init()
    {

        base.Init();

        GetBless();
        Price = (_rarity+1)*50;
        PriceText.text = Price.ToString();

        if (_rarity == 1)
        {
            circleImage.color = BlessDataManager.Instance.rareColor;
            glowImage.color = BlessDataManager.Instance.rareColor;
            NameText.color = BlessDataManager.Instance.rareColor;
            
        }
        else if (_rarity == 2)
        {
            circleImage.color = BlessDataManager.Instance.epicColor;
            glowImage.color = BlessDataManager.Instance.epicColor;
            NameText.color = BlessDataManager.Instance.epicColor;
        }
        NameText.text = _bless.BlessName;
        showImage.sprite = _bless.sprite;
    }

    public override void Click()
    {
        Debug.Log("祝福商品点击");
        if (!isBought)
        {
            //钻石足够
            if (MetaCurrencyManager.Instance.SpendMetaCoin(RewardType.Diamond, Price))
            {
                Buy();
            }
            else
            {
                TipManager.Instance.ShowTip("钻石不足");
            }
        }
    }


    public override void Buy()
    {
        isBought = true;
        if (!DataManager.Instance.PlayerInfo.BlessCount.TryGetValue(_bless.ID, out var counts))
        {
            DataManager.Instance.PlayerInfo.SetBlessCount(_bless.ID, new List<int> { -1, 10, 0 });
            counts = DataManager.Instance.PlayerInfo.BlessCount[_bless.ID];
        }
        counts[_rarity] = counts[_rarity]+ 1;
        DataManager.Instance.PlayerInfo.SetBlessCount(_bless.ID, counts);

        SetClickButton();
        TipManager.Instance.ShowTip("购买成功");
    }
    private float rareWeight = 0.7f;//抽取稀有祝福的概率
    private float epicWeight = 0.3f;//抽取史诗祝福的概率

    /// <summary>
    /// 获取一个祝福
    /// </summary>
    /// <returns></returns>
    private void GetBless()
    {
        _bless = BlessDataManager.Instance.AllBless[Random.Range(0, BlessDataManager.Instance.AllBless.Count)];

        float total = rareWeight + epicWeight ;

        float rand = Random.Range(0f, total);
        if (rand < rareWeight)//抽取稀有祝福
        {
            _rarity = 1;
        }
        else
        {
            _rarity = 2;
        }
    }
}

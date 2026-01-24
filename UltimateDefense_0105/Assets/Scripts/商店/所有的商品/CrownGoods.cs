using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 获得一个皇冠
/// </summary>
public class CrownGoods : GoodsItem
{
    private int Crown => DataManager.Instance.PlayerInfo.Crown;
    public override void Init()
    {
        base.Init();
        Price = 300;
        PriceText.text = Price.ToString();
        NameText.text = "皇冠";
    }

    public override void Click()
    {
        Debug.Log("皇冠商品点击");
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
        DataManager.Instance.PlayerInfo.SetCrown(Crown+1);
        SetClickButton();
        TipManager.Instance.ShowTip("购买成功");
    }
}

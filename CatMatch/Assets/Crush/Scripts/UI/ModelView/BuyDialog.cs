using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyDialog : MonoBehaviour
{
    public Text titleTxt;
    public Text desTxt;
    public Text priceValueTxt;
    public Image currencyIcon;
    public Sprite tileSp;
    public Sprite gemSp;

    private Action yesAction, noAction;

    public void SetUp(string title, string des, string price, Action yesAction, Action noAction, CurrencyType currencyType = CurrencyType.Gem)
    {
        this.yesAction = yesAction;
        this.noAction = noAction;

        titleTxt.text = title;
        desTxt.text = des;
        priceValueTxt.text = price;
        if (currencyType == CurrencyType.Tile)
            currencyIcon.sprite = tileSp;
        else currencyIcon.sprite = gemSp;
    }

    public void Yes()
    {
        yesAction?.Invoke();
    }

    public void No()
    {
        noAction?.Invoke();
    }
}

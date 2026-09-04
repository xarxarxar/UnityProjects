using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum CurrencyType
{
    None,
    PlayAmount,
    SweepAmount,
    Tile,
    Gem
}

public class BuyCurrencyDialog : MonoBehaviour
{
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI desTxt;

    private Action yesAction, noAction;

    public void Setup(CurrencyType type, int price, int amount, int purchaseAmount, Action yesAction, Action noAction)
    {
        string title = "";
        string des = "";
        if (type == CurrencyType.PlayAmount)
        {
            title = LangManager.Instance.Get("OutOfPlayAmount");
            string times = purchaseAmount > 0 ? LangManager.Instance.Get("times") : LangManager.Instance.Get("time");
            des = string.Format(LangManager.Instance.Get("OutOfPlayAmountDes"), amount, price, purchaseAmount, times);
        }
        titleTxt.text = title;
        desTxt.text = des;

        this.yesAction = yesAction;
        this.noAction = noAction;
    }

    public void Yes()
    {
        this.yesAction?.Invoke();
    }

    public void No()
    {
        this.noAction?.Invoke();
    }
}

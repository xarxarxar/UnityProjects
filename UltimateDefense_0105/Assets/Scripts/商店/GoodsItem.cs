using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商品
/// </summary>
public abstract class GoodsItem : MonoBehaviour
{
    public int ID; // 商店商品 ID

    [HideInInspector]
    public string Name;//名称
    [HideInInspector]
    public string Description;
    [HideInInspector]
    public bool isBought = false;//是否已被购买
    [HideInInspector]
    public int Price;           // 价格（金币或钻石）
    
    //UI
    public Button ClickButton;
    public Text NameText;
    public Text PriceText;
    public Image DiamondImage;

    private readonly Color32 lockedColor = new Color32(195, 188, 195, 255);
    private readonly Color32 normalColor = new Color32(33, 135, 211, 255);
    private readonly Color32 lockedTextColor = new Color32(117, 109, 124, 255);
    private readonly Color32 normalTextColor = new Color32(255, 255, 255, 255);

    public virtual void Init()
    {
        ClickButton.onClick.RemoveAllListeners();
        ClickButton.onClick.AddListener(Click);
        isBought = false;
    }

    public abstract void Click();//点击这个商品

    public abstract void Buy();//购买

    public void SetClickButton()
    {
        if (isBought)
        {
            // 已售出
            ClickButton.GetComponent<Image>().color = lockedColor;
            PriceText.color = lockedTextColor;
            PriceText.text = "已售";
            DiamondImage.gameObject.SetActive(false);
        }
        else
        {
            // 未售出
            ClickButton.GetComponent<Image>().color = lockedColor;
            PriceText.color = lockedTextColor;
            DiamondImage.gameObject.SetActive(true);
        }
    }
}

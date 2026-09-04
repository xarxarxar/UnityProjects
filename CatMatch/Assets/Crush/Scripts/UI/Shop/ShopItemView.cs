using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    public static Dictionary<ShopItemType, BeastId> pairs = new Dictionary<ShopItemType, BeastId>()
    {
        {ShopItemType.DarkHunter, BeastId.DarkHunter},{ShopItemType.Merlinus, BeastId.Merlinus},
        {ShopItemType.Atlantus, BeastId.Atlantus},{ShopItemType.Furiosa, BeastId.Furiosa},
        {ShopItemType.Groovine, BeastId.GrooVine}, {ShopItemType.Scud, BeastId.Scud}
    };

    public Button iconBtn;
    public Image iconImg;

    public Text starAmountTxt;
    public RectTransform starRect;
    public Text medalAmountTxt;
    public Text title;

    public Slider process;
    public Text instockTxt;
    public Text rarity;

    public Text processTxt;

    public Text priceTxt;
    public Image currencyIcon;
    public Sprite gemIcon;
    public Sprite tileIcon;
    public Button buyBtn;

    public Color commonColor;
    public Color rareColor;
    public Color epicColor;
    public Color legendColor;

    public BeastId beastId;
    public ChestModel chestModel;

    public void SetUp(ChestModel chestModel)
    {
        this.chestModel = chestModel;

        iconImg.sprite = chestModel.icon;
        iconImg.SetNativeSize();

        medalAmountTxt.text = chestModel.quantity > 1 ? chestModel.quantity.ToString() + "×" : "0";
        medalAmountTxt.gameObject.SetActive(chestModel.quantity > 1);

        title.text = chestModel.name;
        priceTxt.text = chestModel.price.ToString();
        currencyIcon.sprite = chestModel.currencyType == CurrencyType.Gem ? gemIcon : tileIcon;

        var chestTypeLength = Enum.GetNames(typeof(ChestType)).Length;
        var shopItemType = (ShopItemType)(chestModel.typeInt - chestTypeLength);
        if (pairs.ContainsKey(shopItemType))
        {
            beastId = pairs[shopItemType];
            var beastPref = BeastPrefs.Instance.GetBeastPref(beastId);

            var beastData = PlayerData.instance.GetBeastData(beastId);
            // Debug.Log("beastData:" + beastData.curMedal);

            if (beastData != null)
            {
                if (beastData.curStar > 0)
                {
                    starRect.gameObject.SetActive(true);
                    starAmountTxt.text = beastData.curStar.ToString();
                }
                else
                    starRect.gameObject.SetActive(false);

                if (beastData.curStar == Constant.medalByStars.Count) buyBtn.interactable = false;

                if (beastData.curStar < 5)
                {
                    process.maxValue = GetNeedMedalNextStar(beastData.curStar);
                    process.value = (int)beastData.curMedal;
                    processTxt.text = beastData.curMedal + "/" + GetNeedMedalNextStar(beastData.curStar);
                }
                else
                {
                    process.maxValue = 1;
                    process.value = 0;
                    processTxt.text = beastData.curMedal + "/" + "...";
                }
            }
            else
            {
                starRect.gameObject.SetActive(false);

                process.maxValue = GetNeedMedalNextStar(0);
                process.value = 0;
                processTxt.text = 0 + "/" + GetNeedMedalNextStar(0);
            }

            if (chestModel.maxStock == -1) instockTxt.gameObject.SetActive(false);
            else
            {
                instockTxt.gameObject.SetActive(true);
                instockTxt.text = "库存：" + chestModel.curStock + "/" + chestModel.maxStock;
            }

            var bb = beastPref.GetComponent<BeastBase>();
            rarity.text = bb.rarity.ToString();
            if (bb.rarity == Rarity.Common) rarity.color = commonColor;
            else if (bb.rarity == Rarity.Epic) rarity.color = epicColor;
            else if (bb.rarity == Rarity.Rare) rarity.color = rareColor;
            else if (bb.rarity == Rarity.Legend) rarity.color = legendColor;
        }
    }

    int GetNeedMedalNextStar(int star)
    {
        if (star == 5) return 100_000;

        for (int k = Constant.medalByStars.Count - 1; k >= 0; k--)
        {
            if (star >= k)
            {
                return Constant.medalByStars[k];
            }
        }
        return 100_000;
    }
}

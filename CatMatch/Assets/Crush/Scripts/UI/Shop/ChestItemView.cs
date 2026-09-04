using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class ChestItemView : MonoBehaviour
{
    public Image icon;
    public Text titleTxt;
    public TextMeshProUGUI desTxt;
    public Text instockTxt;
    public Text amountTxt;

    public Text priceTxt;
    public Image currencyIcon;

    public Text useAmountTxt;

    public Sprite gemIcon;
    public Sprite tileIcon;

    public List<RectTransform> beastRectTFs;
    public Material material;

    public Button buyBtn;
    public ChestModel mChestModel;

    public void SetUp(ChestModel chestModel)
    {
        mChestModel = chestModel;
        
        icon.sprite = chestModel.icon;
        icon.SetNativeSize();

        if (amountTxt != null)
        {
            amountTxt.text = chestModel.quantity > 1 ? chestModel.quantity.ToString() : "0";
            amountTxt.gameObject.SetActive(chestModel.quantity > 1);
        }

        titleTxt.text = chestModel.name;
        if (chestModel.freeAmount > 0)
        {
            priceTxt.gameObject.SetActive(false);
            currencyIcon.gameObject.SetActive(false);
            useAmountTxt.gameObject.SetActive(true);
            useAmountTxt.text = string.Format("打开({0})", chestModel.freeAmount);
        }
        else
        {
            priceTxt.gameObject.SetActive(true);
            currencyIcon.gameObject.SetActive(true);
            useAmountTxt.gameObject.SetActive(false);
            priceTxt.text = chestModel.price.ToString();
            currencyIcon.sprite = chestModel.currencyType == CurrencyType.Gem ? gemIcon : tileIcon;
        }


        if (chestModel.typeInt == (int)ChestType.Fortune)
        {
            if (chestModel.beastIds != null && beastRectTFs != null)
            {
                for (int k = 0; k < beastRectTFs.Count; k++)
                {
                    if (chestModel.beastIds.Count > k)
                    {
                        Debug.Log("beastId:" + (BeastId)chestModel.beastIds[k]);

                        var skGR = beastRectTFs[k].GetComponent<SkeletonGraphic>();
                        if (skGR == null)
                        {
                            var skeletonDataAsset = SkeletonDatas.Instance.skeletonBeastPair[chestModel.beastIds[k]].skeletonDataAsset;
                            skGR = SkeletonGraphic.AddSkeletonGraphicComponent(beastRectTFs[k].gameObject, skeletonDataAsset, material);

                        }
                        skGR.AnimationState.SetAnimation(0, "idle", true);
                        skGR.gameObject.transform.localScale = new Vector3(-1, 1, 1);
                    }
                    else
                        beastRectTFs[k].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            desTxt.text = chestModel.des;
            if (chestModel.maxStock == 0 || chestModel.maxStock == -1) instockTxt.gameObject.SetActive(false);
            else
            {
                instockTxt.gameObject.SetActive(true);
                instockTxt.text = "库存：" + chestModel.curStock + "/" + chestModel.maxStock;
            }
        }

        // if(chestModel.maxStock > 0 && chestModel.curStock == 0)
        //     buyBtn.interactable = false;
    }

    IEnumerator RunAnim(SkeletonGraphic skeletonGraphic)
    {
        yield return new WaitForSeconds(1f);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Advertisements;

public class PanelShop : PanelBase
{

    int freeGemValue = 3;
    int freeCoinValue = 19;


    int[] buyGemValue = new int[6] {30, 80, 170, 360, 950, 2000};
    int[] buyCoinValue = new int[3] {150, 400, 1200};
    int[] buyCoinCost = new int[3] {20, 50, 140};

    public CanvasGroup[] freeList;
    //public GameObject buttonNoads;

    public TextMeshProUGUI textAds_Cost;

    public TextMeshProUGUI[] textGemValue;
    public TextMeshProUGUI[] textGemCost;

    public TextMeshProUGUI[] textCoinValue;
    public TextMeshProUGUI[] textCoinCost;

    public Transform[] listTransformGem;
    public Transform[] listTransformCoin;


    public override void Open()
    {
        SetFreeButton();

        //if (World.NoAds) {
        //    buttonNoads.gameObject.SetActive(false);
        //}

        for (int i = 0; i < textCoinValue.Length; i++)
        {
            textCoinValue[i].text = Utility.ChangeThousandsSeparator(buyCoinValue[i]);
            textCoinCost[i].text = Utility.ChangeThousandsSeparator(buyCoinCost[i]);
        }

        for (int i = 0; i < buyGemValue.Length; i++)
        {
            textGemValue[i].text = Utility.ChangeThousandsSeparator(buyGemValue[i]);
        }

        base.Open();
    }

    public void Click_RemoveAd()
    {
        // IAPManager.instance.BuyProductID(IAPManager.productID_Noads);
    }

    private bool isClick = false;

    public void Click_Free(int type)
    {

        
    }


    void SetFreeButton()
    {
        

        //if (!ADManager.Instance.IsReady)
        //{

        //    freeList[0].DOFade(0.5f, 0f);
        //    freeList[0].blocksRaycasts = false;
        //    freeList[1].DOFade(0.5f, 0f);
        //    freeList[1].blocksRaycasts = false;
        //}
        //else
        //{
        //    freeList[0].DOFade(1f, 0f);
        //    freeList[0].blocksRaycasts = true;
        //    freeList[1].DOFade(1f, 0f);
        //    freeList[1].blocksRaycasts = true;
        //}
    }


    /// <summary>
    /// Buy Click Gem
    /// </summary>
    void Click_BuyGem(int id)
    {

        switch (id)
        {
            case 0:
                PlayManager.Instance.commonUI._GetItem.GetGem(30, listTransformGem[id].position);
                break;
            case 1:
                PlayManager.Instance.commonUI._GetItem.GetGem(80, listTransformGem[id].position);
                break;
            case 2:
                PlayManager.Instance.commonUI._GetItem.GetGem(170, listTransformGem[id].position);
                break;
            case 3:
                PlayManager.Instance.commonUI._GetItem.GetGem(360, listTransformGem[id].position);
                break;
            case 4:
                PlayManager.Instance.commonUI._GetItem.GetGem(950, listTransformGem[id].position);
                break;
            case 5:
                PlayManager.Instance.commonUI._GetItem.GetGem(2000, listTransformGem[id].position);
                break;
        }
    }





    /// <summary>
    /// Click Coin Purchase
    /// </summary>
    public void Click_BuyCoin(int id)
    {
        if (GameData.Gem >= buyCoinCost[id])
        {

            CtrHome ctrHome = PlayManager.Instance.currentBase as CtrHome;

            ctrHome._PopupBuy.buttonOK.onClick.AddListener(() => { BuySuccess(id); });
            ctrHome._PopupBuy.SetGem(id, buyCoinValue[id]);

        }
        else
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_error);
            PlayManager.Instance.commonUI.SetToast("Not enough coin.");
            //Not enough gem
        }
    }

    /// <summary>
    /// After successful purchase
    /// </summary>
    public void BuySuccess(int id)
    {
        PlayManager.Instance.commonUI._GetItem.GetCoin(buyCoinValue[id], listTransformCoin[id].position);
        GameData.Gem -= buyCoinCost[id];
        PlayManager.Instance.commonUI._CoinGem.SetGem();
    }
}
 
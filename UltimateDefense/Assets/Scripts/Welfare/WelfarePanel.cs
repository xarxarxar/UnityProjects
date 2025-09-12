using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelfarePanel : BasePanel
{
    public Text GlobalCoinText;//全部的金币
    public Text CoinCountText;//金币的数量
    public Button DiamonButton;//钻石购买的按钮
    public RewardStruct DiamonRewardStruct;//用多少钻石购买
    public Button AdButton;//广告领取的按钮
    private int CoinCount = 0;

    protected override void InitPanel()
    {
        CoinCount = UpgradeManager.Instance.RefreshCount.Value * 90;
        CoinCountText.text = $"×{CoinCount}";

        DiamonButton.onClick.RemoveAllListeners();
        AdButton.onClick.RemoveAllListeners();

        DiamonButton.onClick.AddListener(DiamonGetMoney);
        AdButton.onClick.AddListener(GetMoney);

        if(!MetaCurrencyManager.Instance.HasEnoughMoney(DiamonRewardStruct.type, DiamonRewardStruct.count))
        {
            DiamonRewardStruct.countText.color= Color.red;
        }
        else
        {
            DiamonRewardStruct.countText.color = Color.white;
        }
    }

    private void DiamonGetMoney()
    {
        //钻石足够
        if(MetaCurrencyManager.Instance.SpendMetaCoin(DiamonRewardStruct.type, DiamonRewardStruct.count))
        {
            GetMoney();
            OnCloseButton();
        }
    }

    //得到这些金币
    private void GetMoney()
    {
        // 播放金币粒子特效
        Vector3 screenStart = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(null, GlobalCoinText.transform.position);
        GameUIManager.Instance.PlayFlyCoinEffect(screenStart, screenEnd, 5, totalCallback: () =>
        {
            CurrencyManager.Instance.AddCoin(CoinCount,5);
        });
    }
}

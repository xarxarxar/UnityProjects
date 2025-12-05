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
        CoinCount = UpgradeManager.Instance.RefreshCount.Value * 120;//可以获得的金币是RefreshCount的100倍
        CoinCountText.text = $"×{CoinCount}";

        DiamonButton.onClick.RemoveAllListeners();
        AdButton.onClick.RemoveAllListeners();

        DiamonButton.onClick.AddListener(DiamonGetMoney);
        AdButton.onClick.AddListener(ShareForMoney);

        if(!MetaCurrencyManager.Instance.HasEnoughMoney(DiamonRewardStruct.type, DiamonRewardStruct.count))
        {
            DiamonRewardStruct.countText.color= Color.red;
        }
        else
        {
            DiamonRewardStruct.countText.color = Color.white;
        }

        if (BattleManager.Instance.ShareForWelfareCount >= 2)
        {
            AdButton.gameObject.SetActive(false);
        }
        else
        {
            AdButton.gameObject.SetActive(true);
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
        else
        {
            TipManager.Instance.ShowTip("钻石不足");
        }
    }

    private void ShareForMoney()
    {
        if(BattleManager.Instance.ShareForWelfareCount >= 2)
        {
            TipManager.Instance.ShowTip("本此战斗分享次数已用尽");
            return;
        }
        WeChatManager.ShareApp(() =>
        {
            DataManager.Instance.PlayerInfo.DailyTask.ShareCount.Value += 1;
            BattleManager.Instance.ShareForWelfareCount++;
            GetMoney();
            OnCloseButton();
        }, title: "求求你送我点金币吧!");
        DataManager.Instance.SavePlayerInfo();
    }

    //得到这些金币
    private void GetMoney()
    {
        // 播放金币粒子特效
        Vector3 screenStart = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(null, GlobalCoinText.transform.position);
        GameUIManager.Instance.PlayFlyCoinEffect(screenStart, screenEnd, 5, totalCallback: () =>
        {
            CurrencyManager.Instance.AddCoin(CoinCount, 5);
        });
    }
}

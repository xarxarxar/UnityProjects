using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class GetRewards : MonoBehaviour
{
    public static GetRewards Instance;

    [HideInInspector]public Button getTipButton;

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 分享获取金币
    /// </summary>
    public void ShareForMoney()
    {
        WechatManager.ShareApp(() =>
        {
            GameEntrance.instance.CoinCount += 50;
            ShowTipManager.instance.ShowTip("获得金币",50);
            GetComponent<Canvas>().enabled = false;
        });
    }


    /// <summary>
    /// 关闭获取奖励面板
    /// </summary>
    public void CloseGetRewardPanel()
    {
        GetComponent<Canvas>().enabled = false;
    }


    public GameObject getTipWordPanel;//分享获取提示的面板

    /// <summary>
    /// 分享获取单词提示
    /// </summary>
    public void ShareForWordTip()
    {
        WechatManager.ShareApp(() =>
        {
            GameManager.Instance.TipWordCount += 1;
            ShowTipManager.instance.ShowTip("获得提示");
            //GetComponent<Canvas>().enabled = false;
            getTipWordPanel.SetActive(false);
        });
    }

    //通过广告获取提示
    public void AdForTip()
    {
        WXAdsManager.Instance.ShowAd((isOver) =>
        {
            if (isOver)
            {
                GameManager.Instance.TipWordCount += 1;
                ShowTipManager.instance.ShowTip("获得提示");
                getTipWordPanel.SetActive(false);
            }
            else
            {
                ShowTipManager.instance.ShowTip("广告未观看完毕");
            }
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRewards : MonoBehaviour
{
    public static GetRewards Instance;

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

    /// <summary>
    /// 分享获取单词提示
    /// </summary>
    public void ShareForWordTip()
    {
        WechatManager.ShareApp(() =>
        {
            GameEntrance.instance.CoinCount += 50;
            ShowTipManager.instance.ShowTip("获得提示");
            GetComponent<Canvas>().enabled = false;
        });
    }
}

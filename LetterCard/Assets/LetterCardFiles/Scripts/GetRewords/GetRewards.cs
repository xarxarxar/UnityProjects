using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRewards : MonoBehaviour
{
    /// <summary>
    /// 分享获取金币
    /// </summary>
    public void ShareForMoney()
    {
        WechatManager.ShareApp(() =>
        {
            GameEntrance.instance.CoinCount += 100;
        });
    }
}

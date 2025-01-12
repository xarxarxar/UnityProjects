using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Watermelon;
using WeChatWASM;

public class WXAdsManager : MonoBehaviour
{
    static WXAdsManager instance;
    public static WXAdsManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    WXRewardedVideoAd ad;    // 广告单例
    UnityAction<bool> rewardCallback;   // 奖励回调
    public void Init()
    {
        ad = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
        {
            adUnitId = "adunit-868efb8f25f7d5f7" // 自己申请的广告单元 ID
        });

        ad.OnLoad((res) =>
        {
            Debug.Log($"广告加载 = {res.errMsg}");
        });
        ad.OnError((res) =>
        {
            Debug.Log($"广告错误 = {res.errMsg}");
            FloatingMessage.ShowMessage(res.errMsg);
        });
        ad.OnClose((res) =>
        {
            Debug.Log($"广告关闭  是否看完 = {res.isEnded}");
            OnPlayEnd(res.isEnded);
        });
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    /// <param name="callback">看完广告后的回调</param>
    public void ShowAd(UnityAction<bool> callback)
    {
        Debug.Log("显示广告");
        ad.Show();
        rewardCallback = callback;
    }

    public void TestPlayAds()
    {
        PlayAd((isEnd) =>
        {

        });
    }


    public void PlayAd(UnityAction<bool> callback)
    {
        ShowAd(callback);
    }

    /// <summary>
    /// 广告看完回调
    /// </summary>
    /// <param name="isEnd">是否看完</param>
    void OnPlayEnd(bool isEnd)
    {
        rewardCallback?.Invoke(isEnd);
        if (isEnd)
        {
            Debug.Log("完整看完广告");
            
        }
        else
        {
            Debug.Log("没有看完广告");
        }
    }
}

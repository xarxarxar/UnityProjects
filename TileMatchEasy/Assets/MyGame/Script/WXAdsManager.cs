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
    WXCustomAd gridAd;//格子广告单例
    WXCustomAd bannerAd;//横幅广告单例
    WXInterstitialAd interstitialAd;//插屏广告单例
    UnityAction<bool> rewardCallback;   // 奖励回调
    public void Init()
    {
        //格子广告
        WindowInfo windowInfo = WX.GetWindowInfo();
        int gridAdWidth =(int) (windowInfo.windowWidth*0.8f);
        gridAd= WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId= "adunit-bc98eedc1e644a7d",
            adIntervals =30,
            style = new CustomStyle() { left = (int)(windowInfo.windowWidth*0.18f), top = (int)(windowInfo.windowHeight - 106), width = gridAdWidth },

        });
        gridAd.OnLoad((res) =>
        {
            Debug.Log($"格子广告加载 = {res.errMsg}");
        });
        gridAd.OnError((res) =>
        {
            Debug.Log($"格子广告加载错误 = {res.errMsg}");
        });
        gridAd.OnClose(() =>
        {
            Debug.Log($"格子广告关闭 ");
        });


        //横幅广告
        bannerAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = "adunit-5f9c67c498bd26d8",
            adIntervals = 30,
            style = new CustomStyle() { left = (int)(windowInfo.windowWidth * 0.18f), top = (int)(windowInfo.windowHeight - 106*0.8f), width = gridAdWidth },

        });
        bannerAd.OnLoad((res) =>
        {
            Debug.Log($"横幅广告加载 = {res.errMsg}");
        });
        bannerAd.OnError((res) =>
        {
            Debug.Log($"横幅广告加载错误 = {res.errMsg}");
        });
        bannerAd.OnClose(() =>
        {
            Debug.Log($"横幅广告关闭 ");
        });

        //插屏广告
        interstitialAd = WX.CreateInterstitialAd(new WXCreateInterstitialAdParam()
        {
            adUnitId = "adunit-92e9909f7544c467",
        });
        interstitialAd.OnLoad((res) =>
        {
            Debug.Log($"插屏广告加载 = {res.errMsg}");
        });
        interstitialAd.OnError((res) =>
        {
            Debug.Log($"插屏广告加载错误 = {res.errMsg}");
        });
        interstitialAd.OnClose(() =>
        {
            Debug.Log($"插屏广告关闭 ");
        });

        //激励广告
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
    /// 展示格子广告
    /// </summary>
    public void ShowGridAd()
    {
        if (gridAd != null)
        {
            gridAd.Show();
        }
    }

    /// <summary>
    /// 隐藏格子广告
    /// </summary>
    public void CloseGridAds()
    {
        if (gridAd != null)
        {
            gridAd.Hide();
        }
    }

    /// <summary>
    /// 展示横幅广告
    /// </summary>
    public void ShowBannerAd()
    {
        if ( bannerAd != null )
        {
            bannerAd.Show();
        }
        else
        {
            Debug.Log("bannerAd为空");
        }
        
    }

    /// <summary>
    /// 隐藏横幅广告
    /// </summary>
    public void CloseBannerAds()
    {
        if (bannerAd != null)
        {
            bannerAd.Hide();
        }
    }

    /// <summary>
    /// 展示插屏广告
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("interstitialAd为空");
        }
        
    }

    /// <summary>
    /// 随机播放格子广告或者横幅广告
    /// </summary>
    public void RandomPlayCustom()
    {
        CloseGridAds();
        CloseBannerAds();
        // 随机选择播放广告，0 为横幅广告，1 为格子广告
        int randomAdType = Random.Range(0, 2);

        if (randomAdType == 0)
        {
            ShowBannerAd();
        }
        else
        {
            ShowGridAd();
        }
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    /// <param name="callback">看完广告后的回调</param>
    public void ShowAd(UnityAction<bool> callback)
    {
        if (ad != null)
        {
            ad.Show();
        }
        rewardCallback = callback;
    }

    /// <summary>
    /// 广告看完回调
    /// </summary>
    /// <param name="isEnd">是否看完</param>
    void OnPlayEnd(bool isEnd)
    {
        RandomPlayCustom();
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

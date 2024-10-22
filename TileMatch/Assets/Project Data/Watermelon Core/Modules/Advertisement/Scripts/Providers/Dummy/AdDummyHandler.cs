using UnityEngine;

namespace Watermelon
{
    // AdDummyHandler 类用于模拟广告系统的行为（例如横幅广告、插页广告、奖励视频广告）
    // 它继承自 AdProviderHandler 并实现广告的初始化、显示、隐藏、加载等功能
    public class AdDummyHandler : AdProviderHandler
    {
        private AdDummyController dummyController; // 控制广告展示的 DummyController
        private bool isInterstitialLoaded = false; // 用于标识插页广告是否已加载
        private bool isRewardVideoLoaded = false;  // 用于标识奖励视频是否已加载

        // 构造函数，初始化广告提供商类型
        public AdDummyHandler(AdProvider providerType) : base(providerType) { }

        // 初始化广告模块
        public override void Initialise(AdsSettings adsSettings)
        {
            this.adsSettings = adsSettings;

            // 输出日志信息（如果启用了系统日志）
            if (adsSettings.SystemLogs)
                Debug.Log("[AdsManager]: Module " + providerType.ToString() + " has initialized!");

            // 检查是否启用了模拟广告系统
            if (adsSettings.IsDummyEnabled())
            {
                // 获取预设的 Dummy 广告 Canvas
                GameObject dummyCanvasPrefab = AdsManager.InitModule.DummyCanvasPrefab;
                if (dummyCanvasPrefab != null)
                {
                    // 实例化 Dummy 广告 Canvas，并初始化其位置、缩放和旋转
                    GameObject dummyCanvas = GameObject.Instantiate(dummyCanvasPrefab);
                    dummyCanvas.transform.position = Vector3.zero;
                    dummyCanvas.transform.localScale = Vector3.one;
                    dummyCanvas.transform.rotation = Quaternion.identity;

                    // 获取 AdDummyController 组件并进行初始化
                    dummyController = dummyCanvas.GetComponent<AdDummyController>();
                    dummyController.Initialise(adsSettings);
                }
                else
                {
                    Debug.LogError("[AdsManager]: Dummy controller can't be null!"); // 报错信息：Dummy 控制器不能为空
                }
            }

            OnProviderInitialised(); // 调用提供商初始化完成的回调
        }

        // 显示横幅广告
        public override void ShowBanner()
        {
            dummyController.ShowBanner();
            AdsManager.OnProviderAdDisplayed(providerType, AdType.Banner); // 触发横幅广告显示事件
        }

        // 隐藏横幅广告
        public override void HideBanner()
        {
            dummyController.HideBanner();
            AdsManager.OnProviderAdClosed(providerType, AdType.Banner); // 触发横幅广告关闭事件
        }

        // 销毁横幅广告
        public override void DestroyBanner()
        {
            dummyController.HideBanner();
            AdsManager.OnProviderAdClosed(providerType, AdType.Banner); // 触发横幅广告关闭事件
        }

        // 请求加载插页广告
        public override void RequestInterstitial()
        {
            isInterstitialLoaded = true; // 标记插页广告已加载
            AdsManager.OnProviderAdLoaded(providerType, AdType.Interstitial); // 触发插页广告加载完成事件
        }

        // 检查插页广告是否已加载
        public override bool IsInterstitialLoaded()
        {
            return isInterstitialLoaded;
        }

        // 显示插页广告
        public override void ShowInterstitial(InterstitialCallback callback)
        {
            dummyController.ShowInterstitial();
            AdsManager.OnProviderAdDisplayed(providerType, AdType.Interstitial); // 触发插页广告显示事件
        }

        // 请求加载奖励视频广告
        public override void RequestRewardedVideo()
        {
            isRewardVideoLoaded = true; // 标记奖励视频广告已加载
            AdsManager.OnProviderAdLoaded(providerType, AdType.RewardedVideo); // 触发奖励视频广告加载完成事件
        }

        // 检查奖励视频广告是否已加载
        public override bool IsRewardedVideoLoaded()
        {
            return isRewardVideoLoaded;
        }

        // 显示奖励视频广告
        public override void ShowRewardedVideo(RewardedVideoCallback callback)
        {
            dummyController.ShowRewardedVideo();
            AdsManager.OnProviderAdDisplayed(providerType, AdType.RewardedVideo); // 触发奖励视频广告显示事件
        }
    }
}

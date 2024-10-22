using UnityEngine;

namespace Watermelon
{
    // AdProviderHandler 是广告提供商处理的抽象基类
    // 它定义了广告系统的基本操作和广告的生命周期方法
    public abstract class AdProviderHandler
    {
        // 广告提供商类型 (例如 Google、Unity、AdMob 等)
        protected AdProvider providerType;

        // 提供商类型的公共访问器
        public AdProvider ProviderType => providerType;

        // 用于存储广告设置的引用
        protected AdsSettings adsSettings;

        // 用于标识广告提供商是否已初始化
        protected bool isInitialised = false;

        // 构造函数，初始化提供商类型
        public AdProviderHandler(AdProvider providerType)
        {
            this.providerType = providerType;
        }

        // 返回广告提供商是否已初始化
        public bool IsInitialised()
        {
            return isInitialised;
        }

        // 设置提供商为已初始化并触发相关的事件
        protected void OnProviderInitialised()
        {
            isInitialised = true;

            // 通知 AdsManager 广告提供商已初始化
            AdsManager.OnProviderInitialised(providerType);

            // 如果广告设置启用了系统日志，输出初始化信息
            if (adsSettings.SystemLogs)
                Debug.Log(string.Format("[AdsManager]: {0} is initialized!", providerType));
        }

        // 抽象方法，用于初始化广告提供商，子类需要具体实现
        public abstract void Initialise(AdsSettings adsSettings);

        // 抽象方法，用于显示横幅广告
        public abstract void ShowBanner();

        // 抽象方法，用于隐藏横幅广告
        public abstract void HideBanner();

        // 抽象方法，用于销毁横幅广告
        public abstract void DestroyBanner();

        // 抽象方法，请求加载插页广告
        public abstract void RequestInterstitial();

        // 抽象方法，显示插页广告，包含回调函数以处理广告展示完成的事件
        public abstract void ShowInterstitial(InterstitialCallback callback);

        // 抽象方法，返回插页广告是否已加载
        public abstract bool IsInterstitialLoaded();

        // 抽象方法，请求加载奖励视频广告
        public abstract void RequestRewardedVideo();

        // 抽象方法，显示奖励视频广告，包含回调函数以处理广告播放结果
        public abstract void ShowRewardedVideo(RewardedVideoCallback callback);

        // 抽象方法，返回奖励视频广告是否已加载
        public abstract bool IsRewardedVideoLoaded();

        // 虚拟方法，用于设置 GDPR 同意状态，子类可以选择性覆盖此方法
        public virtual void SetGDPR(bool state) { }

        // 委托，用于奖励视频广告的回调，传递是否获得奖励的状态
        public delegate void RewardedVideoCallback(bool hasReward);

        // 委托，用于插页广告的回调，传递广告是否成功展示的状态
        public delegate void InterstitialCallback(bool isDisplayed);
    }
}

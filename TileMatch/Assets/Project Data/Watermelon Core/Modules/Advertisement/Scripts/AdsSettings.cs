#pragma warning disable 0414 // 禁用未使用变量的警告

using UnityEngine;

namespace Watermelon
{
    // 广告设置类，用于配置广告系统的不同提供商和广告行为的相关设置
    [SetupTab("Advertising", texture = "icon_ads")]
    [CreateAssetMenu(fileName = "Ads Settings", menuName = "Settings/Ads Settings")]
    [HelpURL("https://www.notion.so/wmelongames/Advertisement-221053e32d4047bb880275027daba9f0?pvs=4")]
    public class AdsSettings : ScriptableObject
    {
        // 广告横幅类型的提供商，默认使用 Dummy 提供商
        [SerializeField] AdProvider bannerType = AdProvider.Dummy;
        public AdProvider BannerType => bannerType; // 获取横幅广告提供商类型

        // 插页广告类型的提供商，默认使用 Dummy 提供商
        [SerializeField] AdProvider interstitialType = AdProvider.Dummy;
        public AdProvider InterstitialType => interstitialType; // 获取插页广告提供商类型

        // 奖励视频类型的提供商，默认使用 Dummy 提供商
        [SerializeField] AdProvider rewardedVideoType = AdProvider.Dummy;
        public AdProvider RewardedVideoType => rewardedVideoType; // 获取奖励视频广告提供商类型

        // AdMob 广告设置容器
        [SerializeField] AdMobContainer adMobContainer;
        public AdMobContainer AdMobContainer => adMobContainer; // 获取 AdMob 容器

        // Unity Ads 广告设置容器
        [SerializeField] UnityAdsLegacyContainer unityAdsContainer;
        public UnityAdsLegacyContainer UnityAdsContainer => unityAdsContainer; // 获取 Unity Ads 容器

        // IronSource 广告设置容器
        [SerializeField] IronSourceContainer ironSourceContainer;
        public IronSourceContainer IronSourceContainer => ironSourceContainer; // 获取 IronSource 容器

        // Dummy 广告设置容器，提供模拟广告的功能
        [SerializeField] AdDummyContainer dummyContainer;
        public AdDummyContainer DummyContainer => dummyContainer; // 获取 Dummy 容器

        // 是否启用测试模式，用于调试广告提供商的配置
        [Tooltip("Enables development mode to setup advertisement providers.")]
        [SerializeField] bool testMode = false;
        public bool TestMode => testMode; // 获取测试模式设置

        // 系统日志开关，启用后会记录广告系统的日志
        [Group("Settings")]
        [Tooltip("Enables logging. Use it to debug advertisement logic.")]
        [SerializeField] bool systemLogs = false;
        public bool SystemLogs => systemLogs; // 获取系统日志设置

        [Space]

        // 首次启动时插页广告的显示延迟（秒）
        [Group("Settings")]
        [Tooltip("Delay in seconds before interstitial appearings on first game launch.")]
        [SerializeField] float interstitialFirstStartDelay = 40f;
        public float InterstitialFirstStartDelay => interstitialFirstStartDelay; // 获取首次启动时插页广告的延迟

        // 每次启动时插页广告的显示延迟（秒）
        [Group("Settings")]
        [Tooltip("Delay in seconds before interstitial appearings.")]
        [SerializeField] float interstitialStartDelay = 40f;
        public float InterstitialStartDelay => interstitialStartDelay; // 获取插页广告启动时的延迟

        // 两次插页广告之间的显示间隔（秒）
        [Group("Settings")]
        [Tooltip("Delay in seconds between interstitial appearings.")]
        [SerializeField] float interstitialShowingDelay = 30f;
        public float InterstitialShowingDelay => interstitialShowingDelay; // 获取插页广告的展示间隔

        // 是否自动展示插页广告
        [Group("Settings")]
        [SerializeField] bool autoShowInterstitial;
        public bool AutoShowInterstitial => autoShowInterstitial; // 获取自动展示插页广告的设置

        // 是否启用 GDPR (通用数据保护条例) 设置
        [Group("Privacy")]
        [SerializeField] bool isGDPREnabled = false;
        public bool IsGDPREnabled => isGDPREnabled; // 获取 GDPR 设置

        // 是否启用 IDFA（广告标识符）追踪
        [Group("Privacy")]
        [SerializeField] bool isIDFAEnabled = false;
        public bool IsIDFAEnabled => isIDFAEnabled; // 获取 IDFA 设置

        // 追踪描述，用于告知用户数据将用于个性化广告
        [Group("Privacy")]
        [SerializeField] string trackingDescription = "Your data will be used to deliver personalized ads to you.";
        public string TrackingDescription => trackingDescription; // 获取追踪描述

        // 隐私政策链接
        [Group("Privacy")]
        [SerializeField] string privacyLink = "https://mywebsite.com/privacy";
        public string PrivacyLink => privacyLink; // 获取隐私政策链接

        // 使用条款链接
        [Group("Privacy")]
        [SerializeField] string termsOfUseLink = "https://mywebsite.com/terms";
        public string TermsOfUseLink => termsOfUseLink; // 获取使用条款链接

        // 检查是否启用了 Dummy 广告
        public bool IsDummyEnabled()
        {
            // 如果任意广告类型为 Dummy 提供商，则返回 true
            if (bannerType == AdProvider.Dummy)
                return true;

            if (interstitialType == AdProvider.Dummy)
                return true;

            if (rewardedVideoType == AdProvider.Dummy)
                return true;

            return false;
        }
    }

    // 定义横幅广告的位置枚举类型
    public enum BannerPosition
    {
        Bottom = 0, // 横幅广告位于屏幕底部
        Top = 1, // 横幅广告位于屏幕顶部
    }
}

using System.Collections;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Watermelon
{
    // 注册模块，模块名为 "Monetization/Ads Manager"
    [RegisterModule("Monetization/Ads Manager")]
    public class AdsManagerInitModule : InitModule
    {
        // 广告设置
        public AdsSettings Settings;

        // 用于显示广告的假画布预制体
        public GameObject DummyCanvasPrefab;

        // 用于 GDPR 合规的预制体
        public GameObject GDPRPrefab;

        [Space]

        // 在启动时是否加载广告的选项
        public bool LoadAdOnStart = true;

        // 构造函数，初始化模块名称
        public AdsManagerInitModule()
        {
            moduleName = "Ads Manager";
        }

        // 创建组件方法，初始化广告管理器
        public override void CreateComponent(Initialiser initialiser)
        {
            // 初始化广告管理器，传入设置和是否在启动时加载广告的选项
            AdsManager.Initialise(this, LoadAdOnStart);

#if UNITY_IOS
            // 如果启用 IDFA 且尚未确定 IDFA 权限，则请求授权
            if (Settings.IsIDFAEnabled && !AdsManager.IsIDFADetermined())
            {
                if (Settings.SystemLogs)
                    Debug.Log("[Ads Manager]: Requesting IDFA..");

                // 请求 iOS 设备的广告跟踪授权
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#endif
        }
    }
}

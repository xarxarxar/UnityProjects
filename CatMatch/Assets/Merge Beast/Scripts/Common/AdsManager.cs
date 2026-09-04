using UnityEngine;
using UnityEngine.Events;

namespace MergeBeast
{
    /// <summary>
    /// 纯净版广告管理器：已移除 AppLovin MAX / UnityAds / 埋点上报等所有广告 SDK 依赖。
    /// 所有"看广告"行为直接发放奖励，游戏玩法保持完整。
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        private static AdsManager _instance;
        public static AdsManager Instance => _instance;

        private UnityAction _rewardAds;
        private bool _showing;
        public bool inited;
        public bool onInterestial = true;

        // Use this for initialization
        private void Awake()
        {
            if (_instance == null) _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            // 广告 SDK 已移除，无需任何初始化
            inited = true;
        }

        /// <summary>广告是否就绪（纯净版恒为 true，奖励始终可领）</summary>
        public bool IsLoaded(EnumDefine.ADSTYPE type = EnumDefine.ADSTYPE.Rewarded)
        {
            return true;
        }

        /// <summary>展示插屏广告（纯净版无操作）</summary>
        public void OnShowInter()
        {
            // 已移除广告系统，无操作
        }

        /// <summary>
        /// 展示广告（纯净版直接发放"看广告"的奖励）
        /// </summary>
        public void ShowAds(UnityAction callback, string keyEvent = null, EnumDefine.ADSTYPE adType = EnumDefine.ADSTYPE.Rewarded,
            bool required = true, EnumDefine.GameType gameType = EnumDefine.GameType.Beast)
        {
            if (!onInterestial && adType == EnumDefine.ADSTYPE.Interstitial) return;

            _showing = true;

            // 纯净版：视为广告已看完，直接发放奖励
            DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.WatchAds);
            CPlayer.AddVipPoint(1);
            callback?.Invoke();

            _showing = false;
        }

        public bool IsShowing()
        {
            return _showing;
        }
    }
}

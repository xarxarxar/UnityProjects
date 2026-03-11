using UnityEngine;
using UnityEngine.Events;
using WeChatWASM;

public class WXAdsManager : MonoBehaviour
{
    private static WXAdsManager _instance;
    public static WXAdsManager Instance => _instance;

    [Header("广告单元ID")]
    [SerializeField] private string videoAdId = "adunit-85a72a8ea99ac672";

    private WXRewardedVideoAd _rewardedVideoAd;

    private UnityAction<bool> _rewardCallback;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 建议在这里自动初始化，或者在游戏启动页调用 Init()
        Init();
    }

    public void Init()
    {
        // 1. 初始化激励视频 (带自动拉取逻辑)
        _rewardedVideoAd = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam { adUnitId = videoAdId });
        _rewardedVideoAd.OnClose(res => OnPlayEnd(res.isEnded));
        _rewardedVideoAd.OnError(res => Debug.LogError($"激励视频错误: {res.errMsg}"));
    }

    #region 公开调用方法
    /// <summary>
    /// 播放激励视频
    /// </summary>
    public void ShowVideoAd(UnityAction<bool> onComplete)
    {
        if (_rewardedVideoAd == null) return;

        _rewardCallback = onComplete;
        _rewardedVideoAd.Show();
    }
    #endregion

    private void OnPlayEnd(bool isSuccess)
    {
        // 执行回调
        _rewardCallback?.Invoke(isSuccess);
        _rewardCallback = null; // 用完清空，防止内存泄漏或重复调用
    }
}
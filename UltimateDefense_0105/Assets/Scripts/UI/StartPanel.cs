using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// UI
/// </summary>
public class StartPanel : MonoBehaviour
{
    #region 私有变量
    [SerializeField] private Image _towerImage;  //炮塔的image
    [SerializeField] private Image _towerPlatformImage;  //炮塔底座的image
    private RectTransform _rt=> _towerImage.rectTransform;
    private Sequence _scanSeq;
    private const float LEFT_ANGLE = 60f;
    private const float RIGHT_ANGLE = -60f;
    [SerializeField] private BindableButton _startBattleButton;  //开始挑战按钮
    [SerializeField] private BindableButton _onlineRewardButton; //在线奖励按钮
    [SerializeField] private BindableButton _dailyMissionButton; //每日任务按钮
    [SerializeField] private BindableButton _achievementButton;  //成就按钮
    [SerializeField] private BindableButton _settingButton;      //设置按钮
    [SerializeField] private BindableButton _rankButton;         //排行榜按钮
    [SerializeField] private BindableButton _shareButton;        //邀请有礼按钮


    [SerializeField] private BasePanel _onlineRewardPanel;  
    [SerializeField] private BasePanel _dailyMissionPanel;
    [SerializeField] private BasePanel _achievementPanel;
    [SerializeField] private BasePanel _settingPanel;
    [SerializeField] private BasePanel _rankPanel;
    [SerializeField] private BasePanel _sharePanel;
    private Dictionary<BindableButton, BasePanel> _buttonPanelMap;


    #endregion

    #region 公共变量
    /// <summary>
    /// 开始挑战按钮被点击
    /// </summary>
    public static event UnityAction OnSatrtBattle;
    #endregion

    #region 私有方法
    private void Start()
    {
        _startBattleButton.AddListener(StartBattleButton);

        _buttonPanelMap = new Dictionary<BindableButton, BasePanel>
    {
        { _onlineRewardButton, _onlineRewardPanel },
        { _dailyMissionButton, _dailyMissionPanel },
        { _achievementButton, _achievementPanel },
        { _settingButton, _settingPanel },
        { _rankButton, _rankPanel },
        { _shareButton, _sharePanel }
    };

        foreach (var pair in _buttonPanelMap)
        {
            pair.Key.AddListener(() =>
            {
                pair.Value.gameObject.SetActive(true);
            });
        }
    }

    private void OnEnable()
    {
        Init();
    }

    /// <summary>
    /// 开始挑战按钮的点击方法
    /// </summary>
    private void StartBattleButton()
    {
        OnSatrtBattle?.Invoke();//开始挑战按钮被点击
        GameUIManager.Instance.HideMainMenu();//隐藏主菜单
        GameUIManager.Instance.ShowChooseDebuffPanel();
        AudioManager.Instance.PlayBGM("对局中BGM");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化开始界面
    /// </summary>
    public void Init()
    {
        
        _towerImage.sprite = TowerDataManager.Instance.GetCurrentSkin(TowerDataManager.Instance.GetCurrentTowerData()).sprite;
        _towerPlatformImage.sprite = TowerPlatformDataManager.Instance.GetCurrentSkin(TowerPlatformDataManager.Instance.GetCurrentTowerPlatformData()).sprite;
        StartScan();
    }


    #endregion
    public void StartScan()
    {
        StopScan();

        PlayOneScan(30);
    }

    public void StopScan()
    {
        _scanSeq?.Kill();
        _scanSeq = null;

        if (_rt != null)
            _rt.localRotation = Quaternion.identity;
    }

    private void PlayOneScan(float startAngle)
    {
        if (_rt == null) return;

        _scanSeq = DOTween.Sequence();
        _scanSeq.SetUpdate(false);

        AppendScanStep(_scanSeq, startAngle);

        _scanSeq.OnComplete(() =>
        {
            // 下一轮继续扫（方向翻转）
            PlayOneScan(-startAngle);
        });
    }


    private void AppendScanStep(Sequence seq, float targetAngle)
    {
        float rotateDuration = Random.Range(2.0f, 5.0f);
        float pauseDuration = Random.Range(1.0f, 3.0f);

        float finalAngle = targetAngle;

        if (Random.value < 0.25f)
            finalAngle *= Random.Range(0.3f, 0.6f);

        if (Random.value < 0.15f)
        {
            rotateDuration *= 0.4f;
            seq.AppendInterval(0.1f);
        }

        seq.Append(
            _rt.DOLocalRotate(
                new Vector3(0, 0, finalAngle),
                rotateDuration
            ).SetEase(Ease.InOutSine)
        );

        seq.AppendInterval(pauseDuration);
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField]private GameObject towerPanel;//炮塔
    [SerializeField]private GameObject towerPlatformPanel;//炮塔
    [SerializeField]private GameObject startPanel;//开始界面
    [SerializeField]private GameObject sciencePanel;//科技

    [SerializeField]private Button towerButton;
    [SerializeField]private Button towerPlatformButton;
    [SerializeField]private Button challengeButton;
    [SerializeField]private Button scienceButton;

    [SerializeField]private RewardStruct _diamondReward;//钻石奖励
    [SerializeField]private RewardStruct _crownReward;  //王冠奖励
    [SerializeField]private Text _medalCount;  //奖牌数量
    //[SerializeField]private Text _passCountText;//通关次数的Text

    void Start()
    {
        OnDataLoaded();//数据加载完毕，显示数据，绑定事件等等
        //DataManager.OnDataLoaded += OnDataLoaded;
    }

    //切换面板
    private void SwitchToPanel(GameObject targetPanel)
    {
        // 先全部关闭
        towerPanel.SetActive(false);
        startPanel.SetActive(false);
        sciencePanel.SetActive(false);
        towerPlatformPanel.SetActive(false);

        // 打开目标 Panel 和对应的 Outline
        if (targetPanel == towerPanel)
        {
            towerPanel.SetActive(true);
            OnSetButtonActive(towerButton);
        }
        else if (targetPanel == startPanel)
        {
            startPanel.GetComponent<StartPanel>().Init(); 
            OnSetButtonActive(challengeButton);
        }
        else if (targetPanel == sciencePanel)
        {
            sciencePanel.SetActive(true);
            OnSetButtonActive(scienceButton);
        }
        else if(targetPanel== towerPlatformPanel)
        {
            towerPlatformPanel.SetActive(true);
            OnSetButtonActive(towerPlatformButton);
        }
    }

    //局外金币变化时
    private void OnMetaCoinChanged(RewardType rewardType,int amount)
    {
        switch (rewardType)
        {
            case RewardType.Diamond:
                _diamondReward.Init(amount); break;
            case RewardType.Crown:
                _crownReward.Init(amount); break;
            default:break;
        }
    }

    /// <summary>
    /// 数据加载完毕
    /// </summary>
    private void OnDataLoaded()
    {
        DataManager.Instance.PlayerInfo.Diamond.OnValueChanged += (value) =>
        {
            OnMetaCoinChanged(RewardType.Diamond, value);
        };
        DataManager.Instance.PlayerInfo.Crown.OnValueChanged += (value) =>
        {
            OnMetaCoinChanged(RewardType.Crown, value);
        };
        DataManager.Instance.PlayerInfo.Medal.OnValueChanged += (value) =>
        {
            _medalCount.text= DataManager.Instance.PlayerInfo.Medal.Value.ToString();
        };
        _medalCount.text = DataManager.Instance.PlayerInfo.Medal.Value.ToString();
        OnMetaCoinChanged(RewardType.Diamond, DataManager.Instance.PlayerInfo.Diamond.Value);
        OnMetaCoinChanged(RewardType.Crown, DataManager.Instance.PlayerInfo.Crown.Value);

        // 绑定按钮点击事件
        towerButton.onClick.AddListener(() => SwitchToPanel(towerPanel));
        towerPlatformButton.onClick.AddListener(() => SwitchToPanel(towerPlatformPanel));
        challengeButton.onClick.AddListener(() => SwitchToPanel(startPanel));
        scienceButton.onClick.AddListener(() => SwitchToPanel(sciencePanel));

        // 初始默认打开某个面板（如技能）
        SwitchToPanel(startPanel);
    }

    private void OnSetButtonActive(Button activeButton)
    {
        // 找到三个按钮
        Button[] allButtons = new Button[] { towerButton, challengeButton, scienceButton, towerPlatformButton };

        foreach (var btn in allButtons)
        {
            bool isActive = (btn == activeButton);

            // 主图片（背景色）
            Image bg = btn.transform.Find("按钮显示").GetComponent<Image>();

            // 图标节点
            RectTransform icon = btn.transform.Find("按钮显示/Image").GetComponent<RectTransform>();

            // === 停掉旧动画 ===
            bg.DOKill();
            icon.DOKill();

            // === 状态属性 ===
            Color targetBgColor = isActive ? new Color32(32, 103, 172, 255)
                                           : new Color32(55, 66, 118, 255);

            Vector2 targetPos = isActive ? new Vector2(0, 90)
                                         : new Vector2(0, 60);

            Vector2 targetSize = isActive ? new Vector2(180, 180)
                                          : new Vector2(120, 120);

            // === 动画 ===
            float duration = 0.25f;

            // 颜色动画
            bg.DOColor(targetBgColor, duration);

            // 位置动画
            icon.DOAnchorPos(targetPos, duration).SetEase(Ease.OutQuad);


            // 大小动画（sizeDelta 只能直接改，DOTween 没有 DoSizeDelta）
            DOTween.To(() => icon.sizeDelta, x => icon.sizeDelta = x, targetSize, duration)
                   .SetEase(Ease.OutQuad);
        }
    }
}

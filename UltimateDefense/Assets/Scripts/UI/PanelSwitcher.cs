using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField]private GameObject towerPanel;//炮塔
    [SerializeField]private GameObject challengePanel;//挑战
    [SerializeField]private GameObject sciencePanel;//科技

    [SerializeField]private Button towerButton;
    [SerializeField]private Button challengeButton;
    [SerializeField]private Button scienceButton;
    private Outline towerOutline => towerButton.transform.GetChild(0).GetComponent<Outline>();
    private Outline challengeOutline => challengeButton.transform.GetChild(0).GetComponent<Outline>();
    private Outline scienceOutline => scienceButton.transform.GetChild(0).GetComponent<Outline>();

    [SerializeField]private RewardStruct _diamondReward;//钻石奖励
    [SerializeField]private RewardStruct _crownReward;  //王冠奖励
    [SerializeField]private Text _passCountText;//通关次数的Text

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
        challengePanel.SetActive(false);
        sciencePanel.SetActive(false);

        // 全部禁用 Outline
        towerOutline.enabled = false;
        challengeOutline.enabled = false;
        scienceOutline.enabled = false;

        // 打开目标 Panel 和对应的 Outline
        if (targetPanel == towerPanel)
        {
            towerPanel.SetActive(true);
            towerOutline.enabled = true;
        }
        else if (targetPanel == challengePanel)
        {
            challengePanel.SetActive(true);
            challengeOutline.enabled = true;
        }
        else if (targetPanel == sciencePanel)
        {
            sciencePanel.SetActive(true);
            scienceOutline.enabled = true;
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
        DataManager.Instance.PlayerInfo.DiamondCount.OnValueChanged += (value) =>
        {
            OnMetaCoinChanged(RewardType.Diamond, value);
        };
        DataManager.Instance.PlayerInfo.CrownCount.OnValueChanged += (value) =>
        {
            OnMetaCoinChanged(RewardType.Crown, value);
        };

        OnMetaCoinChanged(RewardType.Diamond, DataManager.Instance.PlayerInfo.DiamondCount.Value);
        OnMetaCoinChanged(RewardType.Crown, DataManager.Instance.PlayerInfo.CrownCount.Value);

        // 绑定按钮点击事件
        towerButton.onClick.AddListener(() => SwitchToPanel(towerPanel));
        challengeButton.onClick.AddListener(() => SwitchToPanel(challengePanel));
        scienceButton.onClick.AddListener(() => SwitchToPanel(sciencePanel));

        // 初始默认打开某个面板（如技能）
        SwitchToPanel(challengePanel);
    }
}

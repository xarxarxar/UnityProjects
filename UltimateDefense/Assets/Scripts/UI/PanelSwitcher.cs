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
    private Outline towerOutline => towerButton.GetComponent<Outline>();
    private Outline challengeOutline => challengeButton.GetComponent<Outline>();
    private Outline scienceOutline => scienceButton.GetComponent<Outline>();

    [SerializeField]private Text _coinText;//局外金币Text
    [SerializeField]private Text _passCountText;//通关次数的Text

    void Start()
    {
        MetaCurrencyManager.OnMetaCoinChanged += OnMetaCoinChanged;
        DataManager.OnPassCountChanged += OnPassCountChanged;

        // 绑定按钮点击事件
        towerButton.onClick.AddListener(() => SwitchToPanel(towerPanel));
        challengeButton.onClick.AddListener(() => SwitchToPanel(challengePanel));
        scienceButton.onClick.AddListener(() => SwitchToPanel(sciencePanel));

        // 初始默认打开某个面板（如技能）
        SwitchToPanel(challengePanel);
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
    private void OnMetaCoinChanged(int amount)
    {
        _coinText.text = $"{MetaCurrencyManager.Instance.MetaCoin}";
    }

    //通关次数变化时
    private void OnPassCountChanged(int value)
    {
        _passCountText.text= $"通关次数：{value}";
    }
}

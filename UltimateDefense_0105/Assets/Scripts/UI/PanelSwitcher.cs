using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SerializableDictionary;
using SerializableDictionary.Scripts;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField]private Button challengeButton;

    [SerializeField]private RewardStruct _diamondReward;//钻石奖励
    [SerializeField]private RewardStruct _crownReward;  //王冠奖励
    [SerializeField]private Text _medalCount;  //奖牌数量

   [SerializeField] private SerializableDictionary<Button, GameObject> ButtonAndPanels = new SerializableDictionary<Button, GameObject>();


    void Start()
    {
        OnDataLoaded();//数据加载完毕，显示数据，绑定事件等等
    }

    //切换到对应的面板
    private void SwitchToPanel(Button button)
    {
        // 先全部关闭
        foreach(var item in ButtonAndPanels.Dictionary)
        {
            item.Value.SetActive(false);
        }

        ButtonAndPanels.Dictionary[button].SetActive(true);
        
        OnSetButtonActive(button);

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
        DataManager.Instance.PlayerInfo.OnDiamondChanged += (oldValue, newValue) =>
        {
            OnMetaCoinChanged(RewardType.Diamond, newValue);
        };
        DataManager.Instance.PlayerInfo.OnCrownChanged += (oldValue, newValue) =>
        {
            OnMetaCoinChanged(RewardType.Crown, newValue);
        };
        DataManager.Instance.PlayerInfo.OnMedalChanged += (oldValue, newValue) =>
        {
            _medalCount.text= newValue.ToString();
        };
        _medalCount.text = DataManager.Instance.PlayerInfo.Medal.ToString();
        OnMetaCoinChanged(RewardType.Diamond, DataManager.Instance.PlayerInfo.Diamond);
        OnMetaCoinChanged(RewardType.Crown, DataManager.Instance.PlayerInfo.Crown);

        // 绑定按钮点击事件
        foreach (var item in ButtonAndPanels.Dictionary)
        {
            item.Key.onClick.AddListener(()=> SwitchToPanel(item.Key));
        }

        // 初始默认打开某个面板（如技能）
        SwitchToPanel(challengeButton);
    }

    private void OnSetButtonActive(Button activeButton)
    {
        
        foreach (var item in ButtonAndPanels.Dictionary)
        {
            Button btn = item.Key;
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

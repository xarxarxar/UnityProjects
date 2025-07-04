using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Button _leftButton;  //左边的升级按钮
    [SerializeField]
    private Button _middleButton;//中间的升级按钮
    [SerializeField]
    private Button _rightButton; //右边的升级按钮
    [SerializeField]
    private Text _leftDescription;  //左边的描述文字
    [SerializeField]
    private Text _middleDescription;  //中间的描述文字
    [SerializeField]
    private Text _rightDescription;  //右边的描述文字
    [SerializeField]
    private Text _leftPriceText;  //左边的售价文字
    [SerializeField]
    private Text _middlePriceText;  //中间的售价文字
    [SerializeField]
    private Text _rightPriceText;  //右边的售价文字
    [SerializeField]
    private Text _updatePriceText;  //刷新需要消耗的金币
    [SerializeField]
    private Button _refreshButton;//刷新升级属性的按钮

    private bool _isInited;//是否初始化
    private int  gold => CurrencyManager.Instance.Gold;
    private UpgradeManager _upgradeManager=>UpgradeManager.Instance;
    private List<UpgradeBase> _currentUpgradeBases;

    /// <summary>
    /// 初始化升级面板
    /// </summary>
    /// <param name="upgradeBases"></param>
    public void Init(List<UpgradeBase> upgradeBases)
    {
        if(_isInited) return;
        RefreshUpgradeButtons(upgradeBases);
        _isInited=true;

        _refreshButton.onClick.AddListener(RefreshUpgrade);
        _updatePriceText.text = "0";

        CurrencyManager.OnCoinChange += OnCoinChange;
    }

    /// <summary>
    /// 游戏结束后销毁升级的数据
    /// </summary>
    public void DestroyUpgrade()
    {
        _isInited = false;
        _refreshButton.onClick.RemoveAllListeners();
    }

    //刷新升级属性按钮
    private void RefreshUpgrade()
    {
        if (CurrencyManager.Instance.SpendCoin(UpgradeManager.Instance.UpdateCount * 10))//刷新需要金币
        {
            RefreshUpgradeButtons(UpgradeFactory.GetRandomUpgrades(3));
            UpgradeManager.Instance.UpdateCount++;
        }
        else
        {
            GameUIManager.Instance.ShowTipPanel("金币不足，刷新失败");
        }
        _updatePriceText.text = $"{UpgradeManager.Instance.UpdateCount * 10}";

        SetTextColor(_updatePriceText, gold >= UpgradeManager.Instance.UpdateCount * 10);
        SetButtonStatus(_refreshButton, gold >= UpgradeManager.Instance.UpdateCount * 10);
    }

    /// <summary>
    /// 刷新升级属性的按钮
    /// </summary>
    public void RefreshUpgradeButtons(List<UpgradeBase> upgradeBases)
    {
        _currentUpgradeBases = upgradeBases;

        _leftButton.onClick.RemoveAllListeners();
        _middleButton.onClick.RemoveAllListeners();
        _rightButton.onClick.RemoveAllListeners();

        _leftButton.gameObject.SetActive(true);
        _middleButton.gameObject.SetActive(true);
        _rightButton.gameObject.SetActive(true);

        _leftDescription.text = upgradeBases[0].Description;
        _leftPriceText.text = upgradeBases[0].Cost.ToString();
        SetTextColor(_leftPriceText, gold >= upgradeBases[0].Cost);
        SetButtonStatus(_leftButton, gold >= _currentUpgradeBases[0].Cost);
        _leftButton.onClick.AddListener(() =>
        {
            BuyUpgrade(upgradeBases[0], _leftButton);
        });

        _middleDescription.text = upgradeBases[1].Description;
        _middlePriceText.text = upgradeBases[1].Cost.ToString();
        SetTextColor(_middlePriceText, gold >= upgradeBases[1].Cost);
        SetButtonStatus(_middleButton, gold >= _currentUpgradeBases[1].Cost);
        _middleButton.onClick.AddListener(() =>
        {
            BuyUpgrade(upgradeBases[1], _middleButton);
        });

        _rightDescription.text = upgradeBases[2].Description;
        _rightPriceText.text = upgradeBases[2].Cost.ToString();
        SetTextColor(_rightPriceText, gold >= upgradeBases[2].Cost);
        SetButtonStatus(_rightButton, gold >= _currentUpgradeBases[2].Cost);
        _rightButton.onClick.AddListener(() =>
        {
            BuyUpgrade(upgradeBases[2], _rightButton);
        });
    }

    //购买第一个升级
    private void BuyUpgrade(UpgradeBase upgradeBase,Button button)
    {
        if (_upgradeManager.PurchaseUpgrade(upgradeBase))
        {
            button.gameObject.SetActive(false);
        }
    }

    //金币变化
    private void OnCoinChange(int amount)
    {
        SetTextColor(_updatePriceText, gold >= UpgradeManager.Instance.UpdateCount * 10);
        SetButtonStatus(_refreshButton, gold >= UpgradeManager.Instance.UpdateCount * 10);
        if (_currentUpgradeBases == null || _currentUpgradeBases.Count < 3)
            return;

        SetTextColor(_leftPriceText, gold >= _currentUpgradeBases[0].Cost);
        SetButtonStatus(_leftButton, gold >= _currentUpgradeBases[0].Cost);

        SetTextColor(_middlePriceText, gold >= _currentUpgradeBases[1].Cost);
        SetButtonStatus(_middleButton, gold >= _currentUpgradeBases[1].Cost);

        SetTextColor(_rightPriceText, gold >= _currentUpgradeBases[2].Cost);
        SetButtonStatus(_rightButton, gold >= _currentUpgradeBases[2].Cost);
    }

    //设置为恩本颜色
    private void SetTextColor(Text priceText, bool isEnough)
    {
        priceText.color = isEnough ? new Color32(239, 241, 245, 255) : new Color32(228,73,98,255);
    }

    //设置按钮的状态
    private void SetButtonStatus(Button button, bool active)
    {
        button.interactable = active;

        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup)
        {
            canvasGroup.alpha = active ? 1 : 0.5f;
        }
    }
}

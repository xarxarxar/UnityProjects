using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private Button _refreshButton;//刷新升级属性的按钮

    private bool _isInited;//是否初始化
    private UpgradeManager _upgradeManager=>UpgradeManager.Instance;

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
        RefreshUpgradeButtons(UpgradeFactory.GetRandomUpgrades(3));
    }

    /// <summary>
    /// 刷新升级属性的按钮
    /// </summary>
    public void RefreshUpgradeButtons(List<UpgradeBase> upgradeBases)
    {
        _leftButton.onClick.RemoveAllListeners();
        _middleButton.onClick.RemoveAllListeners();
        _rightButton.onClick.RemoveAllListeners();

        _leftButton.gameObject.SetActive(true);
        _middleButton.gameObject.SetActive(true);
        _rightButton.gameObject.SetActive(true);

        _leftDescription.text = upgradeBases[0].Description;
        _leftPriceText.text = upgradeBases[0].Cost.ToString();
        _leftButton.onClick.AddListener(() =>
        {
            if (_upgradeManager.PurchaseUpgrade(upgradeBases[0]))
            {
                _leftButton.gameObject.SetActive(false);
            }
        });

        _middleDescription.text = upgradeBases[1].Description;
        _middlePriceText.text = upgradeBases[1].Cost.ToString();
        _middleButton.onClick.AddListener(() =>
        {
            if (_upgradeManager.PurchaseUpgrade(upgradeBases[1]))
            {
                _middleButton.gameObject.SetActive(false);
            }
        });

        _rightDescription.text = upgradeBases[2].Description;
        _rightPriceText.text = upgradeBases[2].Cost.ToString();
        _rightButton.onClick.AddListener(() =>
        {
            if (_upgradeManager.PurchaseUpgrade(upgradeBases[2]))
            {
                _rightButton.gameObject.SetActive(false);
            }
        });
    }
}

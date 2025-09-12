using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理玩家已购买的所有升级，处理购买流程与事件通知
/// </summary>
public class UpgradeManager : ManagerBase<UpgradeManager>
{
    #region 私有字段
    private List<UpgradeBase> _purchasedUpgrades = new List<UpgradeBase>();             // 已购买升级列表
    private int _totalSpentGold;                              // 累计花费的金币，可用于成就统计
    private Bindable<int> _refreshCount =new Bindable<int>();//刷新升级面板的次数
    private Bindable<int> _buyUpgradeCount =new Bindable<int>();//购买升级的次数
    private Bindable<int> _freshCost =new Bindable<int>();//购买升级的花费
    [SerializeField]
    private int _upgradeIncreaseCoin = 1;//每次刷新增加的钱数
    #endregion

    #region 公开属性
    /// <summary>
    /// 只读属性，暴露已购买的升级列表（只读视图）
    /// </summary>
    public IReadOnlyList<UpgradeBase> PurchasedUpgrades => _purchasedUpgrades;

    /// <summary>
    /// 刷新升级面板的次数
    /// </summary>
    public Bindable<int> RefreshCount { get => _refreshCount; set => _refreshCount = value; }
    /// <summary>
    /// 购买升级的次数
    /// </summary>
    public Bindable<int> BuyUpgradeCount { get => _buyUpgradeCount;}
    /// <summary>
    /// 每次刷新增加的钱数
    /// </summary>
    public int UpgradeIncreaseCoin { get => _upgradeIncreaseCoin; set => _upgradeIncreaseCoin = value; }

    /// <summary>
    /// 刷新的花费
    /// </summary>
    public Bindable<int> FreshCost=> _freshCost;

    /// <summary>
    /// 相同商品时的折扣
    /// </summary>
    public Bindable<float> SameDiscount { get; set; }=new Bindable<float>() { };
    #endregion

    #region 事件

    /// <summary>
    /// 当任意升级被购买时触发，参数为该升级实例
    /// </summary>
    public event Action<UpgradeBase> OnUpgradePurchased;

    #endregion

    #region Unity 生命周期

    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        Index = 1;
    }

    #endregion

    #region 公有方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Debug.Log("UpgradeManager初始化");
        _purchasedUpgrades.Clear();
        _totalSpentGold = 0;
        _refreshCount.Value = 0;
        _buyUpgradeCount.Value = 0;
        _freshCost.Value = 0;

        if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("SameDiscount"))
        {
            DataManager.Instance.PlayerInfo.Config["SameDiscount"] = 0.5f;
        }
        SameDiscount.Value = DataManager.Instance.PlayerInfo.Config["SameDiscount"];
    }
    /// <summary>
    /// 尝试购买指定升级：返回是否成功
    /// </summary>
    public bool PurchaseUpgrade(UpgradeBase upgrade)
    {
        // 检查金币是否足够
        if (!CurrencyManager.Instance.SpendCoin(upgrade.Cost))
        {
            //BattleUIManager.Instance.ShowToast("金币不足，无法购买升级");
            GameUIManager.Instance.ShowQuickTip("金币不足");
            Debug.Log($"金币不足，无法购买升级");
            return false;
        }

        // 花费成功，记录并触发事件
        _purchasedUpgrades.Add(upgrade);
        _totalSpentGold += upgrade.Cost;
        OnUpgradePurchased?.Invoke(upgrade);
        _buyUpgradeCount.Value++;
        //FreshCost.Value += _upgradeIncreaseCoin;
        //Debug.Log($"购买了{upgrade.Description}");

        // 应用升级效果
        upgrade.Apply();
        return true;
    }

    /// <summary>
    /// 查询玩家是否已购买过某 ID 的升级
    /// </summary>
    /// <param name="upgradeID">升级唯一标识</param>
    /// <returns>如果已购买返回 true，否则 false</returns>
    public bool HasPurchased(string upgradeID)
    {
        return _purchasedUpgrades.Exists(u => u.UpgradeID == upgradeID);
    }

    #endregion
}

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 局外资源管理器，局外货币可以用于升级属性，等等
/// </summary>
public class MetaCurrencyManager : ManagerBase<MetaCurrencyManager>
{
    #region 公共静态事件
    #endregion

    /// <summary>
    /// 只读属性，钻石数量
    /// </summary>
    public Bindable<int> DiamondCount { get => DataManager.Instance.PlayerInfo.DiamondCount; }

    /// <summary>
    /// 只读属性，王冠数量
    /// </summary>
    public Bindable<int> CrownCount { get => DataManager.Instance.PlayerInfo.CrownCount; }

    #region 私有方法
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.OutBattle;
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        BattleManager.OnEndBattle += OnEndBattle;
    }

    /// <summary>
    /// 增加局外货币
    /// </summary>
    public void AddMetaCoin(RewardType rewardType,int amount,bool showUI=false)
    {
        if(rewardType ==RewardType.Diamond)
        {
            DiamondCount.Value += amount;
            
        }
        if(rewardType == RewardType.Crown)
        {
            CrownCount.Value += amount;
        }
        if (showUI)
        {
            GameUIManager.Instance.ShowGetRewardPanel((rewardType, amount));
        }
        
    }

    /// <summary>
    /// 花费局外货币
    /// </summary>
    /// <param name="rewardType">货币类型（只支持 Diamond 和 Crown）</param>
    /// <param name="amount">花费的数量</param>
    /// <returns>余额是否足够并成功扣费</returns>
    public bool SpendMetaCoin(RewardType rewardType, int amount)
    {
        switch (rewardType)
        {
            case RewardType.Crown:
                if (CrownCount.Value >= amount)
                {
                    CrownCount.Value -= amount;
                    return true;
                }
                break;

            case RewardType.Diamond:
                if (DiamondCount.Value >= amount)
                {
                    DiamondCount.Value -= amount;
                    return true;
                }
                break;

            case RewardType.Coin:
                // 局内金币不处理
                return false;
        }
        return false;
    }

    /// <summary>
    /// 是否有足够的局外货币，只判断不扣款
    /// </summary>
    /// <param name="success"></param>
    public bool HasEnoughMoney(RewardType rewardType, int amount)
    {
        switch (rewardType)
        {
            case RewardType.Crown:
                if (CrownCount.Value >= amount)
                {
                    return true;
                }
                break;

            case RewardType.Diamond:
                if (DiamondCount.Value >= amount)
                {
                    return true;
                }
                break;

            case RewardType.Coin:
                // 局内金币不处理
                return false;
        }
        return false;
    }
    #endregion

    #region 私有方法
    private void OnEndBattle(bool success)
    {
        Debug.Log("挑战结束，增加局外金币");
        AddMetaCoin(RewardType.Diamond,BattleManager.Instance.DiamondCount);
        AddMetaCoin(RewardType.Crown,BattleManager.Instance.CrownCount);
    }
    #endregion
}

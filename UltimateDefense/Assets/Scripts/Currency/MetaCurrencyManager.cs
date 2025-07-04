using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 局外资源管理器，局外货币可以用于升级属性，等等
/// </summary>
public class MetaCurrencyManager : ManagerBase<MetaCurrencyManager>,IManager
{
    private int _metaCoin;          //局外货币
    #region 公共静态事件
    /// <summary>
    /// 局外货币数量变化事件，参数为变化的数量，并非当前总数
    /// </summary>
    public static event UnityAction<int> OnMetaCoinChanged;
    #endregion

    /// <summary>
    /// 只读属性，局外货币,暂定为局外只有一种局外货币
    /// </summary>
    public int MetaCoin { get => _metaCoin; }

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
        Debug.Log("MetaCurrencyManager 初始化");
        BattleManager.OnEndBattle += OnEndBattle;
    }

    /// <summary>
    /// 增加局外货币
    /// </summary>
    public void AddMetaCoin(int amount)
    {
        _metaCoin += amount;
        OnMetaCoinChanged?.Invoke(amount);
    }

    /// <summary>
    /// 花费局外货币
    /// </summary>
    /// <param name="amount">花费的数量</param>
    /// <returns>余额是否足够</returns>
    public bool SpendMetaCoin(int amount)
    {
        if (_metaCoin >= amount)
        {
            Debug.Log($"消耗金币{amount}");
            _metaCoin -= amount;
            OnMetaCoinChanged?.Invoke(amount);
            return true;
        }
        return false;
    }
    #endregion

    #region 私有方法
    private void OnEndBattle(bool success)
    {
        Debug.Log("挑战结束，增加局外金币");
        AddMetaCoin(BattleManager.Instance.MetaCoinCount);
    }
    #endregion
}

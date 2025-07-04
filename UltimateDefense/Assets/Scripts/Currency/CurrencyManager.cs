using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 管理局内玩家资源的增减与存档
/// </summary>
public class CurrencyManager : ManagerBase<CurrencyManager>,IManager
{
    #region 私有属性
    private int _gold=0;                          // 当前玩家持有的局内金币数量
    private bool _isInitialized;                // 标记是否已加载过存档
    
    private EnemyManager EnemyManager=>EnemyManager.Instance;//EnemyManager单例
    #endregion

    #region 公共静态变量
    public static UnityAction<int> OnCoinChange;      //金币数量发生变化
    public static UnityAction<Enemy,int> OnGetCoinFromEnemy;      //敌人死亡获取金币
    #endregion

    #region 公开属性

    /// <summary>
    /// 只读属性，暴露当前金币数
    /// </summary>
    public int Gold { get { return _gold; } }
    #endregion

    #region 常量
    //public const string SaveKey_Gold = "Save_Gold"; // 存档用的 PlayerPrefs 键
    #endregion

    #region public 成员方法
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            AddCoin(100);
        }
    }

    private void OnDisable()
    {
        _gold = 0;
        Enemy.OnEnemyDie -= OnEnemyDie;//订阅敌人死亡事件
    }

    public override void Init()
    {
        _gold = 0;
        Enemy.OnEnemyDie += OnEnemyDie;//订阅敌人死亡事件
    }

    /// <summary>
    /// 增加指定数量的金币，立即保存并刷新 UI
    /// </summary>
    /// <param name="amount">要增加的金币数</param>
    public void AddCoin(int amount)
    {
        _gold += amount;
        // UpdateUI();
        // SaveGold();
        OnCoinChange?.Invoke(amount);
    }

    /// <summary>
    /// 扣除指定数量金币，如果不足则返回 false
    /// 扣费成功后刷新 UI 并保存
    /// </summary>
    /// <param name="amount">要花费的金币数</param>
    /// <returns>是否扣费成功</returns>
    public bool SpendCoin(int amount)
    {
        if (_gold >= amount)
        {
            Debug.Log($"消耗金币{amount}");
            _gold -= amount;
            OnCoinChange?.Invoke(amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检查是否拥有足够的金币（>= amount）
    /// </summary>
    /// <param name="amount">所需金币数</param>
    /// <returns>是否足够</returns>
    public bool HasEnoughGold(int amount)
    {
        // return ;
        return _gold >= amount;
    }

    /// <summary>
    /// 保存当前金币数到 PlayerPrefs
    /// </summary>
    public void SaveGold()
    {
        // PlayerPrefs.SetInt(SaveKey_Gold, _gold);
        // PlayerPrefs.Save();
    }

    /// <summary>
    /// 从 PlayerPrefs 中加载金币数（默认值可设为 0 或其他）
    /// </summary>
    public void LoadGold()
    {
        // if (!_isInitialized)
        // {
        //     _gold = PlayerPrefs.GetInt(SaveKey_Gold, 0);
        //     UpdateUI();
        //     _isInitialized = true;
        //     OnGoldChanged?.Invoke(_gold);
        // }
    }
    #endregion

    #region 私有成员方法
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
    }

    //敌人死亡事件
    private void OnEnemyDie(Enemy enemy)
    {
        // 概率掉落金币
        if (Random.value <= EnemyManager.EnemyDieCoinProb)
        {
            // 掉落金币
            AddCoin(EnemyManager.EnemyDieCoin);
            OnGetCoinFromEnemy?.Invoke(enemy,EnemyManager.EnemyDieCoin);
        }
    }

    /// <summary>
    /// 更新 UI 显示金币数
    /// </summary>
    private void UpdateUI()
    {
        // if (_goldUIText != null)
        // {
        //     _goldUIText.text = _gold.ToString();
        // }
    }

    #endregion
}

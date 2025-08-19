using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 管理局内玩家资源的增减与存档
/// </summary>
public class CurrencyManager : ManagerBase<CurrencyManager>,IManager
{
    public GameObject coinText;//金币粒子特效飞向的地方
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
        AddCoin(1000);
        Enemy.OnEnemyDie += OnEnemyDie;//订阅敌人死亡事件
    }

    /// <summary>
    /// 增加指定数量的金币，立即保存并刷新 UI
    /// </summary>
    /// <param name="amount">要增加的金币数</param>
    /// <param name="playMultiSound">是否播放多次音效,播放音效的次数</param>
    public void AddCoin(int amount,int playMultiSound=1)
    {
        _gold += amount;
        // UpdateUI();
        // SaveGold();
        PlayCoinSoundMultiple(playMultiSound,0.06f);
        //AudioManager.Instance.PlaySFX("获得金币");
        OnCoinChange?.Invoke(amount);
    }

    public void PlayCoinSoundMultiple(int times, float interval)
    {
        StartCoroutine(CoinSoundRoutine(times, interval));
    }

    private IEnumerator CoinSoundRoutine(int times, float interval)
    {
        yield return TimerUtility.WaitForGameSeconds(0.8f);//和金币粒子特效飞行时长同步
        for (int i = 0; i < times; i++)
        {
            AudioManager.Instance.PlaySFX("获得金币");
            yield return new WaitForSeconds(interval);
        }
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

    // 敌人死亡事件
    private void OnEnemyDie(Enemy enemy)
    {
        if (enemy == null) return;

        int baseCoin = EnemyManager.EnemyDieCoin.Value;
        int enemyCoin = 0;
        int getCoin = 0;
        bool dropCoin = false;

        // 1. 计算掉落金币
        if (enemy.enemyType == EnemyType.Coin)
        {
            enemyCoin = baseCoin * 10;
            dropCoin = true; // 金币怪必掉落
        }
        else
        {
            // 普通敌人按照概率掉落
            enemyCoin = baseCoin;
            dropCoin = (Random.value <= EnemyManager.EnemyDieCoinProb.Value);
        }

        // 2. 扣欠款 & 计算实际得到的金币
        if (dropCoin && enemyCoin > 0)
        {
            if (BankManager.Instance.CurrentNeedReturn.Value > 0)
            {
                int backCoin = Mathf.RoundToInt(enemyCoin / 2); // 先还欠款
                BankManager.Instance.CurrentNeedReturn.Value -= backCoin;
                getCoin = Mathf.Max(0, enemyCoin - backCoin);
            }
            else
            {
                getCoin = enemyCoin;
            }

            // 3. 增加金币
            if (getCoin > 0)
            {
                int effectCount = (enemy.enemyType == EnemyType.Coin) ? 5 : 1; // 金币怪播放更多特效
                AddCoin(getCoin, effectCount);

                // 播放金币粒子特效
                Vector3 screenStart = Camera.main.WorldToScreenPoint(enemy.transform.position);
                Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(null, coinText.transform.position);
                GameUIManager.Instance.PlayFlyCoinEffect(screenStart, screenEnd, effectCount);
            }
        }

        // 4. 事件通知（无论是不是金币怪都触发）
        OnGetCoinFromEnemy?.Invoke(enemy, getCoin);
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

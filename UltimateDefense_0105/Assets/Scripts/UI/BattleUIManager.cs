using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 用于 UI 组件引用

/// <summary>
/// 管理游戏中的所有局内 UI 界面与显示逻辑
/// </summary>
public class BattleUIManager : ManagerBase<BattleUIManager> 
{
    #region 常量
    public const string UIPrefabPath = "Prefabs/UI/"; // Resources 中 UI 预制体路径前缀
    #endregion

    #region 私有属性
    [SerializeField]
    private Transform _damageTextParent;            //显示伤害文字的父容器
    [SerializeField]private UpgradeUI _upgradePanel;                 // 升级面板预制体引用
    [SerializeField] private GameEndPanel _endPanel;                 // 游戏结算面板引用
    [SerializeField] private PausePanel _pausePanel;                 // 暂停面板引用
    [SerializeField] private GameObject _gamePlayingPanel;            // 游戏挑战时的UI面板引用
    [SerializeField] private GameObject _battleScene;//战斗场景的父物体
    [SerializeField]private DamageText _damageTextPrefab; //伤害文字的预制体引用
    [SerializeField]private GetCoinText _getCoinTextPrefab; //显示从敌人身上获取了多少金币的预制体引用
    [SerializeField]private WelfarePanel _welfarePanelPrefab; //福利面板的预制体引用
    public ObjectPool<DamageText> _damageTextPool;//显示伤害文字的对象池
    private ObjectPool<GetCoinText> _getCoinTextPool;//显示从敌人身上获取了多少金币的文字
    public AnswerMathQues AnswerMathQuesPanel;//回答数学题界面
    public KnifeSwipe knifeSwipe;//水果忍者的小刀
    #endregion

    #region 公开属性
    /// <summary>
    /// 显示伤害文字的对象池
    /// </summary>
    public ObjectPool<DamageText> DamageTextPool { get => _damageTextPool;}
    /// <summary>
    /// 显示敌人掉落金币的对象池
    /// </summary>
    public ObjectPool<GetCoinText> GetCoinTextPool { get => _getCoinTextPool; set => _getCoinTextPool = value; }

    #endregion

    #region public 成员方法
    /// <summary>
    /// 取消订阅所有事件
    /// </summary>
    public void Disable()
    {
        _upgradePanel.DestroyUpgrade();
        HideGamePlayingPanel();
       Enemy.OnEnemyDamaged -= OnEnemyDamaged;//敌人受到伤害
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Debug.Log("BattleUIManager初始化");
        _upgradePanel.Init();
        ShowGamePlayingPanel();
        Enemy.OnEnemyDamaged += OnEnemyDamaged;//敌人受到伤害
        if (_damageTextPool == null) _damageTextPool = new ObjectPool<DamageText>(_damageTextPrefab, 10, _damageTextParent);//初始化伤害文字的对象池
        if (GetCoinTextPool == null) GetCoinTextPool = new ObjectPool<GetCoinText>(_getCoinTextPrefab, 10, _damageTextParent);//初始化伤害文字的对象池
    }
    /// <summary>
    /// 显示战斗场景
    /// </summary>
    public void ShowBattleScene()
    {
        _battleScene.SetActive(true);
    }
    
    /// <summary>
    /// 隐藏战斗场景
    /// </summary>
    public void HideBattleScene()
    {
        Disable();
        _battleScene?.SetActive(false);
    }

    /// <summary>
    /// 显示战斗时的UI
    /// </summary>
    public void ShowGamePlayingPanel()
    {
        _gamePlayingPanel.SetActive(true);
    }
    /// <summary>
    /// 隐藏战斗时的UI
    /// </summary>
    public void HideGamePlayingPanel()
    {
        _gamePlayingPanel.SetActive(false);
    }

    /// <summary>
    /// 显示暂停面板
    /// </summary>
    public void ShowPausePanel()
    {
        _pausePanel.gameObject.SetActive(true);
    }


    /// <summary>
    /// 显示结算面板
    /// </summary>
    public void ShowEndPanel(bool isSuccess)
    {
        _endPanel.Init(isSuccess);
    }
    /// <summary>
    /// 显示福利面板
    /// </summary>
    public void ShowWelfarePanel()
    {
        _welfarePanelPrefab.gameObject.SetActive(true);
    }
    #endregion

    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// 查找并赋值所有 UI 组件：_goldText、_roundText、_debuffContainer、_skillSlotImages、_skillSlotCooldownMasks、
    /// _upgradePanel、_skillPanel、_gameOverPanel、_victoryPanel、_toastPrefab，标记 _isInitialized = false。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        Index = 3;
    }

    /// <summary>
    /// 对敌人造成伤害后，显示伤害文字的UI
    /// </summary>
    private void OnEnemyDamaged(Enemy enemy,bool isCritical,int damage)
    {
        // 世界坐标 → 屏幕坐标
        Vector3 worldPos = enemy.transform.position + Vector3.up * 1.2f;  // 头顶偏移
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("DamageText", out var value))
        {
            DataManager.Instance.PlayerInfo.SetConfig("DamageText", 0); // 不存在 → 默认 false
        }

        if (DataManager.Instance.PlayerInfo.Config["DamageText"] == 1)//伤害文字显示已打开
        {
            DamageText dmgText = _damageTextPool.Get();  // 用对象池
            dmgText.Init(screenPos, isCritical, damage.ToString());
        }
        else
        {
            if (isCritical)
            {
                DamageText dmgText = _damageTextPool.Get();  // 用对象池
                dmgText.Init(screenPos, isCritical, "暴击");
            }
        }
        
    }
    #endregion
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 用于 UI 组件引用

/// <summary>
/// 管理游戏中的所有局内 UI 界面与显示逻辑
/// </summary>
public class BattleUIManager : ManagerBase<BattleUIManager>,IManager
{
    #region 常量
    public const string UIPrefabPath = "Prefabs/UI/"; // Resources 中 UI 预制体路径前缀
    #endregion

    #region 私有属性
    
    
    private Transform _debuffContainer;              // Debuff 图标的父容器
    [SerializeField]
    private Transform _damageTextParent;            //显示伤害文字的父容器
    private Image[] _skillSlotImages;                 // 技能槽位的图标数组
    private Image[] _skillSlotCooldownMasks;          // 技能槽位的冷却遮罩数组
    [SerializeField]private UpgradeUI _upgradePanel;                 // 升级面板预制体引用
    [SerializeField]private BankPanel _bankPanel;                 // 银行面板预制体引用
    private GameObject _skillPanel;                   // 技能面板预制体引用
    [SerializeField] private GameEndPanel _endPanel;                 // 游戏结算面板引用
    [SerializeField] private PausePanel _pausePanel;                 // 暂停面板引用
    [SerializeField] private GameObject _gamePlayingPanel;            // 游戏挑战时的UI面板引用
    private GameObject _toastPrefab;                  // 提示文字（Toast）预制体引用
    [SerializeField] private GameObject _battleScene;//战斗场景的父物体
    [SerializeField]private DamageText _damageTextPrefab; //伤害文字的预制体引用
    [SerializeField]private GetCoinText _getCoinTextPrefab; //显示从敌人身上获取了多少金币的预制体引用
    [SerializeField]private BattleTowerInfoPanel _battleInfoPrefab; //战斗中的炮塔信息面板的预制体引用
    [SerializeField]private EnemyInfoPanel _enemyInfoPrefab; //战斗中的敌人信息面板的预制体引用
    private bool _isInitialized;                      // 标记是否完成 UI 初始化
    private ObjectPool<DamageText> _damageTextPool;//显示伤害文字的对象池
    private ObjectPool<GetCoinText> _getCoinTextPool;//显示从敌人身上获取了多少金币的文字
    #endregion

    #region 公开属性

    /// <summary>
    /// 只读属性，暴露升级面板引用
    /// </summary>
    public UpgradeUI UpgradePanel { get { return _upgradePanel; } }

    /// <summary>
    /// 只读属性，暴露技能面板引用
    /// </summary>
    public GameObject SkillPanel { get { return _skillPanel; } }

    /// <summary>
    /// 只读属性，显示伤害文字的父容器
    /// </summary>
    public Transform DamageTextParent { get => _damageTextParent; }
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
        CurrencyManager.OnGetCoinFromEnemy += OnGetCoinFromEnemy;//敌人掉落金币
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
    /// 显示银行面板
    /// </summary>
    public void ShowBankPanel()
    {
        _bankPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示银行面板
    /// </summary>
    public void HidewBankPanel()
    {
        _bankPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示暂停面板
    /// </summary>
    public void ShowPausePanel()
    {
        _pausePanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示战斗中炮塔的信息
    /// </summary>
    public void ShowBattleTowerInfoPanel()
    {
        _battleInfoPrefab.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏战斗中炮塔的信息
    /// </summary>
    public void HideBattleTowerInfoPanel()
    {
        _battleInfoPrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示战斗中敌人的信息
    /// </summary>
    public void ShowEnemyInfoInfoPanel()
    {
        _enemyInfoPrefab.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏战斗中敌人的信息
    /// </summary>
    public void HideEnemyInfoPanel()
    {
        _enemyInfoPrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示技能面板，并刷新技能图标
    /// </summary>
    public void ShowSkillPanel()
    {
        // _skillPanel.SetActive(true);
        // 调用 SkillPanel 组件的 RefreshSkillIcons 方法
    }

    /// <summary>
    /// 隐藏技能面板
    /// </summary>
    public void HideSkillPanel()
    {
        // _skillPanel.SetActive(false);
    }


    /// <summary>
    /// 显示结算面板
    /// </summary>
    public void ShowEndPanel(bool isSuccess)
    {
        _endPanel.Init(isSuccess);
    }

    

    /// <summary>
    /// 刷新回合显示，参数 round 为当前回合
    /// </summary>
    /// <param name="round">当前回合编号</param>
    public void RefreshRoundDisplay(int round)
    {
        //_roundText.text = $"Round: {round} / {BattleManager.MaxRounds}";
    }

    /// <summary>
    /// 在 Debuff 容器中显示一组 Debuff 图标
    /// </summary>
    /// <param name="debuffs">要显示的 Debuff 列表</param>
    public void ShowDebuffIcons(List<Image> debuffs)
    {
        // 清空 _debuffContainer 子物体
        // 遍历 debuffs，为每条创建一个图标并设置 Tooltip 文本
    }

    /// <summary>
    /// 在指定技能槽（slotIndex）上显示冷却遮罩，percentage 为填充量（0-1）
    /// </summary>
    /// <param name="slotIndex">技能槽索引</param>
    /// <param name="percentage">遮罩填充百分比（剩余冷却/总冷却）</param>
    public void ShowSkillCooldown(int slotIndex, float percentage)
    {
        // _skillSlotCooldownMasks[slotIndex].fillAmount = percentage;
    }

    /// <summary>
    /// 显示一条短暂的提示信息（Toast）
    /// </summary>
    /// <param name="message">提示文本内容</param>
    public void ShowToast(string message)
    {
        // Toast toast = Instantiate(_toastPrefab, this.transform);
        // toast.Show(message);
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
        // _isInitialized = false;
        // _goldText = GameObject.Find("UI/GoldText").GetComponent<Text>();
        // _roundText = GameObject.Find("UI/RoundText").GetComponent<Text>();
        // _debuffContainer = GameObject.Find("UI/DebuffContainer").transform;
        // // 依次查找 SkillSlotImages 和 CooldownMasks
        // _skillSlotImages = new Image[3];
        // _skillSlotCooldownMasks = new Image[3];
        // for (int i = 0; i < 3; i++)
        // {
        //     _skillSlotImages[i] = GameObject.Find($"UI/SkillSlot{i}/Icon").GetComponent<Image>();
        //     _skillSlotCooldownMasks[i] = GameObject.Find($"UI/SkillSlot{i}/CooldownMask").GetComponent<Image>();
        // }
        // _upgradePanel = GameObject.Find("UI/UpgradePanel");
        // _skillPanel = GameObject.Find("UI/SkillPanel");
        // _gameOverPanel = GameObject.Find("UI/GameOverPanel");
        // _victoryPanel = GameObject.Find("UI/VictoryPanel");
        // _toastPrefab = Resources.Load<Toast>(UIPrefabPath + "Toast");
    }


    

    /// <summary>
    /// 收到升级购买事件后，显示提示并刷新金币显示
    /// </summary>
    /// <param name="upgrade">购买的升级实例</param>
    private void OnUpgradePurchased(UpgradeBase upgrade)
    {
        // ShowToast($"已获得升级：{upgrade.Description}");
        // RefreshGoldDisplay();
    }

    /// <summary>
    /// 收到技能冷却更新事件后，调用 ShowSkillCooldown 更新对应槽位的遮罩
    /// </summary>
    /// <param name="slotIndex">技能槽索引</param>
    /// <param name="percentage">剩余冷却比例</param>
    private void OnSkillCooldownChanged(int slotIndex, float percentage)
    {
        // ShowSkillCooldown(slotIndex, percentage);
    }

    /// <summary>
    /// 对敌人造成伤害后，显示伤害文字的UI
    /// </summary>
    private void OnEnemyDamaged(Enemy enemy,bool isCritical,int damage)
    {
        // 世界坐标 → 屏幕坐标
        Vector3 worldPos = enemy.transform.position + Vector3.up * 1.2f;  // 头顶偏移
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 创建伤害Text（属于 screen-space canvas）
        DamageText dmgText = _damageTextPool.Get();  // 用对象池
        dmgText.Init(screenPos, isCritical, damage);
    }

    //敌人掉落金币
    private void OnGetCoinFromEnemy(Enemy enemy, int amount)
    {
        // 世界坐标 → 屏幕坐标
        Vector3 worldPos = enemy.transform.position + Vector3.up * -1.2f;  // 头顶偏移
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 创建伤害Text（属于 screen-space canvas）
        GetCoinText dmgText = GetCoinTextPool.Get();  // 用对象池
        dmgText.Init(screenPos, amount);
    }



    /// <summary>
    /// 取消所有订阅，清理临时资源
    /// </summary>
    private void OnDestroy()
    {
        // if (CurrencyManager.Instance != null) CurrencyManager.Instance.OnGoldChanged -= RefreshGoldDisplay;
        // if (BattleManager.Instance != null) BattleManager.Instance.OnRoundChanged -= RefreshRoundDisplay;
        // if (UpgradeManager.Instance != null) UpgradeManager.Instance.OnUpgradePurchased -= OnUpgradePurchased;
        // if (SkillManager.Instance != null) SkillManager.Instance.OnSkillCooldownChanged -= OnSkillCooldownChanged;
    }
    #endregion
}

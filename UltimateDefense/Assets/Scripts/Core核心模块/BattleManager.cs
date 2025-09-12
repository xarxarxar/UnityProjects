using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 控制战斗流程
/// </summary>
public class BattleManager : MonoBehaviour
{
    #region 常量
    public const int MaxRounds = 100;//最大回合数。
    public const string Scene_MainMenu = "MainMenu";//场景名常量，用于切换到主菜单。
    public const string Scene_Battle = "Battle";//场景名常量，用于切换到战斗场景。
    #endregion

    #region 静态事件
    public static event UnityAction OnStartBattle;//挑战开始
    public static event UnityAction<bool> OnEndBattle;//挑战结束
    #endregion

    #region 私有属性
    private static BattleManager _instance;//单例实例，供全局访问 Instance
    private int _startGold;//本局开局所给的初始金币数（受技能树或其他影响）。
    [SerializeField]
    private Debuff _debuff=new Debuff();//通关之后选择的Debuff
    private int _diamondCount = 0;//单次挑战获取的局外钻石数量
    private int _crownCount = 1;//单次挑战获取的局外王冠数量
    private int lastNonZeroSpeed = 1; // 默认1倍速
    #endregion

    #region 公开属性
    /// <summary>
    /// 单例公开属性，
    /// </summary>
    public static BattleManager Instance { get=>_instance;}
    /// <summary>
    /// 只读属性，暴露本局开局金币数。
    /// </summary>
    public int StartGold { get { return _startGold; } }
    /// <summary>
    /// 只读属性，是否正在战斗中
    /// </summary>
    public bool IsBatting { get; private set; }
    /// <summary>
    /// 公开属性，用于设置游戏整体倍速（同时修改 Time.timeScale）。最小限制 1.0f
    /// </summary>
    public Bindable<int> GameSpeed { get ; set;}=new Bindable<int>();
    /// <summary>
    /// 通关之后选择的Debuff
    /// </summary>
    public Debuff Debuff { get => _debuff;}
    /// <summary>
    /// 单次挑战获取的局外王冠数量
    /// </summary>
    public int DiamondCount { get => _diamondCount; }
    /// <summary>
    /// 单次挑战获取的局外王冠数量
    /// </summary>
    public int CrownCount { get => _crownCount;}

    #endregion

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            Debug.Log("按了3");
            SetGameSpeed(3);
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            SetGameSpeed(1);
        }
    }

    #region public成员方法
    /// <summary>
    /// 开始新一局战斗
    /// 初始化 _currentRound = 0、
    /// 设置 IsFailed = false、
    /// 通过 SceneManager 切换到战斗场景，并调用 WaveManager 启动第一回合。
    /// </summary>
    public void StartBattle(Debuff debuff)
    {
        Debug.Log("开始战斗");
        _debuff = debuff;
        GameSpeed.Value= 1;
        lastNonZeroSpeed = 1;
        _diamondCount = 0;
        IsBatting = true;


        BattleUIManager.Instance.ShowBattleScene();//显示战斗场景
        EnemyManager.OnLastEnemySpawned += OnLastEnemySpawned;
        Crystal.OnCrystalDestroyed += OnCrystalDestroyed;
        ManagerRegistry.InitManagers(InitStage.InBattle);
        OnStartBattle?.Invoke();
    }

    /// <summary>
    /// 结束本局战斗。
    /// success = true -> 玩家成功守住 100 回合 -> 累加 _totalClears，结算奖励，弹出胜利面板；
    /// success = false -> 玩家失败（核心水晶破坏）-> IsFailed = true，弹出失败面板。
    /// </summary>
    /// <param name="success"></param>
    public void EndBattle(bool success)
    {
        IsBatting = false;
        SignleMetaCoinCount();//计算这次挑战获取了多少钻石
        BattleUIManager.Instance.ShowEndPanel(success);//显示游戏结算界面
        
        EnemyManager.OnLastEnemySpawned -= OnLastEnemySpawned;
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged -= OnEnemyCountChanged;
        Crystal.OnCrystalDestroyed -= OnCrystalDestroyed;
        Debug.Log($"挑战结束了{success}");
        OnEndBattle?.Invoke(success);
    }


    /// <summary>
    /// 暂停游戏：_isPaused = true; Time.timeScale = 0;
    /// </summary>
    public void PauseGame()
    {
        GameSpeed.Value = 0;
    }

    /// <summary>
    /// 恢复游戏：_isPaused = false; Time.timeScale = _gameSpeed;
    /// </summary>
    public void ResumeGame()
    {
        GameSpeed.Value = lastNonZeroSpeed;
    }

    /// <summary>
    ///  设置 GameSpeed 属性（重置 Time.timeScale）。
    /// </summary>
    /// <param name="speed"></param>
    public void SetGameSpeed(int speed)
    {
        if(speed<=0) { return; }
        GameSpeed.Value=lastNonZeroSpeed=speed;
    }
    #endregion

    #region private成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject);；否则 Destroy(gameObject).
    /// 初始化默认值：_isGameOver = false; _currentRound = 0; _totalClears = 0; _startGold = 0; _isPaused = false;。
    /// </summary>
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    /// <summary>
    /// 开始时调用 LoadProgress() 从存档读取 _totalClears、_startGold 等。
    /// </summary>
    private void Start()
    {
        Debug.Log("为debuff按钮添加StartBattle点击事件");
        ChooseDebuffPanel.OnDebuffChooseEnd += StartBattle;//选择debuff 完毕之后
    }

    //最后一个敌人生成之后
    private void OnLastEnemySpawned()
    {
        Debug.Log("最后一个敌人已生成");
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged += OnEnemyCountChanged;
    }

    //敌人数量变化
    private void OnEnemyCountChanged(int count)
    {
        
        if (count == 0)//敌人全部死亡了
        {
            Debug.Log("敌人全部死亡了");
            EndBattle(true);//胜利了
        }
    }

    //单次挑战获取的局外金币数量
    private void SignleMetaCoinCount()
    {
        int rounds = WaveManager.Instance.CurrentRound;//当前回合
        int tens = rounds / 3; // 闯过了多少个10关卡
        int bonus = 0;
        for (int i = 1; i <= tens; i++)
        {
            bonus += i;
        }
        _diamondCount = bonus;
    }

    //水晶被破坏了
    private void OnCrystalDestroyed()
    {
        EndBattle(false);
    }

    #endregion
}

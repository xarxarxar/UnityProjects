using Unity.VisualScripting;
using UnityEngine;

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

    #region 私有属性
    private static BattleManager _instance;//单例实例，供全局访问 Instance
    private bool _isGameOver;//标记当前游戏是否结束（胜利或失败）。
    private bool _isPaused=false;//标记游戏是否处于“暂停”状态，用于实现暂停/恢复功能。
    private int _currentRound;//当前进行到的回合编号（1–100）。暂定100为最大回合
    private int _totalClears;//玩家累计通关次数（成功守住 100 回合的次数）。
    private int _startGold;//本局开局所给的初始金币数（受技能树或其他影响）。
    private float _gameSpeed = 1f;//当前游戏速度倍率，默认 1×，用于倍速功能（0.5×、1×、2× 等）。
    
    #endregion

    #region 公开属性
    /// <summary>
    /// 单例公开属性，
    /// </summary>
    public static BattleManager Instance { get=>_instance;}
    /// <summary>
    /// 只读属性，暴露玩家累计通关次数。
    /// </summary>
    public int TotalClears { get { return _totalClears; } }
    /// <summary>
    /// 只读属性，暴露当前回合编号。
    /// </summary>
    public int CurrentRound { get { return _currentRound; } }
    /// <summary>
    /// 只读属性，暴露本局开局金币数。
    /// </summary>
    public int StartGold { get { return _startGold; } }
    /// <summary>
    /// 只读属性，标记本局是否因水晶被破坏而失败。
    /// </summary>
    public bool IsFailed { get; private set; }
    /// <summary>
    /// 公开属性，用于设置游戏整体倍速（同时修改 Time.timeScale）。最小限制 0.1f，避免归零。
    /// </summary>
    public float GameSpeed { get { return _gameSpeed; } set { _gameSpeed = Mathf.Max(0.1f, value); } }
    /// <summary>
    /// 公开属性，用于设置游戏是否处于暂停状态
    /// </summary>
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    #endregion

    #region public成员方法
    /// <summary>
    /// 开始新一局战斗
    /// 初始化 _currentRound = 0、
    /// 设置 IsFailed = false、
    /// 通过 SceneManager 切换到战斗场景，并调用 WaveManager 启动第一回合。
    /// </summary>
    public void StartBattle()
    {
        BattleUIManager.Instance.ShowBattleScene();//显示战斗场景
        _isPaused = false;
        ManagerRegistry.InitManagers(InitStage.InBattle);
        EnemyManager.OnLastEnemySpawned += OnLastEnemySpawned;
    }

    /// <summary>
    /// 结束本局战斗。
    /// success = true -> 玩家成功守住 100 回合 -> 累加 _totalClears，结算奖励，弹出胜利面板；
    /// success = false -> 玩家失败（核心水晶破坏）-> IsFailed = true，弹出失败面板。
    /// </summary>
    /// <param name="success"></param>
    public void EndBattle(bool success)
    {
        Debug.Log("挑战结束了");
        if (success)
        {
            BattleUIManager.Instance.ShowVictoryPanel();//显示游戏胜利界面
        }
        else
        {
            BattleUIManager.Instance.ShowGameOverPanel();//显示游戏失败界面
        }
        
        EnemyManager.OnLastEnemySpawned -= OnLastEnemySpawned;
        EnemyManager.OnEnemyCountChanged -= OnEnemyCountChanged;
        
    }

    /// <summary>
    /// 进入下一回合：
    /// _currentRound++，如果小于 MaxRounds 则调用 WaveManager.StartNextRound()；
    /// 否则调用 EndGame(true)。
    /// </summary>
    public void NextRound()
    {

    }

    /// <summary>
    /// 暂停游戏：_isPaused = true; Time.timeScale = 0;
    /// </summary>
    public void PauseGame()
    {

    }

    /// <summary>
    /// 恢复游戏：_isPaused = false; Time.timeScale = _gameSpeed;
    /// </summary>
    public void ResumeGame()
    {

    }

    /// <summary>
    /// 累积一条通关次数：_totalClears++; SaveProgress();
    /// </summary>
    public void AddClear()
    {

    }

    /// <summary>
    ///  设置 GameSpeed 属性（重置 Time.timeScale）。
    /// </summary>
    /// <param name="speed"></param>
    public void SetGameSpeed(float speed)
    {
        GameSpeed=speed;
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
        StartPanel.OnSatrtBattle += StartBattle;//点击开始挑战按钮之后
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetGameSpeed(2);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetGameSpeed(1);
        }
    }

    //最后一个敌人生成之后
    private void OnLastEnemySpawned()
    {
        EnemyManager.OnEnemyCountChanged += OnEnemyCountChanged;
    }

    //敌人数量变化
    private void OnEnemyCountChanged(int count)
    {
        if (count == 0)//敌人全部死亡了
        {
            EndBattle(true);//胜利了
        }
    }
    #endregion
}

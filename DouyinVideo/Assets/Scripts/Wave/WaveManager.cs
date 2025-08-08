using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 管理怪物波次生成流程
/// </summary>
public class WaveManager : ManagerBase<WaveManager>,IManager
{
    #region 私有属性
    private int _currentRound=0;                         // 当前回合数（1–100）
    [SerializeField]private int _maxRound = 100;        //最大回合数
    private bool _isInKeyWave;                         // 标记本回合是否为关键波
    private bool _isSpawning;                          // 标记当前是否正在生成本回合怪物
    [SerializeField]private int _singleWaveEnemyCount = 20;             //每个回合生成怪物的数量
    [SerializeField]private float _spawnEnemyInterval=3.0f;                 //每个敌人生成的间隔时间
    [SerializeField]private float _spawnWaveInterval=5.0f;                 //每波敌人生成的间隔时间
    #endregion

    #region 公共静态事件
    /// <summary>
    /// 波次更新事件
    /// </summary>
    public static event UnityAction<int> OnWaveChanged;
    #endregion

    #region 公开属性

    /// <summary>
    /// 只读属性，暴露当前回合编号
    /// </summary>
    public int CurrentRound { get { return _currentRound; } 
        set 
        { 
            if(_currentRound != value)
            {
                _currentRound = value;
                OnWaveChanged?.Invoke(value);
            }
        } 
    }

    /// <summary>
    /// 只读属性，暴露是否为关键波
    /// </summary>
    public bool IsInKeyWave { get { return _isInKeyWave; } }
    /// <summary>
    /// 只读属性，每波生成多少敌人
    /// </summary>
    public int SingleWaveEnemyCount { get => _singleWaveEnemyCount; }
    /// <summary>
    /// 只读属性，每个敌人生成的间隔时间
    /// </summary>
    public float SpawnEnemyInterval { get => _spawnEnemyInterval; }
    /// <summary>
    /// 只读属性，每波敌人生成的间隔时间
    /// </summary>
    public float SpawnWaveInterval { get => _spawnWaveInterval; }
    /// <summary>
    /// 最大回合数
    /// </summary>
    public int MaxRound { get => _maxRound;}
    #endregion


    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Debug.Log("wavemanager 初始化");
        _currentRound = 0;
    }
    /// <summary>
    /// 启动下一回合怪物生成流程
    /// _currentRound++，设置 _isSpawning = true
    /// 检查是否存在对应 Wave_SO，如果存在则按关键波配置生成，否则调用 GenerateRandomWave
    /// 生成完成后将 _isSpawning = false
    /// </summary>
    public void StartNextRound()
    {
        // _currentRound++;
        // _isSpawning = true;
        // 查找 _keyWaves 中 RoundNumber == _currentRound 的项
        // if (找到关键波) { PlayKeyWaveSpawn(...) } else { GenerateRandomWave(_currentRound); }
        // _isSpawning = false;
    }

    /// <summary>
    /// 重置波次管理器，供新局使用
    /// _currentRound = 0; _isInKeyWave = false; StopAllCoroutines(); 清理现场怪物
    /// </summary>
    public void ResetWaves()
    {
        // _currentRound = 0;
        // _isInKeyWave = false;
        // StopAllCoroutines();
        // EnemyManager.Instance.ClearAllEnemies(); // 如需清理场景中的残留敌人
    }
    #endregion

    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// 初始化 _keyWaves 与 _poolConfig 引用
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        // _keyWaves = new List<Wave_SO>(Resources.LoadAll<Wave_SO>("Configs/ScriptableObjects/Waves/"));
        // _poolConfig = Resources.Load<EnemyPoolConfig>("Configs/EnemyPoolConfig");
    }


    /// <summary>
    /// 根据给定 round 生成一波随机怪物
    /// 从 _poolConfig.Pools 中找到符合 round 范围的 RoundPool 配置
    /// 计算生成总量与各类型权重，调用 SpawnOneEnemy 逐只生成
    /// </summary>
    /// <param name="round">当前回合编号</param>
    private void GenerateRandomWave(int round)
    {
        // 找到 poolConfig.Pools 中 RoundMin <= round <= RoundMax 的条目
        // 计算 int totalCount = Mathf.CeilToInt(10 + round * 1.5f);
        // 按权重从 EnemyTypes 随机挑选 totalCount 次，调用 SpawnOneEnemy(type)
    }

    /// <summary>
    /// 按照给定类型、数量和间隔启动协程逐只生成怪物
    /// </summary>
    /// <param name="type">要生成的敌人类型</param>
    /// <param name="count">生成数量</param>
    /// <param name="interval">每只生成的时间间隔（秒）</param>
    private IEnumerator SpawnBatch(EnemyType type, int count, float interval)
    {
        // for (int i = 0; i < count; i++)
        // {
        //     SpawnOneEnemy(type);
        //     yield return new WaitForSeconds(interval * BattleManager.Instance.GameSpeed);
        // }
        yield return null;
    }

    /// <summary>
    /// 生成一只指定类型的敌人
    /// 计算随机生成位置、调用 PathfindingHelper 获得路径，然后调用 EnemyManager.SpawnEnemy
    /// </summary>
    /// <param name="type">敌人类型</param>
    private void SpawnOneEnemy(EnemyType type)
    {
        // Vector3 spawnPos = GetRandomSpawnPosition();
        // Vector3[] path = PathfindingHelper.FindPath(spawnPos, CoreCrystal.Instance.Position);
        // EnemyManager.Instance.SpawnEnemy(type, spawnPos, path);
    }

    /// <summary>
    /// 返回屏幕上方随机 x 位置生成点
    /// </summary>
    /// <returns>世界坐标的生成点</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        // float x = Random.Range(-GameArea.Width / 2, GameArea.Width / 2);
        // float y = GameArea.Height / 2 + 1;
        // return new Vector3(x, y, 0);
        return Vector3.zero;
    }

    /// <summary>
    /// Unity OnDestroy 回调，清理资源或取消所有协程
    /// </summary>
    private void OnDestroy()
    {
        // StopAllCoroutines();
    }
    #endregion
}

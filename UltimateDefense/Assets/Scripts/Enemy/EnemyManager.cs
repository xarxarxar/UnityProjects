using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 管理场上所有敌人的生成、注销与查找
/// </summary>
public class EnemyManager : ManagerBase<EnemyManager>,IManager
{
    #region 私有属性
    [SerializeField]private List<Enemy> _allEnemies;            // 场上所有活着的 Enemy 实例列表
    [SerializeField]private List<Enemy> _enemiesInRange;        //进入攻击范围的所有Enemy
    private bool _isInitialized;                // 标记是否已初始化
    [SerializeField] public Enemy _enemyPrefab;     //敌人预制体
    private ObjectPool<Enemy> _enemyPool;           //敌人对象池
    private Bindable<float>  _enemyDieCoinProb=new Bindable<float>();//敌人死亡之后获得金币的概率
    private Bindable<float>  _enemySpeed=new Bindable<float>();//敌人移动速度
    private Bindable<int> _enemyDieCoin = new Bindable<int>();//敌人死亡之后获得的金币数量
    private Bindable<int> _enemyCurrentCount = new Bindable<int>();//当前所有敌人数量
    private Bindable<int> _enemyDamageNullifiedCount = new Bindable<int>();//敌人免疫伤害次数
    private BuildingBase _targetBuilding;              //敌人的目标建筑物
    // 存储暂停前所有敌人的速度
    private Dictionary<Rigidbody2D, Vector2> _enemyVelocityMap = new Dictionary<Rigidbody2D, Vector2>();
    //private int _enemyTotalCount = 0;                //敌人生成的数量,从开始到结束的总数量，包括死亡的
    #endregion

    #region 公开静态事件
    /// <summary>
    /// 场上敌人数量变化
    /// </summary>
    //public static event UnityAction<int> OnEnemyCountChanged;
    /// <summary>
    /// 最后一波的最后一个敌人生成完毕
    /// </summary>
    public static event UnityAction OnLastEnemySpawned;

    public static event UnityAction OnAlmostNextWave;//在马上下一波的时候触发
    #endregion

    #region 公开属性
    /// <summary>
    /// 只读属性，暴露当前场上所有敌人的只读列表
    /// </summary>
    public IReadOnlyList<Enemy> AllEnemies { get { return _allEnemies; } }

    /// <summary>
    /// 只读属性，暴露场上所有进入攻击范围的敌人列表
    /// </summary>
    public List<Enemy> EnemiesInRange { get => _enemiesInRange; }

    /// <summary>
    /// 子弹对象池，供外部调用
    /// </summary>
    public ObjectPool<Enemy> EnemyPool { get => _enemyPool; }
    /// <summary>
    /// 敌人死亡后，掉落金币的概率
    /// </summary>
    public Bindable<float> EnemyDieCoinProb 
    { 
        get => _enemyDieCoinProb; 
        set
        {
            _enemyDieCoinProb=value;
            if (_enemyDieCoinProb.Value > 0.8f)
            {
                _enemyDieCoinProb.Value = 0.8f;//最大百分之80
            }
        } 
    }
    /// <summary>
    /// 敌人死亡后，掉落金币的数量
    /// </summary>
    public Bindable<int> EnemyDieCoin { get => _enemyDieCoin; set => _enemyDieCoin = value; }
    /// <summary>
    /// 只读属性，敌人的目标建筑物
    /// </summary>
    public BuildingBase TargetBuilding { get => _targetBuilding; }
    /// <summary>
    /// 当前活着的敌人数量
    /// </summary>
    public Bindable<int> EnemyCurrentCount { get => _enemyCurrentCount; set => _enemyCurrentCount = value; }
    /// <summary>
    /// 敌人移动速度
    /// </summary>
    public Bindable<float> EnemySpeed { get => _enemySpeed; set => _enemySpeed = value; }
    /// <summary>
    /// 敌人免疫伤害次数
    /// </summary>
    public Bindable<int> EnemyDamageNullifiedCount { get => _enemyDamageNullifiedCount; set => _enemyDamageNullifiedCount = value; }

    #endregion

    #region 常量
    // 暂无常量
    #endregion

    #region public 成员方法
    private void OnDisable()
    {
        Enemy.OnMoveInRange -= OnMoveInRange;
        Enemy.OnMoveOutRange -= OnMoveOutRange;
        Enemy.OnEnemyDie -= UnregisterEnemy;
        BattleManager.OnEndBattle -= OnEndBattle;
        BattleManager.Instance.GameSpeed.OnValueChanged -= GameSpeedChanged;
        StopAllCoroutines();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        Debug.Log("enemymanager初始化");
        _allEnemies.Clear();
        _enemiesInRange.Clear();
        _enemyDieCoinProb.Value = 0.5f;
        _enemyDieCoin.Value = 50;
        _enemyCurrentCount.Value = 0;
        _enemySpeed.Value = 0.5f * (1 + BattleManager.Instance.Debuff.AddSpeed * 0.1f);
        _enemyDamageNullifiedCount.Value = 0 + BattleManager.Instance.Debuff.DamageNullified;
        _targetBuilding = Crystal.Instance;//设置初始目标建筑为水晶

        Enemy.OnMoveInRange += OnMoveInRange;
        Enemy.OnMoveOutRange += OnMoveOutRange;
        Enemy.OnEnemyDie += UnregisterEnemy;
        BattleManager.OnEndBattle += OnEndBattle;
        BattleManager.Instance.GameSpeed.OnValueChanged += GameSpeedChanged;
        BattleManager.Instance.IsPaused.OnValueChanged += GamePausedChanged;
        if (_enemyPool==null) _enemyPool = new ObjectPool<Enemy>(_enemyPrefab, 20, transform);//初始化敌人对象池
        StartCoroutine(GenerateEnemyIE());
    }

    /// <summary>
    /// 在 spawnPos 处生成一只指定类型的敌人，并为其设置路径
    /// </summary>
    /// <param name="type">敌人类型枚举</param>
    /// <param name="spawnPos">生成位置（世界坐标）</param>
    /// <param name="pathPoints">由 PathfindingHelper 计算得到的世界坐标路径点数组</param>
    public void SpawnEnemy(Enemy enemy, float xPos, int level)
    {
        enemy.Init(new Vector3(xPos, 13, 0), level, -1 * (_allEnemies.Count));
        _allEnemies.Add(enemy);
        _enemyCurrentCount.Value = _allEnemies.Count;
    }

    /// <summary>
    /// 从场上敌人列表中注销一个敌人实例（如在其死亡后调用）
    /// </summary>
    /// <param name="e">要注销的敌人实例</param>
    public void UnregisterEnemy(Enemy enemy)
    {
        if (_enemiesInRange.Contains(enemy))
            _enemiesInRange.Remove(enemy);
        if (_allEnemies.Contains(enemy))
            _allEnemies.Remove(enemy);

        for(int i=0;i< _allEnemies.Count; i++)
        {
            _allEnemies[i].SetOrderLayer(-1*i);
        }
        _enemyCurrentCount.Value = _allEnemies.Count;
    }

    #endregion

    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// 初始化 _allEnemies 列表
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        // _allEnemies = new List<Enemy>();
        // _isInitialized = false;
    }

    //游戏速度变化时
    private void GameSpeedChanged(int speed)
    {
        if (speed == 1)
        {
            for (int i = 0; i < _allEnemies.Count; i++)
            {
                if (_allEnemies[i].GetComponent<Rigidbody2D>().gravityScale == 0.25f) return;
                _allEnemies[i].GetComponent<Rigidbody2D>().gravityScale = 0.25f;
                // 获取 Rigidbody2D 组件
                Rigidbody2D rb = _allEnemies[i].GetComponent<Rigidbody2D>();

                // 将当前速度向量乘以 2，实现加倍
                rb.velocity = rb.velocity / 2f;
            }
        }
        if(speed==2)
        {
            for (int i = 0; i < _allEnemies.Count; i++)
            {
                if (_allEnemies[i].GetComponent<Rigidbody2D>().gravityScale == 1) return;
                _allEnemies[i].GetComponent<Rigidbody2D>().gravityScale = 1;
                // 获取 Rigidbody2D 组件
                Rigidbody2D rb = _allEnemies[i].GetComponent<Rigidbody2D>();

                // 将当前速度向量乘以 2，实现加倍
                rb.velocity = rb.velocity * 2f;
            }
        }
    }

    //游戏暂停状态变化时
    private void GamePausedChanged(bool isPaused)
    {
        if (isPaused)
        {
            _enemyVelocityMap.Clear(); // 清空旧数据

            foreach (var enemy in _allEnemies)
            {
                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    _enemyVelocityMap[rb] = rb.velocity; // 记录当前速度
                    rb.velocity = Vector2.zero;          // 速度归零
                    rb.simulated = false;                // 禁用物理模拟
                }
            }
        }
        else
        {
            foreach (var kvp in _enemyVelocityMap)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.simulated = true;             // 启用物理模拟
                    kvp.Key.velocity = kvp.Value;         // 恢复之前的速度
                }
            }
            _enemyVelocityMap.Clear(); // 清理数据，防止数据脏
        }
    }


    //敌人移动至攻击范围
    private void OnMoveInRange(Enemy enemy)
    {
        _enemiesInRange.Add(enemy);
    }
    //敌人移出攻击范围
    private void OnMoveOutRange(Enemy enemy)
    {
        if (_enemiesInRange.Contains(enemy))
        {
            _enemiesInRange.Remove(enemy);
        }
    }

    private IEnumerator GenerateEnemyIE()
    {
        yield return null;
        while (true)
        {
            //Debug.Log($"一波生成前，当前波次为{WaveManager.Instance.CurrentRound}");
            WaveManager.Instance.CurrentRound++;

            int enemyCount = Mathf.RoundToInt(WaveManager.Instance.SingleWaveEnemyCount * (1 + BattleManager.Instance.Debuff.AddCount * 0.1f));
            for (int i = 0; i < enemyCount; i++)
            {
                while (BattleManager.Instance.IsPaused.Value) yield return null;
                yield return TimerUtility.WaitForGameSeconds(WaveManager.Instance.SpawnEnemyInterval);
                Enemy enemy = _enemyPool.Get();
                int currentRound = WaveManager.Instance.CurrentRound;
                SpawnEnemy(enemy, Random.Range(-7.5f, 7.5f), Random.Range(Mathf.Max(1,currentRound - 3),currentRound));

                // 如果是最后一波 且是最后一个敌人
                if (WaveManager.Instance.CurrentRound == WaveManager.Instance.MaxRound &&
                    i == enemyCount - 1)
                {
                    //Debug.Log("最后一个敌人生成完成，发送事件");
                    OnLastEnemySpawned?.Invoke();  // 触发事件
                }
            }
            if (WaveManager.Instance.CurrentRound >= WaveManager.Instance.MaxRound) 
            {
                yield break;
            }
            while (BattleManager.Instance.IsPaused.Value) yield return null;
            // 启动一个提醒协程（监听 GameSpeed 和暂停）
            StartCoroutine(WaitAndNotifyBeforeTime(WaveManager.Instance.SpawnWaveInterval, 3f, () =>
            {
                OnAlmostNextWave?.Invoke();
            }));
            yield return TimerUtility.WaitForGameSeconds(WaveManager.Instance.SpawnWaveInterval);
            //Debug.Log($"一波生成完毕，当前波次为{WaveManager.Instance.CurrentRound}");
        }
    }

    //启动一个提醒协程
    private IEnumerator WaitAndNotifyBeforeTime(float totalTime, float notifyBefore, System.Action callback)
    {
        float timer = 0f;
        float targetTime = totalTime - notifyBefore;

        while (timer < targetTime)
        {
            if (!BattleManager.Instance.IsPaused.Value)
            {
                timer += Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
            }
            yield return null;
        }

        callback?.Invoke();
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        _allEnemies.Clear();
        _enemiesInRange.Clear();
        StopAllCoroutines();
    }
    #endregion
}

public enum EnemyType
{

}

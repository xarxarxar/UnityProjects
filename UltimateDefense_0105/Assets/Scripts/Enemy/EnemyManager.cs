
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SerializableDictionary.Scripts;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 敌人的种类
/// </summary>
public enum EnemyType
{
    Normal,//普通怪，打败之后概率掉落金币
    Coin,//打败该敌人会必定掉落大量金币
    Elite,//精英怪，暂时没想好有什么用
    Boss,//Boss级别的怪，每10个回合出现一次，暂定为有技能，打败之后有特殊效果
}

/// <summary>
/// 管理场上所有敌人的生成、注销与查找
/// </summary>
public class EnemyManager : ManagerBase<EnemyManager>
{
    #region 私有属性
    [SerializeField]private List<Enemy> _allEnemies;            // 场上所有活着的 Enemy 实例列表
    [SerializeField]private List<Enemy> _enemiesInRange;        //进入攻击范围的所有Enemy
    [SerializeField] public ParticleSystem _explosionEffectPrefab;     //爆炸特效预制体
    [SerializeField] public EnemyExplode _explosionAnimPrefab;     //敌人自爆预制体
    [SerializeField] private SerializableDictionary<EnemyType, Enemy> enemyDictionary = new SerializableDictionary<EnemyType, Enemy>();//敌人字典
    private Dictionary<EnemyType, ObjectPool<Enemy>> enemyPools=new Dictionary<EnemyType, ObjectPool<Enemy>>();//敌人的对象池

    private float _attackYValue = 8;//进入攻击范围的y值

    public Transform _enemyPoolParent;           //敌人对象池父物体
    private ObjectPool<ParticleSystem> _explosionEffectPool;   //爆炸特效对象池
    public Transform _explosionEffectPoolParent;   //爆炸特效对象池父物体
    private ObjectPool<EnemyExplode> _explosionAnimPool;   //敌人自爆动画对象池
    [SerializeField] public Transform _explosionAnimPoolParent;     //敌人自爆预制体父物体
    private Bindable<float>  _enemyDieCoinProb=new Bindable<float>();//敌人死亡之后获得金币的概率

    private Bindable<int> _enemyDieCoin = new Bindable<int>();//敌人死亡之后获得的金币数量
    private Bindable<int> _enemyCurrentCount = new Bindable<int>();//当前所有敌人数量
    private Bindable<int> _enemyDamageNullifiedCount = new Bindable<int>();//敌人免疫伤害次数
    private Enemy _currentTargerEnemy;//当前的目标敌人
    private Bindable<Enemy> _currentClickedEnemy=new Bindable<Enemy>();//当前被点击的敌人
    private float _eliteEnemyProportion = 0.2f;//精英怪所占的比例
    //敌人的编号
    private int enemyID = 0;
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
    /// 爆炸特效对象池
    /// </summary>
    public ObjectPool<ParticleSystem> ExplosionEffectPool { get => _explosionEffectPool; } /// <summary>
    /// 敌人自爆对象池
    /// </summary>
    public ObjectPool<EnemyExplode> ExplosionAnimPool { get => _explosionAnimPool; }

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
    /// 敌人死亡后，掉落金币的概率
    /// </summary>
    public float TmpEnemyDieCoinProb {get ;set;}

    /// <summary>
    /// 敌人死亡后，掉落金币的数量
    /// </summary>
    public Bindable<int> EnemyDieCoin { get => _enemyDieCoin; set => _enemyDieCoin = value; }
    /// <summary>
    /// 当前活着的敌人数量
    /// </summary>
    public Bindable<int> EnemyCurrentCount { get => _enemyCurrentCount; set => _enemyCurrentCount = value; }

    /// <summary>
    /// 敌人免疫伤害次数
    /// </summary>
    public Bindable<int> EnemyDamageNullifiedCount { get => _enemyDamageNullifiedCount; set => _enemyDamageNullifiedCount = value; }

    /// <summary>
    /// 单次关卡消灭的敌人数量
    /// </summary>
    public Bindable<int> SignleEnemyDieCount { get; private set; } = new Bindable<int>();
    /// <summary>
    /// 当前的目标敌人
    /// </summary>
    public Enemy CurrentTargerEnemy { get => _currentTargerEnemy; set => _currentTargerEnemy = value; }
    /// <summary>
    /// 当前被点击的敌人
    /// </summary>
    public Bindable<Enemy> CurrentClickedEnemy { get => _currentClickedEnemy; set => _currentClickedEnemy = value; }
    /// <summary>
    /// 进入攻击范围的y值
    /// </summary>
    public float AttackYValue { get => _attackYValue; set => _attackYValue = value; }
    public MultiBuffChain EnemySpeedMultiChain { get; set; } = new MultiBuffChain();//敌人速度的乘法buff链

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
        StopAllCoroutines();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        //临时的
        TmpEnemyDieCoinProb = 0;

        _allEnemies.Clear();
        _enemiesInRange.Clear();
        _currentTargerEnemy = null;
        _currentClickedEnemy.Value = null;
        _enemyDieCoinProb.Value = 0.5f;
        _eliteEnemyProportion = 0.2f + BattleManager.Instance.Debuff.EliteEnemyCount * 0.1f;
        EnemySpeedMultiChain.Clear();

        _enemyDieCoin.Value = Mathf.RoundToInt(10+ScienceManager.Instance.GetUpgradeCountByType(ScienceEffectType.EnemyDieCount)*1);
        _enemyCurrentCount.Value = 0;
        _enemyDamageNullifiedCount.Value = 0 + BattleManager.Instance.Debuff.DamageNullified;
        SignleEnemyDieCount.Value = 0;
        enemyID = 0;

        Enemy.OnMoveInRange += OnMoveInRange;
        Enemy.OnMoveOutRange += OnMoveOutRange;
        Enemy.OnEnemyDie += UnregisterEnemy;
        BattleManager.OnEndBattle += OnEndBattle;

        if (_explosionEffectPool == null) _explosionEffectPool = new ObjectPool<ParticleSystem>(_explosionEffectPrefab, 5, _explosionEffectPoolParent);//初始化敌人对象池
        if (_explosionAnimPool == null) _explosionAnimPool = new ObjectPool<EnemyExplode>(_explosionAnimPrefab, 5, _explosionAnimPoolParent);//初始化敌人对象池
        StartCoroutine(GenerateEnemyIE());
    }

    /// <summary>
    /// 在 spawnPos 处生成一只指定类型的敌人，并为其设置路径
    /// </summary>
    /// <param name="type">敌人类型枚举</param>
    /// <param name="spawnPos">生成位置（世界坐标）</param>
    /// <param name="pathPoints">由 PathfindingHelper 计算得到的世界坐标路径点数组</param>
    public void SpawnEnemy(EnemyType enemyType,float xPos, int level,float yPos=15)
    {
        Enemy enemy = GetEnemy(enemyType);
        enemyID++;
        // 给 enemy 设置一个唯一名字
        enemy.Init(new Vector3(xPos, yPos, 0), level,enemyID);

        EnemyUIManager.Instance.RegisterEnemyUI(enemy, new Vector3(0, 0.0f, 0));
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
        {
            _allEnemies.Remove(enemy);
        }
            
        if(_currentTargerEnemy == enemy)
        {
            _currentTargerEnemy = null;
            if (_enemiesInRange.Count != 0)
            {
                Enemy lowestYEnemy = null;
                float lowestY = float.MaxValue; // 初始设为最大值，方便比较

                foreach (var ene in _enemiesInRange)
                {
                    if (ene.transform.position.y < lowestY)
                    {
                        lowestY = ene.transform.position.y;
                        lowestYEnemy = ene; // 找到 y 值最低的敌人
                    }
                }

                _currentTargerEnemy = lowestYEnemy;
            }
            else
            {
                _currentTargerEnemy = null;
            }
        }
        if (_currentClickedEnemy.Value == enemy)
        {
            _currentClickedEnemy.Value = null;
        }
        _enemyCurrentCount.Value = _allEnemies.Count;
        SignleEnemyDieCount.Value++;
    }

    /// <summary>
    /// 从对象池得到一个Enemy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Enemy GetEnemy(EnemyType enemyType)
    {
        // 如果已经有对象池，直接取
        if (enemyPools.TryGetValue(enemyType, out var pool))
        {
            var b = pool.Get();
            return b ;
        }

        // 2. 查 prefab
        if (!enemyDictionary.Dictionary.TryGetValue(enemyType, out var prefab) || prefab == null)
        {
            Debug.LogError($"[EnemyPool] 未找到 EnemyType: {enemyType}");
            return null;
        }
        // 3. 创建对象池
        pool = new ObjectPool<Enemy>(
            prefab,
            20,   //推荐让 Enemy 自己定义
            _enemyPoolParent
        );

        enemyPools.Add(enemyType, pool);

        // 4. 取对象
        return pool.Get();

    }

    /// <summary>
    /// 将bullet返回对象池
    /// </summary>
    /// <param name="bullet"></param>
    public void ReturnEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("ReturnBullet 失败：bullet 为 null");
            return;
        }

        // 如果对象池存在 → 正常回收
        if (enemyPools.TryGetValue(enemy.enemyType, out var pool))
        {
            pool.Return(enemy);
            return;
        }

        // 找不到对象池 → 说明这个 bullet 并非通过 GetBullet() 创建
        Debug.LogWarning($"回收 Bullet 失败：找不到 {enemy.enemyType} 的对象池，直接销毁该对象");
        Destroy(enemy.gameObject);
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
        Index = 3;
    }

    //敌人移动至攻击范围
    private void OnMoveInRange(Enemy enemy)
    {
        _enemiesInRange.Add(enemy);
        //当前的目标敌人为空或者进来的敌人是被玩家点击的敌人
        if (_currentTargerEnemy == null || _currentClickedEnemy.Value==enemy)
        {
            _currentTargerEnemy = enemy;
        }
    }
    //敌人移出攻击范围
    private void OnMoveOutRange(Enemy enemy)
    {
        if (_enemiesInRange.Contains(enemy))
        {
            _enemiesInRange.Remove(enemy);
            if (_currentTargerEnemy == enemy)
            {
                if (_enemiesInRange.Count != 0)
                {
                    Enemy lowestYEnemy = null;
                    float lowestY = float.MaxValue; // 初始设为最大值，方便比较

                    foreach (var ene in _enemiesInRange)
                    {
                        if (ene.transform.position.y < lowestY)
                        {
                            lowestY = ene.transform.position.y;
                            lowestYEnemy = ene; // 找到 y 值最低的敌人
                        }
                    }

                    _currentTargerEnemy = lowestYEnemy;
                }
                else
                {
                    _currentTargerEnemy = null;
                }
            }
        }
    }

    private IEnumerator GenerateEnemyIE ()
    {
        if (WaveManager.Instance.CurrentRound == 0)
        {
            yield return TimerUtility.WaitForGameSeconds(3);//第一波先等待三秒
        }
        
        while (true)
        {
            WaveManager.Instance.CurrentRound++;
            float enemyInterval = WaveManager.Instance.SignleEnemyInterval;
            if (WaveManager.Instance.CurrentRound % 5 == 0)//每五关生成一个boss
            {
                SpawnEnemy(EnemyType.Boss, 0, WaveManager.Instance.CurrentRound);
                yield return TimerUtility.WaitForGameSeconds(enemyInterval);
            }
            for (int i = 0; i < WaveManager.Instance.WaveEnemyCount; i++)
            {
                EnemyType type = ChooseType();
                SpawnEnemy(type, Random.Range(-5f, 5f), WaveManager.Instance.CurrentRound);
                yield return TimerUtility.WaitForGameSeconds(enemyInterval);
            }
            
           
            // 如果是最后一波 且是第一个生成的敌人（逆序的最后一个）
            if (WaveManager.Instance.CurrentRound == WaveManager.Instance.MaxRound)
            {
                OnLastEnemySpawned?.Invoke();  // 触发事件
            }

            if (WaveManager.Instance.CurrentRound >= WaveManager.Instance.MaxRound)
            {
                yield break;
            }
            // 启动一个提醒协程（监听 GameSpeed 和暂停）
            StartCoroutine(WaitAndNotifyBeforeTime(WaveManager.Instance.SpawnWaveInterval, 3f, () =>
            {
                OnAlmostNextWave?.Invoke();
            }));
            yield return TimerUtility.WaitForGameSeconds(WaveManager.Instance.SpawnWaveInterval);

        }
    }

    //启动一个提醒协程
    private IEnumerator WaitAndNotifyBeforeTime(float totalTime, float notifyBefore, System.Action callback)
    {
        float timer = 0f;
        float targetTime = totalTime - notifyBefore;

        while (timer < targetTime)
        {
            if (BattleManager.Instance.GameSpeed.Value>0)
            {
                timer += Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
            }
            yield return null;
        }

        callback?.Invoke();
    }

    private EnemyType ChooseType()
    {
        // 0 ~ 99
        int rand = Random.Range(0, 100);

        if (rand < 70)
        {
            return EnemyType.Normal;   // 70%
        }
        else if (rand < 95)
        {
            return EnemyType.Elite;    // 25%
        }
        else
        {
            return EnemyType.Coin;     // 5%
        }
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        _allEnemies.Clear();
        _enemiesInRange.Clear();
        _explosionEffectPool.ReturnAll();
        StopAllCoroutines();
    }

    #endregion
}


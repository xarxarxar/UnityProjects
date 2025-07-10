using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// 表示敌人单位，包含血量、移动、攻击、受击和点击逻辑。
/// 使用中央状态机协程（StateMachineLoop）统一管理不同状态下的行为，
/// 使状态切换清晰、易于维护，避免频繁启停多个协程。
/// </summary>
public class Enemy : MonoBehaviour
{
    #region 静态事件
    /// <summary>当敌人进入攻击范围时触发，参数为敌人自身</summary>
    public static UnityAction<Enemy> OnMoveInRange;
    /// <summary>当敌人死亡时触发，参数为敌人自身</summary>
    public static UnityAction<Enemy> OnEnemyDie;
    /// <summary>当敌人受到伤害时触发，参数：敌人自身，是否暴击，伤害值</summary>
    public static UnityAction<Enemy, bool, int> OnEnemyDamaged;
    /// <summary>当敌人被点击时触发，参数为敌人自身</summary>
    public static UnityAction<Enemy> OnEnemyClicked;
    #endregion

    #region 配置参数（可在 Inspector 中配置）
    [Header("UI 元素")]
    public Image shieldSlider;      // UI 护盾条，用于展示当前护盾值
    public Image hpSlider;          // UI 血条，用于展示当前生命值
    public Text levelText;          // 显示敌人等级的文本
    public Canvas enemyCanvas;      // 敌人的 Canvas，用于设置渲染层级

    [Header("敌人属性")]
    [Tooltip("敌人的最大生命值，实际值由等级计算")]
    public int maxHP = 100;
    [Tooltip("敌人的最大护盾值，默认无护盾")]
    public int maxShield = 100;
    //[Tooltip("移动速度，单位：世界单位/秒")]
    //public float moveSpeed = 2f;
    [Tooltip("击败敌人后获得的金币奖励")]
    public int rewardGold = 10;
    [Tooltip("UI 渲染层级顺序，数值越高越靠前")]
    public int enemyLayer = 0;
    [Tooltip("攻击时间间隔，单位：秒")]
    [SerializeField] private float _attackInterval = 0.5f;
    [Tooltip("攻击伤害")]
    [SerializeField] private int _attackDamage = 1;
    #endregion

    #region 私有字段（状态与运行时数据）
    private int _currentHP;         // 当前生命值
    private int _currentShield;     // 当前护盾值
    [SerializeField] private int _damageNullifiedCount;     // 免疫伤害次数
    private BuildingBase _targetBuilding => EnemyManager.Instance.TargetBuilding;  // 攻击目标建筑
    private bool _isDead;           // 是否已死亡
    private bool _isInRangeList;    // 是否已触发进入攻击范围事件
    private bool _isPaused => BattleManager.Instance.IsPaused;  // 游戏是否暂停

    /// <summary>
    /// 当前的HP
    /// </summary>
    public int CurrentHP 
    { 
        get => _currentHP;
        set 
        {
            if (_currentHP != value)
            {
                _currentHP = value;
            }
            levelText.text = _currentHP.ToString();  // 在 UI 上显示等级
            transform.localScale = Vector3.one * (1 + (_currentHP - 1) / 100f);//等级越大，体型越大
        } 
    }

    /// <summary>敌人状态枚举：移动 or 攻击</summary>
    private enum State { Moving, Attacking }
    private State _currentState;    // 当前状态
    private float _attackTimer;     // 计时器，用于控制攻击间隔
    #endregion

    #region Unity 生命周期
    private void OnEnable()
    {
        
    }
    private void OnDisable() 
    { 
        StopAllCoroutines();
    }
    #endregion

    #region 公共方法 （对外 API）
    /// <summary>
    /// 初始化敌人的位置、等级和渲染层级
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="level">敌人等级，用于计算生命值</param>
    /// <param name="layer">UI 渲染层级</param>
    public void Init(Vector3 position, int level, int layer)
    {
        transform.position = position;      // 设置初始位置
        _isDead = false;                    // 重置死亡状态
        _isInRangeList = false;             // 重置范围触发标志
        enemyLayer = layer;                 // 设置渲染层级

        // 根据等级动态计算最大生命值，一级就是一滴血,护盾默认为 0
        maxHP = Mathf.RoundToInt(level * (1+BattleManager.Instance.Debuff.AddHP*0.1f)) ;//算上debuff的，增加敌人10%HP
        maxShield = 0;
        CurrentHP = maxHP;                 // 初始化当前血量为最大值
        _currentShield = maxShield;         // 初始化当前护盾为最大值
        _damageNullifiedCount=EnemyManager.Instance.EnemyDamageNullifiedCount.Value;



        SetOrderLayer(enemyLayer);          // 更新 Canvas 排序层级
        ChangeShield();                     // 初始化护盾条填充
        ChangeHP();                         // 初始化血条填充

        // 设置初始状态为移动，并初始化攻击计时器
        _currentState = State.Moving;
        _attackTimer = _attackInterval;

        // 启动中央状态机协程，管理移动和攻击行为
        StartCoroutine(StateMachineLoop());

        BattleManager.OnEndBattle += OnEndBattle;
    }

    /// <summary>
    /// 响应鼠标点击事件，触发静态点击回调
    /// </summary>
    public void OnClicked()
    {
        OnEnemyClicked?.Invoke(this);
    }

    /// <summary>
    /// 敌人受到伤害，优先扣除护盾，护盾耗尽后才扣血
    /// </summary>
    /// <param name="isCritical">是否暴击</param>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(bool isCritical, int damage)
    {
        if (_isDead) return;               // 如果已死亡，忽略伤害

        if (_damageNullifiedCount > 0)
        {
            _damageNullifiedCount--;
            return;
        }

        OnEnemyDamaged?.Invoke(this, isCritical, damage); // 通知外部

        if (_currentShield > 0)            // 有护盾时先扣护盾
        {
            _currentShield = Mathf.Max(_currentShield - damage, 0);
            ChangeShield();                // 更新护盾 UI
        }
        else                               // 无护盾时扣血
        {
            CurrentHP = Mathf.Max(CurrentHP - damage, 0);
            ChangeHP();                    // 更新血条 UI
            if (CurrentHP == 0)
                Die();                    // 血量耗尽则死亡
        }
    }

    /// <summary>
    /// 设置或更新敌人的 Canvas 渲染层级
    /// </summary>
    /// <param name="layer">渲染层级</param>
    public void SetOrderLayer(int layer)
    {
        enemyLayer = layer;
        enemyCanvas.sortingOrder = enemyLayer;
    }

    
    #endregion

    #region 状态机协程
    /// <summary>
    /// 使用中央状态机协程控制敌人行为：
    /// - Moving：移动到目标建筑附近
    /// - Attacking：停留并定时攻击
    /// </summary>
    private IEnumerator StateMachineLoop()
    {
        while (true)
        {
            if (_isDead)                    // 死亡后退出协程
                yield break;

            if (_isPaused)                  // 游戏暂停时，挂起一帧
            {
                yield return null;
                continue;
            }

            switch (_currentState)
            {
                case State.Moving:
                    HandleMovingState();      // 处理移动行为
                    break;

                case State.Attacking:
                    HandleAttackingState();   // 处理攻击行为
                    break;
            }

            yield return null;              // 每帧更新一次状态机
        }
    }

    /// <summary>
    /// 移动状态逻辑：向目标建筑移动，并在进入有效范围时触发事件
    /// </summary>
    private void HandleMovingState()
    {
        Vector3 targetPos = _targetBuilding.transform.position;
        //float distance = Vector3.Distance(transform.position, targetPos);
        float bottomY = transform.position.y - 0.5f * transform.localScale.y;//敌人的下边缘

        //if (distance > 0.01f)
        if (bottomY>-4.0f)
        {
            // 按移动速度和游戏倍速平滑移动
            transform.position += Vector3.down * EnemyManager.Instance.EnemySpeed.Value * BattleManager.Instance.GameSpeed * Time.deltaTime;

            //transform.position = Vector3.MoveTowards(
            //    transform.position,
            //    targetPos,
            //    EnemyManager.Instance.EnemySpeed.Value * BattleManager.Instance.GameSpeed * Time.deltaTime);

            // 当 Y <= 13（示例值）且首次进入范围，触发 OnMoveInRange
            if (transform.position.y < 8f && !_isInRangeList)
            {
                _isInRangeList = true;
                OnMoveInRange?.Invoke(this);
            }
        }
        else
        {
            // 到达目标点后，切换到攻击状态
            _currentState = State.Attacking;
            _attackTimer = _attackInterval;
        }
    }

    /// <summary>
    /// 攻击状态逻辑：在固定间隔内对目标建筑造成伤害
    /// </summary>
    private void HandleAttackingState()
    {
        //float distance = Vector3.Distance(transform.position, _targetBuilding.transform.position);
        float bottomY = transform.position.y - 0.5f * transform.localScale.y;//敌人的下边缘
        // 如果目标建筑超出攻击范围，则切换回移动状态
        if (bottomY > -4.0f)
        {
            _currentState = State.Moving;
            return;
        }

        // 倒计时逻辑，考虑游戏速度
        _attackTimer -= Time.deltaTime * BattleManager.Instance.GameSpeed;
        if (_attackTimer <= 0f)
        {
            // 对目标建筑造成一次伤害（示例：1点），可扩展为属性化
            _targetBuilding.TakeDamage(_attackDamage);
            _attackTimer = _attackInterval;
        }
    }
    #endregion

    #region 辅助方法
    /// <summary>敌人死亡流程：设置标志、触发事件、回收对象</summary>
    private void Die()
    {
        _isDead = true;
        OnEnemyDie?.Invoke(this);
        EnemyManager.Instance.EnemyPool.Return(this);
    }

    /// <summary>更新血条 UI 填充比例</summary>
    private void ChangeHP()
    {
        hpSlider.fillAmount = maxHP == 0 ? 0f : (float)CurrentHP / maxHP;
    }

    /// <summary>更新护盾条 UI 填充比例</summary>
    private void ChangeShield()
    {
        shieldSlider.fillAmount = maxShield == 0 ? 0f : (float)_currentShield / maxShield;
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        EnemyManager.Instance.EnemyPool.Return(this);
        StopAllCoroutines();
    }
    #endregion
}

using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public enum EnemyState
{
    Walk,
    Attack,
    Death
}

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
    /// <summary>当敌人移出攻击范围时触发，参数为敌人自身</summary>
    public static UnityAction<Enemy> OnMoveOutRange;
    /// <summary>当敌人死亡时触发，参数为敌人自身</summary>
    public static UnityAction<Enemy> OnEnemyDie;
    /// <summary>当敌人受到伤害时触发，参数：敌人自身，是否暴击，伤害值</summary>
    public static UnityAction<Enemy, bool, int> OnEnemyDamaged;
    #endregion

    #region 配置参数
    //敌人的种类
    public EnemyType enemyType;
    //敌人当前的状态
    private  EnemyState enemyState;
    private IEnemyAttack currentTarget;//当前攻击目标
    //敌人的最大生命值，实际值由等级计算
    [HideInInspector] public  Bindable<int> maxHP = new Bindable<int>();
    //敌人的最大护盾值，默认无护盾
    [HideInInspector] public Bindable<int> maxShield = new Bindable<int>();
    #endregion

    #region 私有字段（状态与运行时数据）
    public float _enemySpeed = 0.6f;
    private Bindable<int> _currentHP = new Bindable<int>();       // 当前生命值
    private Bindable<int> _currentShield = new Bindable<int>();   // 当前护盾值
    private int _damageNullifiedCount;         // 免疫伤害次数
    private bool _isInRangeList;    // 是否已触发进入攻击范围
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public float flashDuration = 0.1f; // 闪红时间
    public Color originalColor=new Color32(104,207,220,225);
    [HideInInspector] public Color hitColor = new Color32(255, 102, 51, 255);

    private Coroutine _bleedCoro = null;//流血的协程
    [HideInInspector]public int UniqueID;//该敌人的唯一编号
    public float BlessExp=5;//该敌人提供的祝福经验值
    [HideInInspector] public Animator animator;//动画控制器
    private float _attackInterval=6.0f;//攻击间隔
    private float _attackTimer = 0f;//攻击计时器

    /// <summary>
    /// 敌人当前血量
    /// </summary>
    public Bindable<int> CurrentHP { get => _currentHP; }

    /// <summary>
    /// 敌人当前护盾
    /// </summary>
    public Bindable<int> CurrentShield { get => _currentShield; }
    #endregion

    #region Unity 生命周期

    private void OnDisable() 
    {
        //杀掉颜色动画
        if(spriteRenderer != null)
        {
            spriteRenderer.DOKill();
            spriteRenderer.color = originalColor;
        }
        //transform.localScale = Vector3.one;
        StopAllCoroutines();
    }

    void Update()
    {
        float delta = Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
        // 移动（只有在能移动时）
        if (enemyState==EnemyState.Walk)
        {
            transform.Translate(
                Vector2.down *
                EnemyManager.Instance.EnemySpeedMultiChain.Result *
                _enemySpeed *
                delta *
                (1 + BattleManager.Instance.Debuff.AddSpeed * 0.1f)
            );
        }

        // 攻击冷却计时
        if (_attackTimer > 0f)
            _attackTimer -= delta;

        // 是否进入炮塔攻击区域
        bool inAttackRange = transform.position.y < EnemyManager.Instance.AttackYValue;
        //进入炮塔攻击范围
        if (inAttackRange && !_isInRangeList)
        {
            _isInRangeList=true;
            OnMoveInRange?.Invoke(this);
        }
        //走出炮塔攻击范围
        if (!inAttackRange && _isInRangeList)
        {
            _isInRangeList = false;
            OnMoveOutRange?.Invoke(this);
        }

        //水晶进入自身攻击范围
        bool canAttack = transform.position.y < (-4.5f + transform.localScale.x * 0.5f) && enemyState != EnemyState.Death;
        if(canAttack && enemyState != EnemyState.Attack)
        {
            enemyState = EnemyState.Attack;
        }
        if (enemyState == EnemyState.Attack)
        {
            

            // 冷却完成，才允许攻击
            if (_attackTimer <= 0f)
            {
                PlayAttack();
                _attackTimer = _attackInterval; //重置冷却
            }
        }
        animator.SetBool("IsMoving", enemyState == EnemyState.Walk);
        //if (transform.position.y < (-4.5f + transform.localScale.x * 0.5f) && !_isDead)
        //{
        //    if(enemyType==EnemyType.Elite)
        //    {
        //        EnemyExplode enemyExplode = EnemyManager.Instance.ExplosionAnimPool.Get();
        //        int damage = Mathf.RoundToInt(_currentHP.Value * 0.3f);
        //        enemyExplode.Init(transform.position, originalColor, _enemyScale, () =>
        //        {
        //            Explode();
        //            Crystal.Instance.TakeDamage(damage);
        //        });
        //    }
        //    else
        //    {
        //        isMoving=false;
        //        PlayAttack();
        //        Crystal.Instance.TakeDamage(Mathf.RoundToInt(CurrentHP.Value / 10.0f) );
        //    }

        //    PlayDeath();
        //}
        //animator.SetBool("IsMoving", isMoving);
    }
    #endregion

    #region 公共方法 （对外 API）
    /// <summary>
    /// 初始化敌人的位置、等级和渲染层级
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="level">敌人等级，用于计算生命值</param>
    /// <param name="layer">UI 渲染层级</param>
    public void Init(Vector3 position, int level,int uniqueID)
    {
        transform.position = position;// 设置初始位置
        enemyState = EnemyState.Walk;//默认为行走状态
        currentTarget = Crystal.Instance;//默认目标为下方的城墙
        _isInRangeList = false;       // 重置范围触发标志
        UniqueID = uniqueID;//敌人编号

        // 根据等级动态计算最大生命值，护盾默认为 0
        maxHP.Value = Mathf.RoundToInt(20*(Mathf.Pow(1.14f,level)) * (1 + BattleManager.Instance.Debuff.AddHP * 0.1f));//算上debuff的，增加敌人10%HP
        if (enemyType == EnemyType.Boss)
        {
            maxHP.Value *= 10;
        }
        _currentHP.Value = maxHP.Value;   //初始化血量
        maxShield.Value = 0;            //初始化护盾
        if (this.enemyType==EnemyType.Elite || this.enemyType == EnemyType.Boss)
        {
            maxShield.Value = Mathf.Max(1,Mathf.RoundToInt(maxHP.Value));          //初始化护盾
        }
        _currentShield.Value = maxShield.Value;             // 初始化当前护盾为最大值

        //免疫伤害
        _damageNullifiedCount = EnemyManager.Instance.EnemyDamageNullifiedCount.Value;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        BattleManager.OnEndBattle -= OnEndBattle;
        BattleManager.OnEndBattle += OnEndBattle;
        BattleManager.Instance.GameSpeed.OnValueChanged -= OnGameSpeedChanged;
        BattleManager.Instance.GameSpeed.OnValueChanged += OnGameSpeedChanged;
        animator.speed = BattleManager.Instance.GameSpeed.Value;
        //isMoving = true;
        _attackTimer = _attackInterval;
        //ChangeType(this.enemyType);
    }
    
    private void OnGameSpeedChanged(int speed)
    {
        animator.speed= BattleManager.Instance.GameSpeed.Value;
    }

    /// <summary>
    /// 敌人受到伤害，优先扣除护盾，护盾耗尽后才扣血
    /// </summary>
    /// <param name="isCritical">是否暴击</param>
    /// <param name="damage">伤害值</param>
    /// <param name="isRealDamage">是否是真实伤害</param>
    public void TakeDamage(bool isCritical, int damage,bool isRealDamage=false)
    {
        if (enemyState==EnemyState.Death) return;               // 如果已死亡，忽略伤害

        AudioManager.Instance.PlaySFX("被击中");
        if (_damageNullifiedCount > 0  && !isRealDamage)//有免伤次数,且不是真实伤害
        {
            _damageNullifiedCount--;
            Vector3 worldPos = transform.position + Vector3.up * 1.2f;  // 头顶偏移
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            DamageText dmgText = BattleUIManager.Instance.DamageTextPool.Get();  // 用对象池
            dmgText.Init(screenPos, false, "免疫");
            return;
        }
        if(isCritical) AudioManager.Instance.Vibrate("light");//重震动
        OnEnemyDamaged?.Invoke(this, isCritical, damage); // 通知外部

        if (_currentShield.Value > 0 && !isRealDamage)            // 有护盾时先扣护盾
        {
            _currentShield.Value = Mathf.Max(_currentShield.Value - damage, 0);
        }
        else                               // 无护盾时扣血
        {
            _currentHP.Value = Mathf.Max(_currentHP.Value - damage, 0);
            FlashRed();//闪红
            if (_currentHP.Value <= 0)
            {
                PlayDeath();                    // 血量耗尽则死亡
                Explode();

                return;
            }
        }
        EnemyUIManager.Instance.UpdateEnemyHealth(this,isRealDamage,isCritical);
    }



    public void RecoverHp(int value)
    {
        if (enemyState == EnemyState.Death) return;               // 如果已死亡，忽略
        _currentHP.Value = Mathf.Min(_currentHP.Value + value, maxHP.Value);
        EnemyUIManager.Instance.UpdateEnemyHealth(this);
    }

    /// <summary>
    /// 设置流血效果。可以用于中毒，灼烧等等
    /// </summary>
    /// <param name="damage">每次流血的血量</param>
    /// <param name="timeDelat">每次流血的间隔</param>
    /// <param name="duration">流血持续的时间</param>
    public void SetBleed(int damage, float duration, float timeDelat=1.0f)
    {
        if (duration <= 0 || timeDelat<=0 || damage<=0) return;
        if (_bleedCoro != null)
        {
            StopCoroutine(_bleedCoro);
            _bleedCoro = null;
        }
        _bleedCoro = StartCoroutine(BleedCoro(damage, timeDelat,duration));
    }
    /// <summary>
    /// 到达城墙
    /// </summary>
    public void OnReachWall(IEnemyAttack enemyAttack)
    {
        enemyState = EnemyState.Attack;
        currentTarget = enemyAttack;
    }
    /// <summary>
    /// 离开城墙
    /// </summary>
    public void OnLeaveWall()
    {
        Debug.Log("敌人离开了城墙");
        enemyState = EnemyState.Walk;
        currentTarget = Crystal.Instance;
    }
    #endregion

    #region 辅助方法
    //播放攻击动画
    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
    /// <summary>
    /// 敌人进行攻击，Attack动画event调用
    /// </summary>
    public void Attack()
    {
        int damage =Mathf.Max(1,Mathf.RoundToInt(maxHP.Value / 10.0f)) ;
        currentTarget.TakeDamage(damage);
    }

    //播放死亡动画
    public void PlayDeath()
    {
        enemyState = EnemyState.Death;
        animator.SetTrigger("Death");
        OnEnemyDie?.Invoke(this);
        EnemyUIManager.Instance.RemoveEnemyUI(transform);
    }
    /// <summary>
    /// 敌人死亡流程：播放动画，设置标志、触发事件、回收对象
    /// 死亡动画的event会调用
    /// </summary>
    public void Die()
    {
        EnemyManager.Instance.ReturnEnemy(this);
    }

    /// <summary>
    /// 修改敌人的类型
    /// </summary>
    public void ChangeType(EnemyType type)
    {
        enemyType = type;
        switch (enemyType)
        {
            case EnemyType.Normal:
                originalColor = new Color32(128, 200, 245, 255); // 淡雾蓝（极柔）
                break;

            case EnemyType.Coin:
                originalColor = new Color32(245, 215, 140, 255); // 奶油金（温柔金黄）
                break;

            case EnemyType.Elite:
                originalColor = new Color32(210, 170, 240, 255); // 薰衣草紫（治愈系）
                break;

            case EnemyType.Boss:
                originalColor = new Color32(200, 70, 70, 255);// 深红
                break;
        }
        spriteRenderer.color = originalColor;
    }

    private IEnumerator BleedCoro(int damage, float timeDelat=1, float duration=0)
    {
        float totalTime = 0;
        while (totalTime < duration)
        {
            yield return TimerUtility.WaitForGameSeconds(timeDelat);
            TakeDamage(false,damage);
            totalTime += duration;
        }
        yield break;
    }

    
    //爆炸
    private void Explode()
    {
        if (!BattleManager.Instance.IsBatting) return;//战斗已经结束
        ParticleSystem particleSystem = EnemyManager.Instance.ExplosionEffectPool.Get();
        var main = particleSystem.main;         // 拿到副本
        main.simulationSpeed = BattleManager.Instance.GameSpeed.Value;  // 修改副本
        particleSystem.transform.position = transform.position;
        particleSystem.Play();
        AudioManager.Instance.PlaySFX("敌人爆炸");
        
        TimerUtility.Instance.Timer(0.5f, () => { EnemyManager.Instance.ExplosionEffectPool.Return(particleSystem); });
    }


    //挑战结束
    private void OnEndBattle(bool success)
    {
        Debug.Log("敌人监听战斗结束");
        EnemyManager.Instance.ReturnEnemy(this);
        EnemyUIManager.Instance.RemoveEnemyUI(transform);
        StopAllCoroutines();
    }

    //闪红动画
    private void FlashRed()
    {
        spriteRenderer.DOKill(true);

        // 创建第一个红色过渡
        var toRed = spriteRenderer.DOColor(hitColor, flashDuration)
            .SetEase(Ease.Linear);

        // 创建第二个还原过渡
        var toNormal = spriteRenderer.DOColor(originalColor, flashDuration)
            .SetEase(Ease.Linear);

        // 合并为一个序列
        var seq = DOTween.Sequence();
        seq.Append(toRed);
        seq.Append(toNormal);

        // 初始化时同步 GameSpeed
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        // 临时订阅
        void OnSpeedChanged(int speed)
        {
            if (seq != null && seq.IsActive())
                seq.timeScale = speed;
        }
        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        // 动画结束后解绑
        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }

    #endregion
}

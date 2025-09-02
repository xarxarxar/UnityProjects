using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;


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
    //敌人的最大生命值，实际值由等级计算
    public Bindable<int> maxHP = new Bindable<int>();
    //敌人的最大护盾值，默认无护盾
    public Bindable<int> maxShield = new Bindable<int>();
    //UI 渲染层级顺序，数值越高越靠前
    private int enemyLayer = 0;
    //攻击伤害
    private int _attackDamage = 1;
    //最大弹跳次数
    private int _bounsCount = 5;
    //敌人的速度
    private float _enemySpeed = 2;
    //敌人的体型大小
    private float _enemyScale = 1;
    #endregion

    #region 私有字段（状态与运行时数据）
    private Rigidbody2D _rb = null;
    private Bindable<int> _currentHP = new Bindable<int>();       // 当前生命值
    private Bindable<int> _currentShield = new Bindable<int>();   // 当前护盾值
    private int _damageNullifiedCount;         // 免疫伤害次数
    private bool _isDead;           // 是否已死亡
    private bool _isInRangeList;    // 是否已触发进入攻击范围
    private bool _isPaused => BattleManager.Instance.IsPaused.Value;  // 游戏是否暂停
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f; // 闪红时间
    private Color originalColor=new Color32(104,207,220,225);
    public Color hitColor = new Color32(255, 102, 51, 255);

    private Coroutine _setSpeedCoro = null;//减速的协程
    private float _speedRate = 1;//速度的比例

    private Coroutine _bleedCoro = null;//流血的协程

    //Boss技能
    //public List<BossSkill> Skills = new List<BossSkill>();

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
        _speedRate = 1;
        transform.localScale = Vector3.one;
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        // 如果当前速度为0，则不处理（避免除以0）
        if (_rb.velocity.sqrMagnitude > 0.01f)
        {
            // 保持原方向，调整速度大小
            _rb.velocity = _rb.velocity.normalized * _enemySpeed * _speedRate
                * BattleManager.Instance.GameSpeed.Value;
        }
    }
    #endregion

    #region 公共方法 （对外 API）
    /// <summary>
    /// 初始化敌人的位置、等级和渲染层级
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="level">敌人等级，用于计算生命值</param>
    /// <param name="layer">UI 渲染层级</param>
    public void Init(EnemyType enemyType, Vector3 position, int level, int layer)
    {
        transform.position = position;// 设置初始位置
        _isDead = false;              // 重置死亡状态
        _isInRangeList = false;       // 重置范围触发标志
        enemyLayer = layer;           // 设置渲染层级
        _bounsCount = 5;             //最大弹跳次数
        this.enemyType = enemyType;   //敌人类型

        // 根据等级动态计算最大生命值，一级就是一滴血,护盾默认为 0
        maxHP.Value = Mathf.RoundToInt(level * (1 + BattleManager.Instance.Debuff.AddHP * 0.1f)*10);//算上debuff的，增加敌人10%HP
        
        _currentHP.Value = maxHP.Value;   //初始化血量
        maxShield.Value = 0;            //初始化护盾
        if (this.enemyType==EnemyType.Elite || this.enemyType == EnemyType.Boss)
        {
            maxShield.Value = Mathf.Max(1,Mathf.RoundToInt(maxHP.Value));          //初始化护盾
        }
        _currentShield.Value = maxShield.Value;             // 初始化当前护盾为最大值
        _currentHP.OnValueChanged += ChangeHP;  //添加值变化事件
        _currentShield.OnValueChanged += ChangeShield;//添加值变化事件
        ChangeShield(_currentHP.Value);         // 初始化护盾条填充
        ChangeHP(_currentShield.Value);         // 初始化血条填充
        _enemyScale = 1;
        SetScale();//设置体型大小


        //免疫伤害
        _damageNullifiedCount = EnemyManager.Instance.EnemyDamageNullifiedCount.Value;

        SetOrderLayer(enemyLayer);          // 更新 Canvas 排序层级

        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        _rb.velocity = Vector2.down.normalized;

        BattleManager.OnEndBattle += OnEndBattle;

        switch (this.enemyType)
        {
            case EnemyType.Normal:
                originalColor= new Color32(80, 140, 255, 255);// 柔和蓝
                break;

            case EnemyType.Coin:
                originalColor = new Color32(212, 175, 55, 255);// 金色
                break;

            case EnemyType.Elite:
                originalColor = new Color32(170, 100, 220, 255); // 紫色
                break;

            case EnemyType.Boss:
                originalColor = new Color32(200, 70, 70, 255);// 深红
                //Skills.Add(new BossSkill("加速", 10f, boss => boss.SetSpeed(2.0f,5)));
                BossSkill_05();
                break;
        }
        GetComponent<SpriteRenderer>().color = originalColor;
    }


    /// <summary>
    /// 敌人受到伤害，优先扣除护盾，护盾耗尽后才扣血
    /// </summary>
    /// <param name="isCritical">是否暴击</param>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(bool isCritical, int damage)
    {
        if (_isDead) return;               // 如果已死亡，忽略伤害

        AudioManager.Instance.PlaySFX("被击中");
        if (_damageNullifiedCount > 0)//有免伤次数
        {
            _damageNullifiedCount--;
            return;
        }

        OnEnemyDamaged?.Invoke(this, isCritical, damage); // 通知外部

        if (_currentShield.Value > 0)            // 有护盾时先扣护盾
        {
            _currentShield.Value = Mathf.Max(_currentShield.Value - damage, 0);
        }
        else                               // 无护盾时扣血
        {
            _currentHP.Value = Mathf.Max(_currentHP.Value - damage, 0);
            FlashRed();//闪红
            //EnemyUIManager.Instance.UpdateEnemyHp(transform, _currentHP.Value);
            if (_currentHP.Value <= 0)
            {
                Die();                    // 血量耗尽则死亡
                ParticleSystem particleSystem = EnemyManager.Instance.ExplosionEffectPool.Get();
                particleSystem.transform.position = transform.position;
                particleSystem.Play();
                TimerUtility.Instance.Timer(0.5f, () => { EnemyManager.Instance.ExplosionEffectPool.Return(particleSystem); });
            }
                
        }
        SetScale();//设置体型大小
        EnemyUIManager.Instance.UpdateEnemyHealth(this);
    }

    public void RecoverHp(int value)
    {
        if (_isDead) return;               // 如果已死亡，忽略
        _currentHP.Value = Mathf.Min(_currentHP.Value + value, maxHP.Value);
        EnemyUIManager.Instance.UpdateEnemyHealth(this);
    }

    /// <summary>
    /// 设置或更新敌人的 Canvas 渲染层级
    /// </summary>
    /// <param name="layer">渲染层级</param>
    public void SetOrderLayer(int layer)
    {
        enemyLayer = layer;
    }

    /// <summary>
    /// 设置敌人的速度，可用于干冰子弹
    /// </summary>
    /// <param name="rate">设置速度为百分之多少</param>
    /// <param name="duration">持续时长，若为0，则一直持续</param>
    public void SetSpeed(float rate, float duration = 0,UnityAction callback=null)//自己被设置速度
    {
        if(rate<0) return;
        _speedRate = rate;
        Vector2 dir = Vector2.zero;
        if (rate == 0)
        {
            Debug.Log($"速度为{_rb.velocity.normalized}");
            dir = _rb.velocity.normalized;
        }
        if (duration > 0)
        {
            if (_setSpeedCoro != null)
            {
                StopCoroutine(_setSpeedCoro);
                _setSpeedCoro = null;
            }
            _setSpeedCoro = StartCoroutine(SetSpeedCoro(duration,dir, callback));
        }
        else
        {
            return;
        }
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
    #endregion

    #region 辅助方法

    //每隔一段时间加速一会儿
    private void BossSkill_01()
    {
        TimerUtility.RepeatDoing(
            this,
            10.0f,
            () => { SetSpeed(2.0f, 5.0f); },
            null,
            0
            );
    }

    //每隔一段时间为自己添加当前血量10%的护盾
    private void BossSkill_02()
    {
        TimerUtility.RepeatDoing(
            this,
            10.0f,
            () => { CurrentShield.Value += CurrentHP.Value;
                EnemyUIManager.Instance.UpdateEnemyHealth(this);
            },
            null,
            0
            );
    }

    //每隔一段时间召唤一个小球，血量为2，自身扣除一点血量
    private void BossSkill_03()
    {
        TimerUtility.RepeatDoing(
            this,
            10.0f,
            () => {
                EnemyManager.Instance.SpawnEnemy(EnemyType.Normal,Random.Range(-7.5f, 7.5f),1);
                TakeDamage(false,1);
            },
            ()=>CurrentHP.Value>1,
            5
            );
    }

    //每隔一段时间进行有丝分裂
    private void BossSkill_04()
    {
        TimerUtility.RepeatDoing(
            this,
            10.0f,
            () => {
                EnemyManager.Instance.SpawnEnemy(EnemyType.Boss, transform.position.x-1,
                    Mathf.RoundToInt(CurrentHP.Value/2), transform.position.y);
                EnemyManager.Instance.SpawnEnemy(EnemyType.Boss, transform.position.x + 1,
                    Mathf.RoundToInt(CurrentHP.Value / 2), transform.position.y);
                _isDead = true;
                EnemyUIManager.Instance.RemoveEnemyUI(transform);
                EnemyManager.Instance.EnemyPool.Return(this); ;
            },
            () => CurrentHP.Value > 1,
            10
            );
    }

    private void BossSkill_05()
    {
        bool isInSkill=false;//是否正在使用技能中
        _currentHP.OnValueChanged += null;
        _currentHP.OnValueChanged += OnCurrentHpChanged;
        void OnCurrentHpChanged(int hp)
        {
            if (hp < Mathf.RoundToInt(maxHP.Value / 2) && !isInSkill)
            {
                isInSkill = true;
                SetSpeed(0, 5, () => { RecoverHp(Mathf.RoundToInt(maxHP.Value / 5)); isInSkill = false; });
            }
        }
    }

    

    private IEnumerator SetSpeedCoro(float duration,Vector2 dir, UnityAction callback = null)
    {
        yield return TimerUtility.WaitForGameSeconds(duration);
        Debug.Log($"结束后初始速度为{dir.normalized}");
        if (_speedRate == 0)
        {
            _rb.velocity=dir.normalized;
            Debug.Log($"结束后速度为{_rb.velocity.normalized}");
        }
        _speedRate = 1;
        callback?.Invoke();
        yield break;
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

    /// <summary>敌人死亡流程：设置标志、触发事件、回收对象</summary>
    private void Die()//isExploed是否是自爆
    {
        _isDead = true;
        EnemyUIManager.Instance.RemoveEnemyUI(transform);
        OnEnemyDie?.Invoke(this);
        EnemyManager.Instance.EnemyPool.Return(this);
    }

    /// <summary>更新血条 UI 填充比例</summary>
    private void ChangeHP(int currentHp)
    {
        //hpSlider.fillAmount = maxHP == 0 ? 0f : (float)CurrentHP / maxHP;
    }

    /// <summary>更新护盾条 UI 填充比例</summary>
    private void ChangeShield(int currentShield)
    {
        //shieldSlider.fillAmount = maxShield == 0 ? 0f : (float)_currentShield / maxShield;
    }

    /// <summary>
    /// 设置体型大小
    /// </summary>
    private void SetScale()
    {
        if (CurrentHP.Value <= 0)
        {
            transform.localScale = Vector3.one;
            return;
        }
        float level01 = (CurrentHP.Value - 1f) / 49f;
        float t = Mathf.SmoothStep(0f, 1f, level01);
        _enemyScale = Mathf.Lerp(1.0f, 1.6f, t);
        transform.localScale = Vector3.one * _enemyScale;
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        EnemyManager.Instance.EnemyPool.Return(this);
        EnemyUIManager.Instance.RemoveEnemyUI(transform);
        StopAllCoroutines();
    }

    //闪红动画
    private void FlashRed()
    {
        // 先杀掉之前的颜色动画，避免受击多次时颜色乱掉
        spriteRenderer.DOKill();

        // 颜色切换到红色，然后回到原色
        spriteRenderer.DOColor(hitColor, flashDuration/BattleManager.Instance.GameSpeed.Value)
            .OnComplete(() => spriteRenderer.DOColor(originalColor, flashDuration / BattleManager.Instance.GameSpeed.Value));
    }

    //碰撞
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Building")|| collision.gameObject.CompareTag("Crystal"))
        {
            BuildingBase building = collision.transform.parent.GetComponent<BuildingBase>();
            building.TakeDamage(_attackDamage);

            if (_bounsCount == 1)
            {
                EnemyExplode enemyExplode = EnemyManager.Instance.ExplosionAnimPool.Get();
                BuildingBase targetBuilding = building; // 提前保存引用
                int damage =Mathf.RoundToInt(_currentHP.Value * 0.3f) ;
                enemyExplode.Init(transform.position, originalColor, _enemyScale, () =>
                {
                    Debug.Log($"造成自爆伤，目标是{collision.transform.parent.name}，伤害为{damage}");
                    targetBuilding.TakeDamage(damage);
                });
                Die();
                _bounsCount = 0;
            }
            _bounsCount--;
        }
    }

    //进入区域
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AttackArea"))
        {
            OnMoveInRange?.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("AttackArea"))
        {
            OnMoveOutRange?.Invoke(this);
        }
    }

    
    #endregion
}

//public class BossSkill
//{
//    public string Name;
//    public float Cooldown;      // 技能冷却时间
//    public float Timer;         // 计时器
//    public System.Action<Enemy> Action; // 技能触发的方法

//    public BossSkill(string name, float cooldown, System.Action<Enemy> action)
//    {
//        Name = name;
//        Cooldown = cooldown;
//        Timer = 0;
//        Action = action;
//    }

//    // 更新时间，如果达到冷却时间就触发技能
//    public void UpdateSkill(Enemy boss, float deltaTime)
//    {
//        Timer += deltaTime;
//        if (Timer >= Cooldown)
//        {
//            Action?.Invoke(boss);
//            Timer = 0;
//        }
//    }
//}

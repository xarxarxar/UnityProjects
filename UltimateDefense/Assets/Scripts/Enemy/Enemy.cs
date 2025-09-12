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
    //敌人的速度
    private float _enemySpeed = 0.5f;
    //敌人的体型大小
    private float _enemyScale = 1;
    #endregion

    #region 私有字段（状态与运行时数据）
    private Bindable<int> _currentHP = new Bindable<int>();       // 当前生命值
    private Bindable<int> _currentShield = new Bindable<int>();   // 当前护盾值
    private int _damageNullifiedCount;         // 免疫伤害次数
    private bool _isDead;           // 是否已死亡
    private bool _isInRangeList;    // 是否已触发进入攻击范围
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f; // 闪红时间
    private Color originalColor=new Color32(104,207,220,225);
    public Color hitColor = new Color32(255, 102, 51, 255);

    private Coroutine _setSpeedCoro = null;//减速的协程
    private float _speedRate = 1;//速度的比例

    private Coroutine _bleedCoro = null;//流血的协程


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

    void Update()
    {
        // 每秒向下移动 speed 个单位
        transform.Translate(Vector2.down * _enemySpeed * _speedRate * Time.deltaTime
            * BattleManager.Instance.GameSpeed.Value *(1+BattleManager.Instance.Debuff.AddSpeed*0.1f));

        if(transform.position.y < EnemyManager.Instance.AttackYValue  && !_isInRangeList)
        {
            _isInRangeList=true;
            OnMoveInRange?.Invoke(this);
        }
        if (transform.position.y >= EnemyManager.Instance.AttackYValue && _isInRangeList)
        {
            _isInRangeList = false;
            OnMoveOutRange?.Invoke(this);
        }

        if (transform.position.y < (-4.5f + transform.localScale.x * 0.5f) && !_isDead)//可以攻击
        {
            if(enemyType==EnemyType.Elite)
            {
                EnemyExplode enemyExplode = EnemyManager.Instance.ExplosionAnimPool.Get();
                int damage = Mathf.RoundToInt(_currentHP.Value * 0.3f);
                enemyExplode.Init(transform.position, originalColor, _enemyScale, () =>
                {
                    Explode();
                    Crystal.Instance.TakeDamage(damage);
                });
            }
            else
            {
                Explode();
                Crystal.Instance.TakeDamage(Mathf.RoundToInt(CurrentHP.Value / 10.0f) );
            }
            
            Die();
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
    public void Init(EnemyType enemyType, Vector3 position, int level)
    {
        transform.position = position;// 设置初始位置
        _isDead = false;              // 重置死亡状态
        _isInRangeList = false;       // 重置范围触发标志
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

        _enemyScale = 1;
        SetScale();//设置体型大小

        //免疫伤害
        _damageNullifiedCount = EnemyManager.Instance.EnemyDamageNullifiedCount.Value;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        BattleManager.OnEndBattle += OnEndBattle;

        ChangeType(this.enemyType);
    }


    /// <summary>
    /// 敌人受到伤害，优先扣除护盾，护盾耗尽后才扣血
    /// </summary>
    /// <param name="isCritical">是否暴击</param>
    /// <param name="damage">伤害值</param>
    /// <param name="isRealDamage">是否是真实伤害</param>
    public void TakeDamage(bool isCritical, int damage,bool isRealDamage=false)
    {
        if (_isDead) return;               // 如果已死亡，忽略伤害

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

        OnEnemyDamaged?.Invoke(this, isCritical, damage); // 通知外部

        if (_currentShield.Value > 0 && !isRealDamage)            // 有护盾时先扣护盾
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
                Explode();

                return;
            }
        }
        SetScale();//设置体型大小
        EnemyUIManager.Instance.UpdateEnemyHealth(this,isRealDamage,isCritical);
    }

    public void RecoverHp(int value)
    {
        if (_isDead) return;               // 如果已死亡，忽略
        _currentHP.Value = Mathf.Min(_currentHP.Value + value, maxHP.Value);
        EnemyUIManager.Instance.UpdateEnemyHealth(this);
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
        if (duration > 0)
        {
            if (_setSpeedCoro != null)
            {
                StopCoroutine(_setSpeedCoro);
                _setSpeedCoro = null;
            }
            _setSpeedCoro = StartCoroutine(SetSpeedCoro(duration,callback));
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

    /// <summary>
    /// 修改敌人的类型
    /// </summary>
    public void ChangeType(EnemyType type)
    {
        enemyType = type;
        switch (enemyType)
        {
            case EnemyType.Normal:
                originalColor = new Color32(80, 140, 255, 255);// 柔和蓝
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
        spriteRenderer.color = originalColor;
    }

    

    private IEnumerator SetSpeedCoro(float duration,UnityAction callback = null)
    {
        yield return TimerUtility.WaitForGameSeconds(duration);
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
    //爆炸
    private void Explode()
    {
        if (!BattleManager.Instance.IsBatting) return;//战斗已经结束
        ParticleSystem particleSystem = EnemyManager.Instance.ExplosionEffectPool.Get();
        var main = particleSystem.main;         // 拿到副本
        main.simulationSpeed = BattleManager.Instance.GameSpeed.Value;  // 修改副本
        particleSystem.transform.position = transform.position;
        particleSystem.Play();
        TimerUtility.Instance.Timer(0.5f, () => { EnemyManager.Instance.ExplosionEffectPool.Return(particleSystem); });
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

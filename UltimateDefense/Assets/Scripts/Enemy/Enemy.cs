using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening; // 引入 DOTween


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
    private Bindable<int> maxShield = new Bindable<int>();
    //UI 渲染层级顺序，数值越高越靠前
    private int enemyLayer = 0;
    //攻击伤害
    private int _attackDamage = 1;
    //最大弹跳次数
    private int _bounsCount = 10;
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
    /// <summary>
    /// 敌人当前血量
    /// </summary>
    public Bindable<int> CurrentHP { get => _currentHP; }
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
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        // 如果当前速度为0，则不处理（避免除以0）
        if (_rb.velocity.sqrMagnitude > 0.01f)
        {
            // 保持原方向，调整速度大小
            _rb.velocity = _rb.velocity.normalized * 2*BattleManager.Instance.GameSpeed.Value;
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
        _bounsCount = 10;             //最大弹跳次数

        // 根据等级动态计算最大生命值，一级就是一滴血,护盾默认为 0
        maxHP.Value = Mathf.RoundToInt(level * (1 + BattleManager.Instance.Debuff.AddHP * 0.1f));//算上debuff的，增加敌人10%HP
        _currentHP.Value = maxHP.Value;   //初始化血量
        maxShield.Value = 0;            //初始化护盾
        _currentShield = maxShield;             // 初始化当前护盾为最大值
        _currentHP.OnValueChanged += ChangeHP;  //添加值变化事件
        _currentShield.OnValueChanged += ChangeShield;//添加值变化事件
        ChangeShield(_currentHP.Value);         // 初始化护盾条填充
        ChangeHP(_currentShield.Value);         // 初始化血条填充

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
            EnemyUIManager.Instance.UpdateEnemyHealth(transform, _currentHP.Value, maxHP.Value);
            //EnemyUIManager.Instance.UpdateEnemyHp(transform, _currentHP.Value);
            if (_currentHP.Value == 0)
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
    }

    
    #endregion

    #region 辅助方法
    /// <summary>敌人死亡流程：设置标志、触发事件、回收对象</summary>
    private void Die()
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

    //挑战结束
    private void OnEndBattle(bool success)
    {
        EnemyManager.Instance.EnemyPool.Return(this);
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
            collision.transform.parent.GetComponent<BuildingBase>().TakeDamage(_attackDamage);
            
            if (_bounsCount == 1)
            {
                collision.transform.parent.GetComponent<BuildingBase>().TakeDamage(_currentHP.Value*2);
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

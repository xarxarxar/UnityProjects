using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Crystal : MonoBehaviour,IEnemyAttack
{
    private static Crystal _instance;

    [Header("血量设置")]
    [SerializeField] protected Bindable<int> _currentHP = new Bindable<int>();          // 当前生命值
    [SerializeField] protected Bindable<int> _currentShield = new Bindable<int>();          // 当前生命值

    [SerializeField] private int _recoverHpPerSecond = 1;//水晶每秒恢复的生命值
    [SerializeField] private string _name="水晶";

    //水晶无敌
    private bool _isInvincible=false;//是否处于无敌状态
    private Coroutine _invincibleCoro = null;

    //血条和护盾条
    [SerializeField] private Transform _container;//血条和护盾条的父物体
    [SerializeField] private MySlider hpSlider;//血条
    [SerializeField] private MySlider shieldSlider;//护盾条
    [SerializeField] private MySlider inviSlider;//无敌条
    bool shieldSliderShown =false;//护盾条是否显示

    //受伤
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f; // 闪红时间
    private Color originalColor = new Color32(225, 225, 225, 225);
    public Color hitColor = new Color32(255, 102, 51, 255);

    // Q 弹相关
    public float squashAmount = 0.8f;  // 压缩比例
    public float stretchAmount = 1.2f; // 拉伸比例
    public float squashDuration = 0.15f; // Q 弹单程时长
    private Vector3 originalScale=Vector3.one;

    /// <summary>
    /// 水晶被摧毁事件
    /// </summary>
    public static event UnityAction OnCrystalDestroyed;
    /// <summary>
    /// 水晶受到伤害事件
    /// </summary>
    public static event UnityAction<int> OnCrystalDamaged;

    public static Crystal Instance { get => _instance; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    [field: SerializeField]
    public Bindable<int> MaxHP { get; set; }

    /// <summary>
    /// 当前护盾
    /// </summary>
    public Bindable<int> CurrentHP { get => _currentHP; set => _currentHP = value; }

    /// <summary>
    /// 当前护盾
    /// </summary>
    public Bindable<int> CurrentShield { get => _currentShield; set => _currentShield = value; }

    /// <summary>
    /// 最大护盾值
    /// </summary>
    [field: SerializeField]
    public Bindable<int> MaxShield { get; set; }

    /// <summary>
    /// 水晶是否处于无敌状态
    /// </summary>
    public bool IsInvincible { get => _isInvincible;}

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        
        MaxHP.Value = Mathf.RoundToInt((1000+ScienceManager.Instance.GetUpgradeCountByType(ScienceEffectType.CrystalMaxHP)*100) * (1 - BattleManager.Instance.Debuff.CrystalMaxHpDecrease * 0.1f)) ;//初始值应该从配置文件中读取
        Init(MaxHP.Value,0);
    }

    protected void OnDisable()
    {
        _container.gameObject.SetActive(false);
        //杀掉颜色动画
        if (spriteRenderer != null)
        {
            spriteRenderer.DOKill();
            spriteRenderer.color = originalColor;
        }
        transform.DOKill(); // 防止叠加
        transform.localScale = originalScale;
        StopAllCoroutines();
    }

    /// <summary>
    /// 初始化水晶
    /// </summary>
    /// <param name="maxHP">最大生命值</param>
    public void Init(int maxHP,int maxShield)
    {
        Vector3 worldPos = transform.position;   // 2D物体世界坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos); // 转换到屏幕坐标
        _container.position = screenPos;  // UI 直接设置为屏幕坐标
        _container.gameObject.SetActive(true);

        _currentHP.Value = maxHP;
        _currentShield.Value=MaxShield.Value = maxShield;

        OnHpChanged(_currentHP.Value);
        OnShieldChanged(_currentShield.Value);
        shieldSliderShown = false;
        inviSlider.gameObject.SetActive(false);

        _currentHP.OnValueChanged += OnHpChanged;
        MaxHP.OnValueChanged += OnHpChanged;

        _currentShield.OnValueChanged += OnShieldChanged;
        MaxShield.OnValueChanged += OnShieldChanged;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        //if(_recoverCoro != null) _recoverCoro = null;
        //_recoverCoro = StartCoroutine(RecoverIE());//启动水晶每秒回血的协程
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;//无敌状态

        if (_currentHP.Value <= 0)
        {
            return;
        }

        //FlashRed();
        JellySquash();
        AudioManager.Instance.Vibrate("medium");//重震动
        if (_currentShield.Value > 0)
        {
            OnCrystalDamaged?.Invoke(damage);
            _currentShield.Value = Mathf.Max(_currentShield.Value - damage, 0);
            if (_currentShield.Value <= 0)//护盾破碎
            {
                _currentShield.Value = 0;
                MaxShield.Value = 0;
            }
        }
        else
        {
            OnCrystalDamaged?.Invoke(damage);
            _currentHP.Value = Mathf.Max(_currentHP.Value - damage, 0);
            if (_currentHP.Value <= 0)
            {
                _currentHP.Value = 0;
                _container.gameObject.SetActive(false);
                OnCrystalDestroyed?.Invoke();//水晶被摧毁事件
            }
        }
    }

    /// <summary>
    /// 供外部调用，恢复水晶血量
    /// </summary>
    /// <param name="hp"></param>
    public void Recover(int hp)
    {
        // 先加血
        _currentHP.Value += hp;

        // 如果超过最大生命
        if (_currentHP.Value > MaxHP.Value)
        {
            // 血量封顶
            _currentHP.Value = MaxHP.Value;

        }
    }

    /// <summary>
    /// 为水晶添加护盾
    /// </summary>
    public void AddShield(int count)
    {
        if(count<=0) return;
        if(_isInvincible)
        {
            shieldSliderShown=true;
        }
        else
        {
            shieldSliderShown = true;
            shieldSlider.gameObject.SetActive(true);
        }
        if (MaxShield.Value <= 0)
        {
            MaxShield.Value=count;
            _currentShield.Value = 0;
        }
        else
        {
            MaxShield.Value += count;
        }
        _currentShield.Value += count;
    }

    /// <summary>
    /// 设置水晶暂时无敌
    /// </summary>
    /// <param name="duration">无敌时长</param>
    public void SetInvincible(float duration)
    {

        if (_invincibleCoro != null)
        {
            StopCoroutine( _invincibleCoro);
            _invincibleCoro = null;
        }
        _isInvincible = true;
        _invincibleCoro = StartCoroutine(InvincibleCoro(duration));
    }

    private void OnHpChanged(int hp)
    {
        hpSlider.SetValue((float)_currentHP.Value/MaxHP.Value);
        hpSlider.SetText($"{_currentHP.Value}/{MaxHP.Value}");
    }

    private void OnShieldChanged(int shield)
    {
        if (MaxShield.Value <= 0)
        {
            shieldSlider.SetValue(0);
            shieldSlider.gameObject.SetActive(false);
        }
        else
        {
            shieldSlider.SetValue((float)_currentShield.Value / MaxShield.Value);
        }
        
        shieldSlider.SetText($"{_currentShield.Value}/{MaxShield.Value}");
    }

    //无敌的协程
    private IEnumerator InvincibleCoro(float duration)
    {
        shieldSliderShown = shieldSlider.gameObject.activeSelf;
        hpSlider.gameObject.SetActive(false);
        shieldSlider.gameObject.SetActive(false);
        inviSlider.StartCountDown(duration);
        yield return TimerUtility.WaitForGameSeconds(duration);
        hpSlider.gameObject.SetActive(true);
        shieldSlider.gameObject.SetActive(shieldSliderShown);
        _isInvincible =false;
    }

    /// <summary>
    /// 立刻结束无敌状态
    /// </summary>
    public void SetInvincibleOver()
    {
        if (_invincibleCoro != null)
        {
            StopCoroutine(_invincibleCoro);
            _invincibleCoro = null;
        }
        _isInvincible = false;
    }


    /// <summary>
    /// Q 弹果冻效果
    /// </summary>
    public void JellySquash()
    {
        transform.DOKill();
        transform.localScale = originalScale;

        Sequence seq = DOTween.Sequence();
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        // 临时订阅
        void OnSpeedChanged(int speed)
        {
            if (seq != null) seq.timeScale = speed;
        }

        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        seq.Append(transform.DOScaleY(stretchAmount, squashDuration).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScaleY(originalScale.y, squashDuration).SetEase(Ease.OutBounce));

        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }
}

public interface IEnemyAttack
{
    /// <summary>
    /// 承受伤害
    /// </summary>
    public void TakeDamage(int damage);
}

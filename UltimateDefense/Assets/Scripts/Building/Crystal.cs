using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Crystal : BuildingBase
{
    private static Crystal _instance;
    [SerializeField] private int _recoverHpPerSecond = 1;//水晶每秒恢复的生命值
    [SerializeField] private string _name="水晶";
    //private Coroutine _recoverCoro;     //水晶每秒恢复生命值的协程

    //水晶无敌
    private bool _isInvincible=false;//是否处于无敌状态
    private Coroutine _invincibleCoro = null;

    //血条和护盾条
    [SerializeField] private Transform _container;//血条和护盾条的父物体
    [SerializeField] private MySlider hpSlider;//血条
    [SerializeField] private MySlider shieldSlider;//护盾条

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

    public static Crystal Instance { get => _instance; }
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
        if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("CrystalMaxHp"))
        {
            DataManager.Instance.PlayerInfo.Config["CrystalMaxHp"] = 1000;
        }
        MaxHP.Value = Mathf.RoundToInt(DataManager.Instance.PlayerInfo.Config["CrystalMaxHp"]) ;//初始值应该从配置文件中读取
        Init(MaxHP.Value,0);

        
    }

    protected void OnDisable()
    {
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
        _currentShield.Value = maxShield;

        OnHpChanged(_currentHP.Value);
        OnShieldChanged(_currentShield.Value);

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
    public override void TakeDamage(int damage)
    {
        if (_isInvincible) return;//无敌状态

        
        //FlashRed();
        JellySquash();

        if (_currentShield.Value > 0)
        {
            _currentShield.Value = Mathf.Max(_currentShield.Value - damage, 0);
            if (_currentShield.Value <= 0)//护盾破碎
            {
                _currentShield.Value = 0;
                MaxShield.Value = 0;
            }
        }
        else
        {
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
            // 算出多余的部分
            int overflow = _currentHP.Value - MaxHP.Value;

            // 血量封顶
            _currentHP.Value = MaxHP.Value;

            // 把溢出部分转化为护盾
            //AddShield(overflow);
        }
    }

    /// <summary>
    /// 为水晶添加护盾
    /// </summary>
    public void AddShield(int count)
    {
        if(count<=0) return;
        shieldSlider.gameObject.SetActive(true);
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
        Debug.Log("添加护盾");
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
        yield return TimerUtility.WaitForGameSeconds(duration);
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


    //闪红动画
    private void FlashRed()
    {
        // 先杀掉之前的颜色动画，避免受击多次时颜色乱掉
        spriteRenderer.DOKill();

        // 颜色切换到红色，然后回到原色
        spriteRenderer.DOColor(hitColor, flashDuration / BattleManager.Instance.GameSpeed.Value)
            .OnComplete(() => spriteRenderer.DOColor(originalColor, flashDuration / BattleManager.Instance.GameSpeed.Value));
    }

    /// <summary>
    /// Q 弹果冻效果
    /// </summary>
    public void JellySquash()
    {
        transform.DOKill(); // 防止叠加
        transform.localScale = originalScale;

        Sequence seq = DOTween.Sequence();
        // Y 方向压缩拉伸，X 轴保持原始值
        seq.Append(transform.DOScaleY(stretchAmount, squashDuration / BattleManager.Instance.GameSpeed.Value).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScaleY(originalScale.y, squashDuration / BattleManager.Instance.GameSpeed.Value).SetEase(Ease.OutBounce));
    }
    

}

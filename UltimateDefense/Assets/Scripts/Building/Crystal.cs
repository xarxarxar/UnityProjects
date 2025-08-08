using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Crystal : BuildingBase
{
    private static Crystal _instance;
    [SerializeField] private int _recoverHpPerSecond = 1;//水晶每秒恢复的生命值
    [SerializeField] private string _name="水晶";
    private Coroutine _recoverCoro;     //水晶每秒恢复生命值的协程

    //水晶无敌
    private bool _isInvincible=false;//是否处于无敌状态
    private Coroutine _invincibleCoro = null;

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
        Init(MaxHP,null);
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 初始化水晶
    /// </summary>
    /// <param name="maxHP">最大生命值</param>
    public override void Init(int maxHP,Grid grid)
    {
        _currentHP = MaxHP =maxHP;

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

        _currentHP -= damage;
        if (_currentHP < 0) 
        {
            _currentHP = 0;
            if (_recoverCoro != null)
            {
                StopCoroutine(_recoverCoro);
                _recoverCoro = null;
            }
            OnCrystalDestroyed?.Invoke();//水晶被摧毁事件
        } 
    }

    /// <summary>
    /// 供外部调用，恢复水晶血量
    /// </summary>
    /// <param name="hp"></param>
    public void Recover(int hp)
    {
        _currentHP += hp;
        if (_currentHP > MaxHP)
        {
            _currentHP = MaxHP;
        }
    }

    //每秒恢复血量的协程
    //private IEnumerator RecoverIE()
    //{
    //while (RecoverHpPerSecond > 0)
    // {
    //     Recover(RecoverHpPerSecond);
    //     yield return TimerUtility.WaitForGameSeconds(1);//等待一秒
    // }
    //}

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

}

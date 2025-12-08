using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 防御塔的基类
/// </summary>
public abstract class Tower : MonoBehaviour
{
    //定好
    /// <summary>
    /// 子弹数量Text
    /// </summary>
    protected Text _bulletText; // 拖到Inspector里绑定Text组件
    /// <summary>
    /// 换弹现实的slider
    /// </summary>
    protected MySlider _slider;//炮塔所带的slider
    /// <summary>
    /// 炮管物体
    /// </summary>
    protected GameObject _gunBarrel;//炮管
    /// <summary>
    /// 子弹初始化位置
    /// </summary>
    [SerializeField] protected Transform _bulletInitPos;//子弹初始化位置
    /// <summary>
    /// 每秒旋转多少度
    /// </summary>
    protected float rotateSpeed = 200.0f; // 每秒旋转多少度

    public static UnityAction<int> OnCurrentBulletCountChanged;//当前子弹数量发生变化

    //子类只需修改数值
    private int _currentBulletCount;
    public int CurrentBulletCount
    {
        get => _currentBulletCount;
        set
        {
            if (_currentBulletCount != value)
            {
                _currentBulletCount = value;
                _bulletText.text = value.ToString();
                OnCurrentBulletCountChanged?.Invoke(value);
            }
        }
    }

    public TowerData towerData;//炮塔数据
    public DamageEffect CurDamageEffect=>TowerPlatformDataManager.Instance.GetCurrentTowerPlatformData().damageEffect;

    //子类需重写值的逻辑，炮塔最终的值
    [HideInInspector]public int BaseDamage { get {return CalculateAtkDamage();   } } //子弹伤害
    [HideInInspector] public int BulletCap { get {return CalculateBulletCap(); } } //子弹容量
    [HideInInspector] public float AttackRate { get { return CalculateAtkRate(); } }//攻击间隔
    [HideInInspector] public float ReloadTime { get { return CalculateReloadTime();} }//换弹时长
    [HideInInspector] public float CriticalProb { get { return CalculateCriticalProb(); } }//暴击概率
    [HideInInspector] public float CriticalMult { get { return CalculateCriticalMult(); } }//暴击伤害倍率


    //子类不用修改
    protected Enemy currentTarget;//当前的攻击目标
    private float _gameSpeed => BattleManager.Instance.GameSpeed.Value;
    private bool _isPaused=> BattleManager.Instance.GameSpeed.Value==0;//是否暂停
    private bool _isReloading = false; // 是否正在换弹
    private UnityAction _afterReload = null;//换弹结束之后的回调方法
    private float noAttackTimer = 0f;//未处于攻击状态的时长
    private Coroutine _reloadCoroutine;//换弹协程
    private SpriteRenderer _spriteRenderer;

    // Q 弹相关
    private float squashAmount = 0.8f;  // 压缩比例
    private float stretchAmount = 1.2f; // 拉伸比例
    private float squashDuration = 0.15f; // Q 弹单程时长
    private Vector3 originalScale = Vector3.one;


    // 子类必须实现自己的攻击逻辑
    protected abstract IEnumerator DoAttack();


    //下面是函数
    public void OnEnable()
    {
        BattleManager.OnStartBattle += Init;
        BattleManager.OnEndBattle += OnEndBattle;
    }
    private void OnDisable()
    {
        BattleManager.OnEndBattle -= OnEndBattle;
        //炮管归位
        _gunBarrel.transform.localPosition = Vector3.zero;
        _gunBarrel.transform.localRotation = Quaternion.identity;
        _gunBarrel.transform.localScale = Vector3.one;

        _gunBarrel.transform.DOKill(); // 防止叠加
        _gunBarrel.transform.localScale = originalScale;
    }

    /// <summary>
    /// 给予临时子弹
    /// </summary>
    public void GiveTmpBullet(int count,UnityAction callback)
    {
        _afterReload= callback;
        CurrentBulletCount += count;
        if (_isReloading)
        {
            _slider.gameObject.SetActive(false);
            _isReloading = false;
            StopCoroutine(_reloadCoroutine);
        }
    }

    //攻击逻辑
    public IEnumerator Attack()
    {
        PlayAttackAnim();           // 通用逻辑：播放攻击动画
        
        yield return DoAttack();    // 等待子类执行 DoAttack（也改成协程）
    }

    //播放攻击动画
    protected virtual void PlayAttackAnim()
    {
        // 这里可以用 Animator 或特效触发器等
    }

    //初始化
    protected virtual void Init()
    {
        if (_bulletText == null)
        {
            _bulletText=transform.GetComponentInChildren<Text>();
        }
        if(_slider == null)
        {
            _slider=transform.GetComponentInChildren<MySlider>(true);
        }
        if(_gunBarrel == null)
        {
            _gunBarrel = transform.Find("炮管").gameObject;
        }
        if(_spriteRenderer == null)
        {
            _spriteRenderer = transform.Find("炮管").GetComponent<SpriteRenderer>();
        }
        _spriteRenderer.sprite = TowerDataManager.Instance.GetCurrentSkin(towerData).sprite;

        CalculateAtkDamage();
        CalculateBulletCap();
        CalculateAtkRate();
        CalculateReloadTime();
        CalculateCriticalProb();
        CalculateCriticalMult();
        _slider.gameObject.SetActive(false);
        _isReloading = false;

        StartCoroutine(AttackIE());//开始攻击
    }

    //子类可以重新计算自己的数据
    /// <summary>
    /// 计算攻击伤害
    /// </summary>
    protected virtual int CalculateAtkDamage()
    {
        return Mathf.RoundToInt(towerData.BaseDamage * (1f + TowerManager.Instance.BonusAtk.Value));
    }
    /// <summary>
    /// 计算弹夹容量
    /// </summary>
    protected virtual int CalculateBulletCap()
    {
        return towerData.BaseCap + TowerManager.Instance.BonusCap.Value;
    }
    /// <summary>
    /// 计算每秒攻击次数
    /// </summary>
    protected virtual float CalculateAtkRate()
    {
        return towerData.BaseAtkRate * (1f + TowerManager.Instance.BonusAttackRate.Value)*TowerManager.Instance.BonusTmpAttackRate.Value;
    }
    /// <summary>
    /// 计算暴击概率
    /// </summary>
    protected virtual float CalculateCriticalProb()
    {
        return towerData.BaseCritProb + TowerManager.Instance.BonusCritProb.Value;
    }
    /// <summary>
    /// 计算换弹时间
    /// </summary>
    protected virtual float CalculateReloadTime()
    {
        return towerData.BaseReload * (1f - TowerManager.Instance.BonusReload.Value);
    }
    /// <summary>
    /// 计算暴击伤害倍率
    /// </summary>
    protected virtual float CalculateCriticalMult()
    {
        return towerData.BaseCritMult + TowerManager.Instance.BonusCritMult.Value;
    }

    //子类不用重写
    //旋转并射击的协程
    private IEnumerator RotateAndShootIE()
    {
        if (currentTarget == null) yield break;

        Vector3 dir = currentTarget.transform.position - _gunBarrel.transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        while (Quaternion.Angle(_gunBarrel.transform.rotation, targetRotation) > 0.5f)
        {
            if (_isPaused)
            {
                yield return null;
                continue;
            }

            _gunBarrel.transform.rotation = Quaternion.RotateTowards(
                _gunBarrel.transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime * _gameSpeed
            );

            yield return null;
        }

        // === 真正射击 ===
        if (currentTarget != null)
        {
            yield return StartCoroutine(Attack()); // 等待整个攻击流程完成

            noAttackTimer = 0f; // 在这儿重置计时器

            // === 射击后才等待攻击间隔 ===
            //CalculateAtkRate();//更新攻速
            yield return TimerUtility.WaitForGameSeconds((1f / AttackRate));
        }
    }
    //攻击协程
    private IEnumerator AttackIE()
    {
        yield return null;
        CurrentBulletCount = BulletCap;
        while (true)
        {
            if (_isPaused || _isReloading)
            {
                yield return null;
                continue;
            }

            if (CurrentBulletCount <= 0)
            {
                StartReload();
                yield return null;
                continue;
            }

            if (EnemyManager.Instance.CurrentTargerEnemy != null)
            {
                currentTarget = EnemyManager.Instance.CurrentTargerEnemy;
                yield return StartCoroutine(RotateAndShootIE()); // 等待旋转和射击完成
            }
            else
            {
                noAttackTimer += Time.deltaTime * _gameSpeed;

                if (noAttackTimer >= 3f && CurrentBulletCount < BulletCap)
                {
                    StartReload();
                    noAttackTimer = 0f;
                }

                yield return null;
            }

        }
    }
    //换弹
    private void StartReload()
    {
        if (_isReloading || CurrentBulletCount == BulletCap) return; // 防止重复换弹
        _reloadCoroutine = StartCoroutine(ReloadIE());
    }
    //换弹协程
    private IEnumerator ReloadIE()
    {
        _isReloading = true;

        _bulletText.text = "换弹中";
        // 显示倒计时 UI
        CountDownSlider(ReloadTime);

        // 实时等待，受 GameSpeed 影响
        yield return TimerUtility.WaitForGameSeconds(ReloadTime);

        CurrentBulletCount = BulletCap;

        _isReloading = false;
        _afterReload?.Invoke();
        yield break;
    }

    //进度条倒计时
    private void CountDownSlider(float time)
    {
        _slider.StartCountDown(time);//开始
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        EndBattle();
        StopAllCoroutines();
        enabled = false;
    }

    protected virtual void EndBattle()
    {

    }

    /// <summary>
    /// Q 弹果冻效果
    /// </summary>
    public void JellySquash()
    {
        _gunBarrel.transform.DOKill(); // 防止叠加
        _gunBarrel.transform.localScale = originalScale;

        Sequence seq = DOTween.Sequence();

        // 初始化时同步 GameSpeed
        seq.timeScale = BattleManager.Instance.GameSpeed.Value;

        BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        // 临时订阅方法
        void OnSpeedChanged(int speed)
        {
            if (seq != null && seq.IsActive())
                seq.timeScale = speed;
        }
        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;

        // Y 方向压缩拉伸，X 轴保持原始值
        seq.Append(_gunBarrel.transform.DOScaleY(stretchAmount, squashDuration ).SetEase(Ease.OutQuad));
        seq.Append(_gunBarrel.transform.DOScaleY(originalScale.y, squashDuration).SetEase(Ease.OutBounce));

        // 动画结束后解绑 + 回收
        seq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }
}
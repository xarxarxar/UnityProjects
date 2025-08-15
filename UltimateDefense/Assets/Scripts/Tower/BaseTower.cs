using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 防御塔的基类
/// </summary>
public abstract class BaseTower : MonoBehaviour
{
    //定好
    /// <summary>
    /// 子弹数量Text
    /// </summary>
    [SerializeField] protected Text _bulletText; // 拖到Inspector里绑定Text组件
    /// <summary>
    /// 换弹现实的slider
    /// </summary>
    [SerializeField] protected MySlider _slider;//炮塔所带的slider
    /// <summary>
    /// 炮管物体
    /// </summary>
    [SerializeField] protected GameObject _gunBarrel;//炮管
    /// <summary>
    /// 子弹初始化位置
    /// </summary>
    [SerializeField] protected Transform _bulletInitPos;//子弹初始化位置
    /// <summary>
    /// 每秒旋转多少度
    /// </summary>
    protected float rotateSpeed = 200.0f; // 每秒旋转多少度
    /// <summary>
    /// 子弹预制体
    /// </summary>
    [SerializeField] protected Bullet _bullet;//子弹

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
            }
        }
    }
    /// <summary>
    /// 炮塔的等级，暂定10级为最大级
    /// </summary>
    public int TowerLevel { get; set; }

    //子类需重写值的逻辑，炮塔基础值，也就是一级时候的值
    public abstract int BaseDamage { get; }    //子弹伤害
    public abstract int BaseCap { get; }       //子弹容量
    public abstract float BaseAtkRate { get; } //攻击间隔
    public abstract float BaseReload { get; }  //换弹时长
    public abstract float BaseCritProb { get; } //暴击概率
    public abstract float BaseCritMult { get; } //暴击伤害倍率
    //子类需重写值的逻辑，炮塔最终的值
    public Bindable<int> _bulletDamage = new Bindable<int>(); //子弹伤害
    public Bindable<int> BulletDamage { get { CalculateAtkDamage(); return _bulletDamage; } } //子弹伤害
    public Bindable<int> _bulletCapacity = new Bindable<int>(); //子弹容量
    public Bindable<int> BulletCapacity { get { CalculateBulletCap(); return _bulletCapacity; } } //子弹容量
    public Bindable<float> _attackRate = new Bindable<float>();//攻击间隔
    public Bindable<float> AttackRate { get { CalculateAtkRate(); return _attackRate; } }//攻击间隔
    public Bindable<float> _reloadTime = new Bindable<float>();//换弹时长
    public Bindable<float> ReloadTime { get { CalculateReloadTime(); return _reloadTime; } }//换弹时长
    public Bindable<float> _criticalProb = new Bindable<float>();//暴击概率
    public Bindable<float> CriticalProb { get { CalculateCriticalProb(); return _criticalProb; } }//暴击概率
    public Bindable<float> _criticalMult = new Bindable<float>();//暴击伤害倍率
    public Bindable<float> CriticalMult { get { CalculateCriticalMult(); return _criticalMult; } }//暴击伤害倍率


    //子类不用修改
    public TowerData TowerData => TowerDataManager.Instance.GetTowerData(TowerType);
    protected Enemy currentTarget;//当前的攻击目标
    private List<Enemy> _enemiesInRange => EnemyManager.Instance.EnemiesInRange;//在攻击范围内的所有敌人
    private float _gameSpeed => BattleManager.Instance.GameSpeed.Value;
    private bool _isPaused=> BattleManager.Instance.IsPaused.Value;//是否暂停
    private bool _isRotating = false; // 是否正在旋转
    private bool _isReloading = false; // 是否正在换弹
    private float noAttackTimer = 0f;//未处于攻击状态的时长
    private Coroutine _reloadCoroutine;//换弹协程

    // Q 弹相关
    public float squashAmount = 0.8f;  // 压缩比例
    public float stretchAmount = 1.2f; // 拉伸比例
    public float squashDuration = 0.15f; // Q 弹单程时长
    private Vector3 originalScale = Vector3.one;

    //子类必须写好
    /// <summary>
    /// 该防御塔的类型
    /// </summary>
    public abstract TowerType TowerType { get;}
    // 子类必须实现自己的攻击逻辑
    protected abstract IEnumerator DoAttack();


    //下面是函数
    public void OnEnable()
    {
        Init();
        //InitTower();
        StartCoroutine(AttackIE());//开始攻击
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
        CalculateAtkDamage();
        CalculateBulletCap();
        CalculateAtkRate();
        CalculateReloadTime();
        CalculateCriticalProb();
        CalculateCriticalMult();
    }

    
    //子类可以重新计算自己的数据
    /// <summary>
    /// 计算攻击伤害
    /// </summary>
    protected virtual void CalculateAtkDamage()
    {
        _bulletDamage.Value = Mathf.RoundToInt(BaseDamage * (1f + TowerManager.Instance.BonusAtk.Value));
    }
    /// <summary>
    /// 计算弹夹容量
    /// </summary>
    protected virtual void CalculateBulletCap()
    {
        _bulletCapacity.Value = BaseCap + TowerManager.Instance.BonusCap.Value;
    }
    /// <summary>
    /// 计算每秒攻击次数
    /// </summary>
    protected virtual void CalculateAtkRate()
    {
        _attackRate.Value = BaseAtkRate * (1f + TowerManager.Instance.BonusAttackRate.Value)*TowerManager.Instance.BonusTmpAttackRate.Value;
    }
    /// <summary>
    /// 计算暴击概率
    /// </summary>
    protected virtual void CalculateCriticalProb()
    {
        _criticalProb.Value = BaseCritProb + TowerManager.Instance.BonusCritProb.Value;
    }
    /// <summary>
    /// 计算换弹时间
    /// </summary>
    protected virtual void CalculateReloadTime()
    {
        _reloadTime.Value = BaseReload * (1f - TowerManager.Instance.BonusReload.Value);
    }
    /// <summary>
    /// 计算暴击伤害倍率
    /// </summary>
    protected virtual void CalculateCriticalMult()
    {
        _criticalMult.Value = BaseCritMult + TowerManager.Instance.BonusCritMult.Value;
    }
    /// <summary>
    /// 获取炮塔的描述
    /// </summary>
    /// <returns></returns>
    public abstract string GetDescription();

    /// <summary>
    /// 获取升级的属性的描述
    /// </summary>
    /// <returns></returns>
    public abstract string GetUpgradeDescription();

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
            yield return TimerUtility.WaitForGameSeconds((1f / AttackRate.Value));
        }
    }
    //攻击协程
    private IEnumerator AttackIE()
    {
        yield return null;
        CurrentBulletCount = BulletCapacity.Value;

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

            if (_enemiesInRange.Count != 0)
            {
                currentTarget = _enemiesInRange[0];
                yield return StartCoroutine(RotateAndShootIE()); // 等待旋转和射击完成
            }
            else
            {
                noAttackTimer += Time.deltaTime * _gameSpeed;

                if (noAttackTimer >= 3f && CurrentBulletCount < BulletCapacity.Value)
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
        if (_isReloading || CurrentBulletCount == BulletCapacity.Value) return; // 防止重复换弹
        _reloadCoroutine = StartCoroutine(ReloadIE());
    }
    //换弹协程
    private IEnumerator ReloadIE()
    {
        _isReloading = true;

        //更新等待时间
        //CalculateReloadTime();
        Debug.Log($"换弹时长为{ReloadTime.Value}");
        _bulletText.text = "换弹中";
        // 显示倒计时 UI
        CountDownSlider(ReloadTime.Value);

        // 实时等待，受 GameSpeed 影响
        yield return TimerUtility.WaitForGameSeconds(ReloadTime.Value);

        CurrentBulletCount = BulletCapacity.Value;

        _isReloading = false;
    }

    //进度条倒计时
    private void CountDownSlider(float time)
    {
        _slider.StartCountDown(time);//开始
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        Debug.Log("挑战结束");
        StopAllCoroutines();
    }

    /// <summary>
    /// Q 弹果冻效果
    /// </summary>
    public void JellySquash()
    {
        _gunBarrel.transform.DOKill(); // 防止叠加
        _gunBarrel.transform.localScale = originalScale;

        Sequence seq = DOTween.Sequence();
        // Y 方向压缩拉伸，X 轴保持原始值
        seq.Append(_gunBarrel.transform.DOScaleY(stretchAmount, squashDuration /BattleManager.Instance.GameSpeed.Value).SetEase(Ease.OutQuad));
        seq.Append(_gunBarrel.transform.DOScaleY(originalScale.y, squashDuration / BattleManager.Instance.GameSpeed.Value).SetEase(Ease.OutBounce));
    }
}
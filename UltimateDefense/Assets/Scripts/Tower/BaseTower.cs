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
    [SerializeField] protected float rotateSpeed = 360f; // 每秒旋转多少度
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

    //子类可重写逻辑
    public virtual int BulletDamage { get => TowerManager.Instance.BaseAtk.Value; } //子弹伤害
    public virtual int BulletCapacity { get => TowerManager.Instance.BaseCap.Value; } //子弹容量
    public virtual float AttackSpeed { get => TowerManager.Instance.BaseSpeed.Value; }//攻击间隔
    public virtual float ReloadTime {get => TowerManager.Instance.BaseReload.Value; }//换弹时长
    public virtual float CriticalProb { get => TowerManager.Instance.BaseCritProb.Value; }//暴击概率
    public virtual float CriticalMult { get => TowerManager.Instance.BaseCritMult.Value; }//暴击伤害倍率

    //子类不用修改
    protected Enemy currentTarget;//当前的攻击目标
    private List<Enemy> _enemiesInRange => EnemyManager.Instance.EnemiesInRange;//在攻击范围内的所有敌人
    private float _gameSpeed => BattleManager.Instance.GameSpeed;
    private bool _isPaused=> BattleManager.Instance.IsPaused;//是否暂停
    private bool _isRotating = false; // 是否正在旋转
    private bool _isReloading = false; // 是否正在换弹
    private float noAttackTimer = 0f;//未处于攻击状态的时长
    private Coroutine _reloadCoroutine;//换弹协程

    public void OnEnable()
    {
        Init();
        StartCoroutine(AttackIE());//开始攻击
        BattleManager.OnEndBattle += OnEndBattle;
    }
    private void OnDisable()
    {
        BattleManager.OnEndBattle -= OnEndBattle;
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
        Debug.Log("播放通用攻击动画");
        // 这里可以用 Animator 或特效触发器等
    }

    //初始化
    protected virtual void Init()
    {
        
    }

    // 子类必须实现自己的攻击逻辑
    protected abstract IEnumerator DoAttack();


    //子类不用重写
    //旋转并设计的协程
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
            yield return TimerUtility.WaitForGameSeconds((1f / AttackSpeed));
        }
    }
    //攻击协程
    private IEnumerator AttackIE()
    {
        yield return null;
        CurrentBulletCount = BulletCapacity;

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

                if (noAttackTimer >= 3f && CurrentBulletCount < BulletCapacity)
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
        if (_isReloading || CurrentBulletCount == BulletCapacity) return; // 防止重复换弹
        _reloadCoroutine = StartCoroutine(ReloadIE());
    }
    //换弹协程
    private IEnumerator ReloadIE()
    {
        _isReloading = true;

        // 显示倒计时 UI
        CountDownSlider(ReloadTime);

        // 实时等待，受 GameSpeed 影响
        yield return TimerUtility.WaitForGameSeconds(ReloadTime);

        CurrentBulletCount = BulletCapacity;

        _isReloading = false;
    }

    //进度条倒计时
    private void CountDownSlider(float time)
    {
        _slider.Init(time);//开始
    }

    //挑战结束
    private void OnEndBattle(bool success)
    {
        StopAllCoroutines();
    }
}

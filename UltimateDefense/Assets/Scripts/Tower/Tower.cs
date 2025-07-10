using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 表示一座塔的行为，如攻击、范围、升级等。
/// </summary>
public class Tower : MonoBehaviour
{
    #region 配置参数
    [Header("基本属性")]
    //public float baseFireRate = 1f;        // 攻击频率（每秒几次）
    //public float criticalProb = 0.1f;       //初始暴击概率
    //public int baseDamage = 1;            // 初始单次攻击伤害
    [SerializeField]private int _currentBulletCount;        //当前子弹数量

    [Header("升级属性")]
    public int level = 1;                  // 当前等级
    //public int upgradeCost = 50;           // 升级花费金币

    //总属性
    public int totalDamage => Mathf.RoundToInt(TowerManager.Instance.GlobalAttackBonus.Value);
    public float totalAttackRate => TowerManager.Instance.GlobalAttackSpeedMultiplier.Value;
    public float totalCriticalProb =>TowerManager.Instance.GlobalCriticalShotProb.Value;
    public float ReloadTime=> TowerManager.Instance.GlobalReloadTime.Value;
    public int BulletCapacity=> TowerManager.Instance.GlobalIncreaseBulletCap.Value;
    public float totalCriticalMultiplier=>TowerManager.Instance.GlobalCriticalMultiplier.Value;//暴击伤害倍率
    #endregion

    #region 私有字段
    //private float reloadTime = 3.0f;//装弹时间
    //private int bulletCapacity = 10;//子弹容量
    private Enemy currentTarget;//当前的攻击目标
    private bool _isPaused=>BattleManager.Instance.IsPaused;//是否暂停
    private List<Enemy> _enemiesInRange=>EnemyManager.Instance.EnemiesInRange;//在攻击范围内的所有敌人
    private float _gameSpeed=>BattleManager.Instance.GameSpeed;
    [SerializeField]private MySlider _slider;//炮塔所带的slider
    [SerializeField] private Text _bulletCountText;//子弹数量的Text
    [SerializeField] private GameObject _gunBarrel;//炮管
    [SerializeField] private Transform _bulletInitPos;//子弹初始化位置
    [SerializeField] private float rotateSpeed = 360f; // 每秒旋转多少度
    private bool _isRotating = false; // 是否正在旋转
    private bool _isReloading = false; // 是否正在换弹
    private float noAttackTimer = 0f;//未处于攻击状态的时长
    private Coroutine _reloadCoroutine;//换弹协程


    public int CurrentBulletCount 
    { 
        get => _currentBulletCount; 
        set
        {
            _currentBulletCount=value;
            _bulletCountText.text=value.ToString();
        }
    }

    [SerializeField]private Bullet _bullet;//子弹
    #endregion

    #region Unity 生命周期
    private void OnEnable()
    {
        StartCoroutine(AttackIE());//开始攻击
        BattleManager.OnEndBattle += OnEndBattle;
    }
    private void OnDisable()
    {
        BattleManager.OnEndBattle -= OnEndBattle;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }

    #endregion

    #region 公共方法
    /// <summary>
    /// 摧毁该防御塔
    /// </summary>
    public void Destroy()
    {

    }

    #endregion

    #region 私有方法
    private void Attack(Enemy target)
    {
        if (target != null)
        {
            StartCoroutine(RotateAndShootIE(target));
        }
    }

    private IEnumerator RotateAndShootIE(Enemy target)
    {
        if (target == null) yield break;

        Vector3 dir = target.transform.position - _gunBarrel.transform.position;
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
        if (target != null)
        {
            Bullet bullet = TowerManager.Instance.BulletPool.Get();

            float value = Random.value;
            if (value < totalCriticalProb)
            {
                bullet.Init(_bulletInitPos.position, target, true, Mathf.RoundToInt(totalDamage * totalCriticalMultiplier));
            }
            else
            {
                bullet.Init(_bulletInitPos.position, target, false, totalDamage);
            }

            CurrentBulletCount--;
            noAttackTimer = 0f; // 在这儿重置计时器

            // === 射击后才等待攻击间隔 ===
            yield return TimerUtility.WaitForGameSeconds((1f / totalAttackRate));
        }
    }

    //private void Attack(Enemy target)
    //{
    //    if (target != null)
    //    {
    //        // === 炮管旋转（修正 +90 度）===
    //        Vector3 dir = target.transform.position - _gunBarrel.transform.position;
    //        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //        _gunBarrel.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);


    //        Bullet bullet= TowerManager.Instance.BulletPool.Get();//从对象池拿取

    //        float value = Random.value;

    //        if (value < totalCriticalProb)//暴击
    //        {
    //            bullet.Init(_bulletInitPos.position, target,true, Mathf.RoundToInt(totalDamage*TowerManager.Instance.GlobalCriticalMultiplier.Value));
    //        }
    //        else
    //        {
    //            bullet.Init(_bulletInitPos.position, target, false, totalDamage);
    //        }
    //    }
    //    CurrentBulletCount--;
    //}

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
                yield return StartCoroutine(RotateAndShootIE(currentTarget)); // 等待旋转和射击完成
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

    //挑战结束
    private void OnEndBattle(bool success)
    {
        StopAllCoroutines();
    }

    private void CountDownSlider(float time)
    {
        _slider.Init(time);//开始
    }

   

    //换弹
    private void StartReload()
    {
        if (_isReloading || CurrentBulletCount== BulletCapacity) return; // 防止重复换弹
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
    #endregion
}

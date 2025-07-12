using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理所有炮塔的生成、销毁与全局属性加成
/// </summary>
public class TowerManager : ManagerBase<TowerManager>,IManager
{
    #region 私有属性
    //炮塔全局基础属性
    private Bindable<int> _baseAtk = new Bindable<int>();         //炮塔基础攻击
    private Bindable<int> _baseCap = new Bindable<int>();         //炮塔基础弹夹容量
    private Bindable<float> _baseSpeed = new Bindable<float>();   //炮塔基础射速
    private Bindable<float> _baseCritProb = new Bindable<float>();//炮塔基础暴击概率
    private Bindable<float> _baseCritMult = new Bindable<float>();//炮塔基础暴击伤害倍率
    private Bindable<float> _baseReload = new Bindable<float>();  //炮塔基础换弹时长

    //全局加成
    private Bindable<int> _bonusAtk = new Bindable<int>();         // 全局攻击力加成值
    private Bindable<int> _bonusCap = new Bindable<int>();         //全局弹夹容量加成
    private Bindable<float> _bonusSpd = new Bindable<float>();     // 全局射速加成倍率
    private Bindable<float> _bonusCritProb = new Bindable<float>();//全局暴击概率加成
    private Bindable<float> _bonusCritMult = new Bindable<float>();//全局暴击伤害倍数加成
    private Bindable<float> _bonusReload = new Bindable<float>();  //全局换弹时长

    

    private bool _isInitialized;                      // 标记是否已初始化
    private TowerFactory _towerFactory;               // 引用 TowerFactory 单例，用于创建新塔
    [SerializeField] public Bullet _bulletPrefab;     //子弹预制体
    private ObjectPool<Bullet> _bulletPool;           //子弹对象池
    private List<Tower> _allTowers = new List<Tower>();// 场上所有活着的塔实例列表
    private TowerType _towerType;//当前炮塔的形态
    #endregion

    #region 公开属性
    /// <summary>
    /// 炮塔基础攻击
    /// </summary>
    public Bindable<int> BaseAtk => _baseAtk;

    /// <summary>
    /// 炮塔基础弹夹容量
    /// </summary>
    public Bindable<int> BaseCap => _baseCap;

    /// <summary>
    /// 炮塔基础射速
    /// </summary>
    public Bindable<float> BaseSpeed => _baseSpeed;

    /// <summary>
    /// 炮塔基础暴击概率
    /// </summary>
    public Bindable<float> BaseCritProb => _baseCritProb;

    /// <summary>
    /// 炮塔基础暴击伤害倍率
    /// </summary>
    public Bindable<float> BaseCritMult => _baseCritMult;

    /// <summary>
    /// 炮塔基础换弹时长
    /// </summary>
    public Bindable<float> BaseReload => _baseReload;

    /// <summary>
    /// 全局攻击力加成值
    /// </summary>
    public Bindable<int> GlobalAtkAdd => _bonusAtk;

    //全局属性加成
    /// <summary>
    /// 全局弹夹容量加成
    /// </summary>
    public Bindable<int> GlobalCapAdd => _bonusCap;

    /// <summary>
    /// 全局射速加成倍率
    /// </summary>
    public Bindable<float> GlobalSpdMul => _bonusSpd;

    /// <summary>
    /// 全局暴击概率加成
    /// </summary>
    public Bindable<float> GlobalCritProb => _bonusCritProb;

    /// <summary>
    /// 全局暴击伤害倍数加成
    /// </summary>
    public Bindable<float> GlobalCritMul => _bonusCritMult;

    /// <summary>
    /// 全局换弹时长
    /// </summary>
    public Bindable<float> GlobalReload => _bonusReload;

    /// <summary>
    /// 只读属性，返回场上所有塔的只读列表
    /// </summary>
    public IReadOnlyList<Tower> AllTowers { get { return _allTowers; } }

    /// <summary>
    /// 全局攻击力加成属性，可在外部设置
    /// </summary>
    public Bindable<int> GlobalAttackBonus 
    { get 
        { return _bonusAtk; }
        set 
        {
            _bonusAtk = value; 
        } 
    }

    /// <summary>
    /// 全局攻击速度倍率属性，可在外部设置
    /// </summary>
    public Bindable<float> GlobalAttackSpeedMultiplier { get { return _bonusSpd; } set { _bonusSpd = value; } }

    /// <summary>
    /// 全局暴击概率加成，供外部调用
    /// </summary>
    public Bindable<float> GlobalCriticalShotProb { get => _bonusCritProb; set => _bonusCritProb = value; }
    /// <summary>
    /// 全局暴击倍数加成，供外部调用
    /// </summary>
    public Bindable<float> GlobalCriticalMultiplier { get => _bonusCritMult; set => _bonusCritMult = value; }
    /// <summary>
    /// 子弹对象池，供外部调用
    /// </summary>
    public ObjectPool<Bullet> BulletPool { get => _bulletPool; }

    /// <summary>
    /// 全局换弹时长加成，供外部调用
    /// </summary>
    public Bindable<float> GlobalReloadTime { get => _bonusReload; set => _bonusReload = value; }
    /// <summary>
    /// 全局弹夹容量加成，供外部调用
    /// </summary>
    public Bindable<int> GlobalIncreaseBulletCap { get => _bonusCap; set => _bonusCap = value; }
    /// <summary>
    /// 当前炮塔的形态
    /// </summary>
    public TowerType TowerType { get => _towerType; set => _towerType = value; }

    #endregion

    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void Init()
    {
        //基础
        _baseAtk.Value = 10;
        _baseCap.Value = 10;
        _baseSpeed.Value = 1f;
        _baseCritProb.Value = 0.0f;
        _baseCritMult.Value = 2.0f;
        _baseReload.Value = 3.0f;


        _bonusAtk.Value = 10;
        _bonusSpd.Value = 1;
        _bonusCritProb.Value = 0;
        _bonusCritMult.Value = 1.5f;
        _bonusReload.Value = 3;
        _bonusCap.Value = 10;
        if (_bulletPool == null) _bulletPool = new ObjectPool<Bullet>(_bulletPrefab, 10, transform);
    }

    /// <summary>
    /// 在指定位置生成一座指定类型的塔
    /// 从 TowerFactory 获取预制体并 Instantiate，然后添加到 _allTowers 列表
    /// 自动应用全局加成（BaseAttack、AttackSpeed 等）
    /// </summary>
    /// <param name="type">塔类型枚举</param>
    /// <param name="position">生成位置（世界坐标）</param>
    public void SpawnTower(TowerType type, Vector3 position)
    {
        // Tower tower = _towerFactory.CreateTower(type, position);
        // tower.BaseAttack += _bonusAtk;
        // tower.AttackSpeed *= _bonusSpd;
        // tower.UpdateStats();
        // _allTowers.Add(tower);
    }

    /// <summary>
    /// 销毁指定的塔实例
    /// 从 _allTowers 列表移除并销毁其 GameObject
    /// </summary>
    /// <param name="tower">要销毁的塔实例</param>
    public void DestroyTower(Tower tower)
    {

    }

    /// <summary>
    /// 对所有塔应用一次全局攻击力加成（例如升级时调用）
    /// 遍历 _allTowers，将每座塔的 BaseAttack += bonus，并 UpdateStats
    /// </summary>
    /// <param name="bonus">要增加的攻击力值</param>
    public void ApplyGlobalAttackBonus(int bonus)
    {
        _bonusAtk.Value += bonus;
    }
    #endregion

    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// 初始化 _allTowers 列表，并获取 TowerFactory 单例
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        // _allTowers = new List<Tower>();
        // _towerFactory = TowerFactory.Instance;
        // _isInitialized = false;
    }


    /// <summary>
    /// 取消订阅 UpgradeManager.OnUpgradePurchased
    /// </summary>
    private void OnDisable()
    {
        
    }

    /// <summary>
    /// 当有升级被购买时的回调，根据升级类型决定是否调用 ApplyGlobalAttackBonus 等方法
    /// </summary>
    /// <param name="upgrade">购买的升级实例</param>
    private void HandleUpgradePurchased(UpgradeBase upgrade)
    {
        // if (upgrade is Upgrade_IncreaseTowerAttack atkUp)
        // {
        //     ApplyGlobalAttackBonus(atkUp.AttackBonus);
        // }
        // else if (upgrade is Upgrade_SlowEnemy slowUp)
        // {
        //     foreach (var t in _allTowers)
        //         t.AttackSpeed *= (1f + slowUp.SlowPercent);
        // }
        // // 根据需要处理其他升级类型
    }

    /// <summary>
    /// Unity OnDestroy 回调，取消所有订阅并清理列表
    /// </summary>
    private void OnDestroy()
    {
        // UpgradeManager.Instance.OnUpgradePurchased -= HandleUpgradePurchased;
        // _allTowers.Clear();
    }
    #endregion
}

/// <summary>
/// 塔的形态类型
/// </summary>
public enum TowerType
{
    Basic,          // 基础形态：普通单发塔
    RapidFire,      // 连发形态：每次连续发射两颗子弹
    Ricochet,       // 弹射形态：子弹击中后弹射至另一个敌人
    Spread,         // 散射形态：每次扇形发射三颗子弹
    Sniper,         // 狙击形态：攻击间隔更长但伤害更高
    Piercing        // 穿透形态：子弹穿透敌人造成伤害
}

/// <summary>
/// 所有类型的塔都要实现的接口
/// </summary>
public interface ITowerAttack
{
    void Init(Tower tower);          // 初始化绑定主塔体
    void Attack();                   // 执行攻击逻辑
    bool CanAttack();                // 是否能攻击
}
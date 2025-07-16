using System;
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
    private Bindable<float> _baseAtkIntv = new Bindable<float>();   //炮塔基础攻击间隔
    private Bindable<float> _baseCritProb = new Bindable<float>();//炮塔基础暴击概率
    private Bindable<float> _baseCritMult = new Bindable<float>();//炮塔基础暴击伤害倍率
    private Bindable<float> _baseReload = new Bindable<float>();  //炮塔基础换弹时长

    //全局加成
    private Bindable<float> _bonusAtk = new Bindable<float>();         // 全局攻击力加成值
    private Bindable<int> _bonusCap = new Bindable<int>();         //全局弹夹容量加成
    private Bindable<float> _bonusAtkRate = new Bindable<float>();     // 全局射速加成倍率
    private Bindable<float> _bonusCritProb = new Bindable<float>();//全局暴击概率加成
    private Bindable<float> _bonusCritMult = new Bindable<float>();//全局暴击伤害倍数加成
    private Bindable<float> _bonusReload = new Bindable<float>();  //全局换弹时长

    private bool _isInitialized;                      // 标记是否已初始化
    private TowerFactory _towerFactory;               // 引用 TowerFactory 单例，用于创建新塔
    [SerializeField] public Bullet _bulletPrefab;     //子弹预制体
    private ObjectPool<Bullet> _bulletPool;           //子弹对象池
    private TowerType _towerType;//当前炮塔的形态
    [SerializeField] private GameObject _towerObject;//炮塔物体
    [SerializeField] private BaseTower _currentTower;//当前的炮塔
    #endregion

    #region 公开属性
    ///// <summary>
    ///// 炮塔基础攻击
    ///// </summary>
    //public Bindable<int> BaseAtk => _baseAtk;
    ///// <summary>
    ///// 炮塔基础弹夹容量
    ///// </summary>
    //public Bindable<int> BaseCap => _baseCap;
    ///// <summary>
    ///// 炮塔基础射速
    ///// </summary>
    //public Bindable<float> BaseAtkIntv => _baseAtkIntv;
    ///// <summary>
    ///// 炮塔基础暴击概率
    ///// </summary>
    //public Bindable<float> BaseCritProb => _baseCritProb;
    ///// <summary>
    ///// 炮塔基础暴击伤害倍率
    ///// </summary>
    //public Bindable<float> BaseCritMult => _baseCritMult;
    ///// <summary>
    ///// 炮塔基础换弹时长
    ///// </summary>
    //public Bindable<float> BaseReload => _baseReload;

    //全局属性加成
    /// <summary>
    /// 全局攻击力加成值
    /// </summary>
    public Bindable<float> BonusAtk => _bonusAtk;
    /// <summary>
    /// 全局弹夹容量加成
    /// </summary>
    public Bindable<int> BonusCap => _bonusCap;
    /// <summary>
    /// 每秒攻击次数加成
    /// </summary>
    public Bindable<float> BonusAttackRate => _bonusAtkRate;
    /// <summary>
    /// 全局暴击概率加成
    /// </summary>
    public Bindable<float> BonusCritProb => _bonusCritProb;
    /// <summary>
    /// 全局暴击伤害倍数加成
    /// </summary>
    public Bindable<float> BonusCritMult => _bonusCritMult;
    /// <summary>
    /// 全局换弹时长
    /// </summary>
    public Bindable<float> BonusReload => _bonusReload;

    /// <summary>
    /// 子弹对象池，供外部调用
    /// </summary>
    public ObjectPool<Bullet> BulletPool { get => _bulletPool; }
    /// <summary>
    /// 当前炮塔的形态
    /// </summary>
    public TowerType TowerType { get => _towerType; set => _towerType = value; }
    /// <summary>
    /// 当前的炮塔
    /// </summary>
    public BaseTower CurrentTower { get => _currentTower; }

    #endregion

    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void Init()
    {
        Debug.Log($"TowerManager初始化");
        //基础
        _baseAtk.Value = TowerDataManager.Instance.GetTowerData(TowerType.Basic).Level;//基础伤害就是基础塔的伤害
        _baseCap.Value = 10;
        _baseAtkIntv.Value = 1f;
        _baseCritProb.Value = 0.0f;
        _baseCritMult.Value = 2.0f;
        _baseReload.Value = 3.0f;

        //加成
        _bonusAtk.Value = 0;
        _bonusAtkRate.Value = 0;
        _bonusCritProb.Value = 0;
        _bonusCritMult.Value = 1.5f;
        _bonusReload.Value = 0;
        _bonusCap.Value = 10;
        if (_bulletPool == null) _bulletPool = new ObjectPool<Bullet>(_bulletPrefab, 10, transform);

        //启用对应炮塔的脚本
        TowerType currentType = TowerDataManager.Instance.CurrentTowerType;
        // 获取所有继承自 BaseTower 的脚本（即便禁用了也能拿到）
        BaseTower[] allTowerScripts = _towerObject.GetComponents<BaseTower>();
        foreach (BaseTower script in allTowerScripts)
        {
            // 判断是否和当前选中的类型匹配
            if (script.TowerType== currentType)
            {
                script.enabled = true;  // 启用对应脚本
                _currentTower=script;
            }
            else
            {
                script.enabled = false; // 禁用其他
            }
        }
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
    Basic,    // 基础形态：普通单发塔
    RapidFire,// 连发形态：每次连续发射两颗子弹
    Ricochet, // 弹射形态：子弹击中后弹射至另一个敌人
    Spread,   // 散射形态：每次扇形发射三颗子弹
    Sniper,   // 狙击形态：攻击间隔更长但伤害更高
    Piercing  // 穿透形态：子弹穿透敌人造成伤害
}
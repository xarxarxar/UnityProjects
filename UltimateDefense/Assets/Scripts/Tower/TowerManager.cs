using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理所有炮塔的生成、销毁与全局属性加成
/// </summary>
public class TowerManager : ManagerBase<TowerManager>   
{
    #region 私有属性

    //全局加成
    private Bindable<float> _bonusAtk = new Bindable<float>();         // 全局攻击力加成值
    private Bindable<int> _bonusCap = new Bindable<int>();         //全局弹夹容量加成
    private Bindable<float> _bonusAtkRate = new Bindable<float>();     // 全局射速加成倍率
    private Bindable<float> _bonusCritProb = new Bindable<float>();//全局暴击概率加成
    private Bindable<float> _bonusCritMult = new Bindable<float>();//全局暴击伤害倍数加成
    private Bindable<float> _bonusReload = new Bindable<float>();  //全局换弹时长

    //临时
    private Coroutine _tmpAtkRateCoro = null;
    private Bindable<float> _bonusTmpAtkRate = new Bindable<float>();//临时的攻击力加成，供狂暴模式使用

    //总的
    private Bindable<float> _totalAttackRate = new Bindable<float>();//总攻速

    private TowerFactory _towerFactory;               // 引用 TowerFactory 单例，用于创建新塔
    [SerializeField] public Bullet _bulletPrefab;     //子弹预制体
    private ObjectPool<Bullet> _bulletPool;           //子弹对象池
    private TowerType _towerType;//当前炮塔的形态
    [SerializeField] private GameObject _towerObject;//炮塔物体
    [SerializeField] private BaseTower _currentTower;//当前的炮塔
    #endregion

    #region 公开属性

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

    //临时
    /// <summary>
    /// 每秒攻击次数加成，临时
    /// </summary>
    public Bindable<float> BonusTmpAttackRate => _bonusTmpAtkRate;

    //总计
    /// <summary>
    /// 每秒攻击次数，总的
    /// </summary>
    public Bindable<float> TotalAttackRate => _totalAttackRate;

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

    /// <summary>
    /// 当前子弹的形态
    /// </summary>
    public  BulletKind CurrentBulletKind;
    public int BulletKindCount;//特殊子弹的个数

    #endregion

    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void Init()
    {
        //临时
        _bonusTmpAtkRate.Value = 1.0f;

        //加成
        _bonusAtk.Value = 0;
        _bonusAtkRate.Value = 0;
        _bonusCritProb.Value = 0;
        _bonusCritMult.Value = 0f;
        _bonusReload.Value = 0;
        _bonusCap.Value = 0;
        BulletKindCount = 20;
        CurrentBulletKind = BulletKind.Normal;

        if (_bulletPool == null) _bulletPool = new ObjectPool<Bullet>(_bulletPrefab, 10, transform);

        //启用对应炮塔的脚本
        TowerType currentType = DataManager.Instance.PlayerInfo.CurrentTowerType.Value;
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
    /// 设置临时攻速，持续一段时间
    /// </summary>
    /// <param name="value">临时攻速的倍数</param>
    /// <param name="duration">持续的时间</param>
    public void SetTmpAtkRate(float value,float duration=0)
    {
        _bonusTmpAtkRate.Value = value;
        if (duration != 0)
        {
            if (_tmpAtkRateCoro != null)
            {
                StopCoroutine(_tmpAtkRateCoro);
                _tmpAtkRateCoro = null;
            }
            StartCoroutine(SetTmpAtkRateCoro(duration));
        }

    }
    private IEnumerator SetTmpAtkRateCoro(float duration)
    {
        yield return TimerUtility.WaitForGameSeconds(duration);
        _bonusTmpAtkRate.Value = 1.0f;
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

    /// <summary>
    /// 临时改变子弹的形态
    /// </summary>
    /// <param name="bulletKind">子弹的形态，干冰弹，火焰弹，麻痹弹</param>
    /// <param name="duration">持续的子弹个数</param>
    public void ChangeBullet(BulletKind bulletKind)
    {
        CurrentBulletKind=bulletKind;
        int tmpCount = 0;
        int lastBulletCount=CurrentTower.CurrentBulletCount;
        BaseTower.OnCurrentBulletCountChanged -= OnCurrentBulletCountChanged;
        BaseTower.OnCurrentBulletCountChanged += OnCurrentBulletCountChanged;

        void OnCurrentBulletCountChanged(int value)
        {
            if(value< lastBulletCount)//这才是子弹减少了
            {
                Debug.Log("子弹减少了");
                tmpCount += (lastBulletCount - value);//子弹减少了这么多
                if (tmpCount >= BulletKindCount)
                {
                    CurrentBulletKind = BulletKind.Normal;
                    BaseTower.OnCurrentBulletCountChanged -= OnCurrentBulletCountChanged;
                }
            }
            lastBulletCount = value;
        }
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
        Index = 2;
    }



    #endregion
}

/// <summary>
/// 塔的形态类型
/// </summary>
public enum TowerType
{
    /// <summary>
    /// 基础形态：普通单发塔
    /// </summary>
    Basic,    // 
    /// <summary>
    /// 连发形态：每次连续发射两颗子弹
    /// </summary>
    RapidFire,// 
    /// <summary>
    /// 弹射形态：子弹击中后弹射至另一个敌人
    /// </summary>
    Ricochet, // 
    /// <summary>
    /// 散射形态：每次扇形发射三颗子弹
    /// </summary>
    Spread,   // 
    /// <summary>
    /// 狙击形态：攻击间隔更长但伤害更高
    /// </summary>
    Sniper,   // 
    /// <summary>
    /// 穿透形态：子弹穿透敌人造成伤害
    /// </summary>
    Piercing  // 
}
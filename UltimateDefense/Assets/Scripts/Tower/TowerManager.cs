using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理所有炮塔的生成、销毁与全局属性加成
/// </summary>
public class TowerManager : ManagerBase<TowerManager>,IManager
{
    #region 私有属性
    private List<Tower> _allTowers=new List<Tower>();// 场上所有活着的塔实例列表
    private float _globalAttackBonus=0;                 // 全局攻击力加成值
    private float _globalAttackSpeedMultiplier = 0.0f;  // 全局射速加成倍率
    private float _globalCriticalShotProb = 0.0f;       //全局暴击概率加成
    private float _globalCriticalMultiplier = 2.0f; //全局暴击伤害倍数加成
    private TowerFactory _towerFactory;               // 引用 TowerFactory 单例，用于创建新塔
    private bool _isInitialized;                      // 标记是否已初始化
    [SerializeField] public Bullet _bulletPrefab;     //子弹预制体
    private ObjectPool<Bullet> _bulletPool;           //子弹对象池
    #endregion

    #region 公开属性
    /// <summary>
    /// 只读属性，返回场上所有塔的只读列表
    /// </summary>
    public IReadOnlyList<Tower> AllTowers { get { return _allTowers; } }

    /// <summary>
    /// 全局攻击力加成属性，可在外部设置
    /// </summary>
    public float GlobalAttackBonus { get { return _globalAttackBonus; } set { _globalAttackBonus = value; } }

    /// <summary>
    /// 全局攻击速度倍率属性，可在外部设置
    /// </summary>
    public float GlobalAttackSpeedMultiplier { get { return _globalAttackSpeedMultiplier; } set { _globalAttackSpeedMultiplier = value; } }

    /// <summary>
    /// 全局暴击概率加成，供外部调用
    /// </summary>
    public float GlobalCriticalShotProb { get => _globalCriticalShotProb; set => _globalCriticalShotProb = value; }
    /// <summary>
    /// 全局暴击倍数加成，供外部调用
    /// </summary>
    public float GlobalCriticalMultiplier { get => _globalCriticalMultiplier; set => _globalCriticalMultiplier = value; }
    /// <summary>
    /// 子弹对象池，供外部调用
    /// </summary>
    public ObjectPool<Bullet> BulletPool { get => _bulletPool; }

    #endregion

    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void Init()
    {
        if(_bulletPool == null) _bulletPool = new ObjectPool<Bullet>(_bulletPrefab, 10, transform);
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
        // tower.BaseAttack += _globalAttackBonus;
        // tower.AttackSpeed *= _globalAttackSpeedMultiplier;
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
        // if (_allTowers.Contains(tower))
        // {
        //     _allTowers.Remove(tower);
        //     Destroy(tower.gameObject);
        // }
    }

    /// <summary>
    /// 注册一个刚创建或合成完成的塔实例到 _allTowers 列表
    /// </summary>
    /// <param name="tower">待注册的塔实例</param>
    public void RegisterTower(Tower tower)
    {
        // if (!_allTowers.Contains(tower))
        //     _allTowers.Add(tower);
    }

    /// <summary>
    /// 取消注册一个塔实例（仅从列表移除，不销毁 GameObject）
    /// </summary>
    /// <param name="tower">待取消注册的塔实例</param>
    public void UnregisterTower(Tower tower)
    {
        // if (_allTowers.Contains(tower))
        //     _allTowers.Remove(tower);
    }

    /// <summary>
    /// 对所有塔应用一次全局攻击力加成（例如升级时调用）
    /// 遍历 _allTowers，将每座塔的 BaseAttack += bonus，并 UpdateStats
    /// </summary>
    /// <param name="bonus">要增加的攻击力值</param>
    public void ApplyGlobalAttackBonus(float bonus)
    {
        // _globalAttackBonus += bonus;
        // foreach (var t in _allTowers)
        // {
        //     t.BaseAttack += bonus;
        //     t.UpdateStats();
        // }
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

public enum TowerType
{

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理所有炮塔的生成、销毁与全局属性加成
/// </summary>
public class TowerManager : ManagerBase<TowerManager>   
{
    #region 公共属性
    //全局加成
    /// <summary>
    /// 全局攻击力加成值
    /// </summary>
    public BuffChain AtkBuff { get; set; } = new BuffChain();
    /// <summary>
    /// 全局弹夹容量加成
    /// </summary>
    public BuffChain CapBuff { get; set; } = new BuffChain();
    /// <summary>
    /// 全局射速加成倍率
    /// </summary>
    public BuffChain AtkRateBuff { get; set; } = new BuffChain();
    /// <summary>
    /// 全局暴击概率加成
    /// </summary>
    public BuffChain CritProbBuff { get; set; } = new BuffChain();
    /// <summary>
    /// 全局暴击伤害倍数加成
    /// </summary>
    public BuffChain CritMultBuff { get; set; } = new BuffChain();
    /// <summary>
    /// 全局换弹时长
    /// </summary>
    public BuffChain ReloadBuff { get; set; } = new BuffChain();
    /// <summary>
    /// 当前的炮塔
    /// </summary>
    public Tower CurrentTower { get => _currentTower; }

    //总的
    [SerializeField] private Tower _currentTower;//当前的炮塔
    [SerializeField] private Transform towerGunParent;//炮管的父物体
    [SerializeField] private TowerPlatform towerPlatform;//炮塔底座

    [SerializeField]
    private List<Bullet> bulletPrefabs;
    private Dictionary<System.Type, ObjectPool<Bullet>> bulletPools =new Dictionary<System.Type, ObjectPool<Bullet>>();
    #endregion


    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// 初始化 _allTowers 列表，并获取 TowerFactory 单例
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.InBattle;
        Index = 2;
    }
    #endregion


    #region public 成员方法
    /// <summary>
    /// 初始化
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void Init()
    {
        //加成
        AtkBuff.Clear();
        CapBuff.Clear();
        AtkRateBuff.Clear();
        CritProbBuff.Clear();
        CritMultBuff.Clear();
        ReloadBuff.Clear();
        BattleManager.OnEndBattle -= OnEndBattle;
        BattleManager.OnEndBattle += OnEndBattle;
    }
 
    /// <summary>
    /// 从对象池得到一个Bullet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetBullet<T>() where T : Bullet
    {
        System.Type type = typeof(T);

        // 如果已经有对象池，直接取
        if (bulletPools.TryGetValue(type, out var pool))
        {
            var b = pool.Get();
            return b as T;
        }

        // 没对象池 → 在列表中查找对应类型的 prefab
        Bullet prefab = null;

        if (bulletPrefabs.Count <= 0) return null;

        foreach (var p in bulletPrefabs)
        {
            if (p.GetType() == type)
            {
                prefab = p;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogError($"未在 bulletPrefabs 列表中找到类型 {type} 的 Bullet 预制体！");
            return null;
        }

        // 找到 prefab → 创建对象池
        pool = new ObjectPool<Bullet>(prefab, 20, transform);
        bulletPools.Add(type, pool);

        // 返回对象
        var bullet = pool.Get();

        return bullet as T;
    }

    /// <summary>
    /// 将bullet返回对象池
    /// </summary>
    /// <param name="bullet"></param>
    public void ReturnBullet(Bullet bullet)
    {
        if (bullet == null)
        {
            Debug.LogWarning("ReturnBullet 失败：bullet 为 null");
            return;
        }

        // 获取真实类型
        System.Type type = bullet.GetType();

        // 如果对象池存在 → 正常回收
        if (bulletPools.TryGetValue(type, out var pool))
        {
            pool.Return(bullet);
            return;
        }

        // 找不到对象池 → 说明这个 bullet 并非通过 GetBullet() 创建
        Debug.LogWarning($"回收 Bullet 失败：找不到 {type} 的对象池，直接销毁该对象");
        Destroy(bullet.gameObject);
    }

    /// <summary>
    /// 加载整个炮塔
    /// </summary>
    public void LoadCompleteTower()
    {
        Debug.Log("加载炮塔");
        Tower selectedTower = TowerDataManager.Instance.GetCurrentTowerData().TowerPrefab;
        _currentTower= Instantiate(selectedTower, towerGunParent.transform);
    }

    private void OnEndBattle(bool success)
    {
        Destroy(_currentTower.gameObject);
    }
    #endregion


}
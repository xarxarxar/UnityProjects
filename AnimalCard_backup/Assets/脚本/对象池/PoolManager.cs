using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public Transform DefaultParent;

    //箭矢
    [SerializeField]private Transform arraw;
    public ObjectPool<Transform> ArrawPool;
    //炸弹
    [SerializeField] private Transform boom;
    public ObjectPool<Transform> BoomPool;
    //爱心（治疗）
    [SerializeField] private Transform treat;
    public ObjectPool<Transform> TreatPool;
    //少量金币
    [SerializeField] private Transform littleCoin;
    public ObjectPool<Transform> LittleCoinPool;
    //大量金币
    [SerializeField] private Transform muchCoin;
    public ObjectPool<Transform> MuchCoinPool;
    //护盾
    [SerializeField] private Transform shield;
    public ObjectPool<Transform> ShieldPool;
    //飞行物
    [SerializeField] public List<FlyingObject> flyObjects;
    private Dictionary<FlyingObject, ObjectPool<FlyingObject>> flyPoolDic=new Dictionary<FlyingObject, ObjectPool<FlyingObject>>();
    //SingleCard
    [SerializeField] public List<SingleCard> singleCards;
    private Dictionary<string, ObjectPool<SingleCard>> singleCardsPoolDic = new Dictionary<string, ObjectPool<SingleCard>>();
    //Animal
    [SerializeField] public Animal animal;
    public ObjectPool<Animal> AnimalPool;

    private void Awake()
    {
        Instance = this;

        if (ArrawPool == null)
        {
            ArrawPool = new ObjectPool<Transform>(arraw, 5, DefaultParent);
        }

        if (BoomPool == null)
        {
            BoomPool = new ObjectPool<Transform>(boom, 5, DefaultParent);
        }

        if (TreatPool == null)
        {
            TreatPool = new ObjectPool<Transform>(treat, 5, DefaultParent);
        }

        if (LittleCoinPool == null)
        {
            LittleCoinPool = new ObjectPool<Transform>(littleCoin, 5, DefaultParent);
        }
        if (MuchCoinPool == null)
        {
            MuchCoinPool = new ObjectPool<Transform>(muchCoin, 5, DefaultParent);
        }
        if (ShieldPool == null)
        {
            ShieldPool = new ObjectPool<Transform>(shield, 5, DefaultParent);
        }
        if (AnimalPool == null)
        {
            AnimalPool = new ObjectPool<Animal>(animal, 5, DefaultParent);
        }

        InitFlyingPools();//
        InitCardPools();
    }

    private void Start()
    {
        
    }
    //初始化卡牌的对象池
    private void InitCardPools()
    {
        singleCardsPoolDic.Clear();

        foreach (var prefab in singleCards)
        {
            if (prefab == null) continue;

            ObjectPool<SingleCard> pool = new ObjectPool<SingleCard>(prefab, 5, DefaultParent);

            singleCardsPoolDic.Add(prefab.ID, pool);
        }
    }

    /// <summary>
    /// 根据卡牌 ID 从对象池中获取一个 SingleCard
    /// </summary>
    public SingleCard GetSingleCard(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("GetSingleCard failed: id is null or empty");
            return null;
        }

        if (!singleCardsPoolDic.TryGetValue(id, out var pool))
        {
            Debug.LogError($"GetSingleCard failed: No pool found for id = {id}");
            return null;
        }

        SingleCard card = pool.Get();
        card.SetPool(pool);
        return card;
    }

    //初始化飞行物的对象池
    private void InitFlyingPools()
    {
        flyPoolDic.Clear();

        foreach (var prefab in flyObjects)
        {
            if (prefab == null) continue;

            ObjectPool<FlyingObject> pool = new ObjectPool<FlyingObject>(prefab,5, DefaultParent);

            flyPoolDic.Add(prefab, pool);
        }
    }


    /// <summary>
    /// 随机获取一个飞行物
    /// </summary>
    /// <returns></returns>
    public FlyingObject GetRandomFlyingObject()
    {
        if (flyObjects == null || flyObjects.Count == 0)
            return null;

        int index = Random.Range(0, flyObjects.Count);
        FlyingObject prefab = flyObjects[index];

        if (!flyPoolDic.TryGetValue(prefab, out var pool))
        {
            Debug.LogError("没有找到对应的对象池: " + prefab.name);
            return null;
        }

        FlyingObject obj = pool.Get();
        obj.SetPool(pool); //很重要，回收用
        return obj;
    }
}

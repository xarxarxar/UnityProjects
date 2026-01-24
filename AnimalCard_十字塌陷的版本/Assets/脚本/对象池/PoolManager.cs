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
    //SingleCard
    [SerializeField] public List<SingleCard> singleCards;
    private Dictionary<string, ObjectPool<SingleCard>> singleCardsPoolDic = new Dictionary<string, ObjectPool<SingleCard>>();
    //Animal
    [SerializeField] public Animal animal;
    public ObjectPool<Animal> AnimalPool;
    //Slot
    [SerializeField] public Slot slot;
    public ObjectPool<Slot> SlotPool;
    //角色信息UI
    [SerializeField] public Transform roleDataUIParent;
    [SerializeField] public GameObject roleDataUI;
    public ObjectPool<RectTransform> roleDataUIPool;

    //卡牌信息UI
    [SerializeField] public Transform cardDataUIParent;
    [SerializeField] public GameObject cardDataUI;
    public ObjectPool<RectTransform> cardDataUIPool;

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
        if (SlotPool == null)
        {
            SlotPool = new ObjectPool<Slot>(slot, 5, DefaultParent);
        }
        if (roleDataUIPool == null)
        {
            roleDataUIPool = new ObjectPool<RectTransform>(roleDataUI.GetComponent<RectTransform>(), 2, roleDataUIParent);
        }
        if (cardDataUIPool == null)
        {
            cardDataUIPool = new ObjectPool<RectTransform>(cardDataUI.GetComponent<RectTransform>(), 5, cardDataUIParent);
        }

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

    /// <summary>
    /// 随机获取一张 SingleCard
    /// </summary>
    public SingleCard GetRandomCard()
    {
        if (singleCardsPoolDic.Count == 0)
        {
            Debug.LogError("GetRandomCard failed: no card pools initialized");
            return null;
        }

        // 随机一个 index
        int index = Random.Range(0, singleCardsPoolDic.Count);

        int i = 0;
        foreach (var kv in singleCardsPoolDic)
        {
            if (i == index)
            {
                SingleCard card = kv.Value.Get();
                card.SetPool(kv.Value);
                return card;
            }
            i++;
        }

        return null; // 理论上不会走到
    }

}

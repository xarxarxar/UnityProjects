using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    public static IngredientManager Instance;
    [Header("所有食材预制体")]
    public List<Ingredient> AllIngredientPrefabs=new List<Ingredient>();
    [HideInInspector]
    public Dictionary<string,ObjectPool<Ingredient>> AllIngredientPools=new Dictionary<string, ObjectPool<Ingredient>>();
    [Header("每个池的初始数量")]
    public int initialPoolSize = 5;
    public Transform IngredientPoolParent;//食材对象池的父物体

    public ConveyorBelt01 ConveyorBelt;//传送带
    private List<Ingredient> AllIngredient=new List<Ingredient>();//所有场上的食材

    private Coroutine spawnCoro = null;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LevelManager.OnInit += Init;
        LevelManager.OnEndBattle += OnEndBattle;
    }


    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        CreatePools();
        spawnCoro=StartCoroutine(SpawnIngredientIe());
    }

    //关卡结束
    private void OnEndBattle()
    {
        //移除场上所有的食材
        for (int i = AllIngredient.Count - 1; i >= 0; i--)
        {
            AllIngredient[i].DestroyIngredient();
        }
        AllIngredient.Clear();

        //停止生成食材
        if(spawnCoro != null)
        {
            StopCoroutine(spawnCoro);
            spawnCoro = null;
        }
    }

    /// <summary>
    /// 获取指定名称的食材
    /// </summary>
    public Ingredient GetIngredient(string name)
    {
        if (AllIngredientPools.TryGetValue(name, out var pool))
        {
            Ingredient ingredient = pool.Get();
            AllIngredient.Add(ingredient);
            return ingredient;
        }
        Debug.LogWarning($"没有找到名为{name}的食材对象池");
        return null;
    }

    /// <summary>
    /// 随机从字典中选择一个对象池，然后从池中获取一个食材
    /// </summary>
    public Ingredient GetRandomIngredient()
    {
        if (AllIngredientPools.Count == 0) return null;

        // 将字典的键转换成列表
        var keys = new List<string>(AllIngredientPools.Keys);

        // 随机选择一个 key
        string randomKey = keys[Random.Range(0, keys.Count)];

        Ingredient ingredient = AllIngredientPools[randomKey].Get();
        AllIngredient.Add(ingredient);
        // 从对应的池里取对象
        return ingredient;
    }

    /// <summary>
    /// 将食材回收到对应池
    /// </summary>
    public void ReturnIngredient(Ingredient ingredient)
    {
        if (ingredient == null || ingredient.data == null) return;

        string key = ingredient.data.ingredientName;
        if (AllIngredientPools.TryGetValue(key, out var pool))
        {
            pool.Return(ingredient);
            AllIngredient.Remove(ingredient);
        }
        else
        {
            Debug.LogWarning($"没有找到名为{key}的对象池，无法回收");
            Destroy(ingredient.gameObject);
        }
    }

    /// <summary>
    /// 根据预制体列表创建对象池
    /// </summary>
    private void CreatePools()
    {
        foreach (var prefab in AllIngredientPrefabs)
        {
            if (prefab == null || prefab.data == null) continue;

            string key = prefab.data.ingredientName;

            if (!AllIngredientPools.ContainsKey(key))
            {
                ObjectPool<Ingredient> pool = new ObjectPool<Ingredient>(prefab, initialPoolSize, IngredientPoolParent);
                AllIngredientPools.Add(key, pool);
            }
        }
    }


    //生成食材的IE
    IEnumerator SpawnIngredientIe()
    {
        WaitForSeconds wait = new WaitForSeconds(1.5f);

        yield return wait;  // 第一次延迟

        while (true)
        {
            // 如果暂停或满了，则下一帧继续检查，不进入计时
            if (LevelManager.Instance.isPaused || ConveyorBelt.IsFull)
            {
                yield return null;
                continue;
            }

            // 可以生成
            Ingredient ingredient = GetRandomIngredient();
            ConveyorBelt.SpawnIngredient(ingredient);

            // 生成后等待下一轮
            yield return wait;
        }
    }

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FruitGameManager : MonoBehaviour
{
    public static FruitGameManager Instance;

    [Header("关卡设置")]
    public List<FruitType> allFruitTypes; // 游戏所有果子库
    public List<FruitType> currentLevelFruits = new List<FruitType>(); // 本关随机选出的果子
    public float truckSpawnInterval = 5f;
    public int targetGold = 100;
    public int totalSeconds = 120;
    private float timer = 0;

    [Header("运行状态")]
    public int currentGold = 0;
    public int spawnedTruckCount = 0;
    public List<Truck> activeTrucks = new List<Truck>();
    public Transform[] truckSlots; // 5个货车位置
    public GameObject truckPrefab;
    public FruitTree FruitTreePrefab;//果树的预制体
    public static bool isGaming = true;

    public Text uiText; // 显示金币和关卡进度

    void Awake() { Instance = this; }

    void Start()
    {
        PrepareLevel();
        PlantingPanel.Instance.SetupButtons();
    }

    void PrepareLevel()
    {
        // 随机选取 9-16 种果子
        int typeCount = Random.Range(9, 17);
        for (int i = 0; i < typeCount && i < allFruitTypes.Count; i++)
        {
            currentLevelFruits.Add(allFruitTypes[i]); // 简化：直接取前几个
        }
        InvokeRepeating("SpawnTruck", 0, truckSpawnInterval);
    }

    void SpawnTruck()
    {
        if(timer>=totalSeconds) return;
        if (activeTrucks.Count >= 5) return;//停车位已满

        // 找空位
        for (int i = 0; i < truckSlots.Length; i++)
        {
            if (truckSlots[i].childCount == 0)
            {
                GameObject go = Instantiate(truckPrefab, truckSlots[i]);
                Truck t = go.GetComponent<Truck>();

                // 随机订单逻辑
                FruitType randomFruit = currentLevelFruits[Random.Range(0, currentLevelFruits.Count)];
                t.Init( 60f, 50,null); // 30秒限时，50金币奖励,任务先设置为null

                activeTrucks.Add(t);
                spawnedTruckCount++;
                break;
            }
        }
    }

    public bool TryDeliverFruit(FruitType type)
    {
        // 寻找最急需该果子的车（按剩余时间排序）
        activeTrucks.Sort((a, b) => a.remainTime.CompareTo(b.remainTime));
        foreach (var truck in activeTrucks)
        {
            if (truck.ReceiveFruit(type)) return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
    }

    public void RemoveTruck(Truck t)
    {
        activeTrucks.Remove(t);
    }

    /// <summary>
    /// 在某块树上种一棵树
    /// </summary>
    public void PlantTree(FruitType newData,LandSlot landSlot)
    {
        FruitTree tree = Instantiate(FruitTreePrefab,landSlot.transform);
        tree.InitTree(newData);
    }

    void Update()
    {
        if (timer < totalSeconds)
        {
            timer += Time.deltaTime;
            uiText.text = $"金币: {currentGold} / 目标: {targetGold}\n时间:{timer:F0}/{totalSeconds}s";
        }
        else
        {
            isGaming = false;
            if (currentGold >= targetGold)
            {
                uiText.text = $"游戏结束，完成目标";
            }
            else
            {
                uiText.text = $"游戏结束，关卡失败";
            }
        }

            

        // 胜利检测逻辑...
    }
}
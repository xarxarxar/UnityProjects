using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    public Bee testBee;//测试的小蜜蜂
    public List<Mission> AllMissions = new List<Mission>();
    public List<FruitSource> fruitSources = new List<FruitSource>();//所有的果实来源

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
        testBee.InitBee();//初始化小蜜蜂
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
                int missionCount = Random.Range(1, 5);
                List<Mission> signleTruckMissions = GenerateMissions(t, missionCount);
                t.Init( 60f, 50, signleTruckMissions); // 30秒限时，50金币奖励,任务先设置为null
                AddMissions(signleTruckMissions);//添加进所有的任务中

                activeTrucks.Add(t);
                spawnedTruckCount++;
                break;
            }
        }
    }

    /// <summary>
    /// 添加一个任务
    /// </summary>
    public void AddMission(Mission mission)
    {
        // 任务为空直接返回
        if (mission == null)
            return;

        // 防止重复添加
        if (!AllMissions.Contains(mission))
        {
            AllMissions.Add(mission);
        }
    }
    /// <summary>
    /// 添加一堆任务
    /// </summary>
    /// <param name="missions"></param>
    public void AddMissions(List<Mission> missions)
    {
        if (missions == null || missions.Count == 0)
            return;

        for (int i = 0; i < missions.Count; i++)
        {
            Mission mission = missions[i];

            // 跳过无效任务
            if (mission == null)
                continue;

            // 防止重复
            if (!AllMissions.Contains(mission))
            {
                AllMissions.Add(mission);
            }
        }
    }

    /// <summary>
    /// 获取一个任务
    /// </summary>
    /// <returns></returns>
    public Mission GetMission(Bee bee)
    {
        if (bee.mission != null) return null;
        for(int i = 0; i < AllMissions.Count; i++)
        {
            if (AllMissions[i].employBee == null)
            {
                AllMissions[i].employBee = bee;
                return AllMissions[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 结束任务，可能是玩家通过道具完成的，也可能是蜜蜂完成的这个任务,也可能失败了
    /// </summary>
    public void OnMissionOver(Mission mission,bool finished)
    {
        if (mission == null || !AllMissions.Contains(mission)) return;

        if (mission.employBee != null)
        {
            mission.employBee.OnMissionOver(mission);
        }

        if (finished)//任务成功
        {

        }
        else//任务失败
        {

        }
        AllMissions.Remove(mission);
    }

    public void AddFruitSource(FruitSource newFruitSource)
    {
        fruitSources.Add(newFruitSource);
    }

    /// <summary>
    /// 查找离这个小蜜蜂最近的果实来源
    /// </summary>
    public FruitSource FindFruitSource(FruitType fruitType, Bee bee)
    {
        FruitSource nearestSource = null;
        float nearestDist = float.MaxValue;

        Vector3 beePos = bee.transform.position;

        for (int i = 0; i < fruitSources.Count; i++)
        {
            var source = fruitSources[i];

            // 果实类型不匹配
            if (source.FruitType != fruitType)
                continue;

            // 果实已经被预定完
            if (source.ReserveBees.Count >= source.RipedCount)
                continue;

            float dist = (source.transform.position - beePos).sqrMagnitude;

            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestSource = source;
            }
        }

        return nearestSource;
    }

    /// <summary>
    /// 随机生成一个Mission,一定得指定Truck，因为任务一定是有一个指定的货车
    /// </summary>
    private Mission GenerateMission(Truck truck)
    {
        // 安全检查
        if (truck == null) return null;

        // 如果当前关卡没有水果类型
        if (currentLevelFruits == null || currentLevelFruits.Count == 0)
            return null;

        // 随机选择一个FruitType
        int index = Random.Range(0, currentLevelFruits.Count);
        FruitType fruitType = currentLevelFruits[index];

        // 创建Mission
        Mission mission = new Mission();

        // 初始化Mission
        mission.InitMission(truck, fruitType);

        return mission;
    }

    /// <summary>
    /// 随机生成多个Mission，一定得指定Truck，因为任务一定是有一个指定的货车
    /// </summary>
    private List<Mission> GenerateMissions(Truck truck, int count)
    {
        List<Mission> missions = new List<Mission>();

        if (truck == null || count <= 0)
            return missions;

        while (missions.Count < count)
        {
            Mission mission = GenerateMission(truck);

            if (mission != null)
            {
                missions.Add(mission);
            }
        }

        return missions;
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
        AddFruitSource(tree);
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
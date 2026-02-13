using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using SerializableDictionary.Scripts;



/// <summary>
/// 动物管理
/// </summary>
public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance;
    public List<AnimalBaseData> animalBaseDatas = new List<AnimalBaseData>();
    public List<Animal> AllAnimals = new List<Animal>();
    public List<Transform> pointsTransform=new List<Transform>(); 
    public SerializableDictionary<int, List<Transform>> pointsTransformDict=new SerializableDictionary<int, List<Transform>>();//围栏
    public List<AnimalData> AllAnimalDatas=new List<AnimalData>();

    private float BreedChance = 0.5f;//繁殖成功率

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            GetAnimal(gender:1);
            GetAnimal(gender:0);
        }

        GameManager.OnMonthChanged -= OnMonthChanged;
        GameManager.OnMonthChanged += OnMonthChanged;
    }

    private void OnMonthChanged(int oldValue,int newValue)
    {
        SimulateMonth(newValue-oldValue);
    }

    public List<Animal> TryBreed()
    {
        Debug.Log("尝试繁殖");
        // 1. 按 RoomIndex 分组
        var roomDict = new Dictionary<int, List<Animal>>();

        foreach (var animal in AllAnimals)
        {
            if (!roomDict.ContainsKey(animal.RoomID))
                roomDict[animal.RoomID] = new List<Animal>();

            roomDict[animal.RoomID].Add(animal);
        }
        List<Animal> newBabies=new List<Animal>();
        // 2. 每个房间单独处理
        foreach (var room in roomDict)
        {
            Debug.Log($"{room.Key}is full:{RoomManager.instance.IsRoomFull(room.Key)}");
            if (!RoomManager.instance.IsRoomFull(room.Key))//房间没满
            {
                if (TryBreedInRoom(room.Value,out Animal baby))
                {
                    newBabies.Add(baby);
                }
            }
        }

        return newBabies;
    }
    //一个房间一次性最多只能有一对繁殖成功
    bool TryBreedInRoom(List<Animal> animals,out Animal baby)
    {
        baby = null;
        if (animals.Count < 2) return false;//数量不足两只

        // 筛选冷却结束的
        var males = animals.FindAll(a =>a.Gender == 1 && a.CanBreed());

        var females = animals.FindAll(a =>a.Gender == 0 &&a.CanBreed());

        if (males.Count == 0 || females.Count == 0)
            return false;

        //随机配对一次
        var male = males[Random.Range(0, males.Count)];
        var female = females[Random.Range(0, females.Count)];

        float prob = Random.value;

        // 20% 概率
        if (prob < BreedChance)
        {
            baby= OnBreedSuccess(male, female);
            return true;
        }
        return  false;
    }
    //成功繁殖时
    Animal OnBreedSuccess(Animal male, Animal female)
    {
        male.LastBreedAge = GameManager.Instance.CurrentMonth;
        female.LastBreedAge = GameManager.Instance.CurrentMonth;

        // 生成新动物
        Animal baby = BreedAnimal(male,female);

        return baby;

    }

    /// <summary>
    /// 模拟几个月内的变化
    /// </summary>
    /// <param name="deltaMonth"></param>
    public void SimulateMonth(int deltaMonth)
    {
        int startMonth=GameManager.Instance.CurrentMonth-deltaMonth;
        int endMonth = GameManager.Instance.CurrentMonth;
        int SimulateCount = (endMonth - startMonth) / 5;//度过的整5月的次数
        if (SimulateCount <= 0) return;

        //创建临时数组
        List<Animal> tmp= new List<Animal>();
        
        //先模拟死亡,再模拟繁殖
        for(int i = 0; i < SimulateCount; i++)
        {
            tmp.Clear();
            tmp.AddRange(AllAnimals); // 复制一份快照
            //模拟死亡
            for (int j=0; j<tmp.Count; j++)
            {
                tmp[j].Age += 5;
                tmp[j].TryDie(out string cause);//是否死亡，如果死亡会自动移出AllAnimals
            }
            if (AllAnimals.Count <= 0)
                break; // 已灭绝，提前结束

            //模拟繁殖
            TryBreed();//尝试繁殖，返回的是新生儿的列表，同样会自动Add到AllAnimals
        }
        for(int i = 0;i < AllAnimals.Count; i++)
        {
            AllAnimals[i].Age += deltaMonth % 5;
        }

    }

    /// <summary>
    /// 获取一个动物
    /// </summary>
    /// <param name="race"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    public Animal GetAnimal(string race=null,int gender=-1)
    {
        Animal animal = PoolManager.Instance.AnimalPool.Get();
        AnimalBaseData data = null;
        if (race!=null)//指定种族
        {
            for (int i = 0; i < animalBaseDatas.Count; i++)
            {
                if (animalBaseDatas[i].Race == race)
                {
                    data = animalBaseDatas[i];
                }
            }
        }
        else
        {
            data = animalBaseDatas[Random.Range(0, animalBaseDatas.Count)];
        }

        if (data != null)
        {
            animal.Init(data, gender);
        }
        else
        {
            return null;
        }

        return animal;
    }

    /// <summary>
    /// 生育一个动物
    /// </summary>
    /// <param name="father">父亲</param>
    /// <param name="mother">母亲</param>
    public Animal BreedAnimal(Animal father,Animal mother)
    {
        if(father==null || mother==null || father.animalData.Race!=mother.animalData.Race) return null;
        Animal son = PoolManager.Instance.AnimalPool.Get();
        son.Init(father,mother);
        return son;
    }
}

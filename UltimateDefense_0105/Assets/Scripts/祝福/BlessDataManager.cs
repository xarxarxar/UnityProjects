using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlessDataManager :  ManagerBase<BlessDataManager>
{
    private static System.Random rng = new System.Random();
    public readonly Color32 normalColor = new Color32(51, 178, 200, 255);
    public readonly Color32 rareColor = new Color32(193,133,255,255);
    public readonly Color32 epicColor = new Color32(245,225,75,225);

    public List<Bless> AllBless = new List<Bless>();
    //public static Dictionary<Bless, List<int>> BlessCount = new Dictionary<Bless, List<int>>();


    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;
    }

    public override void Init()
    {
        Debug.Log($"初始化字典");
        foreach (var bless in AllBless)
        {
            if (!DataManager.Instance.PlayerInfo.BlessCount.TryGetValue(bless.ID, out var counts))
            {
                DataManager.Instance.PlayerInfo.SetBlessCount(bless.ID, new List<int> { -1, 10, 0 });
                counts = DataManager.Instance.PlayerInfo.BlessCount[bless.ID];
            }
            // 每个 bless 有 3 个稀有度数量：
            // 例如 R0 无限，R1=10，R2=3
            //BlessCount[bless] = new List<int>() { 99999, 10, 0 };
        }
    }

    /// <summary>
    /// 选取n个稀有度为int的Bless
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<(Bless, int)> ChooseBless(int count)
    {
        // 返回结果
        List<(Bless, int)> result = new List<(Bless, int)>();


        for (int i = 0; i < count; i++)
        {
            (Bless, int) selected = TryChooseOne();//抽取一个

            Bless selectedBlessData = selected.Item1;//祝福
            int rarity = selected.Item2;//稀有度

            if (selectedBlessData != null)
            {
                result.Add(selected);

                if (rarity > 0)
                {
                    if (!DataManager.Instance.PlayerInfo.BlessCount.TryGetValue(selectedBlessData.ID, out var counts))
                    {
                        DataManager.Instance.PlayerInfo.SetBlessCount(selectedBlessData.ID, new List<int> { -1, 10, 0 });
                        counts = DataManager.Instance.PlayerInfo.BlessCount[selectedBlessData.ID];
                    }
                    counts[rarity] =Mathf.Max(counts[rarity] - 1);
                    DataManager.Instance.PlayerInfo.SetBlessCount(selectedBlessData.ID, counts);
                }
            }
            else
            {
                Debug.LogWarning("无法再抽到满足要求的 BlessData！");
                break;
            }
        }
        return result;
    }

    //抽取一个Bless，并指定它的稀有度一起返回
    private (Bless, int) TryChooseOne()
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            int rarity = RollRarity();//按照概率随机返回一个稀有度，返回0或1或2

            // 找出该稀有度下所有可抽的 Bless
            List<Bless> candidates = GetAvailableBlessByRarity(rarity);

            if (candidates.Count == 0)
            {
                // 没有这种稀有度可抽 → 重新 roll
                continue;
            }

            // 随机从候选里抽一项
            int index = rng.Next(candidates.Count);
            return (candidates[index], rarity);
        }

        // 十几次都抽不到说明库存都没了
        // 若 20 次都没中 → 保底抽稀有度 0！
        var commonList = GetAvailableBlessByRarity(0);

        if (commonList.Count == 0)
        {
            Debug.LogError("错误：稀有度 0 竟然没有库存！（理论不可能）");
            return (null, 0);
        }

        return (commonList[rng.Next(commonList.Count)], 0);
    }

    //随机稀有度（按权重）
    private int RollRarity()
    {
        int roll = rng.Next(100); // 0~99

        if (roll < 50) return 0;           // 50%
        else if (roll < 50 + 35) return 1; // 35%
        else return 2;                     // 15%
    }

    //找对应稀有度的 BlessData
    private List<Bless> GetAvailableBlessByRarity(int rarity)
    {
        List<Bless> list = new List<Bless>();

        foreach (var bless in AllBless)
        {
            // 稀有度数量是否足够
            if (rarity == 0)
            {
                // 稀有度0永远有库存
                list.Add(bless);
            }
            else
            {
                if (!DataManager.Instance.PlayerInfo.BlessCount.TryGetValue(bless.ID, out var counts))
                {
                    DataManager.Instance.PlayerInfo.SetBlessCount(bless.ID, new List<int> { -1, 10, 0 });
                    counts = DataManager.Instance.PlayerInfo.BlessCount[bless.ID];
                }

                if (counts[rarity] > 0)
                    list.Add(bless);
            }
        }

        return list;
    }

}


/// <summary>
/// 祝福基类，只用来记录
/// </summary>
public abstract class Bless : ScriptableObject
{
    [Tooltip("祝福的ID")]
    public int ID;

    [Tooltip("祝福的名称")]
    public string BlessName;

    [Tooltip("祝福的描述")]
    [TextArea]
    public string[] Descriptions = new string[3];

    [Tooltip("祝福的售价")]
    public int[] Costs = new int[3];

    [Tooltip("祝福的持续时间")]
    public float[] times = new float[3] { 10f, 20f, 30f };

    [Tooltip("祝福的图标")]
    public Sprite sprite;


    // 生成一个运行时实例
    public abstract BlessInstance CreateInstance(int rarity);
}

//真正跑逻辑的祝福父类
public abstract class BlessInstance
{
    protected int rarity;//稀有度
    protected Bless config;
    protected BlessBuffShow blessBuffShow;//祝福的小圆标

    public BlessInstance(Bless config, int rarity)
    {
        this.config = config;
        this.rarity = rarity;
    }

    public abstract void Start();
    public abstract void End();
}

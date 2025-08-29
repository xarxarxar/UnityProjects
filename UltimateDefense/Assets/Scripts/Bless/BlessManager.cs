using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessManager : ManagerBase<BlessManager>
{
    public override string Description { get; } = "管理对局开始时的祝福，局内的Manager";
    [SerializeField]private Bless _bless;//对局开始前选择的祝福
    [SerializeField]private int _blessCost;//抽取一次祝福需要花费的钻石数量

    // 所有可用的祝福类型（构造函数默认设为普通，稍后我们再设定稀有度）
    private static List<Func<Bless>> _blessFactories = new List<Func<Bless>>()
    {
        () => new DropCoinBless(0),
        () => new CriticalBless(0),
        // 继续添加其他祝福类
    };
    /// <summary>
    /// 对局开始前选择的祝福
    /// </summary>
    public Bless Bless { get => _bless; }
    /// <summary>
    /// 抽取祝福所需的花费
    /// </summary>
    public int BlessCost { get => _blessCost; set => _blessCost = value; }

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.InBattle;//局内Manager
        Index = 0;
    }

    public override void Init()
    {
        _bless = null;//初始化为空
        _blessCost = 100;//花费初始为100
    }

    /// <summary>
    /// 随机获取一个祝福
    /// </summary>
    /// <returns></returns>
    public Bless GetRandomBless()
    {
        return null;
    }
    
}

/// <summary>
/// 祝福基类
/// </summary>
public abstract class Bless
{
    /// <summary>
    /// 稀有度,0表示普通，1表示稀有，2表示史诗。
    /// </summary>
    public int Rarity { get; protected set; }

    // 构造函数
    protected Bless(int rarity)
    {
        Rarity = rarity;
    }

    /// <summary>
    /// 应用祝福：每种祝福的逻辑不同，由子类重写
    /// </summary>
    public abstract void Apply();
}

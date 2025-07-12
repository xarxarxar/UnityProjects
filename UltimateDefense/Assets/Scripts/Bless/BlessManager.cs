using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BlessManager : ManagerBase<BlessManager>,IManager
{
    [SerializeField]private Bless _bless;//对局开始前选择的祝福
    [SerializeField] private int _blessCost;//抽取一次祝福需要花费的钻石数量
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
        Bless bless = new Bless();
        bless.testProp = Random.Range(1,10);
        return bless;
    }
    
}

public class Bless
{
    public int testProp;//测试属性
}

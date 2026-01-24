using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlessManager : ManagerBase<BlessManager>
{
    public override string Description { get; } = "管理对局开始时的祝福，局内的Manager";
    private float fullBlessExp = 10;//满经验值是多少，每次达到之后都会提升
    private float currentBlessExp = 0;//当前经验值是多少，每次抽取祝福之后清零

    public static UnityAction<float, float> OnExpChanged;
    private List<BlessInstance> activeBlesses = new();//当前所有正在运行的祝福
    [SerializeField] private BlessBuffShow _blessBuffPrefab;//祝福Buff的预制体
    [SerializeField] private Transform _blessBuffParent;//祝福Buff的父物体

    public ObjectPool<BlessBuffShow> BlessBuffShowPool;//BlessBuffShow对象池
    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.InBattle;//局内Manager
        Index = 0;
    }

    public override void Init()
    {
        fullBlessExp = 10;
        currentBlessExp = 0;
        Enemy.OnEnemyDie -= OnEnemyDie;
        Enemy.OnEnemyDie += OnEnemyDie;
        BattleManager.OnEndBattle -= OnEndBattle;
        BattleManager.OnEndBattle += OnEndBattle;
        if (BlessBuffShowPool == null)
        {
            BlessBuffShowPool=new ObjectPool<BlessBuffShow> (_blessBuffPrefab,5, _blessBuffParent);
        }
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDie -= OnEnemyDie;
    }

    private void OnEndBattle(bool isSuccess)
    {
        Debug.Log("战斗结束，清理祝福");
        ClearBlessInstance();
    }

    private void OnEnemyDie(Enemy enemy)
    {
        AddExp(enemy.BlessExp);
    }

    //增加经验值
    private void AddExp(float exp)
    {
        if (currentBlessExp >= fullBlessExp) return;
        currentBlessExp=Mathf.Min(fullBlessExp, currentBlessExp+ exp);//增加经验值
        OnExpChanged?.Invoke(currentBlessExp, fullBlessExp);
    }

    /// <summary>
    /// 每次领取完之后都升级
    /// </summary>
    public void UpgradeStage()
    {
        currentBlessExp = 0;
        fullBlessExp *= 1.1f;
        OnExpChanged?.Invoke(currentBlessExp, fullBlessExp);
    }


    /// <summary>
    /// 添加正在运行的祝福
    /// </summary>
    /// <param name="instance"></param>
    public void AddBlessInstance(BlessInstance instance)
    {
        activeBlesses.Add(instance);
    }

    /// <summary>
    /// 移除一个正在执行的祝福
    /// </summary>
    /// <param name="instance"></param>
    public void RemoveBlessInstance(BlessInstance instance)
    {
        if (activeBlesses.Contains(instance))
        {
            activeBlesses.Remove(instance);
        }
    }

    /// <summary>
    /// 清除所有正在运行的祝福
    /// </summary>
    public void ClearBlessInstance()
    {
        Debug.Log($"祝福列表的数量为{activeBlesses.Count}");
        for (int i = activeBlesses.Count - 1; i >= 0; i--)
        {
            activeBlesses[i].End();
        }
        activeBlesses.Clear();
    }

}


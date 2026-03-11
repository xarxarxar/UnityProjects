using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

/// <summary>
/// 蜜蜂当前状态
/// </summary>
public enum BeeState
{
    Idle,       // 空闲
    GetFruit,   // 前往水果来源
    SendFruit,  // 前往货车送水果
    ToTrash     // 前往垃圾桶丢水果
}
[System.Serializable]
/// <summary>
/// 果实来源接口
/// 果树 和 储物箱 都需要实现这个接口
/// </summary>
public abstract class FruitSource : MonoBehaviour
{
    public virtual FruitType FruitType { get; }//它的水果类型

    public virtual int RipedCount { get; }//成熟的果实数量

    //预定的蜜蜂，如果数量大于等于RipedCount，那么就表示这里无法获取成熟的果实，按理说数量不应该会大于RipedCount
    public virtual List<Bee> ReserveBees { get; }

    public static event Action<FruitSource>  OnRipedCountChanged;//果实变化事件，对应蜜蜂的OnFruitSourceRestock方法

    public abstract void RemoveBee(Bee bee);

    // 静态方法触发事件
    protected static void TriggerRipedCountChanged(FruitSource source)
    {
        OnRipedCountChanged?.Invoke(source);
    }
    // 尝试预定水果
    public abstract bool TryReserveFruit(Bee bee);

    // 拿走预定的水果
    public abstract void  TakeReservedFruit(FruitType fruit);
}

/// <summary>
/// 小蜜蜂
/// </summary>
public class Bee : MonoBehaviour
{
    /// <summary>
    /// 蜜蜂移动速度
    /// </summary>
    public float defaultSpeed = 2f;

    /// <summary>
    /// 当前状态
    /// </summary>
    public BeeState state = BeeState.Idle;

    /// <summary>
    /// 当前任务
    /// </summary>
    public Mission mission = null;

    /// <summary>
    /// 当前水果来源（树 或 储物箱）
    /// </summary>
    private FruitSource fruitSource;

    /// <summary>
    /// 蜜蜂手里拿的水果
    /// </summary>
    private FruitType carryFruitType = null;

    /// <summary>
    /// 当前移动目标点
    /// </summary>
    private Vector3 currentTargetPos;

    /// <summary>
    /// 垃圾桶位置
    /// </summary>
    public Transform trashPoint;

    /// <summary>
    /// 任务检测间隔
    /// </summary>
    public float missionInterval = 0.5f;

    /// <summary>
    /// 任务检测计时器
    /// </summary>
    private float missionTimer = 0f;

    /// <summary>
    /// 初始化蜜蜂
    /// </summary>
    public void InitBee()
    {
        state = BeeState.Idle;
        mission = null;
        fruitSource = null;
        carryFruitType = null;

        FruitSource.OnRipedCountChanged -= OnFruitSourceCountChanged;
        FruitSource.OnRipedCountChanged += OnFruitSourceCountChanged;
    }

    /// <summary>
    /// 给蜜蜂分配任务
    /// </summary>
    public void SetMission(Mission _mission)
    {
        Debug.Log("给蜜蜂分配任务");
        mission = _mission;

        missionTimer = 0f;

        StartMission();
    }

    /// <summary>
    /// 每帧更新移动逻辑
    /// </summary>
    void Update()
    {
        switch (state)
        {
            case BeeState.Idle:

                TryGetMission();
                break;
            // 前往水果来源
            case BeeState.GetFruit:
                
                if (fruitSource == null)
                    return;
                Debug.Log($"fruitSource不为null");
                currentTargetPos = fruitSource.transform.position;

                if (MoveToTarget(currentTargetPos))
                {
                    OnArriveFruit();
                }

                break;

            // 前往货车
            case BeeState.SendFruit:

                if (mission == null || mission.needTruck == null)
                    return;

                currentTargetPos = mission.needTruck.transform.position;

                if (MoveToTarget(currentTargetPos))
                {
                    OnArriveTruck();
                }

                break;

            // 前往垃圾桶
            case BeeState.ToTrash:

                if (trashPoint == null)
                    return;

                if (MoveToTarget(trashPoint.position))
                {
                    OnArriveTrash();
                }

                break;
        }
    }

    /// <summary>
    /// 尝试获取任务
    /// Idle状态下每隔一定时间执行一次
    /// </summary>
    void TryGetMission()
    {
        
        missionTimer += Time.deltaTime;

        // 没到时间
        if (missionTimer < missionInterval)
            return;
        // 重置计时
        missionTimer = 0f;

        // 从游戏管理器获取任务
        Mission newMission = FruitGameManager.Instance.GetMission(this);

        // 如果没有任务
        if (newMission == null)
            return;
        // 接任务
        SetMission(newMission);
    }

    /// <summary>
    /// 移动到目标点
    /// </summary>
    bool MoveToTarget(Vector3 target)
    {
        // 向目标移动
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            defaultSpeed * Time.deltaTime
        );

        // 判断是否到达
        float distance = Vector3.Distance(transform.position, target);

        return distance < 0.05f;
    }

    /// <summary>
    /// 到达水果来源
    /// </summary>
    void OnArriveFruit()
    {
        if (fruitSource == null || mission == null)
            return;

        // 从来源取走水果
        fruitSource.TakeReservedFruit(mission.fruitType);

        carryFruitType = mission.fruitType;

        // 开始送货
        state = BeeState.SendFruit;
    }

    /// <summary>
    /// 到达货车
    /// </summary>
    void OnArriveTruck()
    {
        if (mission == null)
            return;

        // 货车接收水果
        mission.needTruck.ReceiveFruit(carryFruitType);

        carryFruitType = null;

        FruitGameManager.Instance.OnMissionOver(mission,true);

        state = BeeState.Idle;
    }

    /// <summary>
    /// 到达垃圾桶
    /// </summary>
    void OnArriveTrash()
    {
        carryFruitType = null;

        state = BeeState.Idle;
    }

    /// <summary>
    /// 查找水果来源（树优先，储物箱其次）
    /// </summary>
    void FindFruitSource()
    {
        // 找树
        fruitSource = FruitGameManager.Instance.FindFruitSource(mission.fruitType,this);

        if (fruitSource != null)
        {
            fruitSource.TryReserveFruit(this);
            return;
        }
        fruitSource = null;
    }

    /// <summary>
    /// 当前任务结束或被取消
    /// </summary>
    public void OnMissionOver(Mission _mission)
    {
        if (mission != _mission) return;

        mission = null;

        if (carryFruitType != null)
        {
            state = BeeState.ToTrash;
        }
        else
        {
            state = BeeState.Idle;
        }
    }

    /// <summary>
    /// 任务开始逻辑
    /// </summary>
    private void StartMission()
    {
        switch (state)
        {
            // 接到订单时是空闲
            case BeeState.Idle:
                FindFruitSource();
                state = BeeState.GetFruit;
                break;


            // 正在去拿水果
            case BeeState.GetFruit:

                if (carryFruitType == mission.fruitType)
                {
                    // 已经拿的是需要的水果
                    // 不需要改变
                }
                else
                {
                    // 重新寻找水果来源
                    FindFruitSource();

                    state = BeeState.GetFruit;
                }

                break;


            // 手里已经拿着水果
            case BeeState.SendFruit:

                if (carryFruitType == mission.fruitType)
                {
                    // 直接修改目标货车
                }
                else
                {
                    // 去垃圾桶丢掉水果
                    state = BeeState.ToTrash;
                }

                break;
        }
    }

    /// <summary>
    /// 如果当前导航去的IFruitSource数量发生变化
    /// </summary>
    public void OnFruitSourceCountChanged(FruitSource changedSource)
    {
        if (fruitSource != null && fruitSource != changedSource)
            return; //变化与自己无关

        // 当前有目标
        if (fruitSource != null)
        {
            // 当前source没有可用果子
            if (changedSource.ReserveBees.Count >= changedSource.RipedCount)
            {
                // 取消旧预约
                changedSource.ReserveBees.Remove(this);

                // 重新寻找
                var newSource = FruitGameManager.Instance.FindFruitSource(mission.fruitType, this);

                if (newSource != null)
                {
                    fruitSource = newSource;
                    newSource.ReserveBees.Add(this);
                }
                else
                {
                    fruitSource = null;
                }
            }
        }
        else
        {
            Debug.Log($"当前没有目标，如果这个source出现可用果子，" +
                $"changedSource.ReserveBees.Count为{changedSource.ReserveBees.Count}，" +
                $"changedSource.RipedCount为{changedSource.RipedCount}");
            // 当前没有目标，如果这个source出现可用果子
            if (changedSource.ReserveBees.Count < changedSource.RipedCount)
            {
                
                fruitSource = changedSource;
                changedSource.ReserveBees.Add(this);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
/// <summary>
/// 分配给蜜蜂的任务：货车（需要果实）-果实（从果树或者储存箱）-货车（送出果实）
/// </summary>
public class Mission
{
    public Truck needTruck = null;//下订单的货车，一辆货车可能有多个任务，比如10个送苹果的任务
    public FruitType fruitType = null;
    public Bee employBee = null;//雇佣的蜜蜂，也就是接到这一单的蜜蜂

    /// <summary>
    /// 初始化任务
    /// </summary>
    /// <param name="truck"></param>
    /// <param name="source"></param>
    public void InitMission(Truck truck,FruitType type)
    {
        needTruck = truck;
        fruitType = type;
        employBee = null;
    }

    /// <summary>
    /// 开始这个任务，有蜜蜂接单了
    /// </summary>
    /// <param name="bee"></param>
    public void StartMission(Bee bee)
    {
        employBee = bee;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BeeState 
{ 
    Idle,//没有成熟的果实，或者没有货车的时候，处于这个状态
    GetFruit,//在拿取果实的路上
    SendFruit,//在将果实送到货车的路上
}

/// <summary>
/// 果实来源接口
/// 树 和 储物箱 都实现这个接口
/// </summary>
public interface IFruitSource
{
    // 尝试预定水果
    FruitType TryReserveFruit(Bee bee);

    // 拿走预定的水果
    void TakeReservedFruit(FruitType fruit);
}

/// <summary>
/// 小蜜蜂
/// </summary>
public class Bee : MonoBehaviour
{
    public float defaultSpeed = 1;//蜜蜂默认移动速度
    public float missionInterval = 0.5f;//蜜蜂每0.5秒遍历一次有没有需要做的事情
    public BeeState state=BeeState.Idle;

    private Mission mission = null;
    private IFruitSource fruitSource;
    // 当前手里拿的水果
    private FruitType carryFruitType= null;


    /// <summary>
    /// 初始化蜜蜂
    /// </summary>
    public void InitBee()
    {
        state= BeeState.Idle;
        mission = null;
    }
    /// <summary>
    /// 给蜜蜂一个新订单
    /// </summary>
    /// <param name="_mission"></param>
    public void SetMission(Mission _mission)
    {
        mission = _mission;
    }

    /// <summary>
    /// 移动去拿水果
    /// </summary>
    void MoveToFruit()
    {
        // 移动逻辑省略

        // 假设到达目标
        bool arrived = true;

        if (!arrived) return;

        // 从来源取走水果
        fruitSource.TakeReservedFruit(mission.fruitType);

        state = BeeState.SendFruit;
    }

    //查找果实的来源
    void FindFruitSource()
    {
        // 1. 找树
        //fruitSource = TreeManager.Instance.FindTree(mission.fruitType);

        if (fruitSource != null)
        {
            fruitSource.TryReserveFruit(this);
            return;
        }

        // 2. 找储物箱
        //fruitSource = StorageManager.Instance.FindStorage(mission.fruitType);

        if (fruitSource != null)
        {
            fruitSource.TryReserveFruit(this);
            return;
        }

        // 没找到
        fruitSource = null;
    }
    void MoveToTruck()
    {
        bool arrived = true;

        if (!arrived) return;

        mission.needTruck.ReceiveFruit(carryFruitType);

        carryFruitType = null;

        mission.FinishMission();

        state = BeeState.Idle;
    }
    //当前正在进行的任务结束了或者终止了
    public void OnMissionOver()
    {
        mission = null;

        // 如果手里有水果
        if (carryFruitType != null)
        {
            StartCoroutine(MoveToTrash());
        }
        else
        {
            state = BeeState.Idle;
        }
    }

    //主要是如果蜜蜂上一个订单没有完成，
    //但是货车就开走了，所以蜜蜂手里还有果实，此时接到一个新的订单该怎么办
    private IEnumerator StartMissionIe()
    {
        switch (state)
        {
            //接到订单时，就是空闲状态，那么就直接开始完整流程，这是正常情况
            case BeeState.Idle:
                FindFruitSource();
                if (fruitSource != null)
                {
                    state = BeeState.GetFruit;
                }

                break;
            //接到订单时，正在去拿水果的路上，此时修改目的水果源头，这个应该和Idle合并，
            case BeeState.GetFruit:
                // 如果新任务和旧任务水果一样
                if (carryFruitType == mission.fruitType)
                {
                    // 继续当前流程
                }
                else
                {
                    // 丢弃水果
                    

                    // 重新寻找水果
                    FindFruitSource();

                    state = BeeState.GetFruit;
                }
                break;
            //接到订单时，手里有未送完的水果，
            //先判断订单的水果和手里的水果是不是同一种类，
            //如果是同一种类，那么修改目的Truck即可，
            //如果不是同一种类，那么移动到垃圾桶将其丢弃
            case BeeState.SendFruit:
                // 如果水果类型一样
                if (carryFruitType == mission.fruitType)
                {
                    // 直接修改送货目标
                    // 不需要丢弃水果
                }
                else
                {
                    // 去垃圾桶丢掉水果
                    yield return MoveToTrash();

                    carryFruitType = null;

                    FindFruitSource();

                    state = BeeState.GetFruit;
                }
                break;
        }
    }

    IEnumerator MoveToTrash()
    {
        // 移动到垃圾桶

        yield return new WaitForSeconds(1f);

        carryFruitType = null;

        state = BeeState.Idle;
    }
}

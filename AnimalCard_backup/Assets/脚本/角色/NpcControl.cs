using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 这个脚本用于控制敌人，机器人自动实现召唤卡牌之类的
/// </summary>
public class NpcControl : RoleControl
{
    public float waitTime;//指定动作的等待时长，并非条件一满足就执行，而是有内置的interval

    private Coroutine SummonCardCoro = null;//召唤卡牌的协程
    bool isMerging;//是否正在合成
    /// <summary>
    ///  初始化
    /// </summary>
    public override void Init()
    {
        MyCardManager.Init(MyRole, PlayerManager.instance.selfRoleControl.MyRole, MySlotManager);

        RoleData data = new RoleData
        {
            maxHp = 200,
            maxStrength = 20,
            Speed = 2
        };
        MyRole.Init(MyCardManager,MyHookController, data);

        if (SummonCardCoro != null)
        {
            StopCoroutine(SummonCardCoro);
            SummonCardCoro = null;
        }
        SummonCardCoro = StartCoroutine(SummonCardIE());
    }

    //召唤卡牌的协程
    private IEnumerator SummonCardIE(UnityAction callback=null)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            if(MyRole.coinCount>= MyCardManager.SummonCardCoin)
            {
                MyRole.LaunchHook();
            }
            yield return new WaitUntil(() => MyCardManager.CanInteract);
            yield return new WaitForSecondsRealtime(waitTime);
            if (!isMerging && MyCardManager.TryGetSameIDCards(out var pair))
            {
                isMerging = true;
                var (a, b) = pair;
                //Debug.Log($"a.CurrentSlot是否为null{a.CurrentSlot==null}，b.CurrentSlot是否为null{b.CurrentSlot == null}");
                b.MoveToSlot(a.CurrentSlot, () =>
                {
                    MyCardManager.TryMerge(b, a);
                });
                

                yield return new WaitForSecondsRealtime(0.2f); // 给动画时间
                isMerging = false;
            }
        }

    }
}

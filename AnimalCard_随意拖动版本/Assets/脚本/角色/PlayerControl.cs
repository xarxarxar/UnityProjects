using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleControl : MonoBehaviour
{
    public Role MyRole;
    public CardManager MyCardManager;
    public SlotManager MySlotManager;

    public virtual void Init()
    {

    }
}


public class PlayerControl : RoleControl
{

    public override void Init()
    {
        MyCardManager.Init(MyRole, BattleManager.instance.NpcRoleControl.MyRole, MySlotManager);
        MyCardManager.FillSlots();//填充所有的卡槽
        MyRole.Init(MyCardManager,new RoleData {maxHp=100,maxStrength=10,Speed=1 });
        BattleManager.OnBattleEnd -= OnBattleEnd;
        BattleManager.OnBattleEnd += OnBattleEnd;
    }

    private void OnBattleEnd(bool success)
    {
        Debug.Log("player:战斗结束");
        MyRole.ResetRole();
        MyCardManager.ResetCardManager();
        MySlotManager.ResetSlotManager();
        RoleDataUIManager.Instance.RemoveRoleUI(MyRole);
    }

    private void Update()
    {
       if(Input.GetKeyUp(KeyCode.G))
       {
            if (MySlotManager.GetFirstEmptySlot(out Slot emptySlot))
            {
                SingleCard singleCard = MyRole.SummonACard();
                singleCard.Init(MyCardManager);
                singleCard.MoveToSlot(emptySlot);
                MyCardManager.AddCard(singleCard);
            }

        }
    }


}

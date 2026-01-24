using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleControl : MonoBehaviour
{
    public Role MyRole;
    public CardManager MyCardManager;
    public SlotManager MySlotManager;
    public Transform SlotParent;
    public virtual void Init()
    {

    }
}


public class PlayerControl : RoleControl
{
    
    public override void Init()
    {
        MySlotManager.Init(8,10, SlotParent);
        MyCardManager.Init(MyRole, MySlotManager);
        MyCardManager.boardManager.GenerateInitialBoard();
        MyRole.Init(new RoleData {maxHp=100,maxStrength=10,Speed=1 });
        BattleManager.OnBattleEnd -= OnBattleEnd;
        BattleManager.OnBattleEnd += OnBattleEnd;
    }

    private void OnBattleEnd(bool success)
    {
        Debug.Log("player:Õ½¶·½áÊø");
        MyRole.ResetRole();
        MyCardManager.ResetCardManager();
        MySlotManager.ResetSlotManager();
        RoleDataUIManager.Instance.RemoveRoleUI(MyRole);
    }



}

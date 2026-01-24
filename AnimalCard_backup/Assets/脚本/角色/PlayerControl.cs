using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleControl : MonoBehaviour
{
    public Role MyRole;
    public SlotManager MySlotManager;
    public CardManager MyCardManager;
    public HookController MyHookController;

    public virtual void Init()
    {

    }
}

public class PlayerControl : RoleControl
{

    public override void Init()
    {
        MyCardManager.Init(MyRole,PlayerManager.instance.npcRoleControl.MyRole, MySlotManager);
        MyRole.Init(MyCardManager, MyHookController,new RoleData {maxHp=100,maxStrength=10,Speed=1 });
        MyHookController.OnCatchCard -= OnCatchCard;
        MyHookController.OnCatchCard += OnCatchCard;
            
    }

    private void Update()
    {
       if(Input.GetKeyUp(KeyCode.G))
       {
           MyRole.LaunchHook();
       }
    }
    //¹³×Ó×¥µ½¿¨ÅÆ
    private void OnCatchCard(SingleCard singleCard)
    {
        //_cardManager.GetCard();
        //SingleCard card = PoolManager.Instance.GetSingleCard(singleCard.ID);
        MyCardManager.GetSpecificCard(singleCard.ID);
        //card.Init(MyCardManager);
        //singleCard.Init(MyCardManager);
    }


}

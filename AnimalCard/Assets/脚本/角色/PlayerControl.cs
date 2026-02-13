using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Role MyRole;
    public CardManager MyCardManager;
    public SlotManager MySlotManager;
    public Transform SlotParent;
    public SpriteRenderer MyRender;

    public void Init(AnimalData animalData)
    {
        MyRender.sprite=animalData.sprite;
        MySlotManager.Init(8,10, SlotParent);
        MyCardManager.Init(MyRole, MySlotManager);
        MyCardManager.boardManager.GenerateInitialBoard();
        MyRole.Init(animalData);
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

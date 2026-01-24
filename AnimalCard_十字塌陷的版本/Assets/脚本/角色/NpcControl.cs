using System;
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

    private float SummonInterval=15.0f;//召唤间隔
    private float CountDownDur = 40;//每隔卡牌存在的时长
    private Dictionary<SingleCard,float> cardCountDown = new Dictionary<SingleCard,float>();
    List<SingleCard> tempKeys = new List<SingleCard>();
    /// <summary>
    ///  初始化
    /// </summary>
    public override void Init()
    {
        cardCountDown.Clear();
        MySlotManager.Init(5,1, SlotParent);
        MyCardManager.Init(MyRole, BattleManager.instance.PlayerRoleControl.MyRole, MySlotManager);
        MyCardManager.FillSlots(3);
        for(int i=0;i< MyCardManager.AllCards.Count; i++)
        {
            CardUIManager.Instance.RegisterCardUI(MyCardManager.AllCards[i], Vector3.zero);
            cardCountDown[MyCardManager.AllCards[i]] = CountDownDur;
        }

        RoleData data = new RoleData
        {
            maxHp = 200,
            maxStrength = 20,
            Speed = 2
        };
        MyRole.Init(MyCardManager,data);

        if (SummonCardCoro != null)
        {
            StopCoroutine(SummonCardCoro);
            SummonCardCoro = null;
        }
        SummonCardCoro = StartCoroutine(SummonCardIE());
        BattleManager.OnBattleEnd -= OnBattleEnd;
        BattleManager.OnBattleEnd += OnBattleEnd;
    }

    void Update()
    {
        if (cardCountDown.Count == 0) return;

        tempKeys.Clear();
        tempKeys.AddRange(cardCountDown.Keys);

        foreach (var card in tempKeys)
        {
            cardCountDown[card] -= Time.deltaTime;
            CardUIManager.Instance.UpdateCardUI(card, cardCountDown[card] / CountDownDur);
            if (cardCountDown[card] <= 0)
            {
                CardUIManager.Instance.RemoveCardUI(card);
                cardCountDown.Remove(card);
                card.Use();
            }
        }
    }
    /// <summary>
    /// 将这个卡牌从NPC手中移除
    /// </summary>
    /// <param name="singleCard"></param>
    public void RemoveCardInDic(SingleCard singleCard)
    {
        CardUIManager.Instance.RemoveCardUI(singleCard);
        cardCountDown.Remove(singleCard);
    }

    private void OnBattleEnd(bool success)
    {
        Debug.Log("NPC:战斗结束");
        MyRole.ResetRole();
        MyCardManager.ResetCardManager();
        MySlotManager.ResetSlotManager();
        if (SummonCardCoro != null)
        {
            StopCoroutine(SummonCardCoro);
            SummonCardCoro = null;
        }
        RoleDataUIManager.Instance.RemoveRoleUI(MyRole);
        tempKeys.Clear();
        tempKeys.AddRange(cardCountDown.Keys);
        foreach (var card in tempKeys)
        {
            CardUIManager.Instance.RemoveCardUI(card);
            cardCountDown.Remove(card);
        }
    }

    //召唤卡牌的协程
    private IEnumerator SummonCardIE(UnityAction callback=null)
    {
        while (true)
        {
            //SummonInterval -= 1.0f;
            yield return new WaitForSecondsRealtime(waitTime);
            if(MyRole.coinCount>= MyCardManager.SummonCardCoin)
            {
                if (MySlotManager.GetFirstEmptySlot(out Slot emptySlot))
                {
                    SingleCard singleCard = MyRole.SummonACard();
                    singleCard.Init(MyCardManager,3);
                    singleCard.MoveToSlot(emptySlot);
                    CardUIManager.Instance.RegisterCardUI(singleCard,Vector3.zero);
                    MyCardManager.AddCard(singleCard);
                    cardCountDown[singleCard] = CountDownDur;
                }
            }
            yield return new WaitUntil(() => MyCardManager.CanInteract);
            yield return new WaitForSecondsRealtime(waitTime);
        }

    }

}

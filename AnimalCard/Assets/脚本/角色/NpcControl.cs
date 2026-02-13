using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 这个脚本用于控制敌人，机器人自动实现召唤卡牌之类的
/// </summary>
public class NpcControl : MonoBehaviour
{
    public float waitTime;//指定动作的等待时长，并非条件一满足就执行，而是有内置的interval

    public NpcCardManager cardManager;
    public NpcSlotManager slotManager;
    public Role MyRole;
    public int SlotCount = 6;

    private Coroutine SummonCardCoro = null;//召唤卡牌的协程
    private float CountDownDur = 40;//每隔卡牌存在的时长
    private Dictionary<SingleCard,float> cardCountDown = new Dictionary<SingleCard,float>();
    List<SingleCard> tempKeys = new List<SingleCard>();
    private Coroutine handleDisappearCoro = null;
    private bool isResolvingCards = false;

    /// <summary>
    ///  初始化
    /// </summary>
    public void Init()
    {
        cardCountDown.Clear();
        slotManager.Init(SlotCount);
        cardManager.Init(3);
        for (int i = 0; i < cardManager.NpcCards.Length; i++)
        {
            if (cardManager.NpcCards[i] == null) continue;
            CardUIManager.Instance.RegisterCardUI(cardManager.NpcCards[i], Vector3.zero);
            cardCountDown[cardManager.NpcCards[i]] = CountDownDur;
            cardManager.NpcCards[i].OnCardDisappear -= OnCardDisappear;
            cardManager.NpcCards[i].OnCardDisappear += OnCardDisappear;
        }

        MyRole.Init(AnimalManager.Instance.AllAnimalDatas[1]);

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

        if (cardCountDown.Count == 0 || isResolvingCards) return;

        tempKeys.Clear();
        tempKeys.AddRange(cardCountDown.Keys);
        List<SingleCard> willDisappear = new List<SingleCard>();
        foreach (var card in tempKeys)
        {
            cardCountDown[card] -= Time.deltaTime;
            CardUIManager.Instance.UpdateCardUI(card, cardCountDown[card] / CountDownDur);
            if (cardCountDown[card] <= 0)
            {
                willDisappear.Add(card);
            }
        }

        if (willDisappear.Count > 0)
        {
            isResolvingCards = true;
            HandleWillDisappear(willDisappear);
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
        cardManager.ResetNpcCardManager();
        slotManager.ResetSlotManager();
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
    private IEnumerator SummonCardIE(UnityAction callback = null)
    {
        while (true)
        {
            //如果正在结算，暂停召唤
            yield return new WaitUntil(() => !isResolvingCards);

            yield return new WaitForSecondsRealtime(waitTime);

            // 再检查一次（防止刚醒就被锁）
            if (isResolvingCards)
                continue;

            Slot emptySlot = slotManager.GetFirstEmptySlot();
            if (emptySlot != null)
            {
                int index = emptySlot.Row + emptySlot.Col;
                cardManager.SpawnCard(index);

                CardUIManager.Instance.RegisterCardUI(
                    cardManager.NpcCards[index], Vector3.zero);

                cardCountDown[cardManager.NpcCards[index]] = CountDownDur;

                cardManager.NpcCards[index].OnCardDisappear -= OnCardDisappear;
                cardManager.NpcCards[index].OnCardDisappear += OnCardDisappear;
            }

            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    /// <summary>
    /// 请求移除 NPC 卡牌（统一入口）
    /// </summary>
    /// <param name="card">要移除的卡</param>
    /// <param name="useCard">是否执行 Use</param>
    public void RequestRemoveCard(string cardId, bool useCard)
    {
        // 正在结算中，直接忽略
        if (isResolvingCards)
            return;

        // 查找卡牌
        SingleCard card = cardManager.FindNpcCardById(cardId);

        // 已不存在（可能已被移除 / 已结算）
        if (card == null)
            return;

        List<SingleCard> list = new List<SingleCard> { card };

        if (useCard)
        {
            HandleWillDisappear(list);
        }
        else
        {
            HandleImmediateRemove(list);
        }
    }

    private void HandleImmediateRemove(List<SingleCard> cards)
    {
        if (handleDisappearCoro != null)
            StopCoroutine(handleDisappearCoro);

        handleDisappearCoro = StartCoroutine(
            HandleImmediateRemoveIE(cards)
        );
    }

    private IEnumerator HandleImmediateRemoveIE(List<SingleCard> cards)
    {
        isResolvingCards = true;

        foreach (var card in cards)
        {
            // UI
            CardUIManager.Instance.RemoveCardUI(card);
            cardCountDown.Remove(card);

            // Slot & 数据
            Slot slot = card.CurrentSlot;
            int index = slot.Col + slot.Row;

            cardManager.NpcCards[index] = null;
            slot.SetUnOccupied();

            // 直接销毁（无 Use）
           yield return  card.Diasppear();
        }

        // 下一帧再排，保证状态稳定
        yield return null;

        yield return cardManager.ReArrange();

        isResolvingCards = false;
        handleDisappearCoro = null;
    }


    private void OnCardDisappear(SingleCard singleCard)
    {
        Slot slot=singleCard.CurrentSlot;
        int index= slot.Col+slot.Row;
        cardManager.NpcCards[index] = null;
        slot.SetUnOccupied();
        singleCard.OnCardDisappear -= OnCardDisappear;
        //cardManager.ReArrange();
    }
    private IEnumerator CardUseWrapper(
    SingleCard card,
    Vector3 pos,
    Action onFinish)
    {
        yield return card.Use(3, pos, RoleEnum.Npc);
        onFinish?.Invoke();
    }

    private void HandleWillDisappear(List<SingleCard> willDisappear)
    {
        if (handleDisappearCoro != null)
            StopCoroutine(handleDisappearCoro);

        handleDisappearCoro = StartCoroutine(HandleWillDisappearIE(willDisappear));
    }

    private IEnumerator HandleWillDisappearIE(List<SingleCard> willDisappear)
    {
        int finishedCount = 0;
        int totalCount = willDisappear.Count;

        foreach (var card in willDisappear)
        {
            // UI & 数据立刻移除
            CardUIManager.Instance.RemoveCardUI(card);
            cardCountDown.Remove(card);

            Vector3 pos = BattleManager.instance.NpcRole.transform.position;

            // 立即销毁卡牌实体
            card.DestroyThisImmediately();

            // 启动 Use，但用回调统计完成
            StartCoroutine(CardUseWrapper(card, pos, () =>
            {
                finishedCount++;
            }));
        }

        //等待所有 Use 完成
        yield return new WaitUntil(() => finishedCount >= totalCount);

        //状态完全稳定后再重排
        yield return cardManager.ReArrange();
        isResolvingCards = false;
        handleDisappearCoro = null;
    }

}

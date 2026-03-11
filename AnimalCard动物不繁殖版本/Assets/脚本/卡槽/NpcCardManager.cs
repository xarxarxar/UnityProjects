using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// NPC的卡牌管理
/// </summary>
public class NpcCardManager : MonoBehaviour
{
    public SingleCard[] NpcCards;
    public NpcSlotManager slotManager;

    private void Awake()
    {
        
    }

    public void Init(int count)
    {
        NpcCards = new SingleCard[slotManager.SlotCount];
        for (int i = 0; i < count; i++)
        {
            SpawnCard(i);
        }
    }

    public SingleCard GetRandomCard()
    {
        return PoolManager.Instance.GetRandomCard();
    }

    public SingleCard FindNpcCardById(string cardId)
    {
        for (int i = 0; i < NpcCards.Length; i++)
        {
            var card = NpcCards[i];
            if (card == null) continue;

            if (card.ID == cardId)
                return card;
        }

        return null;
    }

    public void ResetNpcCardManager()
    {
        for (int i = 0; i < NpcCards.Length; i++)
        {
            SingleCard card = NpcCards[i];
            if (card == null) continue;
            card.DestroyThisImmediately();
            NpcCards[i] = null;
        }
    }

    /// <summary>
    ///获取一张特定ID的卡
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public SingleCard GetSpecificCard(string ID)
    {
        SingleCard singleCard = PoolManager.Instance.GetSingleCard(ID);
        if (singleCard == null) return null;
        return null;
    }

    public void SpawnCard(int i)
    {
        Slot slot = slotManager.GetSlot(i);
        SingleCard card = GetRandomCard();
        card.Init();
        card.SetSlot(slot);
        NpcCards[i] = card;
        slot.SetOccupied();
        card.transform.position = slot.WorldPosition ;
        card.Anim.PlaySummon();
    }

    //重新排列所有的卡牌
    // 重新排列所有的卡牌（并行移动，等待全部完成）
    public IEnumerator ReArrange(Action callback = null)
    {
        List<IEnumerator> moveRoutines = new List<IEnumerator>();
        int finishedCount = 0;

        for (int i = 0; i < NpcCards.Length; i++)
        {
            if (NpcCards[i] != null)
                continue;

            int j = i + 1;
            while (j < NpcCards.Length && NpcCards[j] == null)
            {
                j++;
            }

            if (j >= NpcCards.Length)
                break;

            Debug.Log($"重新排列, j:{j} → i:{i}");

            var card = NpcCards[j];
            var fromSlot = card.CurrentSlot;
            var targetSlot = slotManager.NpcSlos[i];

            // ===== ① 数据 & Slot 状态：立刻修正 =====

            // 数组
            NpcCards[i] = card;
            NpcCards[j] = null;

            // Slot 状态
            fromSlot.SetUnOccupied();
            targetSlot.SetOccupied();

            // Card 绑定
            card.SetSlot(targetSlot);

            // ===== ② 收集动画协程 =====
            moveRoutines.Add(
                MoveCardAndNotify(card, targetSlot, () => finishedCount++)
            );
        }

        if (moveRoutines.Count == 0)
            yield break;

        // 并行启动动画
        foreach (var routine in moveRoutines)
        {
            StartCoroutine(routine);
        }

        // 等全部动画完成
        yield return new WaitUntil(() => finishedCount >= moveRoutines.Count);

        callback?.Invoke();
        Debug.Log("ReArrange 完成，所有卡牌移动结束");
    }

    private IEnumerator MoveCardAndNotify(
    SingleCard card,
    Slot targetSlot,
    System.Action onFinished)
    {
        yield return card.MoveToSlot(targetSlot);
        onFinished?.Invoke();
    }
}

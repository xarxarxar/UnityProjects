using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    private SlotManager _slotManager;
    public Role SelfRole;//自己
    public Role NpcRole;//对方

    public BoardFlowController boardFlowController;
    public BoardManager boardManager;
    public HintController hintController;
    public readonly int SummonCardCoin = 10;//召唤一张卡牌所需要的金币数量

    public static readonly Color32 NormalColor = new Color32(0, 0, 0, 0);
    public static readonly Color32 SecondColor = new Color32(229, 201, 140, 255);
    public static readonly Color32 ThirdColor = new Color32(247, 232, 104, 255);

    public bool CanInteract;

    public SingleCard IsClickingCard;//当前正在点击的卡

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="selfRole"></param>
    /// <param name="targetRole"></param>
    /// <param name="slotManager"></param>
    public void Init(Role selfRole,SlotManager slotManager)
    {
        SelfRole = selfRole;
        _slotManager=slotManager;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void ResetCardManager()
    {
        SelfRole=null;
        _slotManager = null;
        IsClickingCard=null;

        for (int row = 0; row < boardManager.Board.GetLength(0); row++)
        {
            for (int col = 0; col < boardManager.Board.GetLength(1); col++)
            {
                SingleCard card = boardManager.Board[row, col];
                if (card == null) continue;
                card.DestroyThisImmediately();
            }
        }
    }
    #region Get Card

    public SingleCard GetRandomCard()
    {
        return PoolManager.Instance.GetRandomCard();
    }

    /// <summary>
    /// 改变被点击的卡牌
    /// </summary>
    public void ChangeCardClick(SingleCard card)
    {
        if (IsClickingCard == card) return;
        if(card==null)
        {
            IsClickingCard.Anim.SetClick(false);
            IsClickingCard = null;
            return;
        }

        if (AreAdjacent(IsClickingCard,card))//两个卡如果相邻则触发交换
        {
            // 发起交换
            boardFlowController.TrySwap(IsClickingCard, card);
            IsClickingCard.Anim.SetClick(false);
            IsClickingCard =null;
            return;
        }

        if(IsClickingCard != null)
        {
            IsClickingCard.Anim.SetClick(false);
        }
        if(card!=null)
        {
            card.Anim.SetClick(true);
        }
        IsClickingCard = card;
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
        if(_slotManager.TryGetRandomEmpty(out Slot emptySlot))
        {
            return singleCard;
        }
        return null;
    }
    /// <summary>
    /// 判断两个卡是否相邻
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool AreAdjacent(SingleCard a, SingleCard b)
    {
        if (a == null || b == null) return false;

        int r1 = a.CurrentSlot.Row;
        int c1 = a.CurrentSlot.Col;
        int r2 = b.CurrentSlot.Row;
        int c2 = b.CurrentSlot.Col;

        return Mathf.Abs(r1 - r2) + Mathf.Abs(c1 - c2) == 1;
    }
    #endregion

    #region Drag Result
    /// <summary>
    /// 交换卡牌
    /// </summary>
    /// <param name="currentCard"></param>
    /// <param name="dir"></param>
    public void OnSwipe(SingleCard currentCard, Vector2Int dir)
    {
        if (currentCard == null) return;

        Slot slot = currentCard.CurrentSlot;
        int row = slot.Row;
        int col = slot.Col;

        // 计算目标格
        int targetRow = row + dir.y;  // 上：+1，下：-1
        int targetCol = col + dir.x;  // 右：+1，左：-1
        // 检查边界
        if (!boardManager.InBounds(targetRow, targetCol)) return;
        // 获取目标卡
        SingleCard targetCard = boardManager.GetCard(targetRow, targetCol);
        if (targetCard == null) return;
        hintController.StopHints();
        ChangeCardClick(null);
        // 发起交换
        boardFlowController.TrySwap(currentCard, targetCard);
    }

    #endregion


}

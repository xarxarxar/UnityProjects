using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消除执行器（只处理数据，不做动画）
/// </summary>
public class ClearController : MonoBehaviour
{
    public BoardManager boardManager;

    /// <summary>
    /// 清除指定的卡牌
    /// </summary>
    public void ClearCards( List<SingleCard> cards)
    {
        foreach (var card in cards)
        {
            if (card == null) continue;

            Slot slot = card.CurrentSlot; 
            if (slot == null) continue;

            int row = slot.Row;
            int col = slot.Col;
           
            // 1. 清空 Board 数据
            if (boardManager.InBounds(row, col) &&
                boardManager.Board[row, col] == card)
            {
                boardManager.Board[row, col] = null;
            }

            // 2. 释放 Slot
            slot.SetUnOccupied();

            // 3. 回收或销毁卡牌
            //Destroy(card.gameObject);
            // 或：PoolManager.Instance.Recycle(card);
        }
    }
}

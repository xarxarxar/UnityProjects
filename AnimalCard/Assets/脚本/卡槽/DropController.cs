using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 下落控制器（处理数据 + 下落动画）
/// </summary>
public class DropController : MonoBehaviour
{
    public BoardManager boardManager;
    public SlotManager slotManager;
    public CardManager cardManager;

    /// <summary>
    /// 执行一次完整下落
    /// yield 完成时，表示所有下落动画结束
    /// </summary>
    public IEnumerator DropIe(System.Action<bool> onFinished = null)
    {
        bool hasMoved = false;
        int finishedCount = 0;
        List<IEnumerator> moveRoutines = new List<IEnumerator>();

        int width = boardManager.Width;
        int height = boardManager.Height;

        // 按列处理
        for (int col = 0; col < width; col++)
        {
            CollectFallInColumn(col, moveRoutines, ref hasMoved);
        }
        
        // 没有任何下落
        if (!hasMoved)
        {
            Debug.Log("没有任何下落");
            onFinished?.Invoke(false);
            yield break;
        }

        // 并行执行所有下落动画
        foreach (var routine in moveRoutines)
        {
            StartCoroutine(WaitMove(routine, () => finishedCount++));
        }

        // 等全部下落完成
        yield return new WaitUntil(() => finishedCount >= moveRoutines.Count);

        onFinished?.Invoke(true);
    }

    private void CollectFallInColumn(
    int col,
    List<IEnumerator> routines,
    ref bool hasMoved)
    {
        int writeRow = 0;

        // 1. 处理已有卡牌下落
        for (int row = 0; row < boardManager.Height; row++)
        {
            var card = boardManager.Board[row, col];
            if (card == null) continue;

            if (row != writeRow)
            {
                routines.Add(MoveCard(row, writeRow, col));
                hasMoved = true;
            }

            writeRow++;
        }

        // 2. 顶部生成新卡（作为下落的一部分）
        for (int row = writeRow; row < boardManager.Height; row++)
        {
            var routine = SpawnAndFall(row, col);
            routines.Add(routine);
            hasMoved = true;
        }
    }

    private IEnumerator SpawnAndFall(int targetRow, int col)
    {
        Slot targetSlot = slotManager.GetSlot(targetRow, col);

        // 生成卡牌（不放进 Board）
        SingleCard card = cardManager.GetRandomCard();
        boardManager.Board[targetRow, col] = card;
        card.Init(cardManager);
        card.SetSlot(targetSlot);
        targetSlot.SetOccupied();

        // 从更高处生成
        float spawnOffset = 1f + (boardManager.Height - targetRow) * 0.5f;
        card.transform.position =
            targetSlot.WorldPosition + Vector3.up * spawnOffset+new Vector3(0,0,-1);

        // 下落动画
        yield return card.MoveToSlot(targetSlot);
    }

    /// <summary>
    /// 单张卡牌下落（数据 + 动画）
    /// </summary>
    private IEnumerator MoveCard(int fromRow, int toRow, int col)
    {
        var card = boardManager.Board[fromRow, col];
        if (card == null) yield break;

        // 清原位置
        boardManager.Board[fromRow, col] = null;
        card.CurrentSlot.SetUnOccupied();

        // 新位置
        Slot targetSlot = boardManager.slotManager.GetSlot(toRow, col);
        boardManager.Board[toRow, col] = card;

        card.SetSlot(targetSlot);
        targetSlot.SetOccupied();

        // 等动画完成
        yield return card.MoveToSlot(targetSlot);
    }

    private IEnumerator WaitMove(IEnumerator routine, System.Action onDone)
    {
        yield return routine;
        onDone?.Invoke();
    }
}

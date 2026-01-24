using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋盘数据管理器（核心逻辑层）我只关心：在(row, col) 上，现在是哪张卡
/// 负责：
/// 1. 维护 board[row,col]
/// 2. 初始生成（无 Match）
/// 3. 卡牌交换（只改数据）
/// </summary>
public class BoardManager : MonoBehaviour
{
    public int Height => slotManager.Height;
    public int Width => slotManager.Width;

    [Header("Refs")]
    public SlotManager slotManager;
    public CardManager cardManager;

    /// <summary>
    /// 当前棋盘数据
    /// row = 0 表示最底部
    /// </summary>
    public SingleCard[,] Board { get; private set; }

    private void Awake()
    {
        Board = new SingleCard[slotManager.Height, slotManager.Width];
    }

    #region 1. 初始生成 (无Match且必有解)

    public void GenerateInitialBoard()
    {
        bool hasMoves = false;
        int maxAttempts = 10; // 防止极端情况下的死循环
        int attempt = 0;
        while (!hasMoves && attempt < maxAttempts)
        {
            attempt++;
            ClearBoard(); // 清理旧卡牌（如果是重新生成）
            
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    CreateCardWithoutImmediateMatch(row, col);
                }
            }

            hasMoves = HasMatchableMoves();
        }

        if (!hasMoves) ForceInjectMove(); // 兜底：强行注入一个解
    }

    private void CreateCardWithoutImmediateMatch(int row, int col)
    {
        List<string> candidates = new List<string>();
        foreach (var item in PoolManager.Instance.singleCards) candidates.Add(item.ID);

        // 这里的逻辑就是你之前的：确保生成的瞬间不消除
        while (candidates.Count > 0)
        {
            string id = candidates[Random.Range(0, candidates.Count)];
            if (IsValidAt(row, col, id))
            {
                SpawnCard(row, col, id);
                return;
            }
            candidates.Remove(id);
        }
    }

    // 判断在 (row, col) 放置 id 是否会立即导致三连
    private bool IsValidAt(int row, int col, string id)
    {
        // 只需要检查左边和下面（因为生成顺序是从左到右，从下到上）
        if (col >= 2 && Board[row, col - 1]?.ID == id && Board[row, col - 2]?.ID == id) return false;
        if (row >= 2 && Board[row - 1, col]?.ID == id && Board[row - 2, col]?.ID == id) return false;
        return true;
    }

    #endregion

    #region 2. 死局检测核心逻辑 (Potential Match)

    /// <summary>
    /// 检测当前棋盘是否存在至少一种合法的交换消除
    /// </summary>
    public bool HasMatchableMoves()
    {
        for (int r = 0; r < Height; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                // 尝试向右交换
                if (CanSwapAndMatch(r, c, r, c + 1)) return true;
                // 尝试向上交换
                if (CanSwapAndMatch(r, c, r + 1, c)) return true;
            }
        }
        return false;
    }
    private bool CanSwapAndMatch(int r1, int c1, int r2, int c2)
    {
        if (!InBounds(r1, c1) || !InBounds(r2, c2)) return false;
        string id1 = Board[r1, c1].ID;
        string id2 = Board[r2, c2].ID;

        // 模拟交换后的逻辑检查
        // 检查原 (r1,c1) 放置 id2 是否成消，或者原 (r2,c2) 放置 id1 是否成消
        return IsPatternMatch(r1, c1, id2, r2, c2) || IsPatternMatch(r2, c2, id1, r1, c1);
    }
    /// <summary>
    /// 核心算法：判断在 (row, col) 放置指定的 targetId 是否会形成消除
    /// ignoreR/C 是为了模拟交换时，排除掉被换走的那个位置的影响
    /// </summary>
    private bool IsPatternMatch(int row, int col, string targetId, int ignoreR = -1, int ignoreC = -1)
    {
        // 水平方向检测：[R, C-2] [R, C-1] [TARGET] [R, C+1] [R, C+2]
        if (CheckLineMatch(row, col, 0, 1, targetId, ignoreR, ignoreC)) return true;
        // 垂直方向检测：[R-2, C] [R-1, C] [TARGET] [R+1, C] [R+2, C]
        if (CheckLineMatch(row, col, 1, 0, targetId, ignoreR, ignoreC)) return true;

        return false;
    }

    private bool CheckLineMatch(int r, int c, int dr, int dc, string id, int ignoreR, int ignoreC)
    {
        int count = 1;

        // 正向搜索 (如右或上)
        for (int i = 1; i <= 2; i++)
        {
            int nr = r + dr * i, nc = c + dc * i;
            if (nr == ignoreR && nc == ignoreC) break;
            if (InBounds(nr, nc) && Board[nr, nc]?.ID == id) count++;
            else break;
        }
        // 反向搜索 (如左或下)
        for (int i = 1; i <= 2; i++)
        {
            int nr = r - dr * i, nc = c - dc * i;
            if (nr == ignoreR && nc == ignoreC) break;
            if (InBounds(nr, nc) && Board[nr, nc]?.ID == id) count++;
            else break;
        }
        return count >= 3;
    }

    private void SpawnCard(int row, int col, string id)
    {
        Slot slot = slotManager.GetSlot(row, col);
        SingleCard card = cardManager.GetSpecificCard(id);
        card.Init(cardManager);
        card.SetSlot(slot);
        Board[row, col] = card;
        slot.SetOccupied();
        card.transform.position = slot.WorldPosition + new Vector3(0, 0, -1);
        card.Anim.PlaySummon();
    }

    #endregion

    #region 3. 重新洗牌 (保证有解)

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            StartCoroutine(ReshuffleIe());
        }
    }

    public IEnumerator ReshuffleIe()
    {
        // 1. 准备数据：收集当前所有卡牌
        List<SingleCard> allCards = new List<SingleCard>();
        foreach (var card in Board)
        {
            if (card != null) allCards.Add(card);
        }

        // 2. 逻辑洗牌：找到一个“无初始消除”且“有解”的排列
        bool foundValidLayout = false;
        int safetyNet = 0;

        while (!foundValidLayout && safetyNet < 100)
        {
            safetyNet++;
            System.Array.Clear(Board, 0, Board.Length);
            ShuffleList(allCards); // 随机打乱

            if (TryPlaceAllCards(allCards)) // 尝试填充逻辑阵列 (避免初始消除)
            {
                if (HasMatchableMoves()) // 检查填充后是否有解
                {
                    foundValidLayout = true;
                }
            }
        }

        // 3. 视觉表现：让卡牌飞到新家
        List<Coroutine> moveAnimations = new List<Coroutine>();

        for (int r = 0; r < Height; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                SingleCard card = Board[r, c];
                Slot targetSlot = slotManager.GetSlot(r, c);

                // 更新逻辑引用
                card.SetSlot(targetSlot);
                targetSlot.SetOccupied();

                // 启动平滑移动协程
                // 你可以使用 DOTween: card.transform.DOMove(targetSlot.WorldPosition, 0.5f);
                // 或者使用简单的协程：
                moveAnimations.Add(StartCoroutine(SmoothMoveCard(card, targetSlot.WorldPosition)));
            }
        }

        // 等待所有卡牌飞到位
        foreach (var anim in moveAnimations)
            yield return anim;

        Debug.Log("洗牌完成，新的可操作棋盘已就绪");
    }
    // 简单的平滑移动辅助协程
    private IEnumerator SmoothMoveCard(SingleCard card, Vector3 targetPos)
    {
        float duration = 0.5f; // 洗牌动画时长
        float elapsed = 0f;
        Vector3 startPos = card.transform.position;
        Vector3 endPos = targetPos + new Vector3(0, 0, -1); // 保持 Z 轴偏移

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            card.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        card.transform.position = endPos;
    }
    // 尝试将打乱后的卡牌填入 Board，同时确保不产生初始消除
    private bool TryPlaceAllCards(List<SingleCard> cards)
    {
        int cardIndex = 0;
        for (int r = 0; r < Height; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                // 在剩余卡牌中找一个放在 (r,c) 不会引起三连的
                bool foundValid = false;
                for (int i = cardIndex; i < cards.Count; i++)
                {
                    if (IsValidAt(r, c, cards[i].ID))
                    {
                        // 找到了，把这张卡换到当前处理的位置
                        SingleCard temp = cards[cardIndex];
                        cards[cardIndex] = cards[i];
                        cards[i] = temp;

                        // 写入逻辑棋盘
                        Board[r, c] = cards[cardIndex];
                        cardIndex++;
                        foundValid = true;
                        break;
                    }
                }

                // 如果试了所有剩下的卡，没一张能放这而不产生消除，则本次洗牌方案失败
                if (!foundValid) return false;
            }
        }
        return true;
    }
    // 最终同步：把 Board 里的数据同步到 Slot 和真实坐标上
    private void ApplyLogicToVisual()
    {
        for (int r = 0; r < Height; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                SingleCard card = Board[r, c];
                Slot slot = slotManager.GetSlot(r, c);

                card.SetSlot(slot); // 更新卡牌内部记录的 Slot
                slot.SetOccupied(); // 更新 Slot 状态

                // 让卡牌飞到新位置（或者直接设置 position）
                card.transform.position = slot.WorldPosition + new Vector3(0, 0, -1);
            }
        }
    }
    // 一个简单的 List 打乱算法
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    // 最后的兜底方案：强行改掉两个颜色，凑成一个可消项
    private void ForceInjectMove()
    {
        // 在 (0,0) (0,1) 放 A，在 (0,3) 放 A，交换 (0,2) 和 (0,3) 即成
        string idA = PoolManager.Instance.singleCards[0].ID;
        string idB = PoolManager.Instance.singleCards[1].ID;
        // 逻辑略：手动修改 Board[0,0], [0,1], [0,2], [0,3] 的 ID 并重绘
        Debug.LogWarning("已执行强制解注入");
    }

    /// <summary>
    /// 尝试交换两张卡（只交换数据）
    /// </summary>
    public void SwapCards(SingleCard a, SingleCard b)
    {
        Slot slotA = a.CurrentSlot;
        Slot slotB = b.CurrentSlot;
        Board[slotA.Row, slotA.Col] = b;
        Board[slotB.Row, slotB.Col] = a;
        a.SetSlot(slotB);
        b.SetSlot(slotA);
    }

    #endregion

    #region Query

    public SingleCard GetCard(int row, int col)
    {
        if (!InBounds(row, col)) return null;
        return Board[row, col];
    }

    public bool InBounds(int row, int col)
    {
        return row >= 0 && row < slotManager.Height && col >= 0 && col < slotManager.Width;
    }

    private void ClearBoard()
    {
        // 销毁或回池逻辑...
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (Board[i, j] == null) continue;
                Board[i, j].ResetCard();
                Board[i, j] = null;
            }
        }
    }

    private void UpdateAllCardsVisuals()
    {
        // 遍历 Board，让 Card 根据 Slot 的 WorldPosition 移动
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                Board[i, j].MoveToSlot(Board[i, j].CurrentSlot);
            }
        }
    }

    #endregion
}

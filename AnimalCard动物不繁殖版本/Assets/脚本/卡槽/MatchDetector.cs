using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 匹配检测器（纯逻辑）
/// 负责：
/// 1. 扫描 Board
/// 2. 找出所有横 / 纵 >=3 的 Match
/// 不负责：销毁、动画、掉落
/// </summary>
public class MatchDetector : MonoBehaviour
{

    private class MatchLine
    {
        public List<SingleCard> cards = new List<SingleCard>();
    }
    public BoardManager boardManager;

    /// <summary>
    /// 找出当前棋盘上的所有匹配卡牌
    /// </summary>
    public List<List<SingleCard>> FindAllMatchGroups()
    {
        List<MatchLine> lines = CollectAllMatchLines();
        return MergeLines(lines);
    }

    private List<MatchLine> CollectAllMatchLines()
    {
        int height = boardManager.Height;
        int width = boardManager.Width;

        List<MatchLine> lines = new List<MatchLine>();

        // ===== 横向 =====
        for (int row = 0; row < height; row++)
        {
            int count = 1;

            for (int col = 1; col <= width; col++)
            {
                bool same =
                    col < width &&
                    boardManager.GetCard(row, col) != null &&
                    boardManager.GetCard(row, col - 1) != null &&
                    boardManager.GetCard(row, col).ID ==
                    boardManager.GetCard(row, col - 1).ID;

                if (same)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        MatchLine line = new MatchLine();
                        for (int i = 0; i < count; i++)
                        {
                            line.cards.Add(
                                boardManager.GetCard(row, col - 1 - i));
                        }
                        lines.Add(line);
                    }
                    count = 1;
                }
            }
        }

        // ===== 纵向 =====
        for (int col = 0; col < width; col++)
        {
            int count = 1;

            for (int row = 1; row <= height; row++)
            {
                bool same =
                    row < height &&
                    boardManager.GetCard(row, col) != null &&
                    boardManager.GetCard(row - 1, col) != null &&
                    boardManager.GetCard(row, col).ID ==
                    boardManager.GetCard(row - 1, col).ID;

                if (same)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        MatchLine line = new MatchLine();
                        for (int i = 0; i < count; i++)
                        {
                            line.cards.Add(
                                boardManager.GetCard(row - 1 - i, col));
                        }
                        lines.Add(line);
                    }
                    count = 1;
                }
            }
        }

        return lines;
    }

    private bool HasIntersection(MatchLine a, MatchLine b)
    {
        foreach (var card in a.cards)
        {
            if (b.cards.Contains(card))
                return true;
        }
        return false;
    }

    private List<List<SingleCard>> MergeLines(List<MatchLine> lines)
    {
        List<List<SingleCard>> result = new List<List<SingleCard>>();
        bool[] used = new bool[lines.Count];

        for (int i = 0; i < lines.Count; i++)
        {
            if (used[i]) continue;

            List<SingleCard> group = new List<SingleCard>();
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(i);
            used[i] = true;

            while (queue.Count > 0)
            {
                int index = queue.Dequeue();

                foreach (var card in lines[index].cards)
                {
                    if (!group.Contains(card))
                        group.Add(card);
                }

                for (int j = 0; j < lines.Count; j++)
                {
                    if (used[j]) continue;

                    if (HasIntersection(lines[index], lines[j]))
                    {
                        used[j] = true;
                        queue.Enqueue(j);
                    }
                }
            }

            result.Add(group);
        }

        return result;
    }



    private void FloodFill(
    int row,
    int col,
    string targetID,
    bool[,] visited,
    List<SingleCard> group)
    {
        int height = boardManager.Height;
        int width = boardManager.Width;

        // 边界
        if (row < 0 || row >= height || col < 0 || col >= width)
            return;

        if (visited[row, col])
            return;

        SingleCard card = boardManager.GetCard(row, col);
        if (card == null || card.ID != targetID)
            return;

        visited[row, col] = true;
        group.Add(card);

        // 四方向
        FloodFill(row + 1, col, targetID, visited, group);
        FloodFill(row - 1, col, targetID, visited, group);
        FloodFill(row, col + 1, targetID, visited, group);
        FloodFill(row, col - 1, targetID, visited, group);
    }

    /// <summary>
    /// 检测指定位置是否形成 Match
    /// </summary>
    public bool HasMatchAt(int row, int col)
    {
        var card = boardManager.Board[row, col];
        if (card == null) return false;

        string id = card.ID;

        // 横向检测
        int count = 1;

        // 左
        for (int c = col - 1; c >= 0; c--)
        {
            var other = boardManager.Board[row, c];
            if (other != null && other.ID == id)
                count++;
            else
                break;
        }

        // 右
        for (int c = col + 1; c < boardManager.Width; c++)
        {
            var other = boardManager.Board[row, c];
            if (other != null && other.ID == id)
                count++;
            else
                break;
        }

        if (count >= 3)
            return true;

        // 纵向检测
        count = 1;

        // 下
        for (int r = row - 1; r >= 0; r--)
        {
            var other = boardManager.Board[r, col];
            if (other != null && other.ID == id)
                count++;
            else
                break;
        }

        // 上
        for (int r = row + 1; r < boardManager.Height; r++)
        {
            var other = boardManager.Board[r, col];
            if (other != null && other.ID == id)
                count++;
            else
                break;
        }

        return count >= 3;
    }

    /// <summary>
    /// 是否存在任意匹配（常用于：判断交换是否有效）
    /// </summary>
    //public bool HasAnyMatch()
    //{
    //    return FindAllMatchGroups().Count > 0;
    //}
}


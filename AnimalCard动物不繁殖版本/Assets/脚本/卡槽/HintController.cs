using UnityEngine;
using DG.Tweening;

/// <summary>
/// 提示控制器：查找可交换消除并播放提示动画
/// </summary>
public class HintController : MonoBehaviour
{
    public BoardManager board;
    public MatchDetector matchDetector;
    private const string HINT_TWEEN_ID = "HintTween";

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            ShowHint();
        }
    }

    /// <summary>
    /// 对外调用：显示一次提示
    /// </summary>
    public bool ShowHint()
    {
        if (TryFindHint(out var a, out var b))
        {
            PlayHintAnim(a, b);
            PlayHintAnim(b, a);
            return true;
        }

        Debug.Log("当前棋盘无可消除提示");
        return false;
    }

    #region 查找逻辑

    private bool TryFindHint(out SingleCard cardA, out SingleCard cardB)
    {
        int w = board.Width;
        int h = board.Height;

        for (int row = 0; row < h; row++)
        {
            for (int col = 0; col < w; col++)
            {
                var card = board.Board[row, col];
                if (card == null) continue;

                // 只尝试 右 / 上，避免重复
                if (TrySwapCheck(row, col, row, col + 1, out cardA, out cardB))
                    return true;

                if (TrySwapCheck(row, col, row + 1, col, out cardA, out cardB))
                    return true;
            }
        }

        cardA = null;
        cardB = null;
        return false;
    }

    private bool TrySwapCheck(
        int r1, int c1,
        int r2, int c2,
        out SingleCard cardA,
        out SingleCard cardB)
    {
        cardA = null;
        cardB = null;

        if (!board.InBounds(r2, c2))
            return false;

        var a = board.Board[r1, c1];
        var b = board.Board[r2, c2];

        if (a == null || b == null)
            return false;

        // 模拟交换（只换数据）
        Swap(r1, c1, r2, c2);

        bool hasMatch =
            matchDetector.HasMatchAt(r1, c1) ||
            matchDetector.HasMatchAt(r2, c2);

        // 换回来
        Swap(r1, c1, r2, c2);

        if (hasMatch)
        {
            cardA = a;
            cardB = b;
            return true;
        }

        return false;
    }

    private void Swap(int r1, int c1, int r2, int c2)
    {
        var temp = board.Board[r1, c1];
        board.Board[r1, c1] = board.Board[r2, c2];
        board.Board[r2, c2] = temp;
    }

    #endregion

    #region 动画

    private void PlayHintAnim(SingleCard card)
    {
        card.transform.DOKill();

        card.transform
            .DOScale(1.5f, 0.15f)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void PlayHintAnim(SingleCard from, SingleCard to)
    {
        Transform t = from.transform;
        // 1. 杀死该物体上所有 ID 为 HINT_TWEEN_ID 的动画
        DOTween.Kill(t, true);

        Vector3 originPos = from.CurrentSlot.transform.position;
        Vector3 dir = (to.CurrentSlot.transform.position - originPos).normalized;
        Vector3 offset = dir * 0.3f;

        // 2. 赋予特定的 ID
        t.DOMove(originPos + offset, 0.15f)
         .SetLoops(4, LoopType.Yoyo)
         .SetEase(Ease.InOutSine)
         .SetId(HINT_TWEEN_ID)
         .OnKill(() => {
             t.position = originPos;
         });
    }

    public void StopHints(SingleCard a, SingleCard b)
    {
        // 3. 通过 ID 杀死动画，并要求立即完成（触发 OnKill 回位）
        // complete: true 确保它执行 OnKill 里的坐标重置
        DOTween.Kill(a.transform, true);
        DOTween.Kill(b.transform, true);
    }

    public void StopAllHints()
    {
        for (int r = 0; r < board.Height; r++)
        {
            for (int c = 0; c < board.Width; c++)
            {
                var card = board.Board[r, c];
                if (card == null) continue;

                card.transform.DOKill();
                card.transform.position = card.transform.position; // 保持当前
            }
        }
    }

    public void StopHints()
    {
        // 杀死所有 ID 为 "HintTween" 的动画
        // 第二个参数传 true，确保触发 OnKill 里的坐标回位逻辑
        DOTween.Kill("HintTween", true);
    }


    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋盘流程控制器（消消乐主循环）
/// </summary>
public class BoardFlowController : MonoBehaviour
{
    [Header("Refs")]
    public BoardManager boardManager;
    public MatchDetector matchDetector;
    public ClearController clearController;
    public DropController dropController;


    private bool _isBusy;
    public bool IsBusy => _isBusy;

    #region Public API

    /// <summary>
    /// 玩家请求交换两张卡
    /// </summary>
    public void TrySwap(SingleCard a, SingleCard b)
    {
        //a是玩家滑动的卡，b是目标卡
        if (_isBusy) return;

        StartCoroutine(SwapRoutine(a, b));
    }

    #endregion

    #region Core Flow

    private IEnumerator SwapRoutine(SingleCard a, SingleCard b)
    {
        _isBusy = true;
        
        // 1. 交换数据
        yield return StartCoroutine(SwapIe(a, b));

        // 2. 检测是否形成 Match
        var matches = matchDetector.FindAllMatchGroups();
        if (matches.Count == 0)
        {
            // 交换无效 → 换回
            yield return StartCoroutine(SwapIe(a, b));
            _isBusy = false;
            yield break;
        }
        // 3. 进入消消乐主循环
        yield return StartCoroutine(ProcessMatches(matches));

        _isBusy = false;
    }

    /// <summary>
    /// 连锁消除主循环
    /// </summary>
    private IEnumerator ProcessMatches(List<List<SingleCard>>  initialMatches = null)
    {
        // 使用一个局部变量来接管当前的匹配项
        var currentMatches = initialMatches;
        while (true)
        {
            // 如果外部没传，或者是第二轮及以后的循环，则需要自己检测
            if (currentMatches == null)
            {
                currentMatches = matchDetector.FindAllMatchGroups();
            }
            // --- 修改点：如果没有匹配了，说明当前阶段结束 ---
            if (currentMatches == null || currentMatches.Count == 0)
            {
                // 检查棋盘是否还有可能移动的步骤
                if (!boardManager. HasMatchableMoves())
                {
                    Debug.Log("<color=yellow>检测到死局，开始自动洗牌...</color>");
                    // 执行协程洗牌，等待它完成
                    yield return StartCoroutine(boardManager.ReshuffleIe());

                    // 洗牌后，棋盘状态变了，我们不直接退出，而是 continue 
                    // 回到循环开头再检查一次匹配（虽然洗牌逻辑避开了初始消除，但这样写更稳健）
                    continue;
                }

                // 既没有匹配，又有解，这才是真正的逻辑终点
                yield break;
            }

            // 2. 播放消除动画（并行）
            yield return StartCoroutine(CardDisappearIe(currentMatches));

            List<SingleCard> allCards = new List<SingleCard>();

            foreach (var group in currentMatches)
            {
                allCards.AddRange(group);
            }
            // 3. 清理棋盘数据
            clearController.ClearCards(allCards);

            // 4. 下落（等待动画完成）
            bool dropped = false;
            yield return StartCoroutine(
                dropController.DropIe(result => dropped = result)
            );
            // --- 重点：本轮消除和下落处理完了，清空变量以便下一轮重新检测 ---
            currentMatches = null;
            // 6. 如果既没下落也没补牌，说明稳定了
            //如果既没下落也没变动，其实在上面的 FindAllMatches 就会跳出，但为了严谨可以保留
            if (!dropped && matchDetector.FindAllMatchGroups().Count == 0)// && !refilled)
                yield break;

            // 否则继续 while，检测新的 Match
        }
    }

    /// <summary>
    /// 交换动画协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwapIe(SingleCard a, SingleCard b)
    {
        // 1. 交换数据
        boardManager.SwapCards(a, b);
        bool aDone = false;
        bool bDone = false;

        // 同时启动
        StartCoroutine(a.MoveToSlot(a.CurrentSlot, () => aDone = true));
        StartCoroutine(b.MoveToSlot(b.CurrentSlot, () => bDone = true));

        // 等两个都完成
        yield return new WaitUntil(() => aDone && bDone);
    }

    private IEnumerator CardDisappearIe(List<List<SingleCard>>  singleCards)
    {
        if (singleCards == null || singleCards.Count == 0)
            yield break;

        int finishedCount = 0;
        int total = 0;
        foreach (var group in singleCards)
            total += group.Count;


        for (int i = 0; i < singleCards.Count; i++)
        {
            for (int j = 0; j < singleCards[i].Count; j++)
            {
                StartCoroutine(DisappearWrapper(singleCards[i][0].CurrentSlot, singleCards[i][j], () =>
                {
                    finishedCount++;
                }));
            }
            
        }

        // 等所有卡牌都消失
        yield return new WaitUntil(() => finishedCount >= total);
        for (int i = 0; i < singleCards.Count; i++)
        {
            Vector3 startPos = BattleManager.instance.PlayerRole.transform.position;
            for (int j = 0; j < singleCards[i].Count; j++)
            {
                StartCoroutine(singleCards[i][0].Use(singleCards[i].Count, startPos, RoleEnum.Self));
            }

        }
        
    }

    private IEnumerator DisappearWrapper(Slot targerSlot, SingleCard card, Action onFinished)
    {
        if (card == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        yield return card.Merge(targerSlot);
        onFinished?.Invoke();
    }

    #endregion
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    private SlotManager _slotManager;

    [HideInInspector]public Role SelfRole;//自己
    [HideInInspector] public Role TargetRole;//对方


    public readonly int SummonCardCoin = 10;//召唤一张卡牌所需要的金币数量

    public List<SingleCard> AllCards = new List<SingleCard>();

    public static readonly Color32 NormalColor = new Color32(0, 0, 0, 0);
    public static readonly Color32 SecondColor = new Color32(229, 201, 140, 255);
    public static readonly Color32 ThirdColor = new Color32(247, 232, 104, 255);

    private List<SingleCard> _hintCards = new();//十字交叉的所有卡

    public bool CanInteract
    {
        get
        {
            foreach (var card in AllCards)
            {
                if (!card.canInteract)
                    return false;
            }
            return true;
        }
    }

    public SingleCard IsClickingCard;//当前正在点击的卡

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="selfRole"></param>
    /// <param name="targetRole"></param>
    /// <param name="slotManager"></param>
    public void Init(Role selfRole,Role targetRole, SlotManager slotManager)
    {
        SelfRole = selfRole;
        TargetRole = targetRole;
        _slotManager=slotManager;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void ResetCardManager()
    {
        SelfRole=null;
        TargetRole=null;
        _slotManager = null;
        IsClickingCard=null;

        for (int i = AllCards.Count-1; i >=0; i--)
        {
            AllCards[i].ResetCard();
        }
        AllCards.Clear();
    }

    /// <summary>
    /// 填充所有的卡槽
    /// </summary>
    public void FillSlots(int level=1)
    {
        bool isNpc = SelfRole == BattleManager.instance.NpcRoleControl.MyRole;
        for (int i = 0; i < _slotManager.SlotCount; i++)
        {
            SingleCard card = GetRandomCard();
            Slot slot = _slotManager.GetSlot(i);
            //Debug.Log($"card is {card.name},slot is {slot.name}");
            card.Init(this, level: level);
            card.MoveToSlot(slot);
            AllCards.Add(card);
        }
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (AllCards.Count < 2) return;

            int indexA = Random.Range(0, AllCards.Count);
            int indexB;

            do
            {
                indexB = Random.Range(0, AllCards.Count);
            }
            while (indexB == indexA);

            SingleCard a = AllCards[indexA];
            SingleCard b = AllCards[indexB];

            a.DestroyThis();
            b.DestroyThis();
        }
    }

    #region Get Card

    public SingleCard GetRandomCard()
    {
        return PoolManager.Instance.GetRandomCard();
    }

    public void AddCard(SingleCard card)
    {
        AllCards.Add(card);
    }

    public void RemoveCard(SingleCard card)
    {
        if (AllCards.Remove(card))
        {
            
        }
    }

    /// <summary>
    /// 改变被点击的卡牌
    /// </summary>
    public void ChangeCardClick(SingleCard card)
    {
        if (IsClickingCard == card) return;
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
            AddCard(singleCard);

            return singleCard;
        }
        return null;

        
    }

    /// <summary>
    /// 获取鼠标悬停时所在的那个卡，不需要判断两个卡牌的种类是否一致
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public SingleCard FindMergeTargetForPreview(SingleCard card)
    {
        if (TryFindCardOverSlot(card, out Slot slot))
        {
            SingleCard targetCard = GetCardBySlot(slot);
            return targetCard;
        }
        return null;
    }
    /// <summary>
    /// 通过卡槽获取那张卡
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public SingleCard GetCardBySlot(Slot slot)
    {
        for (int i = 0; i < AllCards.Count; i++)
        {
            if (AllCards[i].CurrentSlot == slot)
            {
                return AllCards[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 查找有没有两个一样的卡牌
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool TryGetSameIDCards(
    out (SingleCard, SingleCard) result)
    {
        result = default;

        Dictionary<string, SingleCard> cache = new Dictionary<string, SingleCard>();

        foreach (var card in AllCards)
        {
            string id = card.ID;

            if (cache.TryGetValue(id, out var exist))
            {
                result = (exist, card);
                return true;
            }
            cache.Add(id, card);
        }

        return false;
    }


    #endregion

    #region Drag Result
    /// <summary>
    /// 处理松开手指
    /// </summary>
    /// <param name="card"></param>
    public void HandleRelease(SingleCard card)
    {
        Debug.Log("尝试合并");

        // 默认行为：回到原 Slot
        Slot targetSlot = card.CurrentSlot;

        // 1. 是否在某个 Slot 上
        if (TryFindCardOverSlot(card, out Slot slot))
        {
            SingleCard targetCard = GetCardBySlot(slot);

            if (targetCard == null)
            {
                // 空 Slot → 直接放过去
                targetSlot = slot;
            }
            else
            {
                // 有卡 → 尝试合成
                bool merged = TryMerge(card, targetCard);

                if (merged)
                {
                    // 合成成功，TryMerge 内部已处理
                    return;
                }
                // 合成失败 → 维持默认回原位
            }
        }

        // 2. 统一执行移动
        card.MoveToSlot(targetSlot);
    }

    /// <summary>
    /// 尝试找出卡牌是否在某个slot上
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    private bool TryFindCardOverSlot(SingleCard card,out Slot slot)
    {
        slot = null;
        for (int i = 0; i < _slotManager.allSlots.Count; i++)
        {
            var col = _slotManager.GetSlot(i).Collider;
            if (col == null || !col.enabled) continue;

            if (col.OverlapPoint(card.transform.position))
            {
                slot = _slotManager.GetSlot(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 尝试将 card 合并到某个目标上（不关心是鼠标还是代码）
    /// </summary>
    public bool TryMerge(SingleCard from, SingleCard to)
    {
        if (from == null || to == null) return false;
        if (from == to) return false;
        if (from.ID != to.ID) return false;
        if (from.CurrentLevel != to.CurrentLevel) return false;

        // 真正的合成
        DoMerge(from, to);
        return true;
    }
    //真正的合成
    private void DoMerge(SingleCard from, SingleCard to)
    {
        // 约定：from 吃掉 to（或者反过来，你定）
        from.ClearSlot();
        from.DestroyThisImmediately();

        to.SetLevel(to.CurrentLevel+ from.CurrentLevel);

        // 满级触发效果
        //to.Use();
    }

    #endregion

    #region 排列
    //重新排列所有的卡牌
    public void RearrangeAllCards()
    {
        for (int i = 0; i < AllCards.Count; i++)
        {
            if (AllCards[i].CurrentSlot == _slotManager.GetSlot(i)) continue;
            AllCards[i].MoveToSlot(_slotManager.GetSlot(i));
        }
    }

    public void TryMerge(SingleCard centerCard)
    {
        StopAllCrossHint(); //核心入口清场

        if (!CanMerge(centerCard))
        {
            PlayCrossHint(centerCard);
            return;
        }
        while (CanMerge(centerCard))
        {
            List<List<SingleCard>> allDirectionSlots = GetCrossCardLines(centerCard);//四个方向的卡牌
            for (int i = 0; i < allDirectionSlots.Count; i++)
            {
                if (allDirectionSlots[i].Count == 0) continue;
                if (allDirectionSlots[i][0].ID == centerCard.ID)
                {
                    SingleCard card = allDirectionSlots[i][0];
                    card.MoveToSlot(centerCard.CurrentSlot, () =>
                    {
                        Slot tmp = centerCard.CurrentSlot;
                        card.DestroyThisImmediately();
                        centerCard.SetLevel(centerCard.CurrentLevel + 1);
                        centerCard.MoveToSlot(tmp);
                    });
                    allDirectionSlots[i].RemoveAt(0);
                    for (int j = 0; j < allDirectionSlots[i].Count; j++)
                    {
                        Slot targetSlot = GetStepSlotTowards(allDirectionSlots[i][j].CurrentSlot, centerCard.CurrentSlot);
                        allDirectionSlots[i][j].MoveToSlot(targetSlot);
                    }
                    List<Slot> emptySlots = _slotManager.GetAllEmptySlots();
                    for (int k = 0; k < emptySlots.Count; k++)
                    {
                        Debug.Log($"空的名称为newCard{emptySlots[k].name}");
                        SingleCard newCard = GetRandomCard();
                        newCard.transform.position = emptySlots[k].transform.position;
                        newCard.Init(this, 1);
                        newCard.MoveToSlot(emptySlots[k]);
                        newCard.Anim.Appear();
                        AllCards.Add(newCard);
                    }
                }
            }
        }

       
    }
    //获取上下左右四个方向的所有卡牌
    public List<List<SingleCard>> GetCrossCardLines(SingleCard centerCard)
    {
        var result = new List<List<SingleCard>>();

        var slot = centerCard.CurrentSlot;
        int row = slot.Row;
        int col = slot.Col;

        CollectLine(-1, 0); // 上
        CollectLine(1, 0);  // 下
        CollectLine(0, -1); // 左
        CollectLine(0, 1);  // 右

        return result;

        void CollectLine(int dr, int dc)
        {
            List<SingleCard> line = new();

            int r = row + dr;
            int c = col + dc;

            while (IsValid(r, c))
            {
                var s = _slotManager.GetSlot(r, c);
                var card = GetCardBySlot(s);

                if (card != null)
                {
                    line.Add(card);
                }

                r += dr;
                c += dc;
            }

            if (line.Count > 0)
                result.Add(line);
        }
    }
    //从from往to方向走一格得到的Slot
    public Slot GetStepSlotTowards(Slot from, Slot to)
    {
        int dx = to.Col - from.Col;
        int dy = to.Row - from.Row;

        // 只允许十字方向
        if (dx != 0 && dy != 0)
            return null;

        int stepCol = from.Col + Mathf.Clamp(dx, -1, 1);
        int stepRow = from.Row + Mathf.Clamp(dy, -1, 1);

        if (!IsValid(stepRow, stepCol))
            return null;

        return _slotManager.GetSlot(stepRow, stepCol);
    }

    bool CanMerge(SingleCard center)
    {
        var slot = center.CurrentSlot;
        int r = slot.Row;
        int c = slot.Col;


        return Check(r - 1, c) ||   // 上
               Check(r + 1, c) ||   // 下
               Check(r, c - 1) ||   // 左
               Check(r, c + 1);     // 右

        bool Check(int row, int col)
        {
            if (!IsValid(row, col)) return false;

            var otherSlot = _slotManager.GetSlot(row, col);
            var otherCard = GetCardBySlot(otherSlot);

            return center.ID== otherCard.ID;
        }
    }

    //处理“上方塌陷”
    void CollapseUp(Slot centerSlot)
    {
        int col = centerSlot.Col;

        for (int r = centerSlot.Row - 1; r >= 0; r--)
        {
            var fromSlot = _slotManager.GetSlot(r, col);
            var toSlot = _slotManager.GetSlot(r + 1, col);

            var card = GetCardBySlot(fromSlot);
            if (card == null) continue;

            card.MoveToSlot(toSlot);
        }
    }
    void CollapseDown(Slot centerSlot)
    {
        int col = centerSlot.Col;

        // 从中心下面一格开始，向下遍历
        for (int r = centerSlot.Row + 1; r < _slotManager.Height; r++)
        {
            var fromSlot = _slotManager.GetSlot(r, col);
            var toSlot = _slotManager.GetSlot(r - 1, col);

            var card = GetCardBySlot(fromSlot);
            if (card == null) continue;

            card.MoveToSlot(toSlot);
        }
    }
    void CollapseLeft(Slot centerSlot)
    {
        int row = centerSlot.Row;

        // 从中心左边一格开始，向左遍历
        for (int c = centerSlot.Col - 1; c >= 0; c--)
        {
            var fromSlot = _slotManager.GetSlot(row, c);
            var toSlot = _slotManager.GetSlot(row, c + 1);

            var card = GetCardBySlot(fromSlot);
            if (card == null) continue;

            card.MoveToSlot(toSlot);
        }
    }
    void CollapseRight(Slot centerSlot)
    {
        int row = centerSlot.Row;

        // 从中心右边一格开始，向右遍历
        for (int c = centerSlot.Col + 1; c < _slotManager.Width; c++)
        {
            var fromSlot = _slotManager.GetSlot(row, c);
            var toSlot = _slotManager.GetSlot(row, c - 1);

            var card = GetCardBySlot(fromSlot);
            if (card == null) continue;

            card.MoveToSlot(toSlot);
        }
    }


    void CollapseCross(SingleCard centerCard)
    {
        Slot c = centerCard.CurrentSlot;

        CollapseUp(c);
        CollapseDown(c);
        CollapseLeft(c);
        CollapseRight(c);
    }
    bool TryMergeCenter(SingleCard centerCard)
    {
        var sameCards = GetSameTypeAroundCenter(centerCard);

        if (sameCards.Count == 0)
            return false;

        // 吸到中心
        foreach (var card in sameCards)
        {
            card.MoveToSlot(centerCard.CurrentSlot);
            RemoveCard(card);
        }

        centerCard.SetLevel(centerCard.CurrentLevel + 1);
        return true;
    }

    List<SingleCard> GetSameTypeAroundCenter(SingleCard center)
    {
        List<SingleCard> result = new();
        var s = center.CurrentSlot;

        Check(s.Row - 1, s.Col);
        Check(s.Row + 1, s.Col);
        Check(s.Row, s.Col - 1);
        Check(s.Row, s.Col + 1);

        return result;

        void Check(int r, int c)
        {
            if (!IsValid(r, c)) return;

            var slot = _slotManager.GetSlot(r, c);
            var card = GetCardBySlot(slot);

            if (card != null && card.ID == center.ID)
                result.Add(card);
        }
    }
    public void PlayCrossHint(SingleCard centerCard)
    {

        var centerPos = centerCard.transform.position;
        float pullDistance = 0.2f;
        float duration = 0.15f;

        var cards = GetAllCrossCards(centerCard);

        // 记录当前 Hint 涉及的卡
        _hintCards.Clear();
        _hintCards.AddRange(cards);

        foreach (var card in cards)
        {
            Transform tf = card.transform;

            // 2记录原始位置
            Vector3 originPos = tf.position;

            // 3Kill 自己的旧 Hint Tween
            if (card.Anim.HintTween != null && card.Anim.HintTween.IsActive())
            {
                card.Anim.HintTween.Kill();
                tf.position = originPos;
            }

            // 4计算方向
            Vector3 dir = (centerPos - originPos).normalized;
            Vector3 target = originPos + dir * pullDistance;

            // 5创建 Sequence
            Sequence seq = DOTween.Sequence();

            for (int i = 0; i < 3; i++)
            {
                seq.Append(tf.DOMove(target, duration));
                seq.Append(tf.DOMove(originPos, duration));
            }

            // 6兜底回原位
            seq.OnKill(() => tf.position = originPos);
            seq.OnComplete(() => tf.position = originPos);

            card.Anim.HintTween = seq;
        }
    }

    private void StopAllCrossHint()
    {
        foreach (var card in _hintCards)
        {
            if (card == null) continue;

            if (card.Anim.HintTween != null && card.Anim.HintTween.IsActive())
            {
                card.Anim.HintTween.Kill();
            }
        }

        _hintCards.Clear();
    }

   
    List<SingleCard> GetAllCrossCards(SingleCard center)
    {
        List<SingleCard> result = new();
        var s = center.CurrentSlot;

        // 上
        for (int r = s.Row - 1; r >= 0; r--)
            Add(r, s.Col);

        // 下
        for (int r = s.Row + 1; r < _slotManager.Height; r++)
            Add(r, s.Col);

        // 左
        for (int c = s.Col - 1; c >= 0; c--)
            Add(s.Row, c);

        // 右
        for (int c = s.Col + 1; c < _slotManager.Width; c++)
            Add(s.Row, c);

        return result;

        void Add(int r, int c)
        {
            var slot = _slotManager.GetSlot(r, c);
            var card = GetCardBySlot(slot);
            if (card != null)
                result.Add(card);
        }
    }
    bool IsValid(int row, int col)
    {
        return row >= 0 &&
               row < _slotManager.Height &&
               col >= 0 &&
               col < _slotManager.Width;
    }
    #endregion

}

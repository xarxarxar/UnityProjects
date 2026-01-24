using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardManager : MonoBehaviour
{

    [Header("可生成卡牌 Prefab 列表")]
    public List<SingleCard> AllCards; // 直接存不同卡牌 prefab
    private SlotManager _slotManager;

    [HideInInspector]public Role SelfRole;//自己
    [HideInInspector] public Role TargetRole;//对方

    public readonly int SummonCardCoin = 10;//召唤一张卡牌所需要的金币数量

    private readonly List<SingleCard> _cards = new List<SingleCard>();

    public bool CanInteract
    {
        get
        {
            foreach (var card in _cards)
            {
                if (!card.canInteract)
                    return false;
            }
            return true;
        }
    }


    public void Init(Role selfRole,Role targetRole, SlotManager slotManager)
    {
        SelfRole = selfRole;
        TargetRole = targetRole;
        _slotManager=slotManager;
    }


    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (_cards.Count < 2) return;

            int indexA = Random.Range(0, _cards.Count);
            int indexB;

            do
            {
                indexB = Random.Range(0, _cards.Count);
            }
            while (indexB == indexA);

            SingleCard a = _cards[indexA];
            SingleCard b = _cards[indexB];

            a.DestroyThis();
            b.DestroyThis();
        }
    }

    #region Get Card

    public void GetCard()
    {
        if (AllCards == null || AllCards.Count == 0) return;

        Slot slot = _slotManager.GetFirstEmptySlot();
        if (slot == null)
        {
            Debug.Log("没有位置");
            return;
        }

        // 随机选择一个 prefab
        int index = Random.Range(0, AllCards.Count);
        SingleCard prefab = AllCards[index];

        // 生成新的卡牌实例
        SingleCard card = Instantiate(prefab);
        card.Init(this); // ID 直接继承 prefab 的 ID
        //card.MoveToSlot(slot);

        AddCard(card);
    }

    private void AddCard(SingleCard card)
    {
        _cards.Add(card);
        RearrangeAllCards();
    }

    public void RemoveCard(SingleCard card)
    {
        if (_cards.Remove(card))
        {
            RearrangeAllCards();
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
        Slot slot = _slotManager.GetFirstEmptySlot();
        if (slot == null)
        {
            Debug.Log("没有位置");
            return null;
        }
        AddCard(singleCard);

        return singleCard;
    }

    /// <summary>
    /// 获取鼠标悬停时所在的那个卡，不需要判断两个卡牌的种类是否一致
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public SingleCard FindMergeTargetForPreview(SingleCard card)
    {
        foreach (var other in _cards)
        {
            if (other == card) continue;

            float distance = Vector3.Distance(card.transform.position, other.transform.position);
            if (distance < 0.5f)
                return other;
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

        foreach (var card in _cards)
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
        var target = FindMergeTarget(card);

        if (target != null && TryMerge(card, target))
            return;

        // 合不成就回原位
        card.MoveToSlot(card.CurrentSlot);
    }

    /// <summary>
    /// 尝试将 card 合并到某个目标上（不关心是鼠标还是代码）
    /// </summary>
    public bool TryMerge(SingleCard from, SingleCard to)
    {
        if (from == null || to == null) return false;
        if (from == to) return false;
        if (from.ID != to.ID) return false;

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

        to.AddLevel(from.CurrentLevel);

        // 满级触发效果
        to.Use();
    }

    public bool TryAutoMerge()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            for (int j = i + 1; j < _cards.Count; j++)
            {
                var a = _cards[i];
                var b = _cards[j];

                if (a.ID == b.ID)
                {
                    b.MoveToSlot(a.CurrentSlot);
                    TryMerge(a, b);
                    return true;
                }
            }
        }
        return false;
    }

    //public void TryMergeOrReturn(SingleCard card)
    //{
    //    SingleCard target = FindMergeTarget(card);

    //    if (target != null)
    //    {
    //        Merge(card, target);
    //    }
    //    else
    //    {
    //        card.MoveToSlot(card.CurrentSlot);
    //    }
    //}

    private SingleCard FindMergeTarget(SingleCard card)
    {
        foreach (var other in _cards)
        {
            if (other == card) continue;
            if (other.ID != card.ID) continue;

            float distance = Vector3.Distance(card.transform.position, other.transform.position);
            if (distance < 0.5f) // 合并阈值
                return other;
        }
        return null;
    }

    #endregion

    #region Merge

    private void Merge(SingleCard from, SingleCard to)
    {
        to.AddLevel(from.CurrentLevel);

        from.ClearSlot();
        RemoveCard(from);
        from.DestroyThisImmediately();

        if (to.CanUse())
        {
            UseCard(to);
        }

        RearrangeAllCards();
    }

    private void UseCard(SingleCard card)
    {
        card.OnCardEffect();

        card.ClearSlot();
        RemoveCard(card);
        card.DestroyThisImmediately();
        
    }

    #endregion

    #region Shift

    private void RearrangeAllCards()
    {
        StopAllCoroutines();
        StartCoroutine(RearrangeCoroutine());
    }

    private IEnumerator RearrangeCoroutine()
    {
        // 清空 Slot
        for (int i = 0; i < _slotManager.SlotCount; i++)
            _slotManager.GetSlot(i).SetUnOccupied();

        // 同时移动
        for (int i = 0; i < _cards.Count; i++)
        {
            bool hasAnyMove = false;
            Slot slot = _slotManager.GetSlot(i);

            if (_cards[i].CurrentSlot != slot) // 关键判断
            {
                hasAnyMove = true;
                _cards[i].MoveToSlot(slot);
            }
            // 只有真的动了才等
            if (hasAnyMove)
                yield return new WaitForSeconds(SingleCardAnim.MoveAnimTime/2);
        }

    }
    #endregion
}

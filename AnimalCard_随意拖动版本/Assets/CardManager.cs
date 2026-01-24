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
            var col = _slotManager.allSlots[i].Collider;
            if (col == null || !col.enabled) continue;

            if (col.OverlapPoint(card.transform.position))
            {
                slot = _slotManager.allSlots[i];
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


    #endregion

}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Reflection;
using System.Data;

public abstract class SingleCard : MonoBehaviour
{
    [Header("Data")]
    public string ID;
    public int CurrentLevel = 1;
    public bool canInteract = true;
    protected CardManager _cardManager;

    public TouchHandler2D TouchHandler;

    public  Role selfRole;
    protected Role targetRole;
    
    public Slot CurrentSlot { get; private set; }

    private Vector3 _dragOffset;
    private bool _isDragging;
    private SingleCard _previewTarget;//即将要合并的那张目标卡

    private ObjectPool<SingleCard> _objectPool;

    private bool CanDrag => selfRole== BattleManager.instance.PlayerRoleControl.MyRole
        && canInteract && _cardManager != null && _cardManager.CanInteract;



    private Coroutine CountDownCoro;
    public static event Action<SingleCard> OnCardCoutDown;//卡牌倒计时结束

    public SingleCardAnim Anim { get; private set; }
    private void Awake()
    {
        Anim = GetComponent<SingleCardAnim>();
    }

    #region Init

    public void Init(CardManager cardManager,int level=1)
    {
        _cardManager = cardManager;
        selfRole=_cardManager.SelfRole;
        targetRole = _cardManager.TargetRole;
        canInteract = true;
        SetLevel(level);
        if (TouchHandler == null)
        {
            TouchHandler=GetComponent<TouchHandler2D>();
        }
        TouchHandler.OnClick += OnClick;
        TouchHandler.OnDoubleClick += OnDoubleClick;
        TouchHandler.OnDragBegin += OnDragBegin;
        TouchHandler.OnDragging += OnDragging;
        TouchHandler.OnDragEnd += OnDragEnd;
    }

    /// <summary>
    /// 重置卡片
    /// </summary>
    public void ResetCard()
    {
        Debug.Log("重置卡牌");
        DestroyThisImmediately();
        _cardManager = null;
        selfRole = null;
        targetRole = null;
        canInteract = false;
        
        TouchHandler.OnClick -= OnClick;
        TouchHandler.OnDoubleClick -= OnDoubleClick;
        TouchHandler.OnDragBegin -= OnDragBegin;
        TouchHandler.OnDragging -= OnDragging;
        TouchHandler.OnDragEnd -= OnDragEnd;
        if (CountDownCoro != null) { StopCoroutine(CountDownCoro); CountDownCoro = null; }
        
    }
    #endregion

    #region Slot
    /// <summary>
    /// 移动到一个卡槽
    /// </summary>
    /// <param name="slot"></param>
    public void MoveToSlot(Slot slot,UnityAction onEnd=null)
    {
        canInteract = false;

        Vector3 from = Anim.ShownTransform.position;
        Vector3 to = slot.transform.position-new Vector3(0,0,0.1f);

        if (CurrentSlot != null)
            CurrentSlot.SetUnOccupied();

        CurrentSlot = slot;
        CurrentSlot.SetOccupied();

        transform.position = to;

        Anim.PlayMove(from,to,
            onStart: () =>
            {
            },
            onComplete: () =>
            {
                canInteract = true;
                Anim.SetDefaultLayer(); //动画结束后才恢复层级
                onEnd?.Invoke();
            });
    }

    /// <summary>
    /// 设置该卡对应的对象池
    /// </summary>
    /// <param name="objectPool"></param>
    public void SetPool(ObjectPool<SingleCard> objectPool)
    {
        _objectPool = objectPool;
    }

    public void ClearSlot()
    {
        if (CurrentSlot != null)
        {
            CurrentSlot.SetUnOccupied();
            CurrentSlot = null;
        }
    }

    /// <summary>
    /// 让移动动画瞬间完成
    /// </summary>
    public void FinishMoveImmediately()
    {
        Anim.FinishImmediately();

        if (CurrentSlot != null)
            Anim.ShownTransform.position = CurrentSlot.transform.position;

        canInteract = true;
    }

    #endregion

    #region Drag

    private void OnClick()
    {
        _cardManager.ChangeCardClick(this);

        _cardManager.TryMerge(this);
    }

    private void OnDoubleClick()
    {
        Use();
    }

    private void OnDragBegin(UnityEngine.EventSystems.PointerEventData eventData)
    {

        if (!CanDrag) return;
        if (CurrentSlot == null) return;
        _cardManager.ChangeCardClick(this);
        _isDragging = true;
        Anim.SetDraggingLayer(); //提升 Sorting Layer
        // 直接使用 eventData.position 进行坐标转换和位移
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = transform.position.z;
        _dragOffset = transform.position - worldPos;
    }

    private void OnDragging(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!_isDragging || !CanDrag) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(eventData.position);
        mouseWorld.z = transform.position.z;
        transform.position = mouseWorld + _dragOffset;

        SingleCard newTarget = _cardManager.FindMergeTargetForPreview(this);

        // ===== 自己的高亮 =====
        if (newTarget == null)
        {
            Anim.SetMaskTransparent(CurrentLevel);
        }
        else
        {
            Anim.SetMaskMergeable(newTarget.ID == ID);
        }


        // ===== 处理 target 的高亮切换 =====
        if (_previewTarget != newTarget)
        {
            // 取消旧 target 的高亮
            if (_previewTarget != null)
                _previewTarget.Anim.SetMaskTransparent(_previewTarget.CurrentLevel);

            // 设置新 target 的高亮
            if (newTarget != null)
            {
                newTarget.Anim.SetMaskMergeable(newTarget.ID == ID && newTarget.CurrentLevel==CurrentLevel);
            }

            _previewTarget = newTarget;
        }
    }
    private void OnDragEnd(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;
        Anim.SetMaskTransparent(CurrentLevel); //松手就清 Mask
        if (_previewTarget != null)
        {
            _previewTarget.Anim.SetMaskTransparent(_previewTarget.CurrentLevel);
            _previewTarget = null;
        }
        if (!CanDrag)
        {
            MoveToSlot(CurrentSlot);
            return;
        }

        _cardManager.HandleRelease(this);
    }
    #endregion

    #region Logic

    public void SetLevel(int level)
    {
        if(level<0) level = 1;
        if (level > 3) level = 3;
        CurrentLevel = level;
        Anim.SetMaskTransparent(CurrentLevel);
    }
    //使用
    public void Use()
    {
        //是敌人使用
        if(selfRole== BattleManager.instance.NpcRoleControl.MyRole)
        {
            //直接触发效果
            OnCardEffect();
            ClearSlot();
            _cardManager.RemoveCard(this);
            DestroyThisImmediately();
            _cardManager.RearrangeAllCards();//敌人要自动排序
            return;
        }
        
        //否则就是玩家使用
        for(int i = 0; i < BattleManager.instance.NpcRoleControl.MyCardManager.AllCards.Count; i++)
        {
            if (BattleManager.instance.NpcRoleControl.MyCardManager.AllCards[i].ID==ID 
                && BattleManager.instance.NpcRoleControl.MyCardManager.AllCards[i].CurrentLevel== CurrentLevel)
            {
                (BattleManager.instance.NpcRoleControl as NpcControl).RemoveCardInDic(BattleManager.instance.NpcRoleControl.MyCardManager.AllCards[i]) ;
                BattleManager.instance.NpcRoleControl.MyCardManager.AllCards[i].DestroyThisImmediately();
                BattleManager.instance.NpcRoleControl.MyCardManager.RearrangeAllCards();//敌人要自动排序

                //PlayerManager.instance.NpcRoleControl.MyCardManager.AllCards.RemoveAt(i);
                
                OnCardEffect();
                ClearSlot();
                _cardManager.RemoveCard(this);
                DestroyThisImmediately();
                return;
            }
        }
       
    }

    public abstract void OnCardEffect();

    /// <summary>
    /// 摧毁这个
    /// </summary>
    public void DestroyThisImmediately()
    {
        ClearSlot();
        _cardManager.RemoveCard(this);
        _objectPool.Return(this);
    }

    public void DestroyThis()
    {
        canInteract=false;
        Anim.PlayDisappear(() =>
        {
            ClearSlot();
            _cardManager. RemoveCard(this);
            DestroyThisImmediately();
        });
    }

    #endregion
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;
using System.Collections;

public abstract class SingleCard : MonoBehaviour
{
    [Header("Data")]
    public string ID;
    //public int CurrentLevel = 1;
    public bool canInteract = true;
    public CardManager _cardManager;

    public TouchHandler2D TouchHandler;

    public  Role selfRole;
    protected Role targetRole;
    
    public Slot CurrentSlot { get; private set; }

    private ObjectPool<SingleCard> _objectPool;

    private Coroutine CountDownCoro;
    public event Action<SingleCard> OnCardDisappear;//卡牌消失

    public SingleCardAnim Anim { get; private set; }
    private void Awake()
    {
        Anim = GetComponent<SingleCardAnim>();
    }
    #region Init

    public void Init(CardManager cardManager)
    {
        _cardManager = cardManager;
        
        canInteract = true;
        if (TouchHandler == null)
        {
            TouchHandler=GetComponent<TouchHandler2D>();
        }
        TouchHandler.OnClick -= OnClick;
        TouchHandler.OnClick += OnClick;
        TouchHandler.OnSwipe -= OnSwipe;
        TouchHandler.OnSwipe += OnSwipe;
    }

    /// <summary>
    /// NPC专用Init方法
    /// </summary>
    public void Init()
    {
        if (TouchHandler == null)
        {
            TouchHandler = GetComponent<TouchHandler2D>();
        }
        canInteract = false;
    }

    /// <summary>
    /// 重置卡片
    /// </summary>
    public void ResetCard()
    {
        Debug.Log("重置卡牌");

        //DestroyThisImmediately();
        Anim.SetClick(false);
        _cardManager = null;
        selfRole = null;
        targetRole = null;
        canInteract = false;

        Debug.Log($"TouchHandler==null{TouchHandler == null}");
        TouchHandler.OnClick -= OnClick;
        TouchHandler.OnSwipe -= OnSwipe;
        if (CountDownCoro != null) { StopCoroutine(CountDownCoro); CountDownCoro = null; }
        
    }

    public void SetSlot(Slot slot)
    {
        CurrentSlot = slot;
        CurrentSlot.SetOccupied();
    }
    #endregion

    #region Slot

    /// <summary>
    /// 移动到一个卡槽（协程版，可 yield 等待）
    /// </summary>
    public IEnumerator MoveToSlot(Slot slot,Action callback=null)
    {
        Vector3 from = Anim.ShownTransform.position;
        Vector3 to = slot.transform.position - new Vector3(0, 0, 0.1f);

        bool finished = false;

        transform.position = to;

        Anim.PlayMove(from, to,
            onStart: () =>
            {
            },
            onComplete: () =>
            {
                Anim.SetDefaultLayer(); // 动画结束后才恢复层级
                finished = true;
                callback?.Invoke();
            });

        // 等动画结束
        yield return new WaitUntil(() => finished);
    }
    public IEnumerator Merge(Slot targetSlot, Action callback = null)
    {
        yield return MoveToSlot(targetSlot);
        DestroyThisImmediately();
        callback?.Invoke();
    }


    /// <summary>
    /// 卡牌消失动画
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator Diasppear(Action callback = null)
    {
        bool finished = false;
        Anim.PlayDisappear(() =>
        {
            DestroyThisImmediately();
            finished= true;
            callback?.Invoke();
        });
        // 等动画结束
        yield return new WaitUntil(() => finished);
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
        //Use();
    }

    private void OnSwipe(Vector2Int dir)
    {
        Debug.Log("滑动");
        _cardManager.OnSwipe(this,dir);
    }

    #endregion

    #region Logic
    //使用
    public IEnumerator Use(int level,Vector3 startPos,RoleEnum roleEnum)
    {
        Debug.Log($"使用卡牌,level is {level}");
        if (roleEnum == RoleEnum.Self)
        {
            selfRole = BattleManager.instance.PlayerRole;
            targetRole= BattleManager.instance.NpcRole;
            BattleManager.instance.NpcControl.RequestRemoveCard(ID, useCard: false);
            yield return OnCardEffect(startPos, level-2);
        }
        if(roleEnum == RoleEnum.Npc)
        {
            selfRole = BattleManager.instance.NpcRole;
            targetRole = BattleManager.instance.PlayerRole;      
            
            yield return OnCardEffect(startPos,1);
        }

    }

    public abstract IEnumerator OnCardEffect(Vector3 startPos,int level);

    /// <summary>
    /// 摧毁这个
    /// </summary>
    public void DestroyThisImmediately()
    {
        ResetCard();
        OnCardDisappear?.Invoke(this);
        _objectPool.Return(this);
    }

    public void DestroyThis(Action callback=null)
    {
        canInteract=false;
        Anim.PlayDisappear(() =>
        {
            ClearSlot();
            
            DestroyThisImmediately();
            callback?.Invoke();
        });
    }

    #endregion
}

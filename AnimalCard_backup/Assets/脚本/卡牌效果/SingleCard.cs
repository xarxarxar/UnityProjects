using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public abstract class SingleCard : MonoBehaviour
{
    [Header("Data")]
    public string ID;
    public int CurrentLevel = 1;
    public const int UseLevel = 3;
    public bool canInteract = true;
    protected CardManager _cardManager;

    protected Role selfRole;
    protected Role targetRole;

    [Header("UI")]
    public Text LevelText;
    
    public Slot CurrentSlot { get; private set; }

    private Camera _cam;
    private Vector3 _dragOffset;
    private bool _isDragging;
    private SingleCard _previewTarget;//即将要合并的那张目标卡

    private ObjectPool<SingleCard> _objectPool;

    private bool CanDrag => selfRole==PlayerManager.instance.selfRoleControl.MyRole&& canInteract && _cardManager != null && _cardManager.CanInteract;

    public SingleCardAnim Anim { get; private set; }
    private void Awake()
    {
        _cam = Camera.main;
        Anim = GetComponent<SingleCardAnim>();
    }

    #region Init

    public void Init(CardManager cardManager)
    {
        _cardManager = cardManager;
        selfRole=_cardManager.SelfRole;
        targetRole=_cardManager.TargetRole;
        canInteract = true;
        CurrentLevel = 1;
        RefreshUI();
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
        Vector3 to = slot.transform.position;

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

    private void OnMouseDown()
    {
        if (!CanDrag) return;
        if (CurrentSlot == null) return;

        _isDragging = true;

        Anim.SetDraggingLayer(); //提升 Sorting Layer

        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        _dragOffset = transform.position - mouseWorld;
    }

    private void OnMouseDrag()
    {
        if (!_isDragging || !CanDrag) return;

        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + _dragOffset;

        SingleCard newTarget = _cardManager.FindMergeTargetForPreview(this);

        // ===== 自己的高亮 =====
        if (newTarget == null)
        {
            Anim.SetMaskTransparent();
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
                _previewTarget.Anim.SetMaskTransparent();

            // 设置新 target 的高亮
            if (newTarget != null)
            {
                newTarget.Anim.SetMaskMergeable(newTarget.ID == ID);
            }

            _previewTarget = newTarget;
        }
    }

    private void OnMouseUp()
    {
        if (!_isDragging) return;

        _isDragging = false;

        Anim.SetMaskTransparent(); //松手就清 Mask

        if (_previewTarget != null)
        {
            _previewTarget.Anim.SetMaskTransparent();
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

    public void AddLevel(int add)
    {
        CurrentLevel += add;
        RefreshUI();
    }

    public bool CanUse()
    {
        return CurrentLevel >= UseLevel;
    }

    public void Use()
    {
        if (CanUse())
        {
            OnCardEffect();

            _cardManager.RemoveCard(this);
            DestroyThisImmediately();
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
        Destroy(gameObject);
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

    #region UI

    private void RefreshUI()
    {
        LevelText.text = CurrentLevel > 1 ? CurrentLevel.ToString() : "";
    }

    #endregion
}

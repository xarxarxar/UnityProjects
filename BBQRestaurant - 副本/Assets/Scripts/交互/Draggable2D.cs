using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Draggable2D : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    private Collider2D selfCollider;
    private SpriteRenderer spriteRenderer;//用于控制食材保持在最上层
    private int originalSortingOrder;
    private Vector3 originalPosition;
    public bool isLocked = false;//是否不让拖动
    // --- 双击相关 ---
    private Coroutine clickCoroutine = null;
    public float doubleClickThreshold = 0.15f; // 两次点击判定时间（秒）

    [Header("食材层（用于检测食材重叠）")]
    public LayerMask ingredientLayer;
    [Header("顾客层（用于检测顾客重叠）")]
    public LayerMask customerLayer;
    [Header("垃圾桶层（用于检测垃圾桶重叠）")]
    public LayerMask trashLayer;

    [Header("可被点击的层（过滤用）")]
    public LayerMask draggableLayer;

    [Header("拖动时提高层级")]
    public int dragSortingOrderOffset = 100;

    private Ingredient ingredient = null;


    private void Awake()
    {
        selfCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ingredient=GetComponent<Ingredient>();

        if (spriteRenderer != null)
            originalSortingOrder = spriteRenderer.sortingOrder;
    }

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        isDragging = false;
        isLocked = false;
        if(clickCoroutine != null)
        {
            StopCoroutine(clickCoroutine);
            clickCoroutine = null;
        }
    }


    //处理玩家的输入，也就是手指拖动
    private void HandleInput()
    {
        // ------------------
        // 暂停控制
        // ------------------
        if (LevelManager.Instance.isPaused)
        {
            if (isDragging)
            {
                isDragging = false;
                if (spriteRenderer != null)
                    spriteRenderer.sortingOrder = originalSortingOrder;

                OnRelease(); // 强制释放
            }
            return; // 暂停状态下完全不允许拖拽
        }

        // 支持触摸（优先）和鼠标
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
            worldPos.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                TryBeginDrag(worldPos);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (isDragging) 
                {
                    transform.position = worldPos + offset;
                    ingredient?.OnDropping();
                } 
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (isDragging)
                {
                    isDragging = false;
                    if (spriteRenderer != null) spriteRenderer.sortingOrder = originalSortingOrder;
                    OnRelease();
                }
            }

            return; // 如果有 touch，跳过鼠标逻辑
        }

        // --- 鼠标逻辑（用于编辑器或 PC） ---
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            TryBeginDrag(worldPos);
        }

        if (isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;
                transform.position = worldPos + offset;
                //拖拽中事件
                ingredient?.OnDropping();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                if (spriteRenderer != null) spriteRenderer.sortingOrder = originalSortingOrder;

                OnRelease();
            }
        }
    }

    private void TryBeginDrag(Vector3 worldPos)
    {
        if (isLocked) return;  // 回弹时禁止拖拽
        transform.DOKill();

        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
        Collider2D hit = Physics2D.OverlapPoint(worldPos2D, draggableLayer);

        if (hit != selfCollider) return;

        // 如果当前没有正在等待的点击协程 -> 启动等待协程，确认是单击还是双击
        if (clickCoroutine == null)
        {
            clickCoroutine = StartCoroutine(ClickRoutine(worldPos));
        }
        else
        {
            // 已经有等待协程，说明这是第二次点击（双击）
            // 取消等待协程并触发双击回调（不进入拖拽）
            StopCoroutine(clickCoroutine);
            clickCoroutine = null;

            // 触发双击（Ingredient 中实现）
            ingredient?.OnDoubleClick();
        }
    }



    private void OnRelease()
    {
        if (ingredient == null)
            return;

        // 检测放置的区域，并得到具体目标
        Collider2D target;
        DropResult result = DetectDropArea(out target);

        // 是否与其他食材重叠（使用你现有的 CollisionHelper2D）
        bool overlapping = CollisionHelper2D.IsOverlapping(selfCollider, ingredientLayer);

        // 回调 Ingredient，让食材根据状态决定行为
        ingredient.OnEndDrop(result, target, overlapping);
    }

    /// <summary>
    /// 检查手指松开时位于哪个区域，并返回区域的 Collider
    /// </summary>
    public DropResult DetectDropArea(out Collider2D targetArea)
    {
        targetArea = null;
        Collider2D col = selfCollider;

        // 检查多个烤盘（要求完全在内）
        if (AreaManager.Instance.grillAreas != null)
        {
            foreach (var grill in AreaManager.Instance.grillAreas)
            {
                if (grill != null && CollisionHelper2D.IsCompletelyInside(col, grill))
                {
                    targetArea = grill;
                    return DropResult.OnGrill;
                }
            }
        }

        // 检查垃圾桶（允许部分重叠）
        if (AreaManager.Instance.trashAreas != null)
        {
            if (CollisionHelper2D.IsOverlapping(col, trashLayer))
            {
                targetArea = AreaManager.Instance.trashAreas;
                return DropResult.OnTrash;
            }
        }

        // 检查顾客（允许部分重叠）
        if (AreaManager.Instance.customerAreas != null)
        {
            
            Collider2D nearestCustomer = null;
            float nearestDist = float.MaxValue;

            Vector2 ingredientPos = col.transform.position;

            foreach (var customer in AreaManager.Instance.customerAreas)
            {
                
                if (customer == null) continue;
                
                // 先要确认确实有重叠或接触
                if (CollisionHelper2D.IsOverlapping(col, customerLayer))
                {
                    Debug.Log("接触顾客");
                    // 计算中心点距离
                    float dist = Vector2.Distance(ingredientPos, customer.transform.position);

                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearestCustomer = customer;
                    }
                }
            }

            if (nearestCustomer != null)
            {
                targetArea = nearestCustomer;
                return DropResult.OnCustomer;
            }
        }

        return DropResult.None;
    }

    /// <summary>
    /// 状态逻辑可能需要把食材放回原位，提供这个方法供外部调用
    /// </summary>
    public void ReturnToOriginal()
    {
        isLocked = true;  // ← 锁定，禁止拖拽

        transform.DOMove(originalPosition, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                isLocked = false; // ← 动画结束后解锁
            });
    }

    /// <summary>
    /// 状态逻辑可能需要把食材放回原位，提供这个方法供外部调用
    /// </summary>
    public void MoveToPos(Vector3 pos)
    {
        isLocked = true;  // ← 锁定，禁止拖拽

        transform.DOMove(pos, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                isLocked = false; // ← 动画结束后解锁
                ingredient?.OnReturnBelt();
            });
    }


    /// <summary>
    /// 点击协程：等待 doubleClickThreshold 秒，若期间没有第二次点击则认定为单击并开始拖拽
    /// </summary>
    private IEnumerator ClickRoutine(Vector3 initialWorldPos)
    {
        float elapsed = 0f;

        // 等待期间仍然要响应可能的取消（比如 isLocked 或对象被销毁）
        while (elapsed < doubleClickThreshold)
        {
            if (isLocked || this == null) // 如果锁定或对象销毁则退出
            {
                clickCoroutine = null;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 超时未发生第二次点击 -> 视为单击，开始拖拽
        clickCoroutine = null;


        // ------------------- 关键修改区 -------------------
        if (ingredient != null && ingredient.canDrag == false)
        {
            ingredient.OnBeltDrag(initialWorldPos);

            yield break;
        }

        // ------------------- 原始可拖动物体流程 -------------------
        BeginDragImmediate(initialWorldPos);
    }

    // ----------------------------------------------------------------------
    // 将开始拖拽的逻辑封装（供新克隆食材立即调用）
    // ----------------------------------------------------------------------
    public void BeginDragImmediate(Vector3 initialPos)
    {
        isDragging = true;
        originalPosition = transform.position;
        offset = transform.position - initialPos;

        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = originalSortingOrder + dragSortingOrderOffset;

        ingredient?.OnStartDrop();   // 注意：克隆出来的才会触发
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler2D : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // --- 新增滑动事件 ---
    // Vector2Int 会返回 (1,0)右, (-1,0)左, (0,1)上, (0,-1)下
    public event Action<Vector2Int> OnSwipe;

    [Header("滑动配置")]
    public float swipeThreshold = 50f; // 屏幕像素超过这个值就触发滑动
    private bool swipeTriggered = false;   // 保证一次拖拽只触发一次滑动逻辑
    // --- 定义所有对外暴露的事件 ---
    public event Action OnClick;                          // 单击
    public event Action OnDoubleClick;                    // 双击
    public event Action OnLongPressStart;                 // 长按开始
    public event Action OnLongPressStay;                  // 长按中
    public event Action OnLongPressEnd;                   // 长按抬起
    public event Action<PointerEventData> OnDragBegin;    // 开始拖拽
    public event Action<PointerEventData> OnDragging;     // 拖拽中
    public event Action<PointerEventData> OnDragEnd;       // 结束拖拽

    [Header("配置")]
    public float longPressThreshold = 0.6f;


    [SerializeField] private bool isPointerDown = false;
    [SerializeField] private bool longPressTriggered = false;
    [SerializeField] private float pointerDownTimer = 0f;
    private Coroutine clickTimerCoroutine;
    private readonly float doubleClickThreshold = 0.2f; // 双击判定的缓冲时间

    void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= longPressThreshold)
            {
                longPressTriggered = true;
                OnLongPressStart?.Invoke();
            }
        }
        if (longPressTriggered) OnLongPressStay?.Invoke();
    }

    // --- 接口实现 ---

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击");
        // 如果是第一次点击
        if (eventData.clickCount == 1)
        {
            // 开启一个协程，等待一段时间后再决定是否触发单击
            clickTimerCoroutine = StartCoroutine(SingleClickRoutine());
        }
        // 如果在等待期间触发了第二次点击
        else if (eventData.clickCount == 2)
        {
            // 1. 停止之前的等待协程（核心：防止单击逻辑被执行）
            if (clickTimerCoroutine != null)
            {
                StopCoroutine(clickTimerCoroutine);
            }

            // 2. 立即执行双击逻辑
            OnDoubleClick?.Invoke();
            Debug.Log("检测到双击，已拦截单击");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
        longPressTriggered = false;
        swipeTriggered = false; // 按下时重置滑动判定
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (longPressTriggered) OnLongPressEnd?.Invoke();
        ResetState();
    }

    public void OnPointerExit(PointerEventData eventData) => ResetState();

    public void OnBeginDrag(PointerEventData eventData)
    {
        ResetState(); // 拖拽开始，中断长按计时
        OnDragBegin?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragging?.Invoke(eventData);

        // --- 消消乐滑动逻辑检测 ---
        if (!swipeTriggered)
        {
            Vector2 delta = eventData.position - eventData.pressPosition;

            if (delta.magnitude > swipeThreshold)
            {
                swipeTriggered = true;
                Vector2Int direction = CalculateDirection(delta);
                OnSwipe?.Invoke(direction);
                //Debug.Log($"向 {direction} 方向滑动");
            }
        }
    }
    // 根据向量判断上下左右
    private Vector2Int CalculateDirection(Vector2 delta)
    {
        // 比较 X 和 Y 的绝对值，看是在哪个轴上移动更多
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            // 横向滑动
            return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            // 纵向滑动
            return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
    }

    public void OnEndDrag(PointerEventData eventData) => OnDragEnd?.Invoke(eventData);

    private void ResetState()
    {
        isPointerDown = false;
        longPressTriggered = false;
        pointerDownTimer = 0f;
    }

    // 单击的延迟执行协程
    private IEnumerator SingleClickRoutine()
    {
        // 等待设定的缓冲时间
        yield return new WaitForSeconds(doubleClickThreshold);

        // 时间到了，说明没有第二次点击进来，执行单击
        OnClick?.Invoke();
        Debug.Log("检测到单击");

        clickTimerCoroutine = null;
    }
}

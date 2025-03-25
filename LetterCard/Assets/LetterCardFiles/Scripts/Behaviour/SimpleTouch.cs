using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SimpleTouch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("长按触发时间阈值，单位：秒")]
    [Tooltip("长按触发时间阈值，单位：秒")]
    public float holdTime = 1f; // 长按触发时间阈值，单位：秒
    [Header("双击最大间隔时间，单位：秒")]
    [Tooltip("双击最大间隔时间，单位：秒")]
    public float doubleClickTime = 0.3f; // 双击最大间隔时间

    private Tween holdTween;
    private bool isHolding = false;  // 长按标志
    private bool waitingForSecondClick = false;
    private float lastClickTime = 0;//上次点击时间
    

    /// <summary>
    /// 单击操作
    /// </summary>
    public UnityAction onClick;

    /// <summary>
    /// 双击操作
    /// </summary>
    public UnityAction onDoubleClick;

    /// <summary>
    /// 长按操作
    /// </summary>
    public UnityAction onLongPress;

    /// <summary>
    /// 开始拖拽操作
    /// </summary>
    public UnityAction<PointerEventData> onBeginDrag;

    /// <summary>
    /// 拖拽中操作
    /// </summary>
    public UnityAction<PointerEventData> onDrag;

    /// <summary>
    /// 停止拖拽操作
    /// </summary>
    public UnityAction<PointerEventData> onEndDrag;

    //点击
    // 单击事件
    private void OnClick()
    {
        onClick?.Invoke();
    }

    // 双击事件
    private void OnDoubleClick()
    {
        onDoubleClick?.Invoke();
    }

    // 长按事件
    private void OnLongPress()
    {
        onLongPress?.Invoke();
    }

    // 在按下时处理单击、双击和长按逻辑
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        // 启动 DOTween 来检测长按
        if (holdTween != null)
        {
            holdTween.Kill(); // 如果有正在进行的长按检测动画，停止它
        }

        holdTween = DOVirtual.DelayedCall(holdTime, () =>
        {
            if (!isHolding)
            {
                isHolding = true;
                OnLongPress(); // 触发长按事件
            }
        });


    }

    // 延时触发单击事件
    private void TriggerSingleClick()
    {
        
        if (waitingForSecondClick)
        {
            OnClick();  // 单击事件
            waitingForSecondClick = false; // 清空等待状态
        }
    }

    // 在松开时处理长按逻辑
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isHolding)
        {
            isHolding = false;
            return;
        }

        float currentTime = Time.time;
        // 如果在双击时间窗口内检测到第二次点击
        if (waitingForSecondClick && currentTime - lastClickTime < doubleClickTime)
        {
            OnDoubleClick();  // 双击事件
            waitingForSecondClick = false; // 清空等待状态
        }
        else
        {
            // 否则记录第一次点击时间，并开始等待第二次点击
            waitingForSecondClick = true;
            Invoke("TriggerSingleClick", doubleClickTime); // 在 doubleClickTime 时间后触发单击事件
        }
        lastClickTime = currentTime;

        if (holdTween != null)
        {
            holdTween.Kill(); // 松开时停止长按检测动画
            holdTween = null;
        }
    }

    // 拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            onDrag?.Invoke(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }
}

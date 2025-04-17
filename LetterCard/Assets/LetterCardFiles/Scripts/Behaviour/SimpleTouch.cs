using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SimpleTouch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    /// <summary>
    /// 单击操作
    /// </summary>
    public UnityAction onClick;


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


    //这个必须要有，要不然OnPointerUp不执行
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    // 在按下时处理单击逻辑
    public void OnPointerUp(PointerEventData eventData)
    {
        onClick?.Invoke();
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

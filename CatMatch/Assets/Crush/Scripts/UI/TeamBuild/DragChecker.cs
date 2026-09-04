using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragChecker : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool onDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onDrag = true;
        Debug.Log("OnBeginDrag:" + onDrag);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag = true;
        Debug.Log("OnDrag:" + onDrag);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDrag = false;
        Debug.Log("OnEndDrag:" + onDrag);
    }
}

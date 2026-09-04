using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragUI : MonoBehaviour, IDragHandler
{
    public RectTransform dragObject;
    public RectTransform dragArea;

    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;

    private RectTransform dragObjectInternal
    {
        get
        {
            if (dragObject == null)
                return (transform as RectTransform);
            else
                return dragObject;
        }
    }

    private RectTransform dragAreaInternal
    {
        get
        {
            if (dragArea == null)
            {
                RectTransform canvas = transform as RectTransform;
                while (canvas.parent != null && canvas.parent is RectTransform)
                {
                    canvas = canvas.parent as RectTransform;
                }
                return canvas;
            }
            else
                return dragArea;
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        originalPanelLocalPosition = dragObjectInternal.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragAreaInternal, data.position, data.pressEventCamera, out originalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragAreaInternal, data.position, data.pressEventCamera, out localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
            dragObjectInternal.localPosition = originalPanelLocalPosition + offsetToOriginal;
        }
    }
}

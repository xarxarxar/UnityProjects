using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonBase : Button, IPointerDownHandler, IPointerUpHandler
{

    public bool isOtherTarget = false;
    private Transform target;

    void Awake()
    {
        if (isOtherTarget)
        {
            target = transform.GetChild(0);
        }
        else
        {
            target = transform;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        target.transform.DOKill();
        target.transform.DOScale(0.95f, 0.1f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData data)
    {
        target.transform.DOKill();
        target.transform.DOScale(1f, 0.4f).SetEase(Ease.OutElastic).SetUpdate(true);
    }
}

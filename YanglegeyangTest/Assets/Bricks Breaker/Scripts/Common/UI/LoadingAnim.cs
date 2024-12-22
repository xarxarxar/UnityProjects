using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingAnim : MonoBehaviour
{
    public CanvasGroup canvasGroupLoading;
    public Transform loading;

    public void SetLoading(bool value)
    {
        if (value)
        {
            canvasGroupLoading.gameObject.SetActive(true);
            loading.transform.DORotate(new Vector3(0f, 0f, -360f), 1f).SetRelative(true).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart).SetUpdate(true);
            canvasGroupLoading.DOKill();
            canvasGroupLoading.DOFade(1f, 0.25f).SetEase(Ease.OutSine).SetUpdate(true).SetUpdate(true);
        }
        else
        {
            canvasGroupLoading.DOKill();
            canvasGroupLoading.DOFade(0f, 0.15f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                loading.transform.DOKill();
                canvasGroupLoading.gameObject.SetActive(false);
            }).SetUpdate(true);
        }
    }
}

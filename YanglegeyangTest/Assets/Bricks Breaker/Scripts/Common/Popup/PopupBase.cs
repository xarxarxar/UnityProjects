
using UnityEngine;
using DG.Tweening;

public class PopupBase : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public RectTransform popup;


    public void Open()
    {
        PlayManager.Instance.isPopupOn = true;
        canvasGroup.DOFade(1f, 0.15f).SetEase(Ease.OutCubic);
        popup.DOScale(1f, 0.15f).SetEase(Ease.OutCubic);
    }

    public virtual void Close()
    {
        PlayManager.Instance.isPopupOn = false;
        Destroy(this.gameObject);
    }
}

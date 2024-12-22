using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RocketArea : MonoBehaviour, IPointerClickHandler
{


    public Image[] arrow;
    public Image backImage;

    public void OnEnable()
    {
        backImage.DOFade(0.5f, 0f);
        backImage.DOFade(1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        StartCoroutine(SetAreaCo());
    }


    IEnumerator SetAreaCo()
    {
        for (int i = arrow.Length - 1; i >= 0; i--)
        {
            arrow[i].DOFade(0.75f, 0.35f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SetAreaCo());
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}

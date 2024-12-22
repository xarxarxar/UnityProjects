using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class CommonUI : MonoBehaviour
{
    public TextMeshProUGUI textToast;
    public RectTransform toast;
    public GetItem _GetItem;
    public CoinGem _CoinGem;
    public Image fade;
    Coroutine coroutine;

    public GameObject eventSystem;

    public CanvasGroup canvasGroupLoading;
    public Transform loading;

    private void Awake()
    {

        if (PlayManager.Instance.isFade)
        {
            FadeOut(0f);
        }

        UIReset();
    }

    /// <summary>
    /// Initialize all UI.
    /// </summary>
    void UIReset()
    {
        toast.DOAnchorPosY(200f, 0f);
        canvasGroupLoading.DOFade(0f, 0f);
    }



    private void Start()
    {
        if (PlayManager.Instance.isFade)
        {
            PlayManager.Instance.isFade = fade;
            FadeIn();
        }
    }



    public void FadeOut(float time = 0.25f)
    {
        fade.gameObject.SetActive(true);
        fade.DOKill();
        fade.DOFade(1f, time).SetEase(Ease.OutCubic);
    }

    public void FadeIn(float time = 0.25f)
    {
        fade.DOKill();
        fade.DOFade(0f, time).SetEase(Ease.OutCubic).OnComplete(() => { fade.gameObject.SetActive(false); });
    }



    /// <summary>
    /// Enable toast popup
    /// </summary>
    public void SetToast(string info, float time = 1.5f)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            toast.DOKill();
            toast.DOAnchorPosY(200f, 0f);
        }

        textToast.text = info;
        toast.DOAnchorPosY(0f, 0.2f).SetEase(Ease.OutBack);
        coroutine = StartCoroutine(SetToastCo());
    }

    IEnumerator SetToastCo()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        toast.DOAnchorPosY(200f, 0.25f).SetEase(Ease.OutCubic);
    }





    /// <summary>
    /// Loading Animation On Off
    /// </summary>
    public void Loading(bool value)
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

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetToast("Text", 2f);
        }
    }
#endif
}

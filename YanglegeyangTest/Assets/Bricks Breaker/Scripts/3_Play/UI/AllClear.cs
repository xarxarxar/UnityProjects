using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AllClear : MonoBehaviour
{

    public Image textAllClear;
    public TextMeshProUGUI textScore;

    /// <summary>
    /// Initialize all UI.
    /// </summary>
    public void UIReset()
    {
        transform.DOScale(0f, 0f);
        textAllClear.transform.DOScale(0f, 0f);
        textScore.transform.DOScale(0f, 0f);
    }

    public void Show()
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        this.gameObject.SetActive(true);
        textScore.text = string.Format("+{0}", 100);

    }

    IEnumerator ShowCo()
    {
        yield return new WaitForSeconds(0.1f);
        textScore.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);

    }

    public void Hide()
    {
        textScore.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }
}

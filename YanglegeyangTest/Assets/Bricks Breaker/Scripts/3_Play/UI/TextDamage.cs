using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextDamage : MonoBehaviour
{
    private TextMeshProUGUI textDamage;
    private TextMeshProUGUI textDamageCri;
    private TextMeshProUGUI textDamageMagic;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textDamage = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textDamageCri = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        textDamageMagic = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

    }

    private void Reset()
    {
        textDamage.gameObject.SetActive(false);
        textDamageCri.gameObject.SetActive(false);
        textDamageMagic.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
        transform.DOKill();

        canvasGroup.transform.DOKill();
        canvasGroup.transform.DOScale(0f, 0f);
        canvasGroup.DOFade(1f, 0f);
    }

    public void Damage(int damage, bool isCritical = false, int num = 0)
    {
        Reset();
        this.gameObject.SetActive(true);

        if (isCritical)
        {
            textDamageCri.text = damage.ToString();
            textDamageCri.gameObject.SetActive(true);


            canvasGroup.transform.DOKill();
            canvasGroup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, 0.15f).SetEase(Ease.Linear).SetDelay(0.5f).OnComplete(() => { OffTextEffect(); });
            transform.DOLocalMoveY(55f, 0.15f).SetEase(Ease.OutCubic).SetRelative(true);
        }
        else
        {
            if (num == 0)
            {
                textDamage.text = damage.ToString();
                textDamage.gameObject.SetActive(true);
                textDamageMagic.gameObject.SetActive(false);
            }
            else
            {
                textDamageMagic.text = damage.ToString();
                textDamage.gameObject.SetActive(false);
                textDamageMagic.gameObject.SetActive(true);
            }

            canvasGroup.transform.DOKill();
            canvasGroup.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, 0.15f).SetEase(Ease.Linear).SetDelay(0.5f).OnComplete(() => { OffTextEffect(); });

            transform.DOLocalMoveY(55f, 0.15f).SetEase(Ease.OutCubic).SetRelative(true);
        }
    }

    void OffTextEffect()
    {
        Reset();
        this.gameObject.SetActive(false);
        //PoolManager.Despawn(this.gameObject);
    }
}

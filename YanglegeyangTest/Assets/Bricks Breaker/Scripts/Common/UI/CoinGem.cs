using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CoinGem : MonoBehaviour
{
    public TextMeshProUGUI textCoin;
    public TextMeshProUGUI textGem;
    bool isCoinAnim = false;
    bool isGemAnim = false;



    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void SetCoin()
    {
        textCoin.text = Utility.ChangeThousandsSeparator(GameData.Coin);
    }

    public void SetCoin(int value)
    {
        StartCoroutine(SetCoinCo(value));
    }

    IEnumerator SetCoinCo(int coinValue)
    {
        int count = GameData.Coin - coinValue;
        float time = 0.5f;

        DOTween.To(() => count, x => count = x, GameData.Coin, time).SetEase(Ease.Linear);

        while (time > 0)
        {
            time -= Time.deltaTime;
            textCoin.text = Utility.ChangeThousandsSeparator(count);
            yield return null;
        }

        textCoin.text = GameData.Coin.ToString();
    }

    public void SetGem()
    {
        textGem.text = Utility.ChangeThousandsSeparator(GameData.Gem);
    }

    public void SetGem(int value)
    {
        StartCoroutine(SetGemCo(value));
    }

    IEnumerator SetGemCo(int gemValue)
    {
        int count = GameData.Gem - gemValue;
        float time = 0.5f;

        DOTween.To(() => count, x => count = x, GameData.Gem, time).SetEase(Ease.Linear);

        while (time > 0)
        {
            time -= Time.deltaTime;
            textGem.text = Utility.ChangeThousandsSeparator(count);
            yield return null;
        }

        textGem.text = GameData.Gem.ToString();
    }


    public void SetSmallSize()
    {
        GetComponent<CanvasGroup>().DOKill();
        GetComponent<CanvasGroup>().DOFade(0.5f, 0.25f).SetEase(Ease.OutCubic);

        transform.DOKill();
        transform.DOScale(0.9f, 0.25f).SetEase(Ease.OutCubic);
    }

    public void SetLargeSize()
    {
        GetComponent<CanvasGroup>().DOKill();
        GetComponent<CanvasGroup>().DOFade(1f, 0.25f).SetEase(Ease.OutCubic);

        transform.DOKill();
        transform.DOScale(1f, 0.2f).SetEase(Ease.InOutCubic);
    }
}

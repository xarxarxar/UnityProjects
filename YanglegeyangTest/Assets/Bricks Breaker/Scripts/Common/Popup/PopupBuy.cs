using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Networking;

public class PopupBuy : MonoBehaviour
{
    public Sprite[] spriteCoin;
    public Sprite[] spriteGem;
    public Sprite[] spriteIcon;

    public Image imageIcon;
    public Image imageCoin;
    public Image imageBall;
    public GameObject coinGroup;
    public GameObject ballSkinGroup;


    public TextMeshProUGUI textValue;
    public TextMeshProUGUI textTitle;

    public Transform popup;
    public CanvasGroup canvasGroup;
    public Button buttonOK;


    /// <summary>
    /// Initialize all UI.
    /// </summary>
    public void UIReset()
    {
        buttonOK.onClick.RemoveAllListeners();
        buttonOK.onClick.AddListener(() => { Click_Cancel(); });
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0f);
        popup.transform.DOKill();
        popup.transform.DOScale(0f, 0f);
        this.gameObject.SetActive(false);

        ballSkinGroup.SetActive(false);
        imageIcon.gameObject.SetActive(false);
        coinGroup.SetActive(false);
    }

    /// <summary>
    /// If coin is selected, coin information is displayed.
    /// </summary>
    public void SetCoin(int num, int value)
    {
        coinGroup.SetActive(true);

        textTitle.text = "Get Coins?";
        imageCoin.sprite = spriteCoin[num];
        imageIcon.SetNativeSize();

        textValue.text = Utility.ChangeThousandsSeparator(value);

        Open();
    }


    /// <summary>
    /// //If gem is selected, display gem information
    /// </summary>
    public void SetGem(int num, int value)
    {
        coinGroup.SetActive(true);

        textTitle.text = "Get Gems?";
        imageCoin.sprite = spriteCoin[num];
        imageIcon.SetNativeSize();

        textValue.text = Utility.ChangeThousandsSeparator(value);

        Open();
    }

    /// <summary>
    /// //Shows icon information when icon is selected
    /// </summary>
    public void SetIcon(int num)
    {
        imageIcon.gameObject.SetActive(true);

        textTitle.text = "Get?";

        imageIcon.sprite = spriteIcon[num];
        imageIcon.SetNativeSize();

        Open();
    }

    /// <summary>
    /// //Shows skin information when skin is selected
    /// </summary>
    public void SetBallIcon(Sprite sprite)
    {
        ballSkinGroup.SetActive(true);
        ballSkinGroup.SetActive(true);

        textTitle.text = "Get?";

        imageBall.sprite = sprite;
        imageBall.SetNativeSize();

        Open();
    }


    void Open()
    {
        Debug.Log(this.gameObject.activeSelf);
        this.gameObject.SetActive(true);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.OutSine);
        popup.transform.DOKill();
        popup.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }


    public void Click_Cancel()
    {
        UIReset();
    }
}

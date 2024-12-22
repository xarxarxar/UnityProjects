using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MyInfo : MonoBehaviour
{
    public Image imageIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textLv;
    public Slider slider;
    public CanvasGroup canvasGroup;
    
    private void Awake () {
        canvasGroup.DOFade(0f, 0f);
        this.gameObject.SetActive(false);
    }

    public void SetMyInfo () {
        imageIcon.sprite = (PlayManager.Instance.currentBase as CtrHome).panelMyPage.panelMyPageScroll.contents[GameData.Select_Icon].GetComponent<Image>().sprite;

        this.gameObject.SetActive(true);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.OutSine);
        textName.text = GameData.NickName;
        textLv.text = GameData.PlayerLevel.ToString();
        slider.DOValue(GameData.PlayerExp / GameData.PlayerMaxExp, 0.25f).SetEase(Ease.InOutCubic);
    }
}

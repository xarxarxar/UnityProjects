using UnityEngine;
using DG.Tweening;

public class PanelMyPageScrollContent : MonoBehaviour
{
    private GameObject selectFrame;
    private PanelMyPageScroll panelMyPageScroll;
    private int id;
    public CostType costType;
    public int cost;


    public void InIt(PanelMyPageScroll panelMyPageScroll, int id)
    {
        this.id = id;
        this.panelMyPageScroll = panelMyPageScroll;
        selectFrame = transform.GetChild(0).gameObject;
    }

    public void Active(float time = 0.25f)
    {

        transform.DOScale(1f, time).SetEase(Ease.OutQuad);
        //transform.DOLocalMoveY(0f, time).SetEase(Ease.OutQuad);
        transform.SetAsLastSibling();

    }


    public void Deactive(float time = 0.25f)
    {
        transform.DOScale(0.8f, time).SetEase(Ease.OutQuad);
        //transform.DOLocalMoveY(100f, time).SetEase(Ease.OutQuad);
        transform.SetAsFirstSibling();
    }

    public void Select()
    {
        if (panelMyPageScroll.selectIcon != null)
        {
            panelMyPageScroll.selectIcon.Deselect();
            panelMyPageScroll.selectIcon.Deactive();
        }

        panelMyPageScroll.selectIcon = this;
        if (panelMyPageScroll.isOn)
        {
            selectFrame.SetActive(true);
        }
        else
        {
            selectFrame.SetActive(false);
        }

        GameData.Select_Icon = id;

        panelMyPageScroll.SetButtons();
        Active();
    }

    public void Deselect()
    {
        selectFrame.SetActive(false);
    }


    public void OffSelectFrame()
    {
        selectFrame.SetActive(false);
    }

    public void OnSelectFrame()
    {
        selectFrame.SetActive(true);
    }

    /// <summary>
    /// Buy item
    /// </summary>
    public void BuyItem()
    {

        CtrHome ctrHome = PlayManager.Instance.currentBase as CtrHome;

        switch (costType)
        {
            case CostType.Coin:
                if (GameData.Coin >= cost)
                {
                    ctrHome._PopupBuy.buttonOK.onClick.AddListener(() => { BuySuccess(); });
                    ctrHome._PopupBuy.SetIcon(id);
                }
                else
                {
                    PlayManager.Instance.commonUI.SetToast("Not enough coin.");
                    SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_error);
                    //Not enough gem
                }

                break;
            case CostType.Gem:
                if (GameData.Gem >= cost)
                {
                    ctrHome._PopupBuy.buttonOK.onClick.AddListener(() => { BuySuccess(); });
                    ctrHome._PopupBuy.SetIcon(id);
                }
                else
                {
                    PlayManager.Instance.commonUI.SetToast("Not enough coin.");
                    SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_error);
                    //Not enough gem
                }

                break;
        }
    }

    /// <summary>
    /// Processing after successful purchase
    /// </summary>
    void BuySuccess()
    {
        switch (costType)
        {
            case CostType.Coin:
                GameData.Coin -= cost;
                PlayManager.Instance.commonUI._CoinGem.SetCoin();
                panelMyPageScroll.UnlockIconByNum(id);
                panelMyPageScroll.SetButtons();
                break;
            case CostType.Gem:
                GameData.Gem -= cost;
                PlayManager.Instance.commonUI._CoinGem.SetGem();
                panelMyPageScroll.UnlockIconByNum(id);
                panelMyPageScroll.SetButtons();
                break;
        }
    }
}

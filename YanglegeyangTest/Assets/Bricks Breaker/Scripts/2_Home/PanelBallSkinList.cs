using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;

public class PanelBallSkinList : MonoBehaviour {
    
    public GameObject[] costIcon;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textAdsFree;
    private bool isSelect = false;


    public GameObject selected;
    public GameObject select;
    public GameObject objCost;
    public GameObject objAds;
    public Image imageBall;
    public Image sliderAds;
    public BallSkinData ballSkinData;
    int adsvalue;

    
    public void SetList () {
        //List initialization
        selected.SetActive(false);
        select.SetActive(false);
        objCost.SetActive(false);
        objAds.SetActive(false);

        GetComponent<Button>().onClick.AddListener(() => { Click_List(); });

        //Ball name setting
        textName.text = ballSkinData.ballName;

        //Ball cost setting
        textCost.text = Utility.ChangeThousandsSeparator(ballSkinData.cost);

        //Unlock check
        if (ballSkinData.id == 0) {
            ballSkinData.isUnlock = true;
        } else {
            ballSkinData.isUnlock = PlayerPrefs2.GetBool(string.Format("ball {0}", ballSkinData.id));
        }


        //Ball Image Settings
        imageBall.sprite = (PlayManager.Instance.currentBase as CtrHome).panelBallSkin.GetBallSprite(string.Format("skin_{0}",  ballSkinData.ballName));
        imageBall.SetNativeSize();

        if (ballSkinData.isUnlock) {
            //Turn on the select button when unlocked
            select.SetActive(true);
        } else {
            //If not unlocked, set to suit the purchase type
            for (int i = 0; i < costIcon.Length; i++) { costIcon[i].SetActive(false); }

            switch (ballSkinData.costType) {
                //Purchase with coins
                case CostType.Coin:
                    costIcon[0].SetActive(true);
                    objCost.SetActive(true);
                    break;
                //Purchase with gems
                case CostType.Gem:
                    costIcon[1].SetActive(true);
                    objCost.SetActive(true);
                    break;
                //Purchase with ads
                case CostType.Ads:
                    adsvalue = PlayerPrefs.GetInt(string.Format("ball ads {0}", ballSkinData.id), 3);
                    Debug.Log("AA : " + adsvalue);
                    sliderAds.fillAmount = (float)adsvalue / (float)3;
                    Debug.Log("Bb : " + (float)adsvalue / (float)3);
                    textAdsFree.text = string.Format("FREE\n<color=#ffffff><size=20>{0}/3</size></color>", adsvalue);

                    objAds.SetActive(true);
                    break;
            }


        }
    }

    //Deselect
    public void UnSelect () {
        if (!isSelect) return;
        isSelect = false;

        selected.gameObject.SetActive(false);
        select.gameObject.SetActive(true);
    }

    //Select
    public void Select()
    {
        if (isSelect) return;
        isSelect = true;


        CtrHome ctrHome = PlayManager.Instance.currentBase as CtrHome;

        if (ctrHome.panelBallSkin.selectSkinList != null)
        {
            ctrHome.panelBallSkin.selectSkinList.UnSelect();
        }

        ctrHome.panelBallSkin.selectSkinList = this;
        selected.gameObject.SetActive(true);
        select.gameObject.SetActive(false);
    }


    //Click list
    public void Click_List () {
        if (ballSkinData.isUnlock) {
            //언락
            Select();
            GameData.SelectBallNum = ballSkinData.id;
            SoundManager.Instance.PlayEffect(SoundList.sound_skin_btn_equip);
        } else {
            //언락 아님
            Buy();
        }
    }

    //Purchase
    void Buy ()
    {
        CtrHome ctrHome = PlayManager.Instance.currentBase as CtrHome;
        switch (ballSkinData.costType) {
            case CostType.Coin:
                if (GameData.Coin >= ballSkinData.cost) {
                    //There is enough money to have now than the purchase price.
                    ctrHome._PopupBuy.buttonOK.onClick.AddListener(() => { BuySuccess(); });
                    ctrHome._PopupBuy.SetBallIcon(imageBall.sprite);
                } else {
                    //Not enough money
                    PlayManager.Instance.commonUI.SetToast("Not enough coin.");
                    SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_error);
                }
                break;

            case CostType.Gem:
                if (GameData.Gem >= ballSkinData.cost) {
                    //There is enough money to have now than the purchase price.
                    ctrHome._PopupBuy.buttonOK.onClick.AddListener(() => { BuySuccess(); });
                    ctrHome._PopupBuy.SetBallIcon(imageBall.sprite);
                } else {
                    //Not enough money
                    PlayManager.Instance.commonUI.SetToast("Not enough gem.");
                    SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_error);
                }
                break;

            case CostType.Ads:
                
                //ADManager.Instance.ShowRewardedVideo(result =>
                //{
                //    if (result == ShowResult.Finished)
                //    {
                //        if (adsvalue - 1 <= 0) {
                //            //Unlock Skin
                //            BallUnlockByID();
                //            SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_get_skinmp3);
                //        } else {
                //            //Reduce advertising costs
                //            PlayerPrefs.SetInt(string.Format("ball ads {0}", ballSkinData.id), adsvalue - 1);
                //        }

                //        SetList();
                //    }
                //});
                break;
        }
    }

    /// <summary>
    /// Processing after successful purchase
    /// </summary>
    public void BuySuccess () {
        switch (ballSkinData.costType) {
            case CostType.Coin:
                GameData.Coin -= ballSkinData.cost;
                PlayManager.Instance.commonUI._CoinGem.SetCoin();
                break;

            case CostType.Gem:
                GameData.Gem -= ballSkinData.cost;
                PlayManager.Instance.commonUI._CoinGem.SetGem();
                break;
        }

        SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_get_skinmp3);
        BallUnlockByID();
        SetList();
    }

    /// <summary>
    /// Unlock Ballskin with ID
    /// </summary>
    void BallUnlockByID () {
        PlayerPrefs2.SetBool(string.Format("ball {0}", ballSkinData.id), true);
    }
}

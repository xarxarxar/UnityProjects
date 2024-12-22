using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class PanelMyPageScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    bool isAnim = false;
    bool isDragging = false;
    Vector2 firstPos;
    public Transform contentTransform;
    public float contentSizeWidth;
    public Transform centerTransform;
    bool isInteractable = false;
    public int selectNum = 0;
    public List<PanelMyPageScrollContent> contents = new List<PanelMyPageScrollContent>();
    public PanelMyPageScrollContent selectIcon;
    public bool isOn = false;

    public void SetData()
    {
        //The default skin is unlocked.
        UnlockIconByNum(0);
        selectNum = GameData.Select_Icon;

        for (int i = 0; i < contents.Count; i++)
        {
            contents[i].InIt(this, i);
            contents[i].transform.DOLocalMoveX(contentSizeWidth * i, 0f);
            contents[i].Deactive(0f);
        }

        contentTransform.DOLocalMoveX(-(contentSizeWidth * selectNum), 0f);
        contents[selectNum].Active(0f);

        SetButtons();
        contents[selectNum].Select();

    }

    public void OnScroll()
    {
        isOn = true;
        centerTransform.DOScale(1f, 0.15f).SetEase(Ease.OutSine);

        for (int i = 0; i < contents.Count; i++)
        {
            contents[i].gameObject.SetActive(true);
        }

        if (selectIcon != null)
        {
            selectIcon.OnSelectFrame();
        }

        SetButtons();
    }

    public void OffScroll()
    {
        isOn = false;
        selectNum = GameData.Select_Icon;
        centerTransform.DOScale(0.85f, 0.15f).SetEase(Ease.OutSine);
        for (int i = 0; i < contents.Count; i++)
        {
            if (selectNum != i)
            {
                contents[i].gameObject.SetActive(false);
            }

            contents[i].Deactive();
        }

        if (selectIcon != null)
        {
            selectIcon.OffSelectFrame();
        }

        (PlayManager.Instance.currentBase as CtrHome).panelMyPage.SetInfoGroup(true);
        contents[selectNum].Active();
        SetMoveX();
    }




    public void UnlockIconByNum(int id)
    {
        PlayerPrefs2.SetBool("Icon_Unlock" + id, true);
    }

    public void SetMoveX()
    {
        contentTransform.DOLocalMoveX(-(contentSizeWidth * selectNum), 0f);
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!isOn) return;
        if (isAnim) return;
        isInteractable = false;
        firstPos = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        if (!isOn) return;
        if (isDragging) return;
        isInteractable = true;

        if (firstPos.x - 50 > data.position.x && selectNum < contents.Count - 1)
        {
            isDragging = true;
            isAnim = true;

            SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_swipe);

            contentTransform.DOLocalMoveX(-contentSizeWidth, 0.25f).SetEase(Ease.OutFlash).SetRelative(true).OnComplete(
                () =>
                {
                    isDragging = false;
                    isInteractable = false;
                });

            contents[selectNum].Deactive();
            selectNum += 1;
            contents[selectNum].Active();

            SetButtons();

        }
        else if (firstPos.x + 50 < data.position.x && selectNum > 0)
        {
            isDragging = true;
            isAnim = true;

            SoundManager.Instance.PlayEffect(SoundList.sound_common_sfx_swipe);

            contentTransform.DOLocalMoveX(contentSizeWidth, 0.25f).SetEase(Ease.OutFlash).SetRelative(true).OnComplete(
                () =>
                {
                    isDragging = false;
                    isInteractable = false;
                });

            contents[selectNum].Deactive();
            selectNum -= 1;
            contents[selectNum].Active();

            SetButtons();
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!isOn) return;
        if (isAnim)
        {
            isAnim = false;
        }
        else
        {
            if (isInteractable) return;
            isInteractable = false;

            // if(data.pointerCurrentRaycast.gameObject == null) return;
        }
    }

    public GameObject btnSelect;
    public GameObject btnSelected;
    public GameObject btnBuyCoin;
    public GameObject btnBuyGem;
    public GameObject btnBuyAds;

    public TextMeshProUGUI textCostGem;
    public TextMeshProUGUI textCostCoin;
    public TextMeshProUGUI textAds;


    public void SetButtons()
    {
        if (PlayerPrefs2.GetBool("Icon_Unlock" + selectNum))
        {
            //Unlock
            btnBuyCoin.SetActive(false);
            btnBuyGem.SetActive(false);

            if (selectNum == GameData.Select_Icon)
            {
                btnSelected.SetActive(true);
                btnSelect.SetActive(false);
            }
            else
            {
                btnSelected.SetActive(false);
                btnSelect.SetActive(true);
            }

        }
        else
        {
            //Lock 
            btnSelected.SetActive(false);
            btnSelect.SetActive(false);
            int cost = contents[selectNum].cost;

            switch (contents[selectNum].costType)
            {
                case CostType.Coin:
                    btnBuyCoin.SetActive(true);
                    btnBuyGem.SetActive(false);
                    btnBuyAds.SetActive(false);
                    textCostCoin.text = Utility.ChangeThousandsSeparator(cost);
                    break;
                case CostType.Gem:
                    btnBuyCoin.SetActive(false);
                    btnBuyGem.SetActive(true);
                    btnBuyAds.SetActive(false);
                    textCostGem.text = Utility.ChangeThousandsSeparator(cost);
                    break;
                case CostType.Ads:
                    btnBuyCoin.SetActive(false);
                    btnBuyGem.SetActive(false);
                    btnBuyAds.SetActive(true);
                    textAds.text = "";
                    break;
            }
        }
    }

    /// <summary>
    /// Click Select button
    /// </summary>
    public void Click_Select()
    {
        Debug.Log("Click_Select");
        SoundManager.Instance.PlayEffect(SoundList.sound_skin_btn_equip);

        contents[selectNum].Select();
    }

    /// <summary>
    /// Click the Buy button
    /// </summary>
    public void Click_Buy()
    {
        Debug.Log("Click_Buy");
        contents[selectNum].BuyItem();
    }
}

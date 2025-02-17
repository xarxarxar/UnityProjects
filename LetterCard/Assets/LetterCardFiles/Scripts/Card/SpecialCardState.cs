using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCardState : MonoBehaviour
{
    public static SpecialCardState instance;

    private bool isDeleting;//正在删除卡牌
    public bool IsDeleting 
    { 
        get => isDeleting; 
        set 
        {
            if (value != isDeleting)
            {
                isDeleting = value;
                ShowTipManager.instance.ToggleDustbin(value);
            }
        }
    }

    private bool isChoosingCard;//是否正在选取特殊牌
    private SpecialCard chosenCard;//正在选择的牌
    public SpecialCard ChosenCard 
    { 
        get => chosenCard;
        set 
        {
            if (chosenCard != value)
            {
                chosenCard = value;
                if (chosenCard == null)
                {
                    specialText.text = "";
                    useSpecialButton.onClick.RemoveAllListeners();
                    useSpecialButton.interactable = false;
                }
                else
                {
                    specialText.text = ChosenCard.specialDescribe;
                    useSpecialButton.interactable = true;
                    useSpecialButton.onClick.AddListener(ChosenCard.ActivateEffect);
                }
            }
        } 
    }

    public  Text specialText;//显示功能牌效果的文字

    public Transform specialHandCards;

    public Button useSpecialButton;//使用特殊牌的按钮

    private void Awake()
    {
        instance=this;
    }

    public void Initialize()
    {
        IsDeleting = false;

    }

    /// <summary>
    /// 是否有特殊牌正在被选择
    /// </summary>
    /// <returns></returns>
    public void CardChoosing(Transform specialCard)
    {
        if(specialCard == null)
        {
            ChosenCard?.DownCard();//将原来的牌降下去
            ChosenCard = null;
            return;

        }
        //如果已经选择的牌不是这张牌，那么就将这张牌选择上
        if (ChosenCard!= specialCard.GetComponent<SpecialCard>())
        {
            ChosenCard?.DownCard();//将原来的牌降下去
            specialCard.GetComponent<SpecialCard>().UpCard();
            ChosenCard = specialCard.GetComponent<SpecialCard>();
        }
        else//如果选择的是这张牌
        {
            //将这张牌降下去
            specialCard.GetComponent<SpecialCard>().DownCard();
            ChosenCard = null;
        }
    }
}

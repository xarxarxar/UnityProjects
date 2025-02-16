using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Transform specialHandCards;

    private void Awake()
    {
        instance=this;
    }

    /// <summary>
    /// 是否有特殊牌正在被选择
    /// </summary>
    /// <returns></returns>
    public void CardChoosing(Transform specialCard)
    {
        //如果已经选择的牌不是这张牌，那么就将这张牌选择上
        if (chosenCard!= specialCard.GetComponent<SpecialCard>())
        {
            chosenCard?.DownCard();//将原来的牌降下去
            specialCard.GetComponent<SpecialCard>().UpCard();
            chosenCard = specialCard.GetComponent<SpecialCard>();
        }
        else//如果选择的是这张牌
        {
            //将这张牌降下去
            specialCard.GetComponent<SpecialCard>().DownCard();
            chosenCard = null;

        }
    }
}

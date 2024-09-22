using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    //游戏场景中的卡牌类

    public List<Card> upCards = new List<Card>();//覆盖住该卡牌的卡牌
    public List<Card> downCards = new List<Card>();//被该卡牌覆盖住的卡牌

    public int level = 0;//该卡牌所在的层级
    public bool isclickable = false;//该卡牌是否可以点击
    public bool isUsed = false;//该卡牌是否已经被点击过，也就是说该卡牌在下方的卡牌槽里，或者在去槽里的路上

    public string  type;//该卡牌上承载的类型，一样类型的就可以消除，之后替换成Item类

    Button thisButton ;//该卡牌上搭载的Button组件

    //当卡牌可以点击时，执行某个方法，即将该物体的button状态从不可点击设置为可以点击
    public bool Isclickable
    {
        get => isclickable;
        set
        {
            if (isclickable!=value)
            {
                ChangeButtonState(value);
            }
            isclickable = value;
        }
    }

    // Start is called before the first frame update
   
    public void OnEnable()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(Click);//为此卡牌添加点击事件
        thisButton.interactable = isclickable;
    }



    //将卡牌的可点击状态改变
    void ChangeButtonState(bool isclicked)
    {
        thisButton.interactable = isclicked;
        transform.Find("Mask").gameObject.SetActive(!isclicked);
    }

    //该卡牌的点击方法
    public void Click()
    {
        if(isUsed || CardManager.instance.isFailed)//如果已经点击过了或者已经失败了，则直接返回
        {
            return;
        }
        isUsed = true;//点击完成之后，该卡牌变为已点击

        //当这张卡牌一出之后，只需要遍历被这张卡牌覆盖掉的牌是否可以点击就行
        for (int i = 0; i < downCards.Count; i++)
        {
            CardManager.instance.JudgeClickable(downCards[i]);//
        }
        CardManager.instance.MoveCard(this);

        AudioManager.Instance.PlaySoundEffect(SoundEffectType.ClickCard);//播放点击卡片音效
    }

    /// <summary>
    /// 设置该卡牌的sprite
    /// </summary>
    /// <param name="sprite">需要设置的sprite</param>
    public void SetCardSprite(Sprite sprite)
    {
        transform.Find("Sprite").GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 获取这张卡片的sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetCardSprite()
    {
        return transform.Find("Sprite").GetComponent<Image>().sprite;
    }
}

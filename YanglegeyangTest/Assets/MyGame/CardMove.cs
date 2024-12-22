using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardMove : MonoBehaviour
{
    public List<Card> slotCards = new List<Card>();
    public List<GameObject> slots = new List<GameObject>();
    public static CardMove Instance ;

    private void Awake()
    {
        Instance = this;
    }

    public  void MoveCard(Card oneCard)
    {
        for(int i = slotCards.Count-1; i>=0; i--)
        {
            if (slotCards[i].type == oneCard.type)
            {
                slotCards.Insert(i+1,oneCard);
                Debug.Log("重复"+(i+1));
                UpdateCardPosition();
                return;
            }
        }
        Debug.Log(slotCards.Count);
        slotCards.Insert(slotCards.Count,oneCard);
        UpdateCardPosition();
    }

    void  UpdateCardPosition()
    {
        for (int i = 0; i < slotCards.Count; i++)
        {
            RectTransform slotCardRect = slotCards[i].GetComponent<RectTransform>();
            RectTransform slotRect = slots[i].GetComponent<RectTransform>();
            RectTransform parentRect = slotCardRect.parent.GetComponent<RectTransform>();

            // 将 slotRect 的全局坐标转换为父物体的局部坐标
            Vector2 worldPosition = RectTransformUtility.WorldToScreenPoint(null, slotRect.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, worldPosition, null, out Vector2 localPosition);

            // 使用 DOAnchorPos 移动 UI 元素到目标位置
            slotCardRect.DOAnchorPos(localPosition, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(JudgeSameCard); // 动画结束后执行的方法
        }
    }

    void JudgeSameCard()
    {
        int sameCount = 1;
        for (int i = 1; i < slotCards.Count; i++)
        {
            if (slotCards[i].type== slotCards[i - 1].type)
            {
                sameCount++;
                if (sameCount == 3) 
                { 
                    Card card1= slotCards[i];
                    Card card2= slotCards[i-1];
                    Card card3= slotCards[i-2];
                    slotCards.Remove(card1);
                    slotCards.Remove(card2);
                    slotCards.Remove(card3);
                    Destroy(card1.gameObject);
                    Destroy(card2.gameObject);
                    Destroy(card3.gameObject);
                    UpdateCardPosition();
                }
            }
            else
            {
                sameCount = 1;
            }
        }
    }
    
}

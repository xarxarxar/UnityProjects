using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理手牌的摆放
/// </summary>
public class HandCardContainer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxCards = 10;            // 最大卡牌数量，外部脚本可以修改

    private void OnTransformChildrenChanged()
    {
        // 添加卡牌
        AddCard();
    }
    // 每当容器内容发生变化时调用
    public void AddCard()
    {
        if (transform.childCount >= maxCards)
        {
            Debug.LogWarning("达到最大卡牌数量，不再添加");
            return; // 达到最大卡牌数量，不再添加
        }

        ArrangeCards();//排列卡牌
    }


    // 排列所有手牌
    private void ArrangeCards()
    {
        // 计算容器的宽度和手牌的总宽度
        float containerWidth = GetComponent<RectTransform>().rect.width;
        float totalWidth = 0f;
        foreach (Transform card in transform)
        {
            totalWidth += card.GetComponent<RectTransform>().rect.width;
        }
        if (totalWidth <= containerWidth)
        {
            // 所有手牌宽度小于容器宽度，水平排列
            ArrangeCardsHorizontally(containerWidth);
        }
        else
        {
            // 手牌宽度总和大于容器宽度，卡牌开始叠放
            ArrangeCardsOverlapping(containerWidth,totalWidth);
        }

        // 竖直居中卡牌
        CenterCardsVertically();
    }

    // 水平排列手牌
    private void ArrangeCardsHorizontally(float containerWidth)
    {
        // 获取容器的 RectTransform 并计算左端位置
        RectTransform containerRect = GetComponent<RectTransform>();
        float containerLeftEdge = containerRect.anchoredPosition.x - containerWidth / 2;

        float currentX = containerLeftEdge; // 从容器左端开始

        // 将卡牌水平排列
        foreach (Transform card in transform)
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX + 0.5f * card.GetComponent<RectTransform>().rect.width, 0);
            currentX += card.GetComponent<RectTransform>().rect.width;
        }
    }

    // 叠放手牌
    private void ArrangeCardsOverlapping(float containerWidth, float totalWidth)
    {
        // 获取容器的 RectTransform
        RectTransform containerRect = GetComponent<RectTransform>();
        float containerLeftEdge = containerRect.anchoredPosition.x;

        // 计算每张卡牌的重叠偏移量
        float overlapAmount = (totalWidth - containerWidth) / (transform.childCount - 1);
        if (transform.childCount == 1) overlapAmount = 0; // 如果只有一个卡牌，不需要重叠

        // 从容器的左端开始叠放卡牌
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform card = transform.GetChild(i).GetComponent<RectTransform>();

            // 计算每张卡牌的偏移量，并应用到卡牌位置
            float offsetX = containerLeftEdge + i * overlapAmount - 0.5f * card.rect.width;

            // 设置卡牌的位置
            card.anchoredPosition = new Vector2(offsetX, 0);
        }
    }


    // 竖直居中所有卡牌
    private void CenterCardsVertically()
    {
        // 获取容器的 RectTransform
        RectTransform containerRect = GetComponent<RectTransform>();

        // 计算容器的竖直居中偏移量
        float containerHeight = containerRect.rect.height;
        float cardHeight = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>().rect.height;
        float offsetY = (containerHeight - cardHeight) / 2f;

        // 将最新添加的卡牌位置设定为竖直居中
        Transform newCard = transform.GetChild(transform.childCount - 1);
        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition = new Vector2(cardRect.anchoredPosition.x, offsetY);
    }

    // 允许外部脚本设置最大卡牌数量
    public void SetMaxCards(int max)
    {
        maxCards = max;
    }
}

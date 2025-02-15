using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public GameObject cardPrefab;      // 卡牌的预设（Prefab）
    public Transform poolParent;       // 存放卡牌的父物体
    public int initialSize = 10;       // 初始池大小

    private Queue<Card> cardPool = new Queue<Card>();  // 存放卡牌的队列

    void Start()
    {
        // 初始化对象池
        InitializePool();
    }

    // 初始化池子，创建初始数量的卡牌
    private void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            // 实例化卡牌并设置父物体
            Card newCard = Instantiate(cardPrefab, poolParent).GetComponent<Card>();
            newCard.gameObject.SetActive(false);  // 默认情况下卡牌不可见
            cardPool.Enqueue(newCard);  // 将卡牌加入池中
        }
    }

    // 获取一个卡牌对象，如果池子为空则动态扩展
    public Card GetCard()
    {
        if (cardPool.Count > 0)
        {
            Card card = cardPool.Dequeue();  // 从池中取出一个卡牌
            card.gameObject.SetActive(true);  // 激活卡牌
            ResetCard(card);  // 重置卡牌状态
            return card;
        }
        else
        {
            // 池子没有卡牌了，扩展池并返回新卡牌
            Card newCard = Instantiate(cardPrefab, poolParent).GetComponent<Card>();
            ResetCard(newCard);
            return newCard;
        }
    }

    // 将卡牌返回池中，并隐藏它
    public void ReturnCard(Card card)
    {
        card.gameObject.SetActive(false);  // 隐藏卡牌
        card.transform.SetParent(poolParent);//放回父物体内
        cardPool.Enqueue(card);  // 将卡牌放回池中
    }

    // 重置卡牌状态（比如位置、旋转等）
    private void ResetCard(Card card)
    {
        // 重置位置、旋转和缩放
        //card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = Vector3.one;
        card.gameObject.SetActive(true);

        // 可以在这里重置其他需要初始化的状态
    }
}


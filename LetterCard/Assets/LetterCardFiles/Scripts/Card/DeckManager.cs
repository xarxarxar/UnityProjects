using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// 牌堆管理系统
public class DeckManager : MonoBehaviour
{
    // 牌组配置
    [System.Serializable]
    public class DeckConfig
    {
        public int maxNormalCards = 10;
        public int maxSpecialCards = 3;
        public float specialCardChance = 0.2f; // 特殊牌出现概率
    }

    // 卡牌池
    [SerializeField] private List<LetterCard> letterDeck = new List<LetterCard>();
    [SerializeField] private List<SpecialCard> specialCardPool = new List<SpecialCard>();

    // 当前手牌
    public List<Card> letterHandCards = new List<Card>();
    public List<Card> specialHandCards = new List<Card>();

    // 配置参数
    public DeckConfig config;
    public LetterCard[] allLetterCards; // 所有字母牌预制体
    public SpecialCard[] specialCardTemplates; // 特殊牌模板

    // 事件
    public UnityEvent OnHandFull;
    public UnityEvent<Card> OnCardDrawn;

    void Start()
    {
        InitializeLetterDeck();
        InitializeSpecialCardPool();
    }

    /// <summary>
    /// 初始化字母牌堆
    /// </summary>
    void InitializeLetterDeck()
    {
        // 创建所有字母牌实例
        foreach (var card in allLetterCards)
        {
            for (int i = 0;i<4;i++)
            {
                letterDeck.Add(card);
            }
            
        }
    }

    /// <summary>
    /// 初始化特殊牌池
    /// </summary>
    void InitializeSpecialCardPool()
    {
        // 根据模板生成初始特殊牌池
        foreach (var template in specialCardTemplates)
        {
            for (int i = 0; i < template.initialPoolSize; i++)
            {
                specialCardPool.Add(template);
            }
        }
    }

    /// <summary>
    /// 抽取卡牌
    /// </summary>
    /// <param name="normalCount">抽取字母牌的数量</param>
    /// <param name="specialCount">抽取特殊牌的数量</param>
    public void DrawCards(int letterCount, int specialCount)
    {
        StartCoroutine(DrawCardsRoutine(letterCount, specialCount));
    }

    IEnumerator DrawCardsRoutine(int letterCount, int specialCount)
    {
        for (int i = 0; i < letterCount; i++)
        {
            if (CanDrawNormalCard())
            {
                DrawLetterCard();
                yield return new WaitForSeconds(0.3f); // 抽牌间隔
            }
        }

        for (int i = 0; i < specialCount; i++)
        {
            if (CanDrawSpecialCard())
            {
                DrawSpecialCard();
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    /// <summary>
    /// 抽取字母牌
    /// </summary>
    void DrawLetterCard()
    {
        if (letterDeck.Count == 0)
        {
            Debug.LogWarning("Letter deck is empty!");
            return;
        }
        int letterCardIndex= Random.Range(0, letterDeck.Count);
        LetterCard newCard = Instantiate(letterDeck[letterCardIndex]);
        letterDeck.RemoveAt(letterCardIndex);//移出这个卡牌
        letterHandCards.Add(newCard);
        OnCardDrawn?.Invoke(newCard);
    }

    /// <summary>
    /// 抽取特殊牌
    /// </summary>
    void DrawSpecialCard()
    {
        // 根据权重随机选择特殊牌
        float totalWeight = specialCardPool.Sum(c => c.spawnWeight);
        float randomPoint = Random.Range(0, totalWeight);

        foreach (var card in specialCardPool.OrderBy(c => c.spawnWeight))
        {
            if (randomPoint < card.spawnWeight)
            {
                SpecialCard newCard = Instantiate(card);
                specialHandCards.Add(newCard);
                OnCardDrawn?.Invoke(newCard);
                return;
            }
            randomPoint -= card.spawnWeight;
        }
    }

    /// <summary>
    /// 检查能否抽取普通牌
    /// </summary>
    /// <returns></returns>
    bool CanDrawNormalCard()
    {
        int currentNormal = letterHandCards.Count();
        return currentNormal < GetCurrentMaxNormal();
    }

    /// <summary>
    /// 检查能否抽取特殊牌
    /// </summary>
    /// <returns></returns>
    bool CanDrawSpecialCard()
    {
        int currentSpecial = specialHandCards.Count();
        return currentSpecial < GetCurrentMaxSpecial();
    }

    /// <summary>
    /// 当前最大普通牌容量（可扩展）
    /// </summary>
    /// <returns></returns>
    int GetCurrentMaxNormal()
    {
        int baseValue = config.maxNormalCards;
        // 这里可以添加临时加成逻辑
        return baseValue;
    }

    /// <summary>
    /// 当前最大特殊牌容量（可扩展）
    /// </summary>
    /// <returns></returns>
    int GetCurrentMaxSpecial()
    {
        int baseValue = config.maxSpecialCards;
        // 这里可以添加临时加成逻辑
        return baseValue;
    }


    /// <summary>
    /// 弃牌方法
    /// </summary>
    /// <param name="card">需要丢弃的卡牌</param>
    public void DiscardLetterCard(LetterCard letterCard)
    {
        if (letterHandCards.Contains(letterCard))
        {
            letterHandCards.Remove(letterCard);

            letterDeck.Add(letterCard); // 字母牌返回牌堆底部
        }
    }
}





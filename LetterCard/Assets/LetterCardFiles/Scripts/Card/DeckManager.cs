using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// 牌堆管理系统
public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;
    // 卡牌池
    public CardPool cardPool;
    [SerializeField]private List<(char,char)> letterDeck = new List<(char, char)>();
    private List<(SpecialEffectType,float)> specialCardPool = new List<(SpecialEffectType, float)>();
    
    public SpecialCard specialCardPrefab;


    // 等待出牌的暂存池
    public Transform cachePool;
    public Transform LetterHandCard;
    public Transform SpecialHandCard;

    // 当前手牌
    [HideInInspector]public List<Card> letterHandCards = new List<Card>();
    [HideInInspector] public List<Card> specialHandCards = new List<Card>();

    // 配置参数
    public LetterCard letterCardPrefab; // 字母牌预制体

    // 事件
    public UnityEvent OnHandFull;
    public UnityEvent<Card> OnCardDrawn;

    private void Awake()
    {
        instance = this;
    }

    public void StartLevel()
    {
        InitializeLetterDeck();
        InitializeSpecialCardPool();
        DrawCards(1, 1);//抽取三张字母牌和一张特殊牌
    }

    /// <summary>
    /// 初始化字母牌堆
    /// </summary>
    void InitializeLetterDeck()
    {
        // 生成包含大小写字母的数组
        char[] allLetters = GetAllLetters();
        // 创建所有字母牌实例
        foreach (var cardChar in allLetters)
        {
            // 遍历 ColorType 枚举的所有值
            foreach (char color in "RGBY")
            {
                (char, char) letterCard = (cardChar,color);
                letterDeck.Add(letterCard);
            }
        }
    }

    /// <summary>
    /// 初始化特殊牌池
    /// </summary>
    void InitializeSpecialCardPool()
    {
        // 遍历所有的 SpecialEffectType 枚举类型
        foreach (SpecialEffectType effect in Enum.GetValues(typeof(SpecialEffectType)))
        {
            (SpecialEffectType, float) specialCard = (effect, GameConfig.specialCardWeightDic[effect]);
            specialCardPool.Add(specialCard);
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
                //DrawLetterCard();
                DrawLetterCardNoPool();//不要有卡牌池的限定
                yield return new WaitForSeconds(0.3f); // 抽牌间隔
            }
            else
            {
                ShowTipManager.instance.ShowTip("牌数达到上限，请及时出牌");
                Debug.Log($"达到手牌上限现在有{letterHandCards.Count()}张");
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
        yield break;
    }


    /// <summary>
    /// 抽取指定的卡牌，用于游戏教程等等
    /// </summary>
    /// <param name="cardType">指定卡牌类型（字母牌或特殊牌）</param>
    /// <param name="letter">指定字母（仅适用于字母牌）</param>
    /// <param name="color">指定颜色（仅适用于字母牌）</param>
    void DrawDesignatedCard(CardType cardType, char? letter = null, char? color = null)
    {
        // 如果是字母卡
        if (cardType == CardType.Letter)
        {
            // 先过滤字母牌堆，筛选符合条件的卡牌
            List<(char, char)> validLetterCards = letterDeck
                .Where(card => (!letter.HasValue || card.Item1 == letter.Value) &&
                               (!color.HasValue || card.Item2 == color.Value))
                .ToList();

            if (validLetterCards.Count == 0)
            {
                Debug.LogWarning("没有符合条件的字母牌！");
                return;
            }

            // 从符合条件的卡牌中随机选择一张
            int index = UnityEngine.Random.Range(0, validLetterCards.Count);
            var selectedCard = validLetterCards[index];

            // 创建新的字母卡
            LetterCard newCard = (LetterCard)cardPool.GetCard();
            newCard.Letter = selectedCard.Item1;
            newCard.Color = selectedCard.Item2;

            // 设置父物体
            newCard.transform.SetParent(LetterHandCard);

            // 从字母堆移除已抽取的卡牌
            //letterDeck.Remove(selectedCard);
            letterHandCards.Add(newCard);


            // 调用抽牌事件
            OnCardDrawn?.Invoke(newCard);
        }
        // 如果是特殊卡
        else if (cardType == CardType.Special)
        {
            // 检查是否可以抽取特殊卡
            if (!CanDrawSpecialCard())
            {
                Debug.LogWarning("无法抽取特殊卡！");
                return;
            }

            // 根据权重抽取特殊卡
            float totalWeight = specialCardPool.Sum(c => c.Item2);
            float randomPoint = UnityEngine.Random.Range(0, totalWeight);

            foreach (var card in specialCardPool.OrderBy(c => c.Item2))
            {
                if (randomPoint < card.Item2)
                {
                    // 生成特殊卡实例
                    SpecialCard newCard = Instantiate(specialCardPrefab, SpecialHandCard);
                    newCard.EffectType = card.Item1;
                    specialHandCards.Add(newCard);
                    
                    // 调用抽牌事件
                    OnCardDrawn?.Invoke(newCard);
                    return;
                }
                randomPoint -= card.Item2;
            }
        }
    }

    public  void DrawDesignCard(SpecialEffectType effectType)
    {
        SpecialCard newCard = Instantiate(specialCardPrefab, SpecialHandCard);
        //SpecialCard newCard = (SpecialCard)cardPool.GetCard();
        //newCard.transform.SetParent(SpecialHandCard);
        newCard.EffectType = effectType;
        specialHandCards.Add(newCard);
        OnCardDrawn?.Invoke(newCard);
    }

    /// <summary>
    /// 抽取指定颜色的随机字母卡牌
    /// </summary>
    /// <param name="color">指定的颜色（R、G、B、Y）</param>
    public bool DrawRandomLetterCardByColor(char color)
    {
        if(!CanDrawNormalCard())
        {
            Debug.Log($"达到手牌上限");
            return false;
        }
        // 先过滤字母牌堆，筛选符合颜色条件的卡牌
        List<(char, char)> validLetterCards = letterDeck
            .Where(card => card.Item2 == color)
            .ToList();

        if (validLetterCards.Count == 0)
        {
            Debug.LogWarning("没有符合条件的字母牌！");
            return false;
        }

        // 从符合条件的卡牌中随机选择一张
        int index = UnityEngine.Random.Range(0, validLetterCards.Count);
        var selectedCard = validLetterCards[index];

        // 创建新的字母卡
        LetterCard newCard = (LetterCard)cardPool.GetCard();
        newCard.Letter = selectedCard.Item1;
        newCard.Color = selectedCard.Item2;

        // 设置父物体
        newCard.transform.SetParent(LetterHandCard);

        // 从字母堆移除已抽取的卡牌
        //letterDeck.Remove(selectedCard);
        letterHandCards.Add(newCard);

        // 调用抽牌事件
        OnCardDrawn?.Invoke(newCard);
        return true;
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

        int letterCardIndex = UnityEngine.Random.Range(0, letterDeck.Count);
        LetterCard newCard = (LetterCard)cardPool.GetCard();

        newCard.Letter = letterDeck[letterCardIndex].Item1;
        newCard.Color = letterDeck[letterCardIndex].Item2;

        newCard.transform.SetParent(LetterHandCard);

        letterDeck.RemoveAt(letterCardIndex);//移出这个卡牌
        letterHandCards.Add(newCard);

        OnCardDrawn?.Invoke(newCard);
    }

    /// <summary>
    /// 抽取字母牌,没有卡牌池，随机抽
    /// </summary>
    void DrawLetterCardNoPool()
    {
        if (letterDeck.Count == 0)
        {
            Debug.LogWarning("Letter deck is empty!");
            return;
        }
        int letterCardIndex=UnityEngine.Random.Range(0, letterDeck.Count);
        LetterCard newCard =(LetterCard)cardPool.GetCard();

        newCard.Letter= letterDeck[letterCardIndex].Item1;
        newCard.Color= letterDeck[letterCardIndex].Item2;

        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(newCard.FlipCardToBack(3.0f));//先翻到背面);

        

        sequence.Append(newCard.transform.DOLocalMove(-1.0f*cardPool.transform.position, 1.0f)
            .SetEase(Ease.Linear));

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(newCard.FlipCardToFront(3.0f));//再翻到正面

        sequence.AppendCallback(() =>
        {
            newCard.transform.SetParent(LetterHandCard, worldPositionStays: true);
        });

        
        //letterDeck.RemoveAt(letterCardIndex);//不用移出
        letterHandCards.Add(newCard);
        OnCardDrawn?.Invoke(newCard);
    }

    /// <summary>
    /// 抽取特殊牌
    /// </summary>
    void DrawSpecialCard()
    {
        //根据特殊牌出现的概率执行代码
        //if(UnityEngine.Random.value >=config. specialCardProbability)
        //{
        //    return;
        //}
        // 根据权重随机选择特殊牌
        float totalWeight = specialCardPool.Sum(c => c.Item2);
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        foreach (var card in specialCardPool.OrderBy(c => c.Item2))
        {
            if (randomPoint < card.Item2)
            {
                SpecialCard newCard = Instantiate(specialCardPrefab, SpecialHandCard);
                //SpecialCard newCard = (SpecialCard)cardPool.GetCard();
                //newCard.transform.SetParent(SpecialHandCard);
                newCard.EffectType = card.Item1;
                specialHandCards.Add(newCard);
                OnCardDrawn?.Invoke(newCard);
                return;
            }
            randomPoint -= card.Item2;
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
    /// 检查能否向缓存池中添加卡牌
    /// </summary>
    /// <returns></returns>
    public  bool CanDrawCacheCard()
    {
        int currentCache = cachePool.childCount;
        return currentCache < GetCurrentMaxCache();
    }

    /// <summary>
    /// 当前最大普通牌容量（可扩展）
    /// </summary>
    /// <returns></returns>
    int GetCurrentMaxNormal()
    {
        return 10;
    }

    /// <summary>
    /// 当前最大特殊牌容量（可扩展）
    /// </summary>
    /// <returns></returns>
    int GetCurrentMaxSpecial()
    {
        return 10;
    }

    /// <summary>
    /// 当前最大缓存容量
    /// </summary>
    /// <returns></returns>
    int GetCurrentMaxCache()
    {
        return 10;
    }

    /// <summary>
    /// 出牌方法
    /// </summary>
    public void PlayCard()
    {
        //获取暂存池内的物体
        List<LetterCard> childrenList = new List<LetterCard>();
        // 遍历物体的所有子物体
        foreach (Transform child in cachePool)
        {
            childrenList.Add(child.GetComponent<LetterCard>());  // 将子物体添加到列表中
        }
        if (childrenList.Count == 0)
        {
            Debug.Log("暂存池没有字母牌");
            return;
        }

        int singleScore = ScoreCalculator.CalculateScore(childrenList);

        //销毁暂存池中的所有物体
        foreach (LetterCard child in childrenList)
        {
            //Destroy(child.gameObject);
            cardPool.ReturnCard(child);
            letterHandCards.Remove(child);//从手牌中移出
        }
        //roundOver?.Invoke(singleScore);
    } 

    // 生成全部字母的数组
    char[] GetAllLetters()
    {
        // 使用 'A' 到 'Z' 和 'a' 到 'z' 的字符代码生成字母
        //char[] upperCase = Enumerable.Range('A', 26).Select(i => (char)i).ToArray();
        char[] lowerCase = Enumerable.Range('a', 26).Select(i => (char)i).ToArray();

        // 合并大写字母和小写字母
        //return upperCase.Concat(lowerCase).ToArray();

        //仅使用小写字母
        return lowerCase.ToArray();
    }
}





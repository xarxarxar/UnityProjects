using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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


    // 手牌
    public Transform cachePool;
    public Transform LetterHandCard;
    public Transform SpecialHandCard;
    private bool isDrawing;//是否正处于抽牌动画
    public bool IsDrawing 
    { 
        get => isDrawing;
        set 
        { 
            isDrawing = value;
            ButtonManager.instance.drawCardButton.interactable=!value;
        }
    }

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

    public void Init()
    {
        InitializeLetterDeck();
        InitializeSpecialCardPool();
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
    /// 抽取字母牌
    /// </summary>
    public void DrawLetterCard(int count)
    {
        if (IsDrawing) return;//如果正在抽牌
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < count; i++)
        {
            if (CanDrawNormalCard())
            {

                bool singleDraw = (count == 1);

                // 延迟后调用抽卡
                sequence.AppendCallback(() => DrawLetterCardNoPool(singleDraw));

                sequence.AppendInterval(0.5f);

            }
            else
            {
                // 牌数达到上限时显示提示
                sequence.AppendCallback(() =>
                {
                    ShowTipManager.instance.ShowTip("牌数达到上限，请及时出牌");
                    Debug.Log($"达到手牌上限现在有{letterHandCards.Count()}张");
                });
                break; // 如果达到手牌上限，终止循环
            }
        }
    }
    /// <summary>
    /// 抽取特殊牌
    /// </summary>
    public void DrawSpecialCard(int count)
    {
        if (IsDrawing) return;//如果正在抽牌
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < count; i++)
        {
            //根据特殊牌出现的概率执行代码
            if (UnityEngine.Random.value >= 0.8f)//抽到特殊牌的概率
            {
                continue;
            }
            if (CanDrawSpecialCard())
            {
                // 延迟后调用抽卡
                sequence.AppendCallback(() => DrawSpecialCard());

                // 设置每次抽卡后的间隔
                sequence.AppendInterval(0.5f);
            }
            else
            {
                // 牌数达到上限时显示提示
                sequence.AppendCallback(() =>
                {
                    ShowTipManager.instance.ShowTip("抽到了功能牌，但功能牌数量达到上限");
                });
                break; // 如果达到手牌上限，终止循环
            }
        }
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
            LetterCard newCard = (LetterCard)cardPool.GetCard<LetterCard>();
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

    public void DrawDesignCard(SpecialEffectType effectType)
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
        LetterCard newCard = (LetterCard)cardPool.GetCard<LetterCard>();
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
        LetterCard newCard = (LetterCard)cardPool.GetCard<LetterCard>();

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
    /// <param name="singleDraw">是否是单张地抽，如果不是单张的抽，那就没这么多动画</param>
    void DrawLetterCardNoPool(bool singleDraw)
    {
        if (letterDeck.Count == 0)
        {
            Debug.LogWarning("Letter deck is empty!");
            return ;
        }
        int letterCardIndex=UnityEngine.Random.Range(0, letterDeck.Count);
        LetterCard newCard =(LetterCard)cardPool.GetCard<LetterCard>();

        newCard.Letter= letterDeck[letterCardIndex].Item1;
        newCard.Color= letterDeck[letterCardIndex].Item2;

        if (singleDraw)
        {
            Debug.Log("抽取动画");
            DrawCardAnim(newCard,LetterHandCard);
        }
        else
        {
            newCard.transform.position = Vector3.zero;
            newCard.transform.SetParent(LetterHandCard, worldPositionStays: true);
        }
        

        //letterDeck.RemoveAt(letterCardIndex);//不用移出
        letterHandCards.Add(newCard);
        OnCardDrawn?.Invoke(newCard);
    }

    /// <summary>
    /// 抽取特殊牌
    /// </summary>
    void DrawSpecialCard()
    {
        // 根据权重随机选择特殊牌
        float totalWeight = specialCardPool.Sum(c => c.Item2);
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        foreach (var card in specialCardPool.OrderBy(c => c.Item2))
        {
            if (randomPoint < card.Item2)
            {
                SpecialCard newCard = (SpecialCard)cardPool.GetCard<SpecialCard>();
                newCard.EffectType = card.Item1;
                DrawCardAnim(newCard, SpecialHandCard);
                specialHandCards.Add(newCard);
                OnCardDrawn?.Invoke(newCard);
                return;
            }
            randomPoint -= card.Item2;
        }
    }

    Sequence DrawCardAnim(Card newCard,Transform parents)
    {
        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() =>
        {
            IsDrawing = true;
        });

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(newCard.FlipCardToBack(0.01f));//先翻到背面);



        sequence.AppendCallback(() =>
        {
            //newCard.transform.SetParent(DrawHandCard, worldPositionStays: true);
            newCard.transform.localScale = Vector3.zero;
            newCard.transform.position = Vector3.zero;
        });

        sequence.Append(newCard.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.InOutQuart));

        // 添加停顿一秒
        //sequence.AppendInterval(0.6f);  // 停顿

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(newCard.FlipCardToFront(0.4f));//再翻到正面

        // 添加停顿一秒
        sequence.AppendInterval(0.15f);  // 停顿

        sequence.AppendCallback(() =>
        {
            newCard.transform.SetParent(parents, worldPositionStays: true);
            IsDrawing = false;
        });

        return sequence;
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





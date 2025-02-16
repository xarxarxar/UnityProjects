using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// SpecialCard 类是Card的子类，表示特殊卡牌。
// 特殊卡牌拥有不同的效果，可以通过 ActivateEffect 方法来激活这些效果。
public class SpecialCard : Card
{
    // 特殊效果类型，定义了该特殊卡牌的效果种类，例如：移除卡牌、交换卡牌等。
    private SpecialEffectType effectType;
    public SpecialEffectType EffectType
    { 
        get => effectType; 
        set
        {
            if (effectType != value)
            {
                effectType = value;
                OnInstantiate();
            }
        }
    }

    // spawnWeight：表示这张特殊牌的生成权重，权重越高，生成的概率越大
    public float spawnWeight = 1f;
    // initialPoolSize：表示初始牌池中的特殊牌数量
    public int initialPoolSize = 3;
    public string specialDescribe = "";//特殊牌的描述

    [SerializeField] private Text largeLetter;//中间的大字
    [SerializeField] private Text smallLetter;//左上角小字
    [SerializeField] private Text bottomLetter;//下方的小字

    // 使用字典映射 ColorType 到 Color
    Dictionary<char, Color32> colorMap = new Dictionary<char, Color32>
        {
            { 'R', new Color32(194,24,91,255) },
            { 'G', new Color32(56,142,60,255) },
            { 'B', new Color32(48,63,159,255) },
            { 'Y', new Color32(255,162,0,255) }
        };

    // 目标卡牌的 Image 组件
    private CanvasGroup canvasGroup;


    private void Awake()
    {
        // 获取卡牌的 CanvasGroup 组件，如果没有就添加一个
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        base.OnEnable();
        OnInstantiate();
        GetComponent<Button>().onClick.AddListener(ChooseCard);
    }

    private void OnInstantiate()
    {
        // 这里是具体效果的实现，可能会根据 effectType 来执行不同的操作。
        // 例如，移除卡牌、交换卡牌等。
        switch (EffectType)
        {
            case SpecialEffectType.RemoveCard:
                // 实现移除卡牌的效果
                largeLetter.text = smallLetter.text = "除";
                largeLetter.color = smallLetter.color = Color.black; 
                bottomLetter.text = "-1";

                specialDescribe = "移除一张字母牌";
                break;
            case SpecialEffectType.AddOneLetterHand:
                // 实现增加一个字母手牌上限的效果
                largeLetter.text = smallLetter.text = "字";
                largeLetter.color = smallLetter.color = Color.black;
                bottomLetter.text = "+1";

                specialDescribe = "字母牌上限+1";
                break;
            case SpecialEffectType.AddOneCacheHand:
                // 实现增加一个缓存手牌上限的效果
                largeLetter.text = smallLetter.text = "出";
                largeLetter.color = smallLetter.color = Color.black;
                bottomLetter.text = "+1";

                specialDescribe = "出牌上限+1";
                break;
            case SpecialEffectType.AddOneSpecialHand:
                // 实现增加一个功能手牌上限的效果
                largeLetter.text = smallLetter.text = "功";
                largeLetter.color = smallLetter.color = Color.black;
                bottomLetter.text = "+1";

                specialDescribe = "功能牌上限+1";
                break;
            case SpecialEffectType.RandomRedCard:
                // 实现获取一张随机红色卡牌的效果
                largeLetter.text = smallLetter.text = "取";
                largeLetter.color = smallLetter.color = colorMap['R'];
                bottomLetter.text = "?";

                specialDescribe = "随机抽取一张红色字母牌";
                break;
            case SpecialEffectType.RandomYellowCard:
                // 实现获取一张随机黄色卡牌的效果
                largeLetter.text = smallLetter.text = "取";
                largeLetter.color = smallLetter.color = colorMap['Y'];
                bottomLetter.text = "?";

                specialDescribe = "随机抽取一张黄色字母牌";
                break;
            case SpecialEffectType.RandomBlueCard:
                // 实现获取一张随机蓝色卡牌的效果
                largeLetter.text = smallLetter.text = "取";
                largeLetter.color = smallLetter.color = colorMap['B'];
                bottomLetter.text = "?";

                specialDescribe = "随机抽取一张蓝色字母牌";
                break;
            case SpecialEffectType.RandomGreenCard:
                // 实现获取一张随机绿色卡牌的效果
                largeLetter.text = smallLetter.text = "取";
                largeLetter.color = smallLetter.color = colorMap['G'];
                bottomLetter.text = "?";

                specialDescribe = "随机抽取一张绿色字母牌";
                break;

        }
    }

    // 当点击 UI 时触发的方法
    public void ChooseCard()
    {
        SpecialCardState.instance.CardChoosing(transform);
    }

    public void UpCard()
    {
        GetComponent<RectTransform>().DOAnchorPosY(100, 0.3f).SetEase(Ease.OutQuad);
    }

    public void DownCard()
    { 
        GetComponent<RectTransform>().DOAnchorPosY(0, 0.3f).SetEase(Ease.OutQuad);
    }

    // 重写 ActivateEffect 方法，执行特殊卡牌的具体效果。
    // 该方法会根据卡牌的类型来执行不同的效果。
    public override void ActivateEffect()
    {
        // 这里是具体效果的实现，可能会根据 effectType 来执行不同的操作。
        // 例如，移除卡牌、交换卡牌等。
        switch (EffectType)
        {
            case SpecialEffectType.RemoveCard:
                // 实现移除卡牌的效果
                SpecialCardState.instance.IsDeleting = true;
                break;
            case SpecialEffectType.AddOneLetterHand:
                // 实现增加一个字母手牌上限的效果
                LevelController.instance.levelConfig.maxNormalCards += 1;
                break;
            case SpecialEffectType.AddOneCacheHand:
                // 实现增加一个缓存手牌上限的效果
                LevelController.instance.levelConfig.maxCacheCards += 1;
                break;
            case SpecialEffectType.AddOneSpecialHand:
                // 实现增加一个特殊手牌上限的效果
                LevelController.instance.levelConfig.maxSpecialCards += 1;
                break;
            case SpecialEffectType.RandomRedCard:
                // 实现获取一张随机红色卡牌的效果
                DeckManager.instance.DrawRandomLetterCardByColor('R');
                break;
            case SpecialEffectType.RandomYellowCard:
                // 实现获取一张随机黄色卡牌的效果
                DeckManager.instance.DrawRandomLetterCardByColor('Y');
                break;
            case SpecialEffectType.RandomBlueCard:
                // 实现获取一张随机蓝色卡牌的效果
                DeckManager.instance.DrawRandomLetterCardByColor('B');
                break;
            case SpecialEffectType.RandomGreenCard:
                // 实现获取一张随机绿色卡牌的效果
                DeckManager.instance.DrawRandomLetterCardByColor('G');
                break;

        }
        SpecialCardState.instance.chosenCard = null;//将选择的牌置为空
        DestroyCard();
    }

    /// <summary>
    /// 触发卡牌逐渐消失的动画
    /// </summary>
    private void DestroyCard()
    {
        // 使用 DOTween 来逐渐改变透明度
        canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            // 动画完成后可以销毁卡牌或执行其他操作
            Destroy(gameObject);
        });
    }
}

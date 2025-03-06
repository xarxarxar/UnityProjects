using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// FunctionCard 类是Card的子类，表示特殊卡牌。
// 特殊卡牌拥有不同的效果，可以通过 ActivateEffect 方法来激活这些效果。
public class FunctionCard : Card
{
    // 特殊效果类型，定义了该特殊卡牌的效果种类，例如：移除卡牌、交换卡牌等。
    [SerializeField] private  SpecialEffectType effectType;
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

    SimpleTouch simpleTouch;


    private void OnEnable()
    {
        OnInstantiate();
        simpleTouch = GetComponent<SimpleTouch>();

        simpleTouch.onClick += OnCardClick;//单击的方法
    }

    private void OnInstantiate()
    {
        // 这里是具体效果的实现，可能会根据 effectType 来执行不同的操作。
        // 例如，移除卡牌、交换卡牌等。
        switch (EffectType)
        {
            case SpecialEffectType.RemoveCard:
                // 实现移除卡牌的效果
                largeLetter.text = "字迹\n擦除";
                largeLetter.color = Color.black;
                //bottomLetter.text = "-1";
                specialDescribe = "选择一张字母牌，将其丢弃";
                break;
            case SpecialEffectType.AddOneLetterHand:
                // 实现增加一个字母手牌上限的效果
                largeLetter.text = "字库\n扩容";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+1";

                specialDescribe = "字母牌上限+1";
                break;
            case SpecialEffectType.AddOneCacheHand:
                // 实现增加一个缓存手牌上限的效果
                largeLetter.text = "暂存\n书签";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+1";

                specialDescribe = "出牌上限+1";
                break;
            case SpecialEffectType.AddOneSpecialHand:
                // 实现增加一个功能手牌上限的效果
                largeLetter.text = "特殊\n字库";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+1";

                specialDescribe = "功能牌上限+1";
                break;

            case SpecialEffectType.AddOneStateHand:
                // 实现增加一个功能手牌上限的效果
                largeLetter.text = "状态\n扩充";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+1";

                specialDescribe = "状态上限+1";
                break;
            case SpecialEffectType.RandomRedCard:
                // 实现获取一张随机红色卡牌的效果
                largeLetter.text = "随机\n抽取";
                largeLetter.color = colorMap['R'];
                //bottomLetter.text = "?";

                specialDescribe = "随机抽取一张红色字母牌";
                break;
            case SpecialEffectType.RandomYellowCard:
                // 实现获取一张随机黄色卡牌的效果
                largeLetter.text = "随机\n抽取";
                largeLetter.color = colorMap['Y'];
                //bottomLetter.text = "?";

                specialDescribe = "随机抽取一张黄色字母牌";
                break;
            case SpecialEffectType.RandomBlueCard:
                // 实现获取一张随机蓝色卡牌的效果
                largeLetter.text = "随机\n抽取";
                largeLetter.color = colorMap['B'];
                //bottomLetter.text = "?";

                specialDescribe = "随机抽取一张蓝色字母牌";
                break;
            case SpecialEffectType.RandomGreenCard:
                // 实现获取一张随机绿色卡牌的效果
                largeLetter.text = "随机\n抽取";
                largeLetter.color = colorMap['G'];
                //bottomLetter.text = "?";

                specialDescribe = "随机抽取一张绿色字母牌";
                break;
            case SpecialEffectType.ExtraScoreOnlyOne:
                // 孤字成章,如果只出一张牌的话，该回合额外加分，整个关卡起作用
                largeLetter.text = "孤字\n成章";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "如果只出一张牌的话，该回合额外加分";
                break;
            case SpecialEffectType.CanPlayZeroCard:
                // 空白书卷，可以在没有选择牌的时候结束当前回合
                largeLetter.text = "空白\n书卷";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "可以在没有选择牌的时候出牌来结束当前回合";
                break;
            case SpecialEffectType.AddScoreWhenDelete:
                // 弃字生金，每丢弃三张牌，获取6分
                largeLetter.text = "弃字\n生金";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "每丢弃三张牌，获取6分";
                break;
            case SpecialEffectType.ExtraScoreRounOver:
                // 字量结余，回合结束时，增加额外分数，分数为当前手牌的数量
                largeLetter.text = "字量\n结余";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "回合结束时，增加额外分数，分数为当前手牌的数量";
                break;
            case SpecialEffectType.CanContinuousDraw:
                // 连抽不止，每次抽牌有概率连续抽牌，抽牌次数上限为两次
                largeLetter.text = "连抽\n不止";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "每次抽牌有概率连续抽牌，抽牌次数上限为两次";
                break;
            case SpecialEffectType.AddContinuousDraw:
                // 抽运加成，增加连续抽牌的概率百分之5
                largeLetter.text = "抽运\n加成";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "增加连续抽牌的概率百分之5";
                break;
            case SpecialEffectType.AddContinuousLimit:
                // 抽卡极限，增加连续抽牌的次数上限，每次触发连续抽牌之后，概率变为当前概率一半，下一次抽牌时恢复至初始概率
                largeLetter.text = "抽卡\n极限";
                largeLetter.color = Color.black;
                //bottomLetter.text = "+2";

                specialDescribe = "增加连续抽牌的次数上限，当前为{3}";
                break;

        }
    }

    private void OnCardClick()
    {
        GameManager.Instance.specialIntroductionPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuart);
        GameManager.Instance.specialCardNameText.text = largeLetter.text;
        GameManager.Instance.specialCardNameText.color = largeLetter.color;
        GameManager.Instance.specialCardDescriptionText.text = specialDescribe;
        GameManager.Instance.useSpecialCard = ActivateEffect;
    }

    // 当点击 UI 时触发的方法
    public void ChooseCard()
    {
        AudioManager.instance.PlaySoundEffect("ClickCard");
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
                Debug.Log($"SpecialCardState.instance.IsDeleting is {SpecialCardState.instance.IsDeleting}");
                break;
            case SpecialEffectType.AddOneLetterHand:
                // 实现增加一个字母手牌上限的效果
                //LevelController.instance.IncreaseHandLimit(1);
                break;
            case SpecialEffectType.AddOneCacheHand:
                // 实现增加一个缓存手牌上限的效果
                //LevelController.instance.IncreaseCacheLimit(1);
                break;
            case SpecialEffectType.AddOneSpecialHand:
                // 实现增加一个特殊手牌上限的效果
                //LevelController.instance.IncreaseSpecialLimit(1);
                break;
            case SpecialEffectType.RandomRedCard:
                // 实现获取一张随机红色卡牌的效果
                if (!DeckManager.instance.DrawRandomLetterCardByColor('R'))
                {
                    SpecialCardState.instance.CardChoosing(null);
                    SpecialCardState.instance.ChosenCard = null;//将选择的牌置为空
                    //如果抽取不成功，则不要销毁这个卡牌
                    return;
                }
                break;
            case SpecialEffectType.RandomYellowCard:
                // 实现获取一张随机黄色卡牌的效果
                if (!DeckManager.instance.DrawRandomLetterCardByColor('Y'))
                {
                    SpecialCardState.instance.CardChoosing(null);
                    SpecialCardState.instance.ChosenCard = null;//将选择的牌置为空
                    //如果抽取不成功，则不要销毁这个卡牌
                    return;
                }
                break;
            case SpecialEffectType.RandomBlueCard:
                // 实现获取一张随机蓝色卡牌的效果
                if (!DeckManager.instance.DrawRandomLetterCardByColor('B'))
                {
                    SpecialCardState.instance.CardChoosing(null);
                    SpecialCardState.instance.ChosenCard = null;//将选择的牌置为空
                    //如果抽取不成功，则不要销毁这个卡牌
                    return;
                }
                break;
            case SpecialEffectType.RandomGreenCard:
                // 实现获取一张随机绿色卡牌的效果
                if (!DeckManager.instance.DrawRandomLetterCardByColor('G'))
                {
                    SpecialCardState.instance.CardChoosing(null);
                    SpecialCardState.instance.ChosenCard = null;//将选择的牌置为空
                    //如果抽取不成功，则不要销毁这个卡牌
                    return;
                }
                break;

        }
        SpecialCardState.instance.ChosenCard = null;//将选择的牌置为空
        DestroyCard();
    }

    /// <summary>
    /// 触发卡牌逐渐消失的动画
    /// </summary>
    private void DestroyCard()
    {
        transform.DOScale(0, 0.3f).OnComplete(() =>
        {
            // 动画完成后可以销毁卡牌或执行其他操作
            Destroy(gameObject);
        });
    }


}

using DG.Tweening;
using UnityEngine;

// Card 类是所有卡牌的基类，表示卡牌的基本属性和行为。
// 它是一个抽象类，意味着不能直接实例化。子类需要继承并实现具体的行为。
public abstract class Card : MonoBehaviour
{
    //// 卡牌类型，定义了卡牌的种类。例如字母卡牌、特殊卡牌等。
    //public CardType cardType;

    // 卡牌的图像，使用Sprite类型来显示卡牌的视觉效果。
    //public Sprite cardSprite;//此处直接使用文本代替

    // 激活卡牌效果的虚拟方法，允许子类根据不同的卡牌类型实现具体效果。
    // 这个方法是虚拟的，可以被子类重写，以实现不同的卡牌效果。

    public  bool IsSorted = false;//是否已经排列好了

    [SerializeField] public bool isFront = true;//是否是正面
    [SerializeField] private GameObject backSide;//卡牌的背面
    [SerializeField] private GameObject frontSide;//卡牌的正面
    
    //public bool IsSorted { get => isSorted; set => isSorted = value; }
    public virtual void ActivateEffect()
    {
        // 默认情况下，基类的实现为空，子类会重写此方法。
    }


    /// <summary>
    /// 旋转到背面
    /// </summary>
    public Sequence FlipCardToBack(float duration)
    {
        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();

        // 添加一个检查，如果 isFront 为 false 则停止序列
        sequence.AppendCallback(() =>
        {
            if (!isFront)
            {
                sequence.Kill(); // 立即停止序列
                return; // 已经是背面
            }
        });
        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(frontSide.transform.DOScaleX(0, duration)
            .SetEase(Ease.InOutCubic));

        // 第二个旋转动画：从目标角度旋转回零角度
        sequence.Append(backSide.transform.DOScaleX(1, duration)
            .SetEase(Ease.InOutBack));

        // 在第二个旋转动画完成后更新状态
        sequence.AppendCallback(() =>
        {
            isFront = false; // 现在是背面
        });

        return sequence;
    }

    /// <summary>
    /// 旋转到正面
    /// </summary>
    public Sequence FlipCardToFront(float duration)
    {
        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();

        // 添加一个检查
        sequence.AppendCallback(() =>
        {
            if (isFront)
            {
                sequence.Kill(); // 立即停止序列
                return; // 已经是背面
            }
        });

        // 第二个旋转动画：从目标角度旋转回零角度
        sequence.Append(backSide.transform.DOScaleX(0, duration)
            .SetEase(Ease.InOutCubic));

        // 第一个旋转动画：从当前角度旋转到目标角度
        sequence.Append(frontSide.transform.DOScaleX(1, duration)
            .SetEase(Ease.InOutBack));

        // 在第二个旋转动画完成后更新状态
        sequence.AppendCallback(() =>
        {
            isFront = true; // 现在是背面
        });

        return sequence;
    }

}



// 卡牌类型枚举，用于定义不同的卡牌类型
// 例如：字母卡牌（LetterCard）和特殊卡牌（SpecialCard）。
public enum CardType
{
    Letter,     // 字母卡牌
    Special     // 特殊卡牌
}

// 颜色类型枚举，定义卡牌的颜色类型。可以根据不同的卡牌分配不同的颜色。
//public enum ColorType
//{
//    Red,        // 红色
//    Green,      // 绿色
//    Blue,       // 蓝色
//    Yellow      // 黄色
//}

// 特殊效果类型枚举，定义特殊卡牌的效果种类
// 例如：移除卡牌、交换卡牌等。
public enum SpecialEffectType
{
    RemoveCard,         //  移除一张卡牌
    AddOneLetterHand,   //  增加一个字母手牌最大值
    AddOneCacheHand,    //  增加一个缓存手牌最大值
    AddOneSpecialHand,  //  增加一个特殊手牌最大值
    AddOneStateHand,    //  增加一个状态最大值
    RandomRedCard,      //  随机获取一张红色字母牌
    RandomYellowCard,   //  随机获取一张黄色字母牌
    RandomBlueCard,     //  随机获取一张蓝色字母牌
    RandomGreenCard,    //  随机获取一张绿色字母牌

    //状态
    ExtraScoreOnlyOne,  //  如果只出一张牌的话，每个额外加分，整个关卡起作用
    ExtraDrawLetter,    //  每回合多一次抽字母牌次数
    ExtraDrawSpecial,   //  每回合多一次抽功能牌次数
    ExtraScoreLevelOver,//  关卡结束的时候额外加分，加分值为当前关卡中分值最大的一回合


}


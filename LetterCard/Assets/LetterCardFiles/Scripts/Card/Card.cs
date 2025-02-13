using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Card 类是所有卡牌的基类，表示卡牌的基本属性和行为。
// 它是一个抽象类，意味着不能直接实例化。子类需要继承并实现具体的行为。
public abstract class Card : MonoBehaviour
{
    public static float cardWidth = (float)(390 * 3) / 7;
    //// 卡牌类型，定义了卡牌的种类。例如字母卡牌、特殊卡牌等。
    //public CardType cardType;

    // 卡牌的图像，使用Sprite类型来显示卡牌的视觉效果。
    //public Sprite cardSprite;//此处直接使用文本代替

    // 激活卡牌效果的虚拟方法，允许子类根据不同的卡牌类型实现具体效果。
    // 这个方法是虚拟的，可以被子类重写，以实现不同的卡牌效果。
    public virtual void ActivateEffect()
    {
        // 默认情况下，基类的实现为空，子类会重写此方法。
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
public enum ColorType
{
    Red,        // 红色
    Green,      // 绿色
    Blue,       // 蓝色
    Yellow      // 黄色
}

// 特殊效果类型枚举，定义特殊卡牌的效果种类
// 例如：移除卡牌、交换卡牌等。
public enum SpecialEffectType
{
    RemoveCard,  // 移除一张卡牌
    SwapCard     // 交换一张卡牌
}


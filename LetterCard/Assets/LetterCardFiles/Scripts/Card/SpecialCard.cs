using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SpecialCard 类是Card的子类，表示特殊卡牌。
// 特殊卡牌拥有不同的效果，可以通过 ActivateEffect 方法来激活这些效果。
public class SpecialCard : Card
{
    // 特殊效果类型，定义了该特殊卡牌的效果种类，例如：移除卡牌、交换卡牌等。
    public SpecialEffectType effectType;

    // spawnWeight：表示这张特殊牌的生成权重，权重越高，生成的概率越大
    public float spawnWeight = 1f;

    // initialPoolSize：表示初始牌池中的特殊牌数量
    public int initialPoolSize = 3;


    // 重写 ActivateEffect 方法，执行特殊卡牌的具体效果。
    // 该方法会根据卡牌的类型来执行不同的效果。
    public override void ActivateEffect()
    {
        // 这里是具体效果的实现，可能会根据 effectType 来执行不同的操作。
        // 例如，移除卡牌、交换卡牌等。
        switch (effectType)
        {
            case SpecialEffectType.RemoveCard:
                // 实现移除卡牌的效果
                break;
            case SpecialEffectType.SwapCard:
                // 实现交换卡牌的效果
                break;
                // 其他效果类型可以根据需要添加
        }
    }
}

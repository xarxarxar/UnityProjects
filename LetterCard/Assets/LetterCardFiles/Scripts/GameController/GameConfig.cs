using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    /// <summary>
    /// ×ÖÄ¸ºÍÑÕÉ«µÄÓ³Éä×Öµä
    /// </summary>
    public static Dictionary<char, Color32> colorMap = new Dictionary<char, Color32>
        {
            { 'R', new Color32(194,24,91,255) },
            { 'G', new Color32(56,142,60,255) },
            { 'B', new Color32(48,63,159,255) },
            { 'Y', new Color32(255,162,0,255) }
        };

    /// <summary>
    /// ÌØÊâÅÆºÍ¸ÅÂÊµÄÓ³Éä×Öµä
    /// </summary>
    public static Dictionary<SpecialEffectType, float> specialCardWeightDic =
        new Dictionary<SpecialEffectType, float>()
        {
            {SpecialEffectType.RemoveCard, 0.2f},
            {SpecialEffectType.AddOneLetterHand, 0.2f},
            {SpecialEffectType.AddOneCacheHand, 0.2f},
            {SpecialEffectType.AddOneSpecialHand, 0.2f},
            {SpecialEffectType.AddOneStateHand, 0.2f},

            {SpecialEffectType.RandomRedCard, 0.3f},
            {SpecialEffectType.RandomYellowCard, 0.3f},
            {SpecialEffectType.RandomGreenCard, 0.3f},
            {SpecialEffectType.RandomBlueCard, 0.3f},

            {SpecialEffectType.ExtraScoreOnlyOne, 0.3f},
            {SpecialEffectType.CanPlayZeroCard, 0.3f},
            {SpecialEffectType.AddScoreWhenDelete, 0.3f},
            {SpecialEffectType.ExtraScoreRounOver, 0.3f},
            {SpecialEffectType.CanContinuousDraw, 0.3f},
            {SpecialEffectType.AddContinuousDraw, 0.3f},
            {SpecialEffectType.AddContinuousLimit, 0.3f},
        };
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMultiDamage : SingleCard
{
    public override void OnCardEffect()
    {
        var effect = new MultiArrowEffect(
            CurrentSlot,
            targetRole,
            arrowCount: 3,
            interval: 0.3f,
            flyTime: 0.8f
        );

        CardEffectManager.instance.PlayEffect(effect);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : SingleCard
{
    private float flyTime = 0.8f;

    public override void OnCardEffect()
    {
        var effect = new ArrowDamageEffect(
            CurrentSlot,
            targetRole,
            flyTime,
            damage: 1
        );

        CardEffectManager.instance.PlayEffect(effect);
    }
}

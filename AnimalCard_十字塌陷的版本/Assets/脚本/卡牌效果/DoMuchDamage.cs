using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMuchDamage : SingleCard
{
    private float flyTime = 1.0f;

    public override void OnCardEffect()
    {
        var effect = new DoMuchDamageEffect(
            CurrentSlot,
            targetRole,
            flyTime,
            damage: 1
        );

        CardEffectManager.instance.PlayEffect(effect);
    }
}

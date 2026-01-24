using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShield : SingleCard
{
    public override void OnCardEffect()
    {
        var effect = new AddShieldEffect(
            CurrentSlot,
            targetRole,
            flyTime: 0.8f
        );

        CardEffectManager.instance.PlayEffect(effect);
    }
}

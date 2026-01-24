using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHp : SingleCard
{
    private float flyTime = 1.0f;

    public override void OnCardEffect()
    {
        var effect = new TreatmentEffect(
            CurrentSlot,
            selfRole,
            flyTime,
            treat: 1
        );

        CardEffectManager.instance.PlayEffect(effect);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHp : SingleCard
{
    private float flyTime = 1.0f;

    public override IEnumerator OnCardEffect(Vector3 startPos, int level)
    {
        var effect = new TreatmentEffect(
            startPos,
            selfRole,
            flyTime,
            treat: 1,
            count:level,
            interval:0.3f
        );

        yield return CardEffectManager.instance.PlayEffect(effect);
    }


}

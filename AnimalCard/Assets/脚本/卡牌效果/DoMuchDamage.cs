using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMuchDamage : SingleCard
{
    private float flyTime = 1.0f;

    public override IEnumerator OnCardEffect(Vector3 startPos, int level)
    {
        var effect = new DoMuchDamageEffect(
            fromPos: startPos,
            targetRole: targetRole,
            flyTime: flyTime,
            damage: 1,
            interval:0.3f,
            count:level
        );

      yield return CardEffectManager.instance.PlayEffect(effect);
    }
}

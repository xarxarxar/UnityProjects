using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShield : SingleCard
{
    public override IEnumerator OnCardEffect(Vector3 startPos, int level)
    {
        var effect = new AddShieldEffect(
            startPos,
            targetRole,
            flyTime: 0.8f,
            count:level,
            interval:0.3f
        );

        yield return CardEffectManager.instance.PlayEffect(effect);
    }
}

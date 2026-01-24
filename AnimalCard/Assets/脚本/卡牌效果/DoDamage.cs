using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : SingleCard
{
    private float flyTime = 0.8f;

    public override IEnumerator OnCardEffect(Vector3 startPos, int level)
    {
        var effect = new MultiArrowEffect(
            startPos,
            targetRole,
            arrowCount: level,
            interval: 0.3f,
            flyTime: 0.8f
        );

        yield return CardEffectManager.instance.PlayEffect(effect);
    }
}

using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ForestStrength : SkillBase
{
    public Element elementToChange;
    public float timeLife = 1f;

    public override void Passive(BeastBase attacker)
    {
        StartCoroutine(ChangeElement());
    }

    IEnumerator ChangeElement()
    {
        if (elementToChange == Element.None) yield break;

        GameManager.instance.MyTeamElement = elementToChange;
        yield return new WaitForSeconds(timeLife);
        
        ObjectPool.Instance.ReleaseObject(gameObject);

        GameManager.instance.MyTeamElement = Element.None;
    }
}

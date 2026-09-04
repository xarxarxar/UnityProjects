using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Melee : SkillBase
{
    public int skillIndex;//0, 1
    public bool hitFoot = false;

    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        var attackerInfo = attacker.GetAttackerInfo(effectHitPref,effectHitPref, skillIndex);
        var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);

        var AOE = attacker.skillDes[skillIndex - 1].AOE;
        if (AOE == 0f)
        {
            // Debug.Log("CalDamage enemy attack:" + gameObject.name + " AOE:" + AOE);
            attacker.RaiseDamage(target, attackParam, false);
        }
        else
        {
            var colliders = Physics2D.OverlapCircleAll(target.transform.position, AOE);
            // Debug.Log("CalDamage enemy attack:" + gameObject.name + " colliders:" + colliders.Length);
            var targetEntity = target.GetComponent<EntityBase>();
            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                var entity = collider.GetComponent<EntityBase>();
                if (entity != null && !entity.isDie && entity.currentTeam != attacker.currentTeam)
                {
                    // Debug.Log("CalDamage enemy attack:" + gameObject.name + " entity:" + entity.name);
                    if(hitFoot) attackParam.collidePosition = entity.transform.position;
                    attacker.RaiseDamage(collider.gameObject, attackParam, entity != targetEntity);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ThornSkill02 : SkillBase
{
    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        base.Active(order, attacker, target);
        var skeleton = attacker.skeletonAnimation.skeleton;
        var targetTip = skeleton.FindBone("target_tip");

        // Debug.Log("Thorn OnSkill02");

        var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

        if (targetTip != null)
        {
            // Debug.Log("Thorn OnSkill02 2");

            var startPos = attacker.skeletonAnimation.transform.TransformPoint(new Vector3(targetTip.WorldX, targetTip.WorldY, 0f));

            var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
            var bullet = bulletGo.GetComponent<BulletThornSkill02>();

            targetPos = startPos;
            var med = startPos;

            var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 2);
            var bulletParam = new BulletParam(0, 0, attackerInfo, target.GetComponent<EntityBase>().curIndex, false, null);
            bullet.Setup(attacker.currentTeam, startPos, bulletParam, med, targetPos, attacker.skillDes[1].AOE);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class AtlantusSkill02 : SkillBase
{
    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        base.Active(order, attacker, target);

        var skeleton = attacker.skeletonAnimation.skeleton;
        var boneGunTip1 = skeleton.FindBone("gun_tip2");

        if (bulletPref != null)
        {
            if (boneGunTip1 != null)
            {
                var startPos = attacker.skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));

                var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
                var bullet = bulletGo.GetComponent<BulletAtlantisSkill02>();

                var attackerInfo = attacker.GetAttackerInfo(effectHitPref,effectHitPref, 2);
                var bulletParam = new BulletParam(0, 0, attackerInfo, target.GetComponent<EntityBase>().curIndex, true, null);
                bullet.Setup(attacker.currentTeam, startPos, bulletParam, startPos, startPos, attacker.skillDes[1].AOE);
            }
        }
    }
}

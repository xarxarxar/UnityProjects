using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;

public class GrooVine : BeastBase
{
    // protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill01(skeletonAnimation, target);

    //     Shoot(skeletonAnimation, target, 1, 1);
    // }

    // protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill02(skeletonAnimation, target);

    //     Shoot(skeletonAnimation, target, 1, 2);
    // }

    // private void Shoot(SkeletonAnimation skeletonAnimation, GameObject target, int gunTipIndex, int skillIndex)
    // {
    //     var skeleton = skeletonAnimation.skeleton;
    //     var boneGunTip1 = skeleton.FindBone("gun_tip1");

    //     if (bulletPref01 != null)
    //     {
    //         var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

    //         // var damage = DamageValue(target);

    //         if (boneGunTip1 != null && gunTipIndex == 1)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
    //             GenBulletSkill01(startPos, targetPos, 1.5f, skillIndex, target.GetComponent<EntityBase>().curIndex);
    //         }

    //         // if (boneGunTip2 != null && type == 2)
    //         // {
    //         //     var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip2.WorldX, boneGunTip2.WorldY, 0f));
    //         //     GenBulletSkill01(startPos, targetPos, damage, 2f);

    //         //     skill02Th = 0;
    //         // }
    //     }
    // }

    // private void GenBulletSkill01(Vector3 startPos, Vector3 targetPos, float offset, int skillIndex, int targetIndex)
    // {
    //     var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref01, startPos, Quaternion.identity);
    //     var bullet = bulletGo.GetComponent<BulletBezier>();

    //     targetPos = new Vector3(targetPos.x, startPos.y, 0);
    //     var med = (startPos + targetPos) / 2;

    //     var attackerInfo = GetAttackerInfo(effectHit01Pref, skillIndex);
    //     var bulletParam = new BulletParam(0, 0, attackerInfo, targetIndex);
    //     bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos);
    // }
}

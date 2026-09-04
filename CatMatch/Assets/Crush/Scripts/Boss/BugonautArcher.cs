using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;

// VulcaArcher,BugonautArcher
public class BugonautArcher : BeastBase
{

    // protected override void Start()
    // {
    //     base.Start();
    // }

    // protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill01(skeletonAnimation, target);
    //     // Debug.Log("OnSkill01");
    //     var skeleton = skeletonAnimation.skeleton;
    //     var boneGunTip1 = skeleton.FindBone("gun_tip1");

    //     if (bulletPref01 != null)
    //     {
    //         var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

    //         if (boneGunTip1 != null)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
    //             GenBulletSkill01(startPos, targetPos, 1.5f, 1,target.GetComponent<EntityBase>().curIndex);
    //         }
    //     }
    // }

    // protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill02(skeletonAnimation, target);
    //     // Debug.Log("OnSkill01");
    //     var skeleton = skeletonAnimation.skeleton;
    //     var boneGunTip1 = skeleton.FindBone("gun_tip1");
    //     var boneGunTip2 = skeleton.FindBone("gun_tip2");

    //     if (bulletPref01 != null)
    //     {
    //         var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

    //         // var damage = DamageValue(target, curSp2MOD);

    //         if (boneGunTip1 != null)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
    //             GenBulletSkill01(startPos, targetPos, 1.5f, 2,target.GetComponent<EntityBase>().curIndex);
    //         }

    //         if (boneGunTip2 != null)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip2.WorldY, 0f));
    //             GenBulletSkill01(startPos, targetPos, 2f, 2, target.GetComponent<EntityBase>().curIndex);
    //         }
    //     }
    // }

    // private void GenBulletSkill01(Vector3 startPos, Vector3 targetPos, float offset, int skill, int targetIndex)
    // {
    //     var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref01, startPos, Quaternion.identity);
    //     var bullet = bulletGo.GetComponent<BulletBezier>();

    //     var med = (startPos + targetPos) / 2;
    //     med.y += offset;

    //     var attackerInfo = GetAttackerInfo(effectHit01Pref, skill);
    //     var bulletParam = new BulletParam(0, 0, attackerInfo, targetIndex);
    //     bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos);
    // }
}

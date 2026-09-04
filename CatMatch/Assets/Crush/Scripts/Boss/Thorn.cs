using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;

public class Thorn : BeastBase
{
//     int i = 0;
//     protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
//     {
//         base.OnSkill01(skeletonAnimation, target);
//         Shoot(skeletonAnimation, target, 1, 1);
//     }

//     protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
//     {
//         base.OnSkill02(skeletonAnimation, target);
//         var skeleton = skeletonAnimation.skeleton;
//         var boneGunTip1 = skeleton.FindBone("target_tip");

//         // Debug.Log("Thorn OnSkill02");

//         // if (bulletPref01 != null)
//         // {
//             // Debug.Log("Thorn OnSkill02 1");

//             var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

//             if (boneGunTip1 != null)
//             {
//                 // Debug.Log("Thorn OnSkill02 2");

//                 var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));

//                 var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref02, startPos, Quaternion.identity);
//                 var bullet = bulletGo.GetComponent<BulletThornSkill02>();

//                 targetPos = startPos;
//                 var med = startPos;

//                 var attackerInfo = GetAttackerInfo(effectHit01Pref, 2);
//                 var bulletParam = new BulletParam(0, 0, attackerInfo, target.GetComponent<EntityBase>().curIndex);
//                 bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos, skillDes[1].AOE);
//             }
//         // }
//     }

//     private void Shoot(SkeletonAnimation skeletonAnimation, GameObject target, int type, int skill)
//     {
//         var skeleton = skeletonAnimation.skeleton;
//         var boneGunTip1 = skeleton.FindBone("gun_tip1");

//         if (bulletPref01 != null)
//         {
//             var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

//             // var damage = DamageValue(target);

//             if (boneGunTip1 != null && type == 1)
//             {
//                 var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
//                 GenBulletSkill01(startPos, targetPos, 1.5f, skill, target.GetComponent<EntityBase>().curIndex);
//             }

//             // if (boneGunTip2 != null && type == 2)
//             // {
//             //     var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip2.WorldX, boneGunTip2.WorldY, 0f));
//             //     GenBulletSkill01(startPos, targetPos, damage, 2f);

//             //     skill02Th = 0;
//             // }
//         }
//     }

//     private void GenBulletSkill01(Vector3 startPos, Vector3 targetPos, float offset, int skill, int targetIndex)
//     {
//         var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref01, startPos, Quaternion.identity);
//         var bullet = bulletGo.GetComponent<BulletBezier>();

//         targetPos = new Vector3(targetPos.x, startPos.y, 0);
//         var med = (startPos + targetPos) / 2;

//         var attackerInfo = GetAttackerInfo(effectHit01Pref, skill);
//         var bulletParam = new BulletParam(0, 0, attackerInfo, targetIndex);

//         bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos);
//     }
}

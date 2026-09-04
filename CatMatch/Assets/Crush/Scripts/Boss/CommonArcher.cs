using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;


public class CommonArcher : BeastBase
{
    // public int gunTipSkill01 = 1;//gun_tip1
    // public int gunTipSkill02 = 1;//gun_tip1
    // public float bulletOffsetSkill01 = 0f;
    // public float bulletOffsetSkill02 = 0f;

    // private int skill02BulletTh;

    // protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill01(skeletonAnimation, target);
    //     int skillIndex = 1;
    //     skill02BulletTh = 0;
    //     Shoot(skeletonAnimation, target, skillIndex);
    // }

    // protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill02(skeletonAnimation, target);
    //     int skillIndex = 2;
    //     skill02BulletTh++;
    //     Shoot(skeletonAnimation, target, skillIndex);
    // }

    // private void Shoot(SkeletonAnimation skeletonAnimation, GameObject target, int skillIndex)
    // {
    //     var skeleton = skeletonAnimation.skeleton;
    //     var boneGunTip1 = skeleton.FindBone("gun_tip1");
    //     var boneGunTip2 = skeleton.FindBone("gun_tip2");

    //     var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

    //     if (
    //         (skillIndex == 1 && (gunTipSkill01 == 1 || gunTipSkill01 == 3))
    //         || (skillIndex == 2 && (gunTipSkill02 == 1 || gunTipSkill02 == 3))
    //         )
    //     {
    //         if (boneGunTip1 != null)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
    //             GenBullet(startPos, targetPos, skill02BulletTh * bulletOffsetSkill02, skillIndex, target.GetComponent<EntityBase>().curIndex);
    //         }
    //     }

    //     if (
    //         (skillIndex == 1 && (gunTipSkill01 == 2 || gunTipSkill01 == 3))
    //         || (skillIndex == 2 && (gunTipSkill02 == 2 || gunTipSkill02 == 3))
    //         )
    //     {
    //         if (boneGunTip2 != null)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip2.WorldX, boneGunTip2.WorldY, 0f));
    //             GenBullet(startPos, targetPos, skill02BulletTh * bulletOffsetSkill02, skillIndex, target.GetComponent<EntityBase>().curIndex);
    //         }
    //     }
    // }

    // private void GenBullet(Vector3 startPos, Vector3 targetPos, float offSetY, int skillIndex, int targetIndex)
    // {
    //     var bulletGo = ObjectPool.Instance.GetGameObject(skillIndex == 1 ? bulletPref01 : bulletPref02, startPos, Quaternion.identity);
    //     var bullet = bulletGo.GetComponent<BulletBezier>();

    //     var med = (startPos + targetPos) / 2;
    //     med.y += offSetY;

    //     var attackerInfo = GetAttackerInfo(skillIndex == 1 ? effectHit01Pref : effectHit02Pref, skillIndex);
    //     var bulletParam = new BulletParam(0, 0, attackerInfo, targetIndex);
    //     bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos);
    // }
}

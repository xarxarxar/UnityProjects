using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class CommonFlyer : BeastBase
{
    // protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill01(skeletonAnimation, target);
    //     int skillIndex = 1;
    //     Shoot(skeletonAnimation, target, skillIndex);
    // }

    // protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill02(skeletonAnimation, target);
    //     int skillIndex = 2;
    //     Shoot(skeletonAnimation, target, skillIndex);
    // }

    // private void Shoot(SkeletonAnimation skeletonAnimation, GameObject target, int skillIndex)
    // {
    //     var skeleton = skeletonAnimation.skeleton;
    //     var boneGunTip1 = skeleton.FindBone("gun_tip1");
    //     var boneGunTip2 = skeleton.FindBone("gun_tip2");

    //     var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

    //     if (boneGunTip1 != null)
    //     {
    //         var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
    //         GenBullet(startPos, targetPos, 1.5f, skillIndex, target.GetComponent<EntityBase>().curIndex);
    //     }

    //     if (boneGunTip2 != null)
    //     {
    //         var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip2.WorldX, boneGunTip2.WorldY, 0f));
    //         GenBullet(startPos, targetPos, 1.5f, skillIndex, target.GetComponent<EntityBase>().curIndex);
    //     }
    // }

    // private void GenBullet(Vector3 startPos, Vector3 targetPos, float offset, int skillIndex, int targetIndex)
    // {
    //     var bulletGo = ObjectPool.Instance.GetGameObject(skillIndex == 1 ? bulletPref01 : bulletPref02, startPos, Quaternion.identity);
    //     var bullet = bulletGo.GetComponent<BulletBezier>();

    //     var med = (startPos + targetPos) / 2;

    //     var attackerInfo = GetAttackerInfo(skillIndex == 1 ? effectHit01Pref : effectHit02Pref, skillIndex);
    //     var bulletParam = new BulletParam(0, 0, attackerInfo, targetIndex);
    //     bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos);
    // }
}

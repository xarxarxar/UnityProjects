
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;

// Atlantus, MechaValken
public class Atlantus : BeastBase
{
    // protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill01(skeletonAnimation, target);

    //     int skillIndex = 1;
    //     var attackerInfo = GetAttackerInfo(effectHit01Pref, skillIndex);
    //     var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);
    //     CalDamage(skillDes[skillIndex - 1].AOE, target, attackParam);
    // }

    // private void CalDamage(float AOE, GameObject target, AttackParam atpr)
    // {
    //     if (AOE == 0f)
    //     {
    //         // Debug.Log("CalDamage enemy attack:" + gameObject.name + " AOE:" + AOE);
    //         RaiseDamage(target, atpr, false);
    //     }
    //     else
    //     {
    //         var colliders = Physics2D.OverlapCircleAll(target.transform.position, AOE);
    //         // Debug.Log("CalDamage enemy attack:" + gameObject.name + " colliders:" + colliders.Length);
    //         var targetEntity = target.GetComponent<EntityBase>();
    //         for (int i = 0; i < colliders.Length; i++)
    //         {
    //             var collider = colliders[i];
    //             var entity = collider.GetComponent<EntityBase>();
    //             if (entity != null && !entity.isDie && entity.currentTeam != this.currentTeam)
    //             {
    //                 // Debug.Log("CalDamage enemy attack:" + gameObject.name + " entity:" + entity.name);
    //                 RaiseDamage(collider.gameObject, atpr, targetEntity != entity);
    //             }
    //         }
    //     }
    // }

    // protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill02(skeletonAnimation, target);

    //     // Debug.Log("OnSkill02");

    //     var skeleton = skeletonAnimation.skeleton;
    //     var boneGunTip1 = skeleton.FindBone("gun_tip2");

    //     if (bulletPref02 != null)
    //     {
    //         // var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

    //         // var damage = DamageValue(target);

    //         if (boneGunTip1 != null)
    //         {
    //             var startPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
    //             GenBulletSkill02(startPos, startPos, this.curSp2MOD, 0f, target.GetComponent<EntityBase>().curIndex, skillDes[1].AOE);
    //         }
    //     }
    // }

    // private void GenBulletSkill02(Vector3 startPos, Vector3 targetPos, float damage, float offset, int targetIndex, float aoe)
    // {
    //     var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref02, startPos, Quaternion.identity);
    //     var bullet = bulletGo.GetComponent<BulletAtlantisSkill02>();

    //     targetPos = new Vector3(targetPos.x, startPos.y, 0);
    //     var med = (startPos + targetPos) / 2;
    //     // med.y += offset;

    //     var attackerInfo = GetAttackerInfo(effectHit02Pref, 2);
    //     var bulletParam = new BulletParam(0, 0, attackerInfo, targetIndex);
    //     bullet.Setup(currentTeam, startPos, bulletParam, med, targetPos, aoe);
    // }
}
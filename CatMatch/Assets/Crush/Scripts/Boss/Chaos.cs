using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using Spine;

public class Chaos : BeastBase
{
    // private int numHitSkill02;

    // protected override void OnSkill01(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill01(skeletonAnimation, target);

    //     // reset
    //     numHitSkill02 = 0;
    //     int skillIndex = 1;

    //     var attackerInfo = GetAttackerInfo(effectHit01Pref, skillIndex);
    //     var atpr = new AttackParam(Constant.Vector3Default, attackerInfo);
    //     CalDamage(skillDes[skillIndex - 1].AOE, target, atpr);
    // }

    // protected override void OnSkill02(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     base.OnSkill02(skeletonAnimation, target);
    //     numHitSkill02++;
    //     int skillIndex = 2;
    //     // Debug.Log(instanceId + " OnSkill01 numHitSkill02:" + numHitSkill02);

    //     var attackerInfo = GetAttackerInfo(effectHit01Pref, skillIndex);
    //     var atpr = new AttackParam(Constant.Vector3Default, attackerInfo);
    //     CalDamage(skillDes[skillIndex - 1].AOE, target, atpr);
    // }

    // private void CalDamage(float AOE, GameObject target, AttackParam atpr)
    // {
    //     if (AOE == 0f)
    //     {
    //         Debug.Log("CalDamage enemy attack:" + gameObject.name + " AOE:" + AOE);
    //         RaiseDamage(target, atpr, false);
    //     }
    //     else
    //     {
    //         var colliders = Physics2D.OverlapCircleAll(target.transform.position, AOE);
    //         Debug.Log("CalDamage enemy attack:" + gameObject.name + " colliders:" + colliders.Length);
    //         var targetEntity = target.GetComponent<EntityBase>();
    //         for (int i = 0; i < colliders.Length; i++)
    //         {
    //             var collider = colliders[i];
    //             var entity = collider.GetComponent<EntityBase>();
    //             if (entity != null && !entity.isDie && entity.currentTeam != this.currentTeam)
    //             {
    //                 Debug.Log("CalDamage enemy attack:" + gameObject.name + " entity:" + entity.name);
    //                 RaiseDamage(collider.gameObject, atpr, entity != targetEntity);
    //             }
    //         }
    //     }
    // }

    // protected override void OnSkill03(SkeletonAnimation skeletonAnimation, GameObject target)
    // {
    //     var startPos = transform.position + new Vector3(0, 2, 0);

    //     var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref03, startPos, Quaternion.identity);
    //     var bullet = bulletGo.GetComponent<BulletDarkKnight03>();

    //     var attackerInfo = GetAttackerInfo(effectHit02Pref, 3);
    //     var bulletParam = new BulletParam(0, 0, attackerInfo, target.GetComponent<EntityBase>().curIndex);
    //     bullet.Setup(currentTeam, startPos, bulletParam, transform.position);
    // }
}

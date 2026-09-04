using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloosLust : SkillBase
{
    // public GameObject bat;
    public int numHit = 1;

    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        base.Active(order, attacker, target);
        var opTeam = attacker.currentTeam == Team.My ? Team.Enemy : Team.My;

        // var enemies = GameManager.instance.GetEntityInTeam(opTeam);
        // var enemies = GameManager.instance.GetEntityInTeam(opTeam, true);

        // Debug.Log("enemies:" + enemies.Count());

        // var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 2, onHitCombo: false);
        // var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);

        // if (enemies != null && enemies.Count() > 0)
        // {
            StartCoroutine(Run(attacker, target));
        // }
    }
    IEnumerator Run(BeastBase attacker, GameObject target)
    {
        Debug.Log("attackerInfo.specialSkill: numHit " + numHit);
        var skeleton = attacker.skeletonAnimation.skeleton;
        var boneGunTip1 = skeleton.FindBone("gun_tip1");

        var startPos = attacker.skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f));
        // var enemy = enemies.ElementAt(UnityEngine.Random.Range(0, enemies.Count()));
        var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

        for (int k = 0; k < numHit; k++)
        {
            // var startPos = GameManager.instance.myGate.position - new Vector3(2, 0, 0);
            // var targetPos = GameManager.instance.enemyGate.position + new Vector3(2, 0, 0);

            // startPos.y = GameManager.instance.GetRandomLanePosition().y;
            // targetPos.y = startPos.y;

            var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
            var bullet = bulletGo.GetComponent<BulletVladSkill3>();

            var med = (startPos + targetPos) / 2;

            var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 2, false);
            Debug.Log("attackerInfo.specialSkill: " + attackerInfo.specialSkill);

            var bulletParam = new BulletParam(speed, Vector2.Distance(startPos, targetPos), attackerInfo, -1, false, null);
            bullet.Setup(attacker.currentTeam, startPos, bulletParam, med, targetPos);

            yield return new WaitForSeconds(0.2f);
        }

        // int tmpHit = numHit;
        // while (tmpHit > 0)
        // {
        //     yield return new WaitForSeconds(0.2f);
        //     tmpHit--;
        //     var enemy = enemies.ElementAt(UnityEngine.Random.Range(0, enemies.Count()));
        //     var dmg = attacker.RaiseDamage(enemy, attackParam, false);

        //     if (dmg <= 0) continue;

        //     var myBeasts = GameManager.instance.GetEntityInTeam(attacker.currentTeam, true);
        //     if (myBeasts != null)
        //     {
        //         // var _myBeasts = myBeasts.Where(b => b.curHp < b.maxHP);
        //         var _myBeasts = myBeasts;
        //         var myBeast = _myBeasts.ElementAt(UnityEngine.Random.Range(0, _myBeasts.Count()));
        //         var _myBeast = myBeast as BeastBase;
        //         if (_myBeast != null)
        //         {
        //             var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, enemy.centerPoint.position, Quaternion.identity);
        //             var bullet = bulletGo.GetComponent<BulletLifeSteal>();

        //             var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 2);
        //             // Debug.Log("attackerInfo.specialSkill: " + attackerInfo.specialSkill);

        //             var distance = Vector2.Distance(enemy.GetCenterWorldPos(), _myBeast.GetCenterWorldPos());
        //             Debug.Log("attackerInfo.specialSkill distance: " + distance);

        //             // var offset = myBeast.centerPoint.position - enemy.centerPoint.position;
        //             // offset = offset + myBeast.centerPoint.localPosition;

        //             var bulletParam = new BulletParam(speed, distance, attackerInfo, -1, false, null);

        //             bullet.Setup(attacker.currentTeam, bulletParam, enemy, myBeast, dmg);
        //         }
        //     }
        // }
        // ObjectPool.Instance.ReleaseObject(gameObject);
    }
}

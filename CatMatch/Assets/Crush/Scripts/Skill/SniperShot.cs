using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SniperShot : SkillBase
{
    public int numHit;

    public override void Passive(BeastBase attacker)
    {
        var enemies = GameManager.instance.GetEnemiesInScreen();

        Debug.Log("enemies:" + enemies.Count());

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3, onHitCombo: false);
        var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);

        if (enemies != null && enemies.Count() > 0)
        {
            StartCoroutine(Run(attacker, attackParam, enemies));
        }
    }

    IEnumerator Run(BeastBase attacker, AttackParam attackParam, IEnumerable<EntityBase> enemies)
    {
        int tmpHit = numHit;
        while (tmpHit > 0)
        {
            tmpHit--;

            var enemy = enemies.ElementAt(UnityEngine.Random.Range(0, enemies.Count()));

            var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, enemy.GetCenterWorldPos(), Quaternion.identity);
            var bullet = bulletGo.GetComponent<BulletSorrow3>();

            // Debug.Log(attacker.name + " skill:" + skillIndex + " med:" + med);

            var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3, onHitCombo:false);
            var bulletParam = new BulletParam(0, 0, attackerInfo, 0, false, null);
            bullet.Setup(attacker.currentTeam, attacker, bulletParam, enemy);

            yield return new WaitForSeconds(0.2f);
        }
        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}

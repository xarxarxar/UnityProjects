using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GladiusStrike : SkillBase
{
    public override void Passive(BeastBase attacker)
    {
        EntityBase entityBase = null;
        var enemies = GameManager.instance.GetEnemiesInScreen(true);
        Debug.Log("enemies:" + enemies.Count());
        if (enemies.Count() == 0)
        {
            entityBase = GameManager.instance.GetNearestEnemy();
        }
        else
        {
            entityBase = enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }
        var startPos = entityBase.transform.position + new Vector3(0, 5, 0);
        var targetPos = entityBase.transform.position;

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3, onHitCombo: false);
        var bulletParam = new BulletParam(speed, Vector2.Distance(startPos, targetPos), attackerInfo, entityBase.curIndex, false, gameObject);

        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, entityBase.transform.position, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletSiegFried3>();
        bullet.Setup(attacker.currentTeam, startPos, bulletParam, entityBase);
    }
}
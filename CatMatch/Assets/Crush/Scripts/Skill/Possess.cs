using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Possess : SkillBase
{
    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        EntityBase entityBase = null;
        var opTeam = attacker.currentTeam == Team.My ? Team.Enemy : Team.My;
        var enemies = GameManager.instance.GetBeastInTeam(opTeam);
        enemies = enemies.Where(en=>en.beastId != BeastId.Circle && en.caster == null).ToList();

        Debug.Log("Possess team:" + enemies.Count());

        if (enemies.Count() == 0) return;

        entityBase = enemies[UnityEngine.Random.Range(0, enemies.Count)];

        var startPos = entityBase.transform.position + new Vector3(0, 5, 0);
        var targetPos = entityBase.transform.position;

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3, onHitCombo: false);
        var bulletParam = new BulletParam(speed, Vector2.Distance(startPos, targetPos), attackerInfo, entityBase.curIndex, false, gameObject);

        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, entityBase.transform.position, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletCircle2>();
        bullet.Setup(attacker.currentTeam, attacker, bulletParam, entityBase);
    }
}

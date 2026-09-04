using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Heal : SkillBase
{
    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        EntityBase entityBase = null;
        var enemies = GameManager.instance.GetEntityInTeam(attacker.currentTeam, true);

        // var myBeasts = enemies.Where(en => en.curIndex != attacker.curIndex && en.curHp < en.maxHP).ToList();
        var myBeasts = enemies.Where(en => en.curHp < en.maxHP).ToList();

        Debug.Log("my team:" + myBeasts.Count());

        if (myBeasts.Count() == 0) return;

        entityBase = myBeasts[UnityEngine.Random.Range(0, myBeasts.Count)];

        var startPos = entityBase.transform.position + new Vector3(0, 5, 0);
        var targetPos = entityBase.transform.position;

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3, onHitCombo: false);
        var bulletParam = new BulletParam(speed, Vector2.Distance(startPos, targetPos), attackerInfo, entityBase.curIndex, false, gameObject);

        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, entityBase.transform.position, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletMerlinus2>();
        bullet.Setup(attacker.currentTeam, attacker, bulletParam, entityBase);
    }
}

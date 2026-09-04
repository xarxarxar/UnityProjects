using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;

public class HitRandom : SkillBase
{
    public int numHit;

    public override void Passive(BeastBase attacker)
    {
        var enemies = GameManager.instance.GetEnemiesInScreen();

        Debug.Log("enemies:" + enemies.Count());

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref,effectHitPref, 3, onHitCombo: false);
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
            yield return new WaitForSeconds(0.2f);
            tmpHit--;
            var enemy = enemies.ElementAt(UnityEngine.Random.Range(0, enemies.Count()));
            attacker.RaiseDamage(enemy, attackParam, false);
        }
        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}

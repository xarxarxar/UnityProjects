using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletVladSkill3 : BulletBezier
{
    public GameObject bulletLifeStealPref;

    public override void Setup(Team team, Vector3 startPoint, BulletParam param, Vector3 med, Vector3 target)
    {
        base.Setup(team, startPoint, param, med, target);
        // if(team == Team.My)
        // {
        //     transform.localScale = 
        // }
    }

    protected override void OnCollideOrExplode(EntityBase currentEntity)
    {
        eff.Stop();
        ObjectPool.Instance.ReleaseObject(eff.gameObject);
        ObjectPool.Instance.ReleaseObject(gameObject);

        if (currentEntity == null) return;

        var attackerInfo = bulletParam.attackerInfo;
        // attackerInfo.damage = GetDamage(attackerInfo, currentEntity, currentEntity.curDef, currentEntity.element);
        attackerInfo.damage = GetDamage(attackerInfo, currentEntity, out var critFactor);
        attackerInfo.critFactor = critFactor;

        // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);
        var collPos = bulletParam.hitOnFoot ? currentEntity.transform.position : Constant.Vector3Default;
        var attackParam = new AttackParam(collPos, attackerInfo);
        var dmg = currentEntity.TakeDamage(attackParam, false);
        if (dmg <= 0) return;

        var attacker = GameManager.instance.GetEntity(bulletParam.attackerInfo.attackerIndex) as BeastBase;
        var beasts = GameManager.instance.GetEntityInTeam(attacker.currentTeam, false);
        if (beasts != null)
        {
            var _beast = beasts.Where(b => b.curHp < b.maxHP);
            if (_beast.Count() == 0) return;

            // var _beast = beasts;
            var myBeast = _beast.ElementAt(UnityEngine.Random.Range(0, _beast.Count()));
            var _myBeast = myBeast as BeastBase;
            if (_myBeast != null)
            {
                var bulletGo = ObjectPool.Instance.GetGameObject(bulletLifeStealPref, Vector3.zero, Quaternion.identity);
                var bullet = bulletGo.GetComponent<BulletLifeSteal>();

                // var attackerInfo = attacker.GetAttackerInfo(null, null, 3);
                // Debug.Log("attackerInfo.specialSkill: " + attackerInfo.specialSkill);

                var distance = Vector2.Distance(currentEntity.GetCenterWorldPos(), _myBeast.GetCenterWorldPos());
                Debug.Log("attackerInfo.specialSkill distance: " + distance);

                // var offset = myBeast.centerPoint.position - enemy.centerPoint.position;
                // offset = offset + myBeast.centerPoint.localPosition;

                var bulletParam = new BulletParam(7, distance, attackerInfo, -1, false, null);
                bullet.Setup(attacker.currentTeam, bulletParam, currentEntity, myBeast, dmg);
            }
        }
    }
}

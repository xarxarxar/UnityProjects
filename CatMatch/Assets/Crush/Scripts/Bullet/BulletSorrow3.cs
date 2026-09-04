using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSorrow3 : BulletBase
{
    private ParticleSystem eff, eff2;

    private EntityBase targetEntity;
    private float countTime;
    private Vector3 oldPos;

    public void Setup(Team team, EntityBase attacker, BulletParam param, EntityBase target)
    {
        base.Setup(team, attacker.transform.position, param);
        targetEntity = target;

        transform.localScale = new Vector3(1, 1, 1);

        transform.position = target.GetCenterWorldPos();
        oldPos = transform.position;

        var effGo = ObjectPool.Instance.GetGameObject(effPref, Vector3.zero, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();
        eff.transform.parent = transform;
        eff.transform.localPosition = target.GetCenterLocalPos();
        eff.Play();

        eff2 = null;

        // effGo = ObjectPool.Instance.GetGameObject(explosionEffPref, Vector3.zero, Quaternion.identity);
        // eff2 = effGo.GetComponent<ParticleSystem>();
        // eff2.transform.position = target.transform.position;
        // eff2.Play();

        countTime = 0;
    }

    protected override void Update()
    {
        if (targetEntity != null && !targetEntity.isDie)
            transform.position = targetEntity.centerPoint.position;
        else
            transform.position = oldPos;

        countTime += Time.deltaTime;
        if (countTime >= 2 && eff2 == null)
        {
            ObjectPool.Instance.ReleaseObject(eff.gameObject);

            var effGo = ObjectPool.Instance.GetGameObject(explosionEffPref, Vector3.zero, Quaternion.identity);
            eff2 = effGo.GetComponent<ParticleSystem>();
            eff2.transform.parent = transform;
            eff2.transform.localPosition = targetEntity.GetCenterLocalPos();
            eff2.Play();
            countTime = 0;
        }

        if (eff2 != null && countTime >= 1)
        {
            var attackerInfo = bulletParam.attackerInfo;

            // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
            // attackerInfo.damage = GetDamage(attackerInfo, targetEntity, targetEntity.curDef, targetEntity.element);
            attackerInfo.damage = GetDamage(attackerInfo, targetEntity, out var critFactor);
            attackerInfo.critFactor = critFactor;
            Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

            var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);
            targetEntity.TakeDamage(attackParam, false);

            countTime = -10000;
        }

        if (eff2 != null && eff2.isStopped)
        {
            ObjectPool.Instance.ReleaseObject(eff2.gameObject);
            ObjectPool.Instance.ReleaseObject(gameObject);
            Debug.Log("sssssssssssssssssssss");
        }

        oldPos = transform.position;
    }
}
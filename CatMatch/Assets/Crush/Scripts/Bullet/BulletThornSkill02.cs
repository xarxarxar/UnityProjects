
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;

public class BulletThornSkill02 : BulletBase
{
    public List<GameObject> effPrefs;
    private float range = 2;
    // public GameObject effHit;

    private ParticleSystem eff;

    private Vector3 curStart, curMed, curTarget;
    private float passedTime;

    private int effIndex;

    protected override void Start()
    {
        base.Start();
    }

    public void Setup(Team team, Vector3 startPoint, BulletParam param, Vector3 med, Vector3 target, float range)
    {
        base.Setup(team, startPoint, param);

        curStart = startPoint;
        curMed = med;
        curTarget = target;
        this.range = range;

        passedTime = 0;

        effIndex = 0;
        var effGo = ObjectPool.Instance.GetGameObject(effPrefs[effIndex], startPoint, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();

        // ShowParticle(eff);
    }

    protected override void Update()
    {
        if (!isActive) return;
        if (eff.isStopped)
        {
            effIndex++;

            if (effIndex < effPrefs.Count)
            {
                ObjectPool.Instance.ReleaseObject(eff.gameObject);
                var effGo = ObjectPool.Instance.GetGameObject(effPrefs[effIndex], curStart, Quaternion.identity);
                eff = effGo.GetComponent<ParticleSystem>();
                // ShowParticle(eff);
            }
            else
            {
                isActive = false;

                RaiseDamage();

                ReleaseObject();
            }
        }
    }

    private void RaiseDamage()
    {
        var colliders = Physics2D.OverlapCircleAll(curStart, range);
        // Debug.Log("enemy attack:" + gameObject.name + " colliders:" + colliders.Length);

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];
            // Debug.Log("BulletThronSkill02 attacked: " + collider.gameObject.name);

            var targetEntity = collider.GetComponent<EntityBase>();
            if (targetEntity != null && targetEntity.currentTeam != bulletTeam)
            {
                var beast = targetEntity as BeastBase;
                if (beast != null && beast.moveType == MoveType.Fly) continue;

                var attackerInfo = bulletParam.attackerInfo;
                // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                // attackerInfo.damage = GetDamage(attackerInfo, targetEntity, targetEntity.curDef, targetEntity.element);
                attackerInfo.damage = GetDamage(attackerInfo, targetEntity, out var critFactor);
                attackerInfo.critFactor = critFactor;
                Debug.Log("BulletThornSkill02 " + targetEntity.gameObject.name + " attackerInfo.damage: " + attackerInfo.damage);

                var atpr = new AttackParam(Constant.Vector3Default, attackerInfo);
                targetEntity.TakeDamage(atpr, true);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (curStart != null)
        {
            Gizmos.DrawSphere(curTarget, 0.01f);
        }
        Gizmos.DrawSphere(curStart, 0.01f);
    }

    void ReleaseObject()
    {
        // base.ReleaseObject();

        eff.Stop();
        ObjectPool.Instance.ReleaseObject(eff.gameObject);
        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletIcePillars : BulletBase
{
    private List<GameObject> effPrefs;

    private Vector3 curStart;
    private float passedTime;

    private int effIndex;

    private float countTime;

    private List<Tuple<Vector3, Vector3>> list = new List<Tuple<Vector3, Vector3>>();

    protected override void Start()
    {
        base.Start();
    }

    public override void Setup(Team team, Vector3 startPoint, BulletParam param)
    {
        base.Setup(team, startPoint, param);

        curStart = startPoint;

        passedTime = 0;

        list.Clear();

        effPrefs = new List<GameObject>();

        countTime = 0f;

        for (int k = 0; k < 4; k++)
            effPrefs.Add(effPref);

        effIndex = 0;
        var effGo = ObjectPool.Instance.GetGameObject(effPrefs[effIndex], startPoint, Quaternion.identity);
        var eff = effGo.GetComponent<ParticleSystem>();

        particles.Add(eff);

        // ShowParticle(eff);

        // Debug.Log(this.name + " Setup ");

        RaiseDamage(startPoint);
    }

    protected override void Update()
    {
        countTime += Time.deltaTime;

        if (effIndex == 3 && countTime >= 1f)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                var effect = particles[i];
                if (effect.isStopped)
                {
                    i--;
                    particles.Remove(effect);
                    ObjectPool.Instance.ReleaseObject(effect.gameObject);
                }
            }

            if (particles.Count == 0)
            {
                ObjectPool.Instance.ReleaseObject(gameObject);
                if (bulletParam.skill != null)
                    ObjectPool.Instance.ReleaseObject(bulletParam.skill);
            }
        }

        if (countTime >= 0.25f && effIndex < effPrefs.Count - 1)
        {
            effIndex++;

            if (effIndex < effPrefs.Count)
            {
                var pos = curStart + effIndex * new Vector3(1.5f, 0, 0);
                var effGo = ObjectPool.Instance.GetGameObject(effPrefs[effIndex], pos, Quaternion.identity);
                var eff = effGo.GetComponent<ParticleSystem>();
                // ShowParticle(eff);
                particles.Add(eff);

                RaiseDamage(pos);
            }
            countTime -= 0.25f;
        }
    }

    private void RaiseDamage(Vector3 position)
    {
        var colliders = Physics2D.OverlapAreaAll(position + new Vector3(0.7f, 2f, 0), position - new Vector3(0.7f, 0f, 0));
        // Debug.Log("enemy attack:" + gameObject.name + " colliders:" + colliders.Length);
        list.Add(new Tuple<Vector3, Vector3>(position + new Vector3(0.7f, 2f, 0), position - new Vector3(0.7f, 0f, 0)));

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];

            // Debug.Log("BulletIcePillars attacked: " + collider.gameObject.name);
            var enemy = collider.GetComponent<EntityBase>();

            // Debug.Log(this.name + " attacked: " + damage);

            if (enemy != null && enemy.currentTeam != bulletTeam)
            {
                if (enemy.entityType == EntityType.Beast && ((BeastBase)enemy).moveType != MoveType.Ground) continue;

                // bulletParam.attackerInfo.critFactor = StatHelper.GetCritFactor(bulletParam.attackerInfo.critRate, bulletParam.attackerInfo.critFactor);
                // bulletParam.attackerInfo.damage = GetDamage(bulletParam.attackerInfo, enemy, enemy.curDef, enemy.element);
                bulletParam.attackerInfo.damage = GetDamage(bulletParam.attackerInfo, enemy, out var critFactor);
                bulletParam.attackerInfo.critFactor = critFactor;

                var attackParam = new AttackParam(Constant.Vector3Default, bulletParam.attackerInfo);
                enemy.TakeDamage(attackParam, true);
            }
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(curStart - new Vector3(2.1f, 0, 0), 3);
        for (int k = 0; k < list.Count; k++)
        {
            Gizmos.DrawSphere(list[k].Item1, 0.2f);
            Gizmos.DrawSphere(list[k].Item2, 0.2f);
        }
        // Gizmos.DrawSphere(curStart + new Vector3(0.7f, 2f, 0), 0.2f);
        // Gizmos.DrawSphere(curStart - new Vector3(4.9f, 0f, 0), 0.2f);
    }
}

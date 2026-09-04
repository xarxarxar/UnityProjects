
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;

public class BulletAtlantisSkill02 : BulletBase
{
    public List<GameObject> effPrefs;
    // public GameObject effHit;

    private ParticleSystem eff;

    private Vector3 curStart, curMed, curTarget;
    private float passedTime;

    private int effIndex;
    private float range;

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

        // Debug.Log(this.name + " Setup ");

        RaiseDamage();
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

                ReleaseObject();
            }
        }
    }

    private void RaiseDamage()
    {
        var colliders = Physics2D.OverlapCircleAll(curStart, range);
        Debug.Log("enemy attack:" + gameObject.name + " colliders:" + colliders.Length);

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];
            if (collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                Debug.Log("BulletAtlantus attacked: " + collider.gameObject.name);
                var enemy = collider.GetComponent<EntityBase>();

                // Debug.Log(this.name + " attacked: " + damage);

                if (enemy != null && enemy.currentTeam != bulletTeam)
                {
                    var beast = enemy as BeastBase;
                    if (beast != null && beast.moveType == MoveType.Fly) continue;
                    // var attackerInfo = bulletParam.attackerInfo.Clone();

                    // bulletParam.attackerInfo.critFactor = StatHelper.GetCritFactor(bulletParam.attackerInfo.critRate, bulletParam.attackerInfo.critFactor);
                    bulletParam.attackerInfo.damage = GetDamage(bulletParam.attackerInfo, enemy, out var critFactor);
                    bulletParam.attackerInfo.critFactor = critFactor;

                    var attackParam = new AttackParam(Constant.Vector3Default, bulletParam.attackerInfo);
                    enemy.TakeDamage(attackParam, true);
                }
            }
        }

    }
    void OnDrawGizmos()
    {
        // if (curStart != null)
        // {
        //     Gizmos.DrawSphere(curTarget, 0.01f);
        // }
        Gizmos.DrawWireSphere(curStart, range);
    }

    void ReleaseObject()
    {
        // base.ReleaseObject();

        eff.Stop();
        ObjectPool.Instance.ReleaseObject(eff.gameObject);
        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}


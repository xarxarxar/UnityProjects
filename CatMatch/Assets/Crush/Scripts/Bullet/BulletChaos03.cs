using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;

enum BulletEffOrder
{
    Start,
    PrevMove,
    Move,
    End
}

public class BulletChaos03 : BulletBase
{
    public GameObject effAtStartPref, effAtMovePref, effAtEndPref;
    // [SerializeField] private Transform centerPoint;

    private ParticleSystem effAtStart, effAtMove, effAtEnd;
    private Vector3 curStartPoint, curPivot;

    private float angle = 0;
    private BulletEffOrder bulletChaos03State;

    protected override void Start()
    {
        base.Start();
    }

    public void Setup(Team team, Vector3 startPoint, BulletParam param, Vector3 pivot)
    {
        base.Setup(team, startPoint, param);

        var effAtStartGo = ObjectPool.Instance.GetGameObject(effAtStartPref, startPoint, Quaternion.identity);
        effAtStart = effAtStartGo.GetComponent<ParticleSystem>();
        effAtStart.Play();

        effAtEnd = null;
        effAtMove = null;

        angle = 0;
        bulletChaos03State = BulletEffOrder.Start;
        curStartPoint = startPoint;
        curPivot = pivot;
    }

    protected override void Update()
    {
        if (effAtStart != null && effAtStart.isStopped)
        {
            if (bulletChaos03State == BulletEffOrder.Start)
            {
                Debug.Log("wtf nhanh qua");
                bulletChaos03State = BulletEffOrder.Move;

                ObjectPool.Instance.ReleaseObject(effAtStart.gameObject);
                var effMoveGo = ObjectPool.Instance.GetGameObject(effAtMovePref, transform.position, Quaternion.identity);
                effAtMove = effMoveGo.GetComponent<ParticleSystem>();
                effAtMove.Play();
            }
        }

        if (effAtEnd != null && effAtEnd.isStopped)
        {
            if (bulletChaos03State == BulletEffOrder.End)
            {
                ObjectPool.Instance.ReleaseObject(effAtEnd.gameObject);
                if (bulletParam.skill != null) ObjectPool.Instance.ReleaseObject(bulletParam.skill);
                ObjectPool.Instance.ReleaseObject(gameObject);
            }
        }

        if (bulletChaos03State == BulletEffOrder.Move)
        {
            // if (fromLeft)
            // {
            angle -= 5;
            if (angle <= -90)
            {
                angle = -90;

                bulletChaos03State = BulletEffOrder.End;

                ObjectPool.Instance.ReleaseObject(effAtMove.gameObject);
                var effEndGo = ObjectPool.Instance.GetGameObject(effAtEndPref, transform.position, Quaternion.identity);
                effAtEnd = effEndGo.GetComponent<ParticleSystem>();
                effAtEnd.Play();

                OnCollideOrExplode();
            }
            // }
            // else
            // {
            //     angle += 5;
            //     if (angle >= 80)
            //     {
            //         angle = 80;

            //         bulletChaos03State = BulletChaos03State.End;
            //         HideParticle(effAtMove);
            //         ShowParticle(effAtEnd);
            //     }
            // }

            var point = RotatePointAroundPivot(curStartPoint, curPivot, angle);
            transform.position = point;
        }

        if (effAtStart != null && effAtStart.gameObject.activeInHierarchy) effAtStart.transform.position = transform.position;
        if (effAtMove != null && effAtMove.gameObject.activeInHierarchy) effAtMove.transform.position = transform.position;
        if (effAtEnd != null && effAtEnd.gameObject.activeInHierarchy) effAtEnd.transform.position = transform.position;

        // Debug.Log("point 2:" + point + " transform.position: " + transform.position);
    }

    void OnDrawGizmos()
    {
        if (bulletChaos03State == BulletEffOrder.End)
        {
            Gizmos.DrawWireSphere(transform.position, bulletParam.attackerInfo.AOE);
        }
    }

    void OnCollideOrExplode()
    {
        var attackerInfo = bulletParam.attackerInfo;

        var colliders = Physics2D.OverlapCircleAll(transform.position, attackerInfo.AOE);
        for (int i = 0; i < colliders.Length; i++)
        {
            var currentEntity = colliders[i].gameObject.GetComponent<EntityBase>();
            if (currentEntity != null && bulletTeam != currentEntity.currentTeam && !currentEntity.isDie)
            {
                if (currentEntity.entityType == EntityType.Beast && ((BeastBase)currentEntity).moveType != MoveType.Ground) continue;

                // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                // attackerInfo.damage = GetDamage(attackerInfo, currentEntity, currentEntity.curDef, currentEntity.element);
                attackerInfo.damage = GetDamage(attackerInfo, currentEntity, out var critFactor);
                attackerInfo.critFactor = critFactor;
                // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

                var attackParam = new AttackParam(currentEntity.transform.position, attackerInfo);
                currentEntity.TakeDamage(attackParam, true);
            }
        }
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        var dir = point - pivot;
        dir = Quaternion.Euler(new Vector3(0, 0, angle)) * dir;
        point = dir + pivot;
        return point;
    }
}
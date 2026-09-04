using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// arrow shower
// through all, collide fly, explode on lane
public class BulletBezier3 : BulletBase
{
    private ParticleSystem effAtStart, effAtMove, effAtEnd;

    private Vector3 curStart, curMed, curTarget;
    private float passedTime;
    private float maxTime;
    private List<int> collideWorldIndexs;
    private BulletEffOrder bulletEffOrder;

    protected override void Awake()
    {
        base.Awake();
        collideWorldIndexs = new List<int>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void Setup(Team team, Vector3 startPoint, BulletParam param, Vector3 med, Vector3 target)
    {
        base.Setup(team, startPoint, param);

        curStart = startPoint;
        curMed = med;
        curTarget = target;

        passedTime = 0;
        if (param.speed == 0)
            maxTime = 1;
        else
            maxTime = param.range / param.speed;

        if (startEffPref != null)
        {
            var effStartGo = ObjectPool.Instance.GetGameObject(startEffPref, startPoint, Quaternion.identity);
            effAtStart = effStartGo.GetComponent<ParticleSystem>();
            effAtStart.Play();
            effAtMove = null;
            bulletEffOrder = BulletEffOrder.Start;
        }
        else
        {
            effAtStart = null;

            var effMoveGo = ObjectPool.Instance.GetGameObject(effPref, transform.position, Quaternion.identity);
            effAtMove = effMoveGo.GetComponent<ParticleSystem>();
            effAtMove.Play();

            bulletEffOrder = BulletEffOrder.Move;
        }

        effAtEnd = null;

    }

    protected override void Update()
    {
        if (!isActive) return;

        if (effAtStart != null && effAtStart.isStopped)
        {
            if (bulletEffOrder == BulletEffOrder.Start)
            {
                bulletEffOrder = BulletEffOrder.Move;

                ObjectPool.Instance.ReleaseObject(effAtStart.gameObject);
                var effMoveGo = ObjectPool.Instance.GetGameObject(effPref, transform.position, Quaternion.identity);
                effAtMove = effMoveGo.GetComponent<ParticleSystem>();
                effAtMove.Play();
            }
        }

        if (effAtEnd != null && effAtEnd.isStopped)
        {
            if (bulletEffOrder == BulletEffOrder.End)
            {
                ObjectPool.Instance.ReleaseObject(effAtEnd.gameObject);
                if (bulletParam.skill != null) ObjectPool.Instance.ReleaseObject(bulletParam.skill);
                ObjectPool.Instance.ReleaseObject(gameObject);

                isActive = false;
                return;
            }
        }

        if (bulletEffOrder == BulletEffOrder.Move)
        {
            passedTime += Time.deltaTime;

            var percent = passedTime / maxTime;

            transform.position = Curve(curStart, curMed, curTarget, percent);
            effAtMove.transform.position = transform.position;

            var angleRad = 0f;
            var sign = (fromLeft ? 1 : -1);
            var cond = fromLeft ? curMed.x > transform.position.x : curMed.x < transform.position.x;
            if (cond) angleRad = Mathf.Atan2(sign * (curMed.y - transform.position.y), sign * (curMed.x - transform.position.x));
            else angleRad = Mathf.Atan2(sign * (curTarget.y - transform.position.y), sign * (curTarget.x - transform.position.x));

            transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg);

            if (percent >= 1 || transform.position.y < GameManager.instance.landBottom.position.y)
            {
                bulletEffOrder = BulletEffOrder.End;

                var effEndGo = ObjectPool.Instance.GetGameObject(explosionEffPref, transform.position, Quaternion.identity);
                effAtEnd = effEndGo.GetComponent<ParticleSystem>();
                effAtEnd.Play();
                effAtMove.Stop();
                ObjectPool.Instance.ReleaseObject(effAtMove.gameObject);

                for (int l = 0; l < effAtMove.transform.childCount; l++)
                {
                    var tr = effAtMove.transform.GetChild(l).GetComponent<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.Clear();
                    }
                }

                // Debug.Log("isActive = false");
                OnCollideOrExplode();
            }
        }
    }

    void OnDrawGizmos()
    {
        var attackerInfo = bulletParam.attackerInfo;
        Gizmos.DrawWireSphere(transform.position, attackerInfo.AOE);
    }

    void OnTriggerEnter2D(Collider2D collision2D)
    {
        if (this.isActive && (collision2D.CompareTag("Player") || collision2D.CompareTag("Gate")))
        {
            var entity = collision2D.GetComponent<BeastBase>();
            if (entity != null && entity.moveType == MoveType.Fly && entity.currentTeam != bulletTeam
               && !collideWorldIndexs.Contains(entity.curIndex))
            {
                collideWorldIndexs.Add(entity.curIndex);

                var attackerInfo = bulletParam.attackerInfo;
                // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                // attackerInfo.damage = GetDamage(attackerInfo, entity, entity.curDef, entity.element);
                attackerInfo.damage = GetDamage(attackerInfo, entity, out var critFactor);
                attackerInfo.critFactor = critFactor;
                // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

                var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);
                entity.TakeDamage(attackParam, false);
            }
        }
    }

    void OnCollideOrExplode()
    {
        var attackerInfo = bulletParam.attackerInfo;
        var targetEntity = GameManager.instance.GetEntity(bulletParam.targetIndex);

        if (bulletParam.hitOnFoot && attackerInfo.effectHitSplash != null)
        {
            attackerInfo.effectHit = attackerInfo.effectHitSplash;
        }

        if (attackerInfo.AOE == 0f)
        {
            // Debug.Log("BulletBezier enemy attack:" + entity.name);

            if (targetEntity != null && bulletTeam != targetEntity.currentTeam && !targetEntity.isDie)
            {
                // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                // attackerInfo.damage = GetDamage(attackerInfo, targetEntity, targetEntity.curDef, targetEntity.element);
                attackerInfo.damage = GetDamage(attackerInfo, targetEntity, out var critFactor);
                attackerInfo.critFactor = critFactor;
                // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

                var collPos = bulletParam.hitOnFoot ? targetEntity.transform.position : Constant.Vector3Default;
                var attackParam = new AttackParam(collPos, attackerInfo);
                targetEntity.TakeDamage(attackParam, false);
            }
        }
        else
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, attackerInfo.AOE);
            for (int i = 0; i < colliders.Length; i++)
            {
                var currentEntity = colliders[i].gameObject.GetComponent<EntityBase>();
                if (currentEntity != null && bulletTeam != currentEntity.currentTeam && !currentEntity.isDie)
                {
                    // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                    // attackerInfo.damage = GetDamage(attackerInfo, currentEntity, currentEntity.curDef, currentEntity.element);
                    attackerInfo.damage = GetDamage(attackerInfo, currentEntity, out var critFactor);
                    attackerInfo.critFactor = critFactor;
                    Debug.Log("BulletBezier3 target entity:" + targetEntity?.gameObject.name + " dmg:" + attackerInfo.damage);

                    var collPos = bulletParam.hitOnFoot ? currentEntity.transform.position : Constant.Vector3Default;
                    var attackParam = new AttackParam(collPos, attackerInfo);
                    currentEntity.TakeDamage(attackParam, currentEntity != targetEntity);
                }
            }
        }
        // ObjectPool.Instance.ReleaseObject(gameObject);
    }

    private Vector3 Curve(Vector3 start, Vector3 med, Vector3 target, float per)
    {
        var t = per;
        var x = (1 - t) * (1 - t) * start.x + 2 * (1 - t) * t * med.x + t * t * target.x;
        var y = (1 - t) * (1 - t) * start.y + 2 * (1 - t) * t * med.y + t * t * target.y;
        return new Vector3(x, y);
    }
}

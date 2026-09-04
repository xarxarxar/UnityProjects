using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectExplodeType
{
    None,
    CurPos,
    TargetFoot
}

public class BulletBezier : BulletBase
{
    [Header("Nổ khi kết thúc đường đi")]
    public bool explodeOnLane = false;
    public EffectExplodeType effectExplodeType;

    protected ParticleSystem eff;

    private Vector3 curStart, curMed, curTarget;
    private float passedTime;
    private float maxTime;

    protected override void Start()
    {
        base.Start();
    }

    public virtual void Setup(Team team, Vector3 startPoint, BulletParam param, Vector3 med, Vector3 target)
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

        var effGo = ObjectPool.Instance.GetGameObject(effPref, startPoint, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();
        eff.Play();
        particles.Add(eff);

        if (team == Team.My) eff.transform.localScale = new Vector3(1, 1, 1);
        else eff.transform.localScale = new Vector3(-1, 1, 1);
    }

    protected override void Update()
    {
        base.Update();

        if (!isActive)
        {
            countTimeOffEffect += Time.deltaTime;
            if (countTimeOffEffect >= 0.3f && particles.Contains(eff))
            {
                eff.Stop();
                for (int l = 0; l < eff.transform.childCount; l++)
                {
                    var tr = eff.transform.GetChild(l).GetComponent<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.Clear();
                    }
                }
                particles.Remove(eff);
                ObjectPool.Instance.ReleaseObject(eff.gameObject);

                countTimeOffEffect = 0f;
            }
            return;
        }

        passedTime += Time.deltaTime;

        var percent = passedTime / maxTime;

        transform.position = Curve(curStart, curMed, curTarget, percent);

        var angleRad = 0f;
        var sign = (fromLeft ? 1 : -1);
        var cond = fromLeft ? curMed.x > transform.position.x : curMed.x < transform.position.x;
        if (cond) angleRad = Mathf.Atan2(sign * (curMed.y - transform.position.y), sign * (curMed.x - transform.position.x));
        else angleRad = Mathf.Atan2(sign * (curTarget.y - transform.position.y), sign * (curTarget.x - transform.position.x));

        transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg);

        eff.transform.position = transform.position;

        if (percent >= 1 || transform.position.y < GameManager.instance.landBottom.position.y)
        {
            isActive = false;
            // Debug.Log("isActive = false");
            OnCollideOrExplode(null);
        }
    }

    void OnDrawGizmos()
    {
        // if (isActive)
        // {
        var attackerInfo = bulletParam.attackerInfo;
        Gizmos.DrawWireSphere(transform.position, attackerInfo.AOE);
        // }
    }

    void OnTriggerEnter2D(Collider2D collision2D)
    {
        if (explodeOnLane) return;

        if (this.isActive && (collision2D.CompareTag("Player") || collision2D.CompareTag("Gate")))
        {
            var entity = collision2D.GetComponent<EntityBase>();
            if (entity.currentTeam != bulletTeam)
            {
                isActive = false;
                OnCollideOrExplode(entity);
            }
        }
    }

    protected virtual void OnCollideOrExplode(EntityBase entityBase)
    {
        var attackerInfo = bulletParam.attackerInfo;
        EntityBase targetEntity = entityBase;
        if (entityBase == null)
            targetEntity = GameManager.instance.GetEntity(bulletParam.targetIndex);

        // Debug.Log("attackerInfo.AOE:" + attackerInfo.AOE + " targetEntity != null " + (targetEntity != null));
        // if (bulletParam.hitOnFoot)
        // {
        //     bulletParam.attackerInfo.effectHit = bulletParam.attackerInfo.effectHitSplash;
        // }

        if (attackerInfo.AOE == 0f)
        {
            // Debug.Log("BulletBezier enemy attack:" + entity.name);

            if (targetEntity != null && bulletTeam != targetEntity.currentTeam && !targetEntity.isDie)
            {
                // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                attackerInfo.damage = GetDamage(attackerInfo, targetEntity, out var critFactor);
                attackerInfo.critFactor = critFactor;
                // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

                var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);
                targetEntity.TakeDamage(attackParam, false);
            }
        }
        else
        {
            if (explosionEffPref != null)
            {
                var explosionPos = transform.position;
                if (effectExplodeType == EffectExplodeType.TargetFoot && targetEntity != null && !targetEntity.isDie)
                    explosionPos = targetEntity.transform.position;

                var effGo = ObjectPool.Instance.GetGameObject(explosionEffPref, explosionPos, Quaternion.identity);
                var eff = effGo.GetComponent<ParticleSystem>();
                eff.Play();
                particles.Add(eff);
            }

            List<EntityBase> entities = new List<EntityBase>();
            if (targetEntity != null && !targetEntity.isDie)
                entities.Add(targetEntity);
            var colliders = Physics2D.OverlapCircleAll(transform.position, attackerInfo.AOE);
            for (int i = 0; i < colliders.Length; i++)
            {
                var currentEntity = colliders[i].gameObject.GetComponent<EntityBase>();
                if (currentEntity != null && currentEntity != targetEntity && bulletTeam != currentEntity.currentTeam && !currentEntity.isDie)
                {
                    entities.Add(currentEntity);
                }
            }
            for (int k = 0; k < entities.Count; k++)
            {
                var currentEntity = entities[k];
                // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
                attackerInfo.damage = GetDamage(attackerInfo, currentEntity, out var critFactor);
                attackerInfo.critFactor = critFactor;
                // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

                var collPos = bulletParam.hitOnFoot ? currentEntity.transform.position : Constant.Vector3Default;
                var attackParam = new AttackParam(collPos, attackerInfo);
                currentEntity.TakeDamage(attackParam, currentEntity != targetEntity);
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

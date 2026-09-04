using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// multiple collide and not explode(explode) and not AOE
public class BulletBezier2 : BulletBase
{
    public bool explodeWhenCollide = false;

    private ParticleSystem eff;

    private Vector3 curStart, curMed, curTarget;
    private float passedTime;
    private float maxTime;

    private List<int> collideWorldIndexs;

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

        collideWorldIndexs.Clear();

        passedTime = 0;
        if (param.speed == 0)
            maxTime = 1;
        else
            maxTime = param.range / param.speed;

        var effGo = ObjectPool.Instance.GetGameObject(effPref, startPoint, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();
        eff.Play();
        // particles.Add(eff);

        // ShowParticle(eff);
        var ls = eff.transform.localScale;
        if (team == Team.My) eff.transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);
        else eff.transform.localScale = new Vector3(-Mathf.Abs(ls.x), ls.y, ls.z);
    }

    protected override void Update()
    {
        if (!isActive) return;

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

        if (percent >= 1 || transform.position.y < GameManager.instance.landBottom.position.y && isActive)
        {
            isActive = false;

            eff.Stop();
            ObjectPool.Instance.ReleaseObject(eff.gameObject);
            ObjectPool.Instance.ReleaseObject(gameObject);
            if (bulletParam.skill != null) ObjectPool.Instance.ReleaseObject(bulletParam.skill);
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

    void OnTriggerEnter2D(Collider2D collision2D)
    {
        if (this.isActive
            && (collision2D.CompareTag("Player") || collision2D.CompareTag("Gate"))
                )
        {
            var entity = collision2D.GetComponent<EntityBase>();
            if (entity.currentTeam != bulletTeam && !collideWorldIndexs.Contains(entity.curIndex))
            {
                collideWorldIndexs.Add(entity.curIndex);
                OnCollide(entity);

                if (explodeWhenCollide)
                {
                    isActive = false;

                    eff.Stop();
                    ObjectPool.Instance.ReleaseObject(eff.gameObject);
                    ObjectPool.Instance.ReleaseObject(gameObject);
                    if (bulletParam.skill != null) ObjectPool.Instance.ReleaseObject(bulletParam.skill);
                }
            }
        }
    }

    void OnCollide(EntityBase targetEntity)
    {
        var attackerInfo = bulletParam.attackerInfo;
        if (targetEntity != null && bulletTeam != targetEntity.currentTeam && !targetEntity.isDie)
        {
            // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
            // attackerInfo.damage = GetDamage(attackerInfo, targetEntity, targetEntity.curDef, targetEntity.element);
            attackerInfo.damage = GetDamage(attackerInfo, targetEntity, out var critFactor);
            attackerInfo.critFactor = critFactor;

            Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + "dmg:" + attackerInfo.damage + " specialSkill:" + attackerInfo.specialSkill);

            var attackParam = new AttackParam(Constant.Vector3Default, attackerInfo);
            targetEntity.TakeDamage(attackParam, false);
        }
    }

    private Vector3 Curve(Vector3 start, Vector3 med, Vector3 target, float per)
    {
        var t = per;
        var x = (1 - t) * (1 - t) * start.x + 2 * (1 - t) * t * med.x + t * t * target.x;
        var y = (1 - t) * (1 - t) * start.y + 2 * (1 - t) * t * med.y + t * t * target.y;
        return new Vector3(x, y);
    }
}

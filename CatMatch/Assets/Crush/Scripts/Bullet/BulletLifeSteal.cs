using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;

public class BulletLifeSteal : BulletBase
{
    private ParticleSystem eff;

    private Vector3 curStart, curMed, curTarget;
    private float passedTime;
    private float maxTime;
    private Vector3 oldPos;
    private EntityBase mTarget;
    private BigIntCSharp mDamage;
    public Transform sourceTf;

    protected override void Start()
    {
        base.Start();
    }

    //source thằng bị bắn
    // target thằng bắn
    public void Setup(Team team, BulletParam param, EntityBase source, EntityBase target, BigIntCSharp dmg)
    {
        base.Setup(team, source.centerPoint.position, param);
        mTarget = target;
        this.mDamage = dmg;

        // transform.localScale = new Vector3(1, 1, 1);
        transform.position = target.GetCenterWorldPos();
        oldPos = transform.position;

        sourceTf.position = source.GetCenterWorldPos();

        // var offset = source.GetCenterWorldPos() - target.GetCenterWorldPos();

        // curStart = target.GetCenterLocalPos() + offset;
        curStart = sourceTf.localPosition;
        curTarget = target.GetCenterLocalPos();
        curMed = (curStart + curTarget) / 2;

        passedTime = 0;
        if (param.speed == 0)
            maxTime = 1;
        else
            maxTime = param.range / param.speed;

        // maxTime = 10;

        Debug.Log("BulletLifeSteal maxTime:" + maxTime);

        var effGo = ObjectPool.Instance.GetGameObject(effPref, Vector3.zero, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();
        eff.transform.parent = transform;
        eff.transform.localPosition = curStart;
        eff.Play();
        particles.Add(eff);
    }

    protected override void Update()
    {
        base.Update();

        if (!isActive)
        {
            countTimeOffEffect += Time.deltaTime;
            if (countTimeOffEffect >= 0.2f && particles.Contains(eff))
            {
                ObjectPool.Instance.ReleaseObject(eff.gameObject);
                ObjectPool.Instance.ReleaseObject(gameObject);
                particles.Remove(eff);
                countTimeOffEffect = 0f;
            }
            return;
        }

        // var targetEntity = GameManager.instance.GetEntity(bulletParam.targetIndex);
        if (mTarget != null && !mTarget.isDie)
            transform.position = mTarget.centerPoint.position;
        else
            transform.position = oldPos;

        passedTime += Time.deltaTime;
        var percent = passedTime / maxTime;
        if (percent < 1)
        {
            eff.transform.localPosition = Curve(curStart, curMed, curTarget, percent);
            var angleRad = GetAngle(eff.transform, curMed, curTarget);
            eff.transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg);
        }

        oldPos = transform.position;

        if (percent >= 1 || transform.position.y < GameManager.instance.landBottom.position.y)
        {
            isActive = false;
            var beast = this.mTarget as BeastBase;
            if (this.mTarget != null && !this.mTarget.isDie)
            {
                float perHp = 400 + 160 * (beast.curStar - 2);
                var hp = BigIntegerHelper.BigMultiplyFloat(mDamage, perHp / 100);
                // Debug.Log("BulletLifeSteal hp:" + hp);
                beast.AddHp(hp);
            }
            // Debug.Log("isActive = false");
        }
    }

    float GetAngle(Transform tf, Vector3 curMed, Vector3 curTarget)
    {
        var angleRad = 0f;
        var sign = (fromLeft ? 1 : -1);
        var cond = fromLeft ? curMed.x > tf.position.x : curMed.x < tf.position.x;
        if (cond) angleRad = Mathf.Atan2(sign * (curMed.y - tf.position.y), sign * (curMed.x - tf.position.x));
        else angleRad = Mathf.Atan2(sign * (curTarget.y - tf.position.y), sign * (curTarget.x - tf.position.x));

        return angleRad;
    }

    void OnDrawGizmos()
    {
        var attackerInfo = bulletParam.attackerInfo;
        Gizmos.DrawWireSphere(transform.position, attackerInfo.AOE);
    }

    private Vector3 Curve(Vector3 start, Vector3 med, Vector3 target, float per)
    {
        var t = per;
        var x = (1 - t) * (1 - t) * start.x + 2 * (1 - t) * t * med.x + t * t * target.x;
        var y = (1 - t) * (1 - t) * start.y + 2 * (1 - t) * t * med.y + t * t * target.y;
        return new Vector3(x, y);
    }
}
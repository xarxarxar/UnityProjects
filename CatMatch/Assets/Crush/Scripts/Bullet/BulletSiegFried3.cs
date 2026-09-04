using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSiegFried3 : BulletBase
{
    private ParticleSystem eff, eff2, eff3;

    private Vector3 curStart, curMed, curTarget;
    private Vector3 curStart2, curMed2, curTarget2;
    private Vector3 curStart3, curMed3, curTarget3;
    private Vector3 curStart4;

    private float passedTime, passedTime2, passedTime3;
    private float maxTime;
    private Vector3 oldPos, startPos;
    private float countTimeSpawn = 0f;

    protected override void Start()
    {
        base.Start();
    }

    public void Setup(Team team, Vector3 startPoint, BulletParam param, EntityBase target)
    {
        base.Setup(team, startPoint, param);

        fromLeft = false;
        transform.localScale = new Vector3(1, 1, 1);

        transform.position = target.centerPoint.position;

        curStart4 = target.centerPoint.localPosition;

        curStart = target.centerPoint.localPosition + new Vector3(0, 5, 0) - new Vector3(1.5f, 0, 0);
        curTarget = target.centerPoint.localPosition;
        curMed = (curStart + curTarget) / 2;

        curStart2 = target.centerPoint.localPosition + new Vector3(0, 5, 0) + new Vector3(1.5f, 0, 0);
        curTarget2 = target.centerPoint.localPosition;
        curMed2 = (curStart2 + curTarget2) / 2;

        curStart3 = new Vector3(target.centerPoint.localPosition.x, target.centerPoint.localPosition.y - 5, 0);
        curTarget3 = target.centerPoint.localPosition;
        curMed3 = (curStart3 + curTarget3) / 2;

        Debug.Log("curStart:" + curStart + " curTarget" + curTarget + " curStart2:" + curStart2 + " curStart3:" + curStart3);

        passedTime = passedTime2 = passedTime3 = 0;
        // if (param.speed == 0)
        //     maxTime = 1;
        // else
        //     maxTime = param.range / param.speed;
        maxTime = 0.5f;

        var effGo = ObjectPool.Instance.GetGameObject(effPref, Vector3.zero, Quaternion.identity);
        eff = effGo.GetComponent<ParticleSystem>();
        eff.transform.parent = transform;
        eff.transform.localPosition = curStart;
        eff.Play();

        eff2 = eff3 = null;
        countTimeSpawn = 0f;
    }

    protected override void Update()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            var effect = particles[i];
            if (effect.isStopped)
            {
                i--;
                for (int l = 0; l < effect.transform.childCount; l++)
                {
                    var tr = effect.transform.GetChild(l).GetComponent<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.Clear();
                    }
                }
                particles.Remove(effect);
                ObjectPool.Instance.ReleaseObject(effect.gameObject);
            }
        }
        if (!isActive && particles.Count == 0 && this.enabled)
        {
            Debug.Log("OK conde");
            ObjectPool.Instance.ReleaseObject(eff.gameObject);
            ObjectPool.Instance.ReleaseObject(eff2.gameObject);
            ObjectPool.Instance.ReleaseObject(eff3.gameObject);
            ObjectPool.Instance.ReleaseObject(gameObject);
        }

        if (!isActive) return;

        var targetEntity = GameManager.instance.GetEntity(bulletParam.targetIndex);
        if (targetEntity != null && !targetEntity.isDie)
            transform.position = targetEntity.centerPoint.position;
        else
            transform.position = oldPos;

        float percent = 0f, percent2 = 0f, percent3 = 0f;

        countTimeSpawn += Time.deltaTime;
        if (countTimeSpawn >= 0.2f)
        {
            if (eff2 == null)
            {
                var effGo2 = ObjectPool.Instance.GetGameObject(effPref, Vector3.zero, Quaternion.identity);
                eff2 = effGo2.GetComponent<ParticleSystem>();
                eff2.transform.parent = transform;
                eff2.transform.localPosition = curStart2;
                eff2.Play();
            }
            else if (eff3 == null)
            {
                var effGo3 = ObjectPool.Instance.GetGameObject(effPref, Vector3.zero, Quaternion.identity);
                eff3 = effGo3.GetComponent<ParticleSystem>();
                eff3.transform.parent = transform;
                eff3.transform.localPosition = curStart3;
                eff3.Play();
            }
            countTimeSpawn -= 0.2f;
        }

        if (eff != null)
        {
            passedTime += Time.deltaTime;
            percent = passedTime / maxTime;
            if (percent < 1)
            {
                eff.transform.localPosition = Curve(curStart, curMed, curTarget, percent);
                // Debug.Log("percent:" + percent + " curTarget:" + curTarget + " eff.transform.localPosition: " + eff.transform.localPosition);
                var angleRad = GetAngle(eff.transform, curMed, curTarget);
                eff.transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg);
            }
        }

        if (eff2 != null)
        {
            passedTime2 += Time.deltaTime;
            percent2 = passedTime2 / maxTime;
            if (percent2 < 1)
            {
                eff2.transform.localPosition = Curve(curStart2, curMed2, curTarget2, percent2);
                var angleRad = GetAngle(eff2.transform, curMed2, curTarget2);
                eff2.transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg);
            }
        }

        if (eff3 != null)
        {
            passedTime3 += Time.deltaTime;
            percent3 = passedTime3 / maxTime;
            if (percent3 < 1)
            {
                eff3.transform.localPosition = Curve(curStart3, curMed3, curTarget3, percent3);
                var angleRad = GetAngle(eff3.transform, curMed3, curTarget3);
                eff3.transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg);
            }
        }

        oldPos = transform.position;

        if (percent3 >= 1)
        {
            isActive = false;
            // Debug.Log("isActive = false");

            OnCollideOrExplode();
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

    void OnCollideOrExplode()
    {
        var attackerInfo = bulletParam.attackerInfo;
        var targetEntity = GameManager.instance.GetEntity(bulletParam.targetIndex);

        if (explosionEffPref != null)
        {
            // var explosionPos = targetEntity.centerPoint.localPosition;
            var effGo4 = ObjectPool.Instance.GetGameObject(explosionEffPref, Vector3.zero, Quaternion.identity);
            var eff4 = effGo4.GetComponent<ParticleSystem>();
            eff4.transform.parent = transform;
            eff4.transform.localPosition = curStart4;

            eff4.Play();
            particles.Add(eff4);
        }

        if (targetEntity != null && bulletTeam != targetEntity.currentTeam && !targetEntity.isDie)
        {
            // attackerInfo.critFactor = StatHelper.GetCritFactor(attackerInfo.critRate, attackerInfo.critFactor);
            // attackerInfo.damage = GetDamage(attackerInfo, targetEntity, targetEntity.curDef, targetEntity.element);
            attackerInfo.damage = GetDamage(attackerInfo, targetEntity, out var critFactor);
            attackerInfo.critFactor = critFactor;
            // Debug.Log("BulletBezier target entity:" + targetEntity.gameObject.name + " dmg:" + attackerInfo.damage);

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
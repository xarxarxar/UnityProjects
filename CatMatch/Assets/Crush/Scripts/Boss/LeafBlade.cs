using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;

public class LeafBlade : BeastBase
{
    public GameObject shieldPref;
    private ParticleSystem shieldEff;

    private bool inShieldLB;
    private bool inCheckShowShield;

    public override void Active(bool activeOnScreen, Team team, Vector3 position, int lane, int worldIndex)
    {
        base.Active(activeOnScreen, team, position, lane, worldIndex);
        inCheckShowShield = false;
    }

    protected override void Update()
    {
        base.Update();
        if (shieldEff != null)
        {
            var position = centerPoint.position;
            shieldEff.transform.position = position;
        }
    }

    public override BigIntCSharp TakeDamage(AttackParam attackParam, bool isSplash)
    {
        if (inShieldLB) return 0;

        var dmg = base.TakeDamage(attackParam, isSplash);
        if (!inCheckShowShield)
        {
            Debug.Log("attackParam.attackerInfo.damage:" + attackParam.attackerInfo.damage + " curHp:" + curHp + " maxHP:" + maxHP);
            var per = (float)((double)curHp / (double)maxHP) * 100;
            Debug.Log("per:" + per);

            if (curHp > 0 && per <= 10)
            {
                inCheckShowShield = true;
                StartCoroutine(StartShieldIE());
            }
        }
        return dmg;
    }

    IEnumerator StartShieldIE()
    {
        while (inCheckShowShield)
        {
            inShieldLB = true;

            var targetTip = skeletonAnimation.skeleton.FindBone("target_tip");
            var position = centerPoint.position;
            if (targetTip != null) position = skeletonAnimation.transform.TransformPoint(new Vector3(targetTip.WorldX, targetTip.WorldY, 0f));
            var effGo = ObjectPool.Instance.GetGameObject(shieldPref, position, Quaternion.identity);
            // Debug.Log("GenEffect: " + effGo.activeSelf + " goName:" + gameObject.name + " effectPos:" + position);

            shieldEff = effGo.GetComponent<ParticleSystem>();
            shieldEff.Play();
            yield return new WaitForSeconds(5f);
            shieldEff.Stop();
            ObjectPool.Instance.ReleaseObject(shieldEff.gameObject);
            shieldEff = null;

            inShieldLB = false;

            yield return new WaitForSeconds(10f);
        }
    }
}

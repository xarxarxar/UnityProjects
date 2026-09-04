using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class NormalShoot : SkillBase
{
    public int skillIndex = 1;//1,2
    public int gunTipIndex = 1;//gun_tip1
    public float offsetGunTip1 = 0f;
    public float offsetGunTip2 = 0f;
    public bool hitOnFoot = false;

    public int numHit = 1;

    // public bool fatalWeakness = false;

    public Vector3 offsetStartPos;
    public Vector3 offsetStartPosGT2;

    [Header("true: tất cả đạn theo 1 đường, có thể cong hoặc thẳng")]
    [Header("false: đạn theo đường khác nhau")]
    public bool fixOrderOffset = false;
    [Header("true: tất cả guntip nhưng theo thứ tự ra lần lượt")]
    [Header("false: tất cả guntip ra cùng 1 lúc")]
    public bool allGunTipOrder = false;
    [Header("true: thẳng theo y attacker false: tới target")]
    public bool shootOnLine = false;

    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        // if (fatalWeakness)
        // {
        //     var _target = GetWeaknessEnemy(attacker.currentTeam);
        //     if(_target != null) target = _target;
        // }

        var skeleton = attacker.skeletonAnimation.skeleton;
        var boneGunTip1 = skeleton.FindBone("gun_tip1");
        var boneGunTip2 = skeleton.FindBone("gun_tip2");

        var targetPos = target.GetComponent<EntityBase>().centerPoint.position;

        var orderTmp = fixOrderOffset ? 1 : order;
        var remainder = order % 2;

        if (
            gunTipIndex == 1
            || (gunTipIndex == 3 && !allGunTipOrder)
            || (gunTipIndex == 3 && allGunTipOrder && remainder == 1)
        )
        {
            if (boneGunTip1 != null)
            {
                var offsetStartPos1 = offsetStartPos;
                if (attacker.currentTeam == Team.Enemy)
                    offsetStartPos1 = new Vector3(-offsetStartPos.x, offsetStartPos.y);
                // Debug.Log("offsetStartPos1:" + offsetStartPos1);

                var startPos = attacker.skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip1.WorldX, boneGunTip1.WorldY, 0f)) +
                        offsetStartPos1;

                if (shootOnLine) targetPos = new Vector3(targetPos.x, startPos.y, 0);

                StartCoroutine(GenBullets(attacker, startPos, targetPos, orderTmp * offsetGunTip1, target.GetComponent<EntityBase>().curIndex));
            }
        }

        if (
            gunTipIndex == 2
            || (gunTipIndex == 3 && !allGunTipOrder)
            || (gunTipIndex == 3 && allGunTipOrder && remainder == 0)
        )
        {
            if (boneGunTip2 != null)
            {
                var offsetStartPos2 = offsetStartPosGT2;
                if (attacker.currentTeam == Team.Enemy)
                    offsetStartPos2 = new Vector3(-offsetStartPosGT2.x, offsetStartPosGT2.y);
                var startPos = attacker.skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip2.WorldX, boneGunTip2.WorldY, 0f)) +
                     offsetStartPos2;

                if (shootOnLine) targetPos = new Vector3(targetPos.x, startPos.y, 0);

                StartCoroutine(GenBullets(attacker, startPos, targetPos, orderTmp * offsetGunTip2, target.GetComponent<EntityBase>().curIndex));
            }
        }
    }

    private GameObject GetWeaknessEnemy(Team teamAttacker)
    {
        var opTeam = teamAttacker == Team.My ? Team.Enemy : Team.My;
        var opEns = GameManager.instance.GetEntityInTeam(opTeam);

        double minPer = float.MaxValue;
        EntityBase en = null;
        foreach (var opEn in opEns)
        {
            var per = (double)opEn.curHp / (double)opEn.maxHP;
            if (minPer > per)
            {
                minPer = per;
                en = opEn;
            }
        }
        return en != null ? en.gameObject : null;
    }

    private IEnumerator GenBullets(BeastBase attacker, Vector3 startPos, Vector3 targetPos, float offSetY, int targetIndex)
    {
        var delay = numHit > 1 ? attacker.curAttackRate / numHit : 0;
        delay = Mathf.Floor(delay * 100) / 100;

        for (int k = 0; k < numHit; k++)
        {
            // Debug.Log("GenBullets:" + delay);
            GenBullet(attacker, startPos, targetPos, offSetY, targetIndex);
            if (numHit > 1)
                yield return new WaitForSeconds(delay);
        }
    }

    private void GenBullet(BeastBase attacker, Vector3 startPos, Vector3 targetPos, float offSetY, int targetIndex)
    {
        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletBezier>();

        var med = (startPos + targetPos) / 2;
        med.y += offSetY;

        // Debug.Log(attacker.name + " skill:" + skillIndex + " med:" + med);

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, skillIndex);
        var bulletParam = new BulletParam(speed, Range(startPos, targetPos), attackerInfo, targetIndex, hitOnFoot, null);
        bullet.Setup(attacker.currentTeam, startPos, bulletParam, med, targetPos);
    }

    float Range(Vector3 start, Vector3 end)
    {
        return Vector2.Distance(start, end);
    }
}

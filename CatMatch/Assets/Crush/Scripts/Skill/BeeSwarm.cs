using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeSwarm : SkillBase
{
    public bool toBeast = false;
    public int gunTipIndex = 1;
    public int skillIndex = 3;

    public override void Active(int order, BeastBase attacker, GameObject target)
    {
        base.Active(order, attacker, target);

        var startPos = GameManager.instance.myGate.position - new Vector3(2, 0, 0);
        var targetPos = GameManager.instance.enemyGate.position + new Vector3(2, 0, 0);
        if (attacker.currentTeam == Team.Enemy)
        {
            var tmpPos = startPos;
            startPos = targetPos;
            targetPos = tmpPos;
        }

        var skeleton = attacker.skeletonAnimation.skeleton;
        var gunTip = skeleton.FindBone("gun_tip" + gunTipIndex);
        if (gunTip != null)
        {
            startPos = attacker.skeletonAnimation.transform.TransformPoint(new Vector3(gunTip.WorldX, gunTip.WorldY, 0f));
            targetPos.y = startPos.y;
            Debug.Log("startPos: " + startPos);
            Debug.Log("targetPos: " + targetPos);
        }

        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletBezier2>();

        var med = (startPos + targetPos) / 2;

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, skillIndex);
        Debug.Log("attackerInfo.specialSkill: " + attackerInfo.specialSkill);

        var bulletParam = new BulletParam(speed, Vector2.Distance(startPos, targetPos), attackerInfo, -1, false, skillIndex == 3 ? gameObject : null);
        bullet.Setup(attacker.currentTeam, startPos, bulletParam, med, targetPos);
    }
}

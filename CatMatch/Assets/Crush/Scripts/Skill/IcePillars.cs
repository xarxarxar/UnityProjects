using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class IcePillars : SkillBase
{
   public override void Passive(BeastBase attacker)
    {
        var camPos = CameraController.instance.transform.position;
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        var startPos = GameManager.instance.GetPosGroundLanes()[0];

        startPos.x = camPos.x + halfWidth - 4 * 1.4f;

        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletIcePillars>();

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3);
        var bulletParam = new BulletParam(0, 0, attackerInfo, -1, true, gameObject);
        bullet.Setup(attacker.currentTeam, startPos, bulletParam);
    }
}

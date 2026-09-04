using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ChaosAxeSmash : SkillBase
{
    public override void Passive(BeastBase attacker)
    {
        var camPos = CameraController.instance.transform.position;
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        var rotatePos = GameManager.instance.GetPosGroundLanes()[3];
        rotatePos.x = camPos.x;
        // rotatePos.y = attacker.transform.position.y;

        var startPos = GameManager.instance.axeSmash.position;
        startPos.x = camPos.x;

        var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
        var bullet = bulletGo.GetComponent<BulletChaos03>();

        var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, 3, false);
        var bulletParam = new BulletParam(0, 0, attackerInfo, -1, true, gameObject);
        bullet.Setup(attacker.currentTeam, startPos, bulletParam, rotatePos);
    }
}

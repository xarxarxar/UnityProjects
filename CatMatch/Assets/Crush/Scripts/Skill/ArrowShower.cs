using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ArrowShower : SkillBase
{
    public int numHit;
    public float distanceBetWeenHit = 1f;
    public float distanceFromRightScreen = 0f;
    public bool hitOnFoot = true;

    public override void Passive(BeastBase attacker)
    {
        StartCoroutine(Gen(attacker));
    }

    IEnumerator Gen(BeastBase attacker)
    {
        var camPos = CameraController.instance.transform.position;
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        var startPos = GameManager.instance.GetPosGroundLanes()[0];

        startPos.x = camPos.x + distanceFromRightScreen;
        startPos.y = GameManager.instance.landTop.position.y;

        List<BulletBezier3> list = new List<BulletBezier3>();

        for (int k = 0; k < numHit; k++)
        {
            var _startPos = startPos;
            _startPos.x += k / 2 * distanceBetWeenHit + (k % 2 * distanceBetWeenHit * 0.5f);
            _startPos.y += (k % 2 == 0 ? -1f : 0);

            var targetPos = new Vector3(_startPos.x + 0.5f, GameManager.instance.GetRandomLanePosition().y);

            var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, _startPos, Quaternion.identity);
            var bullet = bulletGo.GetComponent<BulletBezier3>();

            var med = (_startPos + targetPos) / 2;

            var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitSplashPref, 3, onHitCombo: false);
            var bulletParam = new BulletParam(speed, Vector2.Distance(_startPos, targetPos), attackerInfo, -1, hitOnFoot, null);
            bullet.Setup(attacker.currentTeam, _startPos, bulletParam, med, targetPos);

            list.Add(bullet);

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(5f);

        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSwarm : SkillBase
{
    public int skillIndex = 3;
    public int numHit = 1;
    public float delay = 0.2f;

    public override void Passive(BeastBase attacker)
    {
        StartCoroutine(GenBullets(attacker));
    }

    IEnumerator GenBullets(BeastBase attacker)
    {
        Debug.Log("attackerInfo.specialSkill: numHit " + numHit);

        for (int k = 0; k < numHit; k++)
        {
            var startPos = GameManager.instance.myGate.position - new Vector3(2, 0, 0);
            var targetPos = GameManager.instance.enemyGate.position + new Vector3(2, 0, 0);

            startPos.y = GameManager.instance.GetRandomLanePosition().y;
            targetPos.y = startPos.y;

            var bulletGo = ObjectPool.Instance.GetGameObject(bulletPref, startPos, Quaternion.identity);
            var bullet = bulletGo.GetComponent<BulletVladSkill3>();

            var med = (startPos + targetPos) / 2;

            var attackerInfo = attacker.GetAttackerInfo(effectHitPref, effectHitPref, skillIndex, false);
            Debug.Log("attackerInfo.specialSkill: " + attackerInfo.specialSkill);

            var bulletParam = new BulletParam(speed, Vector2.Distance(startPos, targetPos), attackerInfo, -1, false, null);
            bullet.Setup(attacker.currentTeam, startPos, bulletParam, med, targetPos);

            yield return new WaitForSeconds(delay);
        }
        ObjectPool.Instance.ReleaseObject(gameObject);
    }
}


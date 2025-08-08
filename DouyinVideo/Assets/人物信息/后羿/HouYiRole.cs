using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouYiRole : BaseRole
{
    public VideoGameBulletBase bird;
    private Coroutine bigCoro = null;
    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(1.5f);
        bird.Recycle();
        Broadcast.instance.BroadCastNews($"{RoleName}发射了小鸟", roleColor);
        Vector3 target = otherBaseRole.transform.position;
        Vector3 direction = (target - transform.position).normalized;
        // 计算2D角度（Z轴旋转）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float yRotation = direction.x >= 0 ? 0f : 180f;
        bird.transform.parent.rotation = Quaternion.Euler(0, yRotation, 0);
        bird.Init(direction, angle, 10, transform.position, this,new string[] {"wall" } );//忽略掉墙体
        bird.OnBulletHitRole = (targetRole) =>
        {
            Broadcast.instance.BroadCastNews($"{RoleName}的鸟射中了{targetRole.RoleName}造成了30点伤害，并眩晕", roleColor);
            targetRole.TakeDamage(30);
            targetRole.SetSpeed(0,3);
            bird.Recycle();
        };
        yield return new WaitForSeconds(5);
        if (bird.transform.parent.gameObject.activeSelf)
        {
            bird.Recycle();
        }
        yield break;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炸弹妹脚本
/// </summary>
public class BoomSisterRole : BaseRole
{
    public VideoGameBulletBase boom;
    private Coroutine bigCoro = null;
    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }
        boom.Recycle();
        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(1.5f);
        Broadcast.instance.BroadCastNews($"{RoleName}发射了导弹", roleColor);
        Vector3 target = otherBaseRole.transform.position;
        Vector3 direction = (target - transform.position).normalized;
        // 计算2D角度（Z轴旋转）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        boom.Init(direction, angle, 70, transform.position, this);
        boom.OnBulletHitRole = (targetRole) =>
        {
            Broadcast.instance.BroadCastNews($"{RoleName}的导弹击中了{targetRole.RoleName}造成了70点伤害", roleColor);
            targetRole.TakeDamage(70);
            boom.Recycle();
        };
        yield return new WaitForSeconds(3);
        if (boom.transform.parent.gameObject.activeSelf)
        {
            boom.Recycle();
        }
        yield break;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            UseBig();
        }
    }

}

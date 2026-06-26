using System.Collections;
using UnityEngine;

/// <summary>
/// 炸弹妹英雄脚本，负责发射导弹大招。
/// </summary>
public class BoomSisterRole : BaseRole
{
    #region 字段配置

    [Tooltip("导弹子弹脚本")]
    public VideoGameBulletBase boom;

    #endregion

    #region 协程引用

    private Coroutine bigCoro;

    #endregion

    #region 大招入口

    /// <summary>
    /// 释放炸弹妹大招。
    /// </summary>
    public override void Big()
    {
        StopBigCoroutine();
        boom.Recycle();
        bigCoro = StartCoroutine(BigCoro());
    }

    #endregion

    #region 大招流程

    /// <summary>
    /// 执行导弹发射流程。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
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
            VideoGameCombatUtility.BroadcastThenDamage(targetRole, 70, $"{RoleName}的导弹击中了{targetRole.RoleName}造成了70点伤害", roleColor);
            boom.Recycle();
        };
        yield return new WaitForSeconds(3);
        if (boom.transform.parent.gameObject.activeSelf)
        {
            boom.Recycle();
        }
        yield break;
    }

    #endregion

    #region 开发调试入口

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            UseBig();
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 停止正在执行的大招协程。
    /// </summary>
    private void StopBigCoroutine()
    {
        if (bigCoro == null)
        {
            return;
        }

        StopCoroutine(bigCoro);
        bigCoro = null;
    }

    #endregion
}
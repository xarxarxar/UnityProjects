using System.Collections;
using UnityEngine;

/// <summary>
/// 维斯英雄脚本，负责释放范围缴械大招。
/// </summary>
public class WeiSiRole : BaseRole
{
    #region 字段配置

    [Tooltip("荆棘范围对象")]
    public GameObject Jingji;//荆棘的范围

    #endregion

    #region 协程引用

    private Coroutine bigCoro;
    private CircleCollider2D jingjiCollider;

    #endregion

    #region 大招入口

    /// <summary>
    /// 释放维斯大招。
    /// </summary>
    public override void Big()
    {
        StopBigCoroutine();
        bigCoro = StartCoroutine(BigCoro());
    }

    #endregion

    #region 大招流程

    /// <summary>
    /// 执行范围缴械流程。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(0.1f);
        //
        Jingji.SetActive(true);
        Jingji.transform.SetParent(null);
        yield return new WaitForSeconds(3);
        // 获取圆形范围参数
        Vector2 center = Jingji.transform.position;
        CircleCollider2D rangeCollider = GetJingjiCollider();
        float radius = rangeCollider.radius * Jingji.transform.lossyScale.x;

        // 检测该范围内所有碰撞体
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D col in hits)
        {
            // 你可以根据tag或者组件筛选目标，比如
            if (col.CompareTag("Role") && col.gameObject!=gameObject)
            {
                BaseRole targetRole = col.GetComponent<BaseRole>();
                VideoGameCombatUtility.SetCanGetWeaponThenBroadcast(targetRole, false, 10, $"{RoleName}缴械了{targetRole.RoleName},无法拾取武器", roleColor);
            }
        }
        SetSpeed(1.2f,5);
        Jingji.SetActive(false);
        Jingji.transform.SetParent(transform);
        Jingji.transform.localPosition = Vector3.zero;
        yield break;
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

    /// <summary>
    /// 获取并缓存荆棘范围碰撞体。
    /// </summary>
    /// <returns>荆棘范围上的圆形碰撞体。</returns>
    private CircleCollider2D GetJingjiCollider()
    {
        if (jingjiCollider == null)
        {
            jingjiCollider = Jingji.GetComponent<CircleCollider2D>();
        }

        return jingjiCollider;
    }

    #endregion
}

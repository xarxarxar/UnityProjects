using System.Collections;
using UnityEngine;

/// <summary>
/// 奶妈英雄脚本，负责释放治疗大招。
/// </summary>
public class NaiMaRole : BaseRole
{
    #region 协程引用

    private Coroutine bigCoro;

    #endregion

    #region 大招入口

    /// <summary>
    /// 释放奶妈大招。
    /// </summary>
    public override void Big()
    {
        StopBigCoroutine();
        bigCoro = StartCoroutine(BigCoro());
    }

    #endregion

    #region 大招流程

    /// <summary>
    /// 执行治疗大招流程。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(0.1f);
        Broadcast.instance.BroadCastNews($"{RoleName}使用了治疗，恢复了30点生命", roleColor);
        RecoverHp(30);
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

    #endregion
}
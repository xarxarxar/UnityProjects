using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 奶妈脚本
/// </summary>
public class NaiMaRole : BaseRole
{
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
        yield return new WaitForSeconds(0.1f);
        Broadcast.instance.BroadCastNews($"{RoleName}使用了治疗，恢复了30点生命", roleColor);
        RecoverHp(30);
        yield break;
    }
}

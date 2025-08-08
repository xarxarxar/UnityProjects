using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class YuanShiRole : BaseRole
{
    public YuanShiBigKnief BigKnief;//源氏的大刀
    private Coroutine bigCoro=null;
    public override void Big()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        
        SetSpeed(1.0f);
        BigKnief.BigKniefDisappear();
        bigCoro = StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(1.5f);
        SetSpeed(1.5f);
        Broadcast.instance.BroadCastNews($"{RoleName}拔出了大刀", roleColor);
        BigKnief.transform.parent.gameObject.SetActive(true);
        BigKnief.role=this;
        yield return new WaitForSeconds(5);
        SetSpeed(1.0f);
        if (BigKnief.transform.parent.gameObject.activeSelf)
        {
            BigKnief.BigKniefDisappear();
        }
        yield break;
    }
}

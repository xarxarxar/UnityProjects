using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YeLuRole : BaseRole
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
        yield return new WaitForSeconds(1.0f);
        Broadcast.instance.BroadCastNews($"{RoleName}使用了大招，进入了虚空", roleColor);
        transform.Find("Canvas").GetComponent<CanvasGroup>().alpha = 0.3f;
        gameObject.layer = LayerMask.NameToLayer("YeLu");
        SetSpeed(1.5f, 7.0f);
        yield return new WaitForSeconds(7.0f);
        transform.Find("Canvas").GetComponent<CanvasGroup>().alpha = 1.0f;
        gameObject.layer = LayerMask.NameToLayer("Default");
        yield break;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopAllCoroutines();
            Big();
        }
    }

}

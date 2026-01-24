using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class RankPanel : BasePanel
{
    public override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(DelayDoing());
    }

    IEnumerator DelayDoing()
    {
        yield return null;
        GameUIManager.Instance.RankButton();
    }

    public override void OnEnd()
    {
        WX.HideOpenData();
        gameObject.SetActive(false);
    }
}

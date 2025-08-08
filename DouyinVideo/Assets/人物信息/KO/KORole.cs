using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KORole : BaseRole
{
    public KOBig circle;//大招的那个圈圈
    public AudioClip yazhi;//压制的声音

    private Coroutine bigCoro=null;
    public override void Big()
    {
        circle.gameObject.SetActive(false);
        if(bigCoro != null )
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }

        bigCoro=StartCoroutine(BigCoro());
    }

    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(0.1f);
        circle.gameObject.SetActive(true);
        circle.role=this;
        for (int i = 0; i < 5; i++)
        {
            PlayAudio(yazhi);
            circle.canYazhi = true;
            yield return StartCoroutine(PlayWaterWave());
            circle.canYazhi = false;
            // 如果你要再做别的，比如淡出，写在这里
            yield return new WaitForSeconds(1f); // 可选等待一段时间
        }
        circle.gameObject.SetActive(false);
        yield break;
    }

    private IEnumerator PlayWaterWave()
    {
        // 初始缩放归零
        circle.transform.localScale = Vector3.zero;

        // 播放一次水波动画
        Tween t = circle.transform.DOScale(5f, 1f).SetEase(Ease.OutCubic);

        yield return t.WaitForCompletion(); // 等待动画完成

        
    }
}

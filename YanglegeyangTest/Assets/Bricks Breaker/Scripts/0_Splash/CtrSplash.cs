using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CtrSplash : CtrBase {
    public Image imageLayerlab;

    void Awake () {
        imageLayerlab.DOFade(0f, 0f);
    }


    protected override void Start()
    {
        StartCoroutine(StartCo());
        base.Start();
    }

    IEnumerator StartCo () {
        
        imageLayerlab.DOFade(1, 0.25f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(2f);

        imageLayerlab.DOFade(0, 0.2f).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.2f);
        PlayManager.Instance.isFade = true;

        //업데이트 체킹 기다리기
        yield return StartCoroutine(CheckUpdateCo());

        PlayManager.Instance.LoadScene(Data.scene_title);
    }

    IEnumerator CheckUpdateCo () {
        bool isCheckUpdate = true;

        while (isCheckUpdate) {
            yield return new WaitForSeconds(1f);
            isCheckUpdate = false;
        }
    }
}

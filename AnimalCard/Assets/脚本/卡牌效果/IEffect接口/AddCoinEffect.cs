using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddCoinEffect : IEffect
{
    private Vector3 fromPos;
    private Role targetRole;
    private float flyTime;
    private int count;
    private Transform showImage;

    private Tween moveTween;
    private bool finished;


    public AddCoinEffect(
        Vector3 fromPos,
        Role targetRole,
        float flyTime,
        int count,
        Transform showImage)
    {
        this.fromPos = fromPos;
        this.targetRole = targetRole;
        this.flyTime = flyTime;
        this.count = count;
        this.showImage = showImage;
    }

    public IEnumerator Play()
    {
        if (fromPos == null || targetRole == null || showImage == null)
            yield break;

        finished = false;

        Vector3 startPos = fromPos;
        Vector3 targetPos = targetRole.transform.position;

        showImage.position = startPos;


        moveTween = showImage.DOMove(targetPos, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetRole.AddCoin(count);
                Cleanup();
            });

        // 等动画完成
        yield return new WaitUntil(() => finished);
    }

    public void ForceStop()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (finished) return;
        finished = true;

        moveTween?.Kill();

        //统一回收入口
        PoolManager.Instance.LittleCoinPool.Return(showImage);
    }
}

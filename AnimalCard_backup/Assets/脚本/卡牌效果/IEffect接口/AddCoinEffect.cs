using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddCoinEffect : IEffect
{
    private Slot fromSlot;
    private Role targetRole;
    private float flyTime;
    private int count;
    private Transform showImage;

    public AddCoinEffect(
        Slot fromSlot,
        Role targetRole,
        float flyTime,
        int count,
        Transform showImage)
    {
        this.fromSlot = fromSlot;
        this.targetRole = targetRole;
        this.flyTime = flyTime;
        this.count = count;
        this.showImage = showImage;
    }

    public IEnumerator Play()
    {
        if (fromSlot == null || targetRole == null)
            yield break;

        

        Vector3 startPos = fromSlot.transform.position;
        Vector3 targetPos = targetRole.transform.position;

        showImage.position = startPos;


        bool finished = false;

        showImage.DOMove(targetPos, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetRole.AddCoin(count);
                PoolManager.Instance.TreatPool.Return(showImage);
                finished = true;
            });

        // 等动画完成
        yield return new WaitUntil(() => finished);
    }
}

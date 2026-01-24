using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShieldEffect : IEffect
{
    private Slot fromSlot;
    private Role targetRole;
    private float flyTime;

    public AddShieldEffect(
        Slot fromSlot,
        Role targetRole,
        float flyTime)
    {
        this.fromSlot = fromSlot;
        this.targetRole = targetRole;
        this.flyTime = flyTime;
    }

    public IEnumerator Play()
    {
        if (fromSlot == null || targetRole == null)
            yield break;

        Vector3 startPos = fromSlot.transform.position;
        Vector3 targetPos = targetRole.transform.position;

        Transform showImage = PoolManager.Instance.ShieldPool.Get();

        showImage.position = startPos;

        bool finished = false;

        showImage.DOMove(targetPos, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetRole.AddShieldCount(1);
                PoolManager.Instance.ShieldPool.Return(showImage);
                finished = true;
            });

        // 等动画完成
        yield return new WaitUntil(() => finished);
    }
}

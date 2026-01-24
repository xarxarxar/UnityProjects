using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
///  治疗效果
/// </summary>
public class TreatmentEffect : IEffect
{
    private Slot fromSlot;
    private Role targetRole;
    private float flyTime;
    private int treat;//治疗量

    public TreatmentEffect(
        Slot fromSlot,
        Role targetRole,
        float flyTime,
        int treat)
    {
        this.fromSlot = fromSlot;
        this.targetRole = targetRole;
        this.flyTime = flyTime;
        this.treat = treat;
    }

    public IEnumerator Play()
    {
        Debug.Log("调用治疗");
        if (fromSlot == null || targetRole == null)
            yield break;

        Transform treatment = PoolManager.Instance.TreatPool.Get();

        Vector3 startPos = fromSlot.transform.position;
        Vector3 targetPos = targetRole.transform.position;

        treatment.position = startPos;

        // ===== 朝向目标（弓箭）=====
        bool finished = false;

        treatment.DOMove(targetPos, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetRole.RecoverHp(treat);
                PoolManager.Instance.TreatPool.Return(treatment);
                finished = true;
            });

        // 等动画完成
        yield return new WaitUntil(() => finished);
    }
}

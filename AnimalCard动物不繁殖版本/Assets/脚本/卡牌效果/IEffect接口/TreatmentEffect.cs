using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 治疗效果（多发射 + 可强制中断）
/// </summary>
public class TreatmentEffect : IEffect
{
    private Vector3 fromPos;
    private Role targetRole;
    private float flyTime;
    private int treat;
    private int count;
    private float interval;

    private bool forceStopped;
    private readonly List<Transform> treatments = new();

    public TreatmentEffect(
        Vector3 fromPos,
        Role targetRole,
        float flyTime,
        int treat,
        int count,
        float interval)
    {
        this.fromPos = fromPos;
        this.targetRole = targetRole;
        this.flyTime = flyTime;
        this.treat = treat;
        this.count = count;
        this.interval = interval;
    }

    public IEnumerator Play()
    {
        forceStopped = false;

        if (targetRole == null)
            yield break;

        for (int i = 0; i < count; i++)
        {
            if (forceStopped || targetRole == null)
                yield break;

            Transform treatment = PoolManager.Instance.TreatPool.Get();
            treatments.Add(treatment);

            FireTreatment(treatment);

            yield return new WaitForSeconds(interval);
        }
    }

    private void FireTreatment(Transform treatment)
    {
        Vector3 startPos = fromPos;
        Vector3 targetPos = targetRole.transform.position;

        treatment.position = startPos;

        treatment.DOMove(targetPos, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (!forceStopped && targetRole != null)
                {
                    targetRole.RecoverHp(treat);
                }

                RecycleTreatment(treatment);
            });
    }

    private void RecycleTreatment(Transform treatment)
    {
        if (treatment == null)
            return;

        treatment.DOKill();
        PoolManager.Instance.TreatPool.Return(treatment);
        treatments.Remove(treatment);
    }

    /// <summary>
    /// 强制中断（战斗结束 / 切场景）
    /// </summary>
    public void ForceStop()
    {
        if (forceStopped)
            return;

        forceStopped = true;

        for (int i = treatments.Count - 1; i >= 0; i--)
        {
            RecycleTreatment(treatments[i]);
        }

        treatments.Clear();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiArrowEffect : IEffect
{
    private Vector3 fromPos;
    private Role target;
    private int arrowCount;
    private float interval;
    private float flyTime;

    private bool forceStopped;

    // 当前活跃的箭
    private readonly List<Transform> arrows = new();

    public MultiArrowEffect(
        Vector3 fromPos,
        Role target,
        int arrowCount,
        float interval,
        float flyTime)
    {
        this.fromPos = fromPos;
        this.target = target;
        this.arrowCount = arrowCount;
        this.interval = interval;
        this.flyTime = flyTime;
    }

    public IEnumerator Play()
    {
        forceStopped = false;

        for (int i = 0; i < arrowCount; i++)
        {
            if (forceStopped || target == null)
                yield break;

            Transform arrow = PoolManager.Instance.ArrawPool.Get();
            arrows.Add(arrow);

            FireArrow(arrow);

            yield return new WaitForSeconds(interval);
        }
    }

    private void FireArrow(Transform arrow)
    {
        Vector3 start = fromPos;
        Vector3 end = target.transform.position;

        arrow.position = start;

        Vector3 dir = (end - start).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        arrow.DOMove(end, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (!forceStopped && target != null)
                {
                    target.TakeDamage(1);
                }

                RecycleArrow(arrow);
            });
    }

    private void RecycleArrow(Transform arrow)
    {
        if (arrow == null)
            return;

        arrow.DOKill();
        PoolManager.Instance.ArrawPool.Return(arrow);
        arrows.Remove(arrow);
    }

    /// <summary>
    /// 强制终止（游戏结束 / 战斗结束）
    /// </summary>
    public void ForceStop()
    {
        if (forceStopped)
            return;

        forceStopped = true;

        // 回收所有仍在飞的箭
        for (int i = arrows.Count - 1; i >= 0; i--)
        {
            RecycleArrow(arrows[i]);
        }

        arrows.Clear();
    }
}

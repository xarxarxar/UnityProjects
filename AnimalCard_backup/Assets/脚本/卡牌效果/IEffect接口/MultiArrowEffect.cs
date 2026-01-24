using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MultiArrowEffect : IEffect
{
    private Slot fromSlot;
    private Role target;
    private int arrowCount;
    private float interval;
    private float flyTime;

    public MultiArrowEffect(
        Slot fromSlot,
        Role target,
        int arrowCount,
        float interval,
        float flyTime)
    {
        this.fromSlot = fromSlot;
        this.target = target;
        this.arrowCount = arrowCount;
        this.interval = interval;
        this.flyTime = flyTime;
    }

    public IEnumerator Play()
    {
        for (int i = 0; i < arrowCount; i++)
        {
            Transform arrow = PoolManager.Instance.ArrawPool.Get();

            FireArrow(arrow);

            yield return new WaitForSeconds(interval);
        }
    }

    private void FireArrow(Transform arrow)
    {
        Vector3 start = fromSlot.transform.position;
        Vector3 end = target.transform.position;

        arrow.position = start;

        Vector3 dir = (end - start).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        arrow.DOMove(end, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                target.TakeDamage(1);
                PoolManager.Instance.ArrawPool.Return(arrow);
            });
    }
}

using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ArrowDamageEffect : IEffect
{
    private Slot fromSlot;
    private Role targetRole;
    private float flyTime;
    private int damage;

    public ArrowDamageEffect(
        Slot fromSlot,
        Role targetRole,
        float flyTime,
        int damage)
    {
        this.fromSlot = fromSlot;
        this.targetRole = targetRole;
        this.flyTime = flyTime;
        this.damage = damage;
    }

    public IEnumerator Play()
    {
        if (fromSlot == null || targetRole == null)
            yield break;

        Transform arrow = PoolManager.Instance.ArrawPool.Get();

        Vector3 startPos = fromSlot.transform.position;
        Vector3 targetPos = targetRole.transform.position;

        arrow.position = startPos;

        // ===== 朝向目标（弓箭）=====
        Vector3 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        bool finished = false;

        arrow.DOMove(targetPos, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                targetRole.TakeDamage(damage);
                PoolManager.Instance.ArrawPool.Return(arrow);
                finished = true;
            });

        // 等动画完成
        yield return new WaitUntil(() => finished);
    }
}

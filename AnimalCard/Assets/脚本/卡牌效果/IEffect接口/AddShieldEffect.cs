using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShieldEffect : IEffect
{
    private Vector3 fromPos;
    private Role targetRole;
    private int shieldCount;
    private float interval;
    private float flyTime;

    private bool forceStopped;

    // 当前活跃的护盾图标
    private readonly List<Transform> shields = new();

    public AddShieldEffect(
        Vector3 fromPos,
        Role targetRole,
        int count,
        float interval,
        float flyTime)
    {
        this.fromPos = fromPos;
        this.targetRole = targetRole;
        this.shieldCount = count;
        this.interval = interval;
        this.flyTime = flyTime;
    }

    public IEnumerator Play()
    {
        forceStopped = false;

        if (targetRole == null)
            yield break;

        for (int i = 0; i < shieldCount; i++)
        {
            if (forceStopped || targetRole == null)
                yield break;

            Transform shield = PoolManager.Instance.ShieldPool.Get();
            shields.Add(shield);

            FireShield(shield);

            yield return new WaitForSeconds(interval);
        }
    }

    private void FireShield(Transform shield)
    {
        Vector3 start = fromPos;
        Vector3 end = targetRole.transform.position;

        shield.position = start;

        shield.DOMove(end, flyTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if (!forceStopped && targetRole != null)
                {
                    targetRole.AddShieldCount(1);
                }

                RecycleShield(shield);
            });
    }

    private void RecycleShield(Transform shield)
    {
        if (shield == null)
            return;

        shield.DOKill();
        PoolManager.Instance.ShieldPool.Return(shield);
        shields.Remove(shield);
    }

    /// <summary>
    /// 强制中断（游戏结束 / 战斗结束）
    /// </summary>
    public void ForceStop()
    {
        if (forceStopped)
            return;

        forceStopped = true;

        for (int i = shields.Count - 1; i >= 0; i--)
        {
            RecycleShield(shields[i]);
        }

        shields.Clear();
    }
}

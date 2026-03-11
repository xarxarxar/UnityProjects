using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMuchDamageEffect : IEffect
{
    private Vector3 fromPos;
    private Role targetRole;
    private int damage;
    private int count;
    private float interval;
    private float flyTime;

    private bool forceStopped;

    // 当前所有正在飞行的炸弹
    private readonly List<Transform> bombs = new();

    // 可调参数
    private const float MaxScale = 1.5f;
    private const float MinScale = 0.5f;

    public DoMuchDamageEffect(
        Vector3 fromPos,
        Role targetRole,
        int damage,
        int count,
        float interval,
        float flyTime)
    {
        this.fromPos = fromPos;
        this.targetRole = targetRole;
        this.damage = damage;
        this.count = count;
        this.interval = interval;
        this.flyTime = flyTime;
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

            Transform bomb = PoolManager.Instance.BoomPool.Get();
            bombs.Add(bomb);

            FireBomb(bomb);

            yield return new WaitForSeconds(interval);
        }
    }

    private void FireBomb(Transform bomb)
    {
        Vector3 startPos = fromPos;
        Vector3 targetPos = targetRole.transform.position;

        bomb.position = startPos;
        bomb.localScale = Vector3.one * MinScale;

        float elapsed = 0f;

        // 用 DOTween.To 驱动抛物线逻辑
        DOTween.To(() => 0f, t =>
        {
            if (forceStopped || bomb == null)
                return;

            // 位置
            bomb.position = Vector3.Lerp(startPos, targetPos, t);

            // 缩放模拟抛物线
            float heightFactor = Mathf.Sin(Mathf.PI * t);
            float scale = Mathf.Lerp(MinScale, MaxScale, heightFactor);
            bomb.localScale = Vector3.one * scale;

        }, 1f, flyTime)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            if (!forceStopped && targetRole != null)
            {
                targetRole.TakeDamage(damage);
            }

            RecycleBomb(bomb);
        });
    }

    private void RecycleBomb(Transform bomb)
    {
        if (bomb == null)
            return;

        bomb.DOKill();
        PoolManager.Instance.BoomPool.Return(bomb);
        bombs.Remove(bomb);
    }

    /// <summary>
    /// 强制中断（战斗结束 / 切场景）
    /// </summary>
    public void ForceStop()
    {
        if (forceStopped)
            return;

        forceStopped = true;

        for (int i = bombs.Count - 1; i >= 0; i--)
        {
            RecycleBomb(bombs[i]);
        }

        bombs.Clear();
    }
}

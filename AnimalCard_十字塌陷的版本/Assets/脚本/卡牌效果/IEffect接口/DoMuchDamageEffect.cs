using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoMuchDamageEffect : IEffect
{
    private Slot fromSlot;
    private Role targetRole;
    private float flyTime;
    private int damage;

    public DoMuchDamageEffect(
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

        // 从对象池获取炸弹
        Transform bomb = PoolManager.Instance.BoomPool.Get();

        Vector3 startPos = fromSlot.transform.position;
        Vector3 targetPos = targetRole.transform.position;

        bomb.position = startPos;

        float duration = flyTime;
        float elapsed = 0f;

        // 高度参数，可调节炸弹飞行弧线
        float maxScale = 1.5f; // 炸弹最大放大倍数
        float minScale = 0.5f; // 炸弹起始和落地大小

        // 起始缩放
        bomb.localScale = Vector3.one * minScale;


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 线性水平移动
            bomb.position = Vector3.Lerp(startPos, targetPos, t);

            // 抛物线高度模拟：用缩放表示
            float heightFactor = Mathf.Sin(Mathf.PI * t); // 0->1->0
            float scale = Mathf.Lerp(minScale, maxScale, heightFactor);
            bomb.localScale = Vector3.one * scale;

            yield return null;
        }

        // 确保到达目标位置和最小缩放
        bomb.position = targetPos;
        bomb.localScale = Vector3.one * minScale;

        // 造成伤害
        targetRole.TakeDamage(damage);

        // 回收炸弹
        PoolManager.Instance.BoomPool.Return(bomb);
        yield return null;
    }
}

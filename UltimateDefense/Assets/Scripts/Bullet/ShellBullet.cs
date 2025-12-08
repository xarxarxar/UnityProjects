using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 炮弹型子弹，飞到预定地点爆炸
/// </summary>
public class ShellBullet : Bullet
{
    [HideInInspector]public Vector3 targetPosition;//目标位置
    public override float Speed => 8.0f;
    // 可配置
    private readonly float maxScale = 1.5f;
    private Vector3 startPosition;//起始位置
    private float totalDistance;//到目标点的总距离

    // 初始化（传入起点和目标位置）
    public virtual void Init(int damage, bool isCritical, Vector3 startPos, Vector3 targetPos)
    {
        base.Init(damage, isCritical, startPos);
        targetPosition = targetPos;
        startPosition = startPos;
        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }

    protected override void OnUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition,
            Speed * BattleManager.Instance.GameSpeed.Value * Time.deltaTime);

        //当前离起点的距离
        // 计算进度 p（从 0 到 1）
        float distFromStart = Vector3.Distance(startPosition, transform.position);
        float p = Mathf.Clamp01(distFromStart / totalDistance);

        // 抛物线形缩放：peak 在 p=0.5
        //float t = 4f * p * (1f - p); // 0..1..0, peak=1 at p=0.5
        float t = Mathf.Pow(4f * p * (1f - p), 2f);  // 强化中点，拉开两端的差异
        float scale = Mathf.Lerp(1.0f, maxScale, t);
        transform.localScale = new Vector3(scale, scale, 1);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            ReachTarget();
        }
    }


    private void ReachTarget()
    {
        ReturnToPool();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 激光
/// </summary>
public class LaserBullet : Bullet
{
    private Vector3 startPos;
    private Enemy targetEnemy;
    private float damageInterval = 0.2f;//伤害间隔

    private float damageTimer = 0f;
    private LineRenderer lineRenderer;
    private float maxLaserDistance = 15f;

    private int _targetID;//敌人的ID

    // 初始化激光
    public void Init(int damage, bool isCritical, Enemy target)
    {
        base.Init(damage, isCritical, transform.position);
        targetEnemy = target;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        damageTimer = 0f;

        // 激光启用
        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    protected override void OnUpdate()
    {
        if (targetEnemy == null || targetEnemy.UniqueID != _targetID)
        {
            EndLaser();
            return;
        }

        UpdateLaserVisual();
        ApplyDamageOverTime();
    }

    /// <summary>
    /// 控制激光的线段样式
    /// </summary>
    private void UpdateLaserVisual()
    {
        Vector3 start = transform.position;
        Vector3 end = targetEnemy.transform.position;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // 激光朝向目标
        Vector3 dir = (end - start).normalized;
        transform.right = dir;
    }

    /// <summary>
    /// 定时造成伤害
    /// </summary>
    private void ApplyDamageOverTime()
    {
        damageTimer += Time.deltaTime * BattleManager.Instance.GameSpeed.Value;

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            targetEnemy.TakeDamage(IsCritical, Damage);
        }
    }

    /// <summary>
    /// 激光结束（目标死亡或自己结束）
    /// </summary>
    private void EndLaser()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        ReturnToPool();
    }
}

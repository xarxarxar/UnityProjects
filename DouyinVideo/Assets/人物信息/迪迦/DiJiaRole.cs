using System.Collections;
using UnityEngine;

/// <summary>
/// 迪迦英雄，负责释放持续抖动激光并造成伤害。
/// </summary>
public class DiJiaRole : BaseRole
{
    #region 常量

    private const string WallTag = "wall";
    private const string RoleTag = "Role";
    private const string IgnoreLayerYeLu = "YeLu";
    private const string IgnoreLayerWeapon = "Weapon";
    private const float CastDelay = 1.0f;
    private const float NoiseSpeed = 5.0f;
    private const float NoiseAmplitude = 15f;
    private const float LaserDuration = 5f;
    private const float DamageInterval = 0.2f;
    private const float LaserStartOffset = 2.5f;
    private const float LaserDistance = 100f;
    private const int HitDamage = 3;

    #endregion

    #region Inspector 字段

    [Tooltip("迪迦激光线段渲染器")]
    public LineRenderer lineRenderer;

    #endregion

    #region 运行时状态

    private Coroutine bigCoro = null;
    private Coroutine updateLaserCoro = null;

    #endregion

    #region 大招入口

    /// <summary>
    /// 释放迪迦大招。
    /// </summary>
    public override void Big()
    {
        StopBigCoroutine();
        CloseLaser();
        bigCoro = StartCoroutine(BigCoro());
    }

    /// <summary>
    /// 延迟后开始激光流程。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator BigCoro()
    {
        yield return new WaitForSeconds(CastDelay);
        Broadcast.instance.BroadCastNews($"{RoleName}使用大招", roleColor);
        StartLaser();
        yield break;
    }

    #endregion

    #region 激光流程

    /// <summary>
    /// 开启激光显示并启动逐帧更新。
    /// </summary>
    private void StartLaser()
    {
        lineRenderer.enabled = true;
        StopUpdateLaserCoroutine();
        updateLaserCoro = StartCoroutine(UpdateLaser());
    }

    /// <summary>
    /// 按原规则持续更新激光方向、命中和伤害。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator UpdateLaser()
    {
        float damageTimer = 0f;
        float elapsedTime = 0f;

        if (otherBaseRole == null)
        {
            // 激光结束，关闭LineRenderer
            CloseLaser();
            yield break;
        }

        Transform target = otherBaseRole.transform;
        SetSpeed(0);
        int layerMask = GetLaserLayerMask();

        while (lineRenderer.enabled && elapsedTime < LaserDuration)
        {
            if (otherBaseRole == null)
            {
                // 激光结束，关闭LineRenderer
                CloseLaser();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            damageTimer += Time.deltaTime;

            // 获取目标方向
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            Vector2 startPos = (Vector2)transform.position + directionToTarget * LaserStartOffset;
            Vector2 finalDirection = GetNoisyLaserDirection(directionToTarget);

            // 发射射线
            RaycastHit2D hit = Physics2D.Raycast(startPos, finalDirection, LaserDistance, layerMask);
            Vector2 endPos;

            if (IsLaserBlockingHit(hit))
            {
                endPos = hit.point;

                // 如果命中Role并且时间到达伤害间隔，造成伤害
                if (hit.collider.CompareTag(RoleTag) && damageTimer >= DamageInterval)
                {
                    damageTimer = 0f;
                    hit.collider.GetComponent<BaseRole>().TakeDamage(HitDamage);
                }
            }
            else
            {
                endPos = startPos + finalDirection * LaserDistance;
            }

            UpdateLaserLine(startPos, endPos);

            yield return null;
        }

        SetSpeed(1);
        // 激光结束，关闭LineRenderer
        CloseLaser();
        yield break;
    }

    /// <summary>
    /// 计算带平滑抖动的激光方向。
    /// </summary>
    /// <param name="directionToTarget">朝向目标的基础方向。</param>
    /// <returns>加入抖动后的激光方向。</returns>
    private Vector2 GetNoisyLaserDirection(Vector2 directionToTarget)
    {
        float time = Time.time * NoiseSpeed;
        float smoothAngleOffset = Mathf.Sin(time) * NoiseAmplitude;
        Quaternion rotationOffset = Quaternion.Euler(0, 0, smoothAngleOffset);
        return rotationOffset * directionToTarget;
    }

    /// <summary>
    /// 判断射线是否命中墙体或角色。
    /// </summary>
    /// <param name="hit">射线命中结果。</param>
    /// <returns>命中墙体或角色时返回 true。</returns>
    private bool IsLaserBlockingHit(RaycastHit2D hit)
    {
        return hit.collider != null &&
               (hit.collider.CompareTag(WallTag) || hit.collider.CompareTag(RoleTag));
    }

    /// <summary>
    /// 刷新激光线段起点和终点。
    /// </summary>
    /// <param name="startPos">激光起点。</param>
    /// <param name="endPos">激光终点。</param>
    private void UpdateLaserLine(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    /// <summary>
    /// 关闭激光显示。
    /// </summary>
    private void CloseLaser()
    {
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// 获取激光检测用的 LayerMask。
    /// </summary>
    /// <returns>排除夜露层和武器层后的射线检测 LayerMask。</returns>
    private int GetLaserLayerMask()
    {
        int ignoreLayer = LayerMask.GetMask(IgnoreLayerYeLu, IgnoreLayerWeapon);
        return ~ignoreLayer;
    }

    #endregion

    #region 协程工具

    /// <summary>
    /// 停止大招延迟协程。
    /// </summary>
    private void StopBigCoroutine()
    {
        if (bigCoro != null)
        {
            StopCoroutine(bigCoro);
            bigCoro = null;
        }
    }

    /// <summary>
    /// 停止激光更新协程。
    /// </summary>
    private void StopUpdateLaserCoroutine()
    {
        if (updateLaserCoro != null)
        {
            StopCoroutine(updateLaserCoro);
            updateLaserCoro = null;
        }
    }

    #endregion
}
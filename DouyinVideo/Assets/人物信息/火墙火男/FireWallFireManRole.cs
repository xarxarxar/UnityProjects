using System.Collections;
using UnityEngine;

/// <summary>
/// 火墙火男英雄，负责释放持续伤害火墙和触碰火墙回血。
/// </summary>
public class FireWallFireManRole : BaseRole
{
    #region 常量

    private const string IgnoreLayerName = "YeLu";
    private const float CastDelay = 1.0f;
    private const float FireWallLength = 20f;
    private const float ExtendDuration = 0.5f;
    private const float DamageInterval = 0.15f;
    private const float ActiveDuration = 5.0f;
    private const int HitDamage = 3;
    private const int HealTimes = 6;
    private const float HealInterval = 0.5f;
    private const int HealAmount = 2;

    #endregion

    #region Inspector 字段

    [Tooltip("火墙线段渲染器")]
    public LineRenderer lineRenderer;

    [Tooltip("火焰帧图数组")]
    public Texture[] flameFrames;

    [Tooltip("火焰每秒播放帧数")]
    public float frameRate = 15f;

    #endregion

    #region 运行时状态

    private bool isRecovered = false;
    private Coroutine fireCoro = null;
    private Coroutine playFlameLoopCoro = null;
    private Coroutine recoverHealthCoro = null;

    #endregion

    #region 大招入口

    /// <summary>
    /// 释放火墙大招。
    /// </summary>
    public override void Big()
    {
        ResetFireWallBeforeCast();
        StopFireCoroutine();
        fireCoro = StartCoroutine(DrawFireWall());
    }

    #endregion

    #region 开发调试入口

    private void Update()
    {
        // 开发调试入口：保留原 O 键触发大招和回血。
        if (Input.GetKeyUp(KeyCode.O))
        {
            UseBig();
            RecoverHp(2);
        }
    }

    #endregion

    #region 火墙流程

    /// <summary>
    /// 按原顺序绘制火墙、检测命中并处理持续伤害。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator DrawFireWall()
    {
        yield return new WaitForSeconds(CastDelay);

        Vector3 startPos = transform.position;
        Vector3 direction = (otherBaseRole.transform.position - startPos).normalized;
        Vector3 endPos = startPos + direction * FireWallLength;
        float time = 0f;

        InitializeFireWallLine(startPos);
        StartFlameLoop();

        // 步骤 1：激光逐步延伸
        while (time < ExtendDuration)
        {
            time += Time.deltaTime;
            float t = time / ExtendDuration;
            Vector3 currentEnd = Vector3.Lerp(startPos, endPos, t);
            lineRenderer.transform.position = currentEnd; // 如果你要控制整体位置，否则可以删掉
            lineRenderer.SetPosition(1, currentEnd);
            yield return null;
        }

        // 步骤 2：激光延伸完成，设置终点
        lineRenderer.SetPosition(1, endPos); // 最终精确对齐

        // 步骤 3：持续检测是否有角色被击中，5 秒内持续造成伤害
        GameObject currentTarget = null;
        BaseRole currentTargetRole = null;
        float damageTimer = 0f;
        int layerMask = GetFireWallLayerMask();

        float activeTime = 0f;

        while (activeTime < ActiveDuration)
        {
            activeTime += Time.deltaTime;

            RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, Vector3.Distance(startPos, endPos), layerMask);
            GameObject newTarget = null;

            foreach (RaycastHit2D h in hits)
            {
                if (h.collider != null && h.collider.CompareTag("Role"))
                {
                    if (h.collider.gameObject == gameObject)
                    {
                        TryStartRecoverHealth();
                        continue; // 跳过自己，不当作攻击目标
                    }
                    newTarget = h.collider.gameObject;

                    break;
                }
            }

            if (newTarget != null)
            {
                if (newTarget == currentTarget)
                {
                    damageTimer += Time.deltaTime;
                    if (damageTimer >= DamageInterval)
                    {
                        damageTimer = 0f;
                        currentTargetRole?.TakeDamage(HitDamage);
                        Broadcast.instance.BroadCastNews($"{RoleName}的火墙对 {newTarget.name} 造成了3点伤害", roleColor);
                    }
                }
                else
                {
                    currentTarget = newTarget;
                    currentTargetRole = currentTarget.GetComponent<BaseRole>();
                    damageTimer = 0f;
                }
            }
            else
            {
                currentTarget = null;
                currentTargetRole = null;
                damageTimer = 0f;
            }

            yield return null;
        }

        // 步骤 4：时间结束，关闭激光
        CloseFireWall();
        StopFlameLoop();
        StopFireCoroutine();
        Debug.Log("火墙结束");
        yield break;
    }

    /// <summary>
    /// 初始化火墙释放前的可见状态。
    /// </summary>
    private void ResetFireWallBeforeCast()
    {
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.enabled = false;
        isRecovered = false;
    }

    /// <summary>
    /// 初始化火墙线段的起点和终点。
    /// </summary>
    /// <param name="startPos">火墙起点。</param>
    private void InitializeFireWallLine(Vector3 startPos)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos); // 初始长度为 0
    }

    /// <summary>
    /// 关闭火墙显示并恢复本次回血标记。
    /// </summary>
    private void CloseFireWall()
    {
        lineRenderer.enabled = false;
        lineRenderer.gameObject.SetActive(false);
        isRecovered = false;
    }

    /// <summary>
    /// 获取火墙检测用的 LayerMask。
    /// </summary>
    /// <returns>排除夜露层后的射线检测 LayerMask。</returns>
    private int GetFireWallLayerMask()
    {
        int ignoreLayer = LayerMask.GetMask(IgnoreLayerName);
        return ~ignoreLayer;
    }

    #endregion

    #region 火焰动画

    /// <summary>
    /// 启动火焰帧动画协程。
    /// </summary>
    private void StartFlameLoop()
    {
        StopFlameLoop();
        if (!CanPlayFlameLoop())
        {
            return;
        }

        playFlameLoopCoro = StartCoroutine(PlayFlameLoop());// 火焰的粒子或视觉效果
    }

    /// <summary>
    /// 停止火焰帧动画协程。
    /// </summary>
    private void StopFlameLoop()
    {
        if (playFlameLoopCoro != null)
        {
            StopCoroutine(playFlameLoopCoro);
            playFlameLoopCoro = null;
        }
    }

    /// <summary>
    /// 判断当前火焰帧配置是否可以播放。
    /// </summary>
    /// <returns>配置有效时返回 true。</returns>
    private bool CanPlayFlameLoop()
    {
        return frameRate > 0f && flameFrames != null && flameFrames.Length > 0;
    }

    /// <summary>
    /// 循环播放火焰帧图。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator PlayFlameLoop()
    {
        if (!CanPlayFlameLoop())
        {
            yield break;
        }

        int frame = 0;
        float waitTime = 1f / frameRate;
        WaitForSeconds wait = new WaitForSeconds(waitTime);

        while (true)
        {
            lineRenderer.material.mainTexture = flameFrames[frame];
            frame = (frame + 1) % flameFrames.Length;
            yield return wait;
        }
    }

    #endregion

    #region 回血

    /// <summary>
    /// 停止旧回血协程，并启动新的回血检测流程。
    /// </summary>
    private void TryStartRecoverHealth()
    {
        if (recoverHealthCoro != null)
        {
            StopCoroutine(recoverHealthCoro);
            recoverHealthCoro = null;
        }

        recoverHealthCoro = StartCoroutine(RecoverHealth());
    }

    /// <summary>
    /// 火男触碰火墙后按原次数和间隔回血。
    /// </summary>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator RecoverHealth()
    {
        if (isRecovered) yield break;
        Debug.Log("火男回血");
        Broadcast.instance.BroadCastNews($"{RoleName}碰到了火墙，开始回血", roleColor);
        isRecovered = true;

        for (int i = 0; i < HealTimes; i++)
        {
            RecoverHp(HealAmount);

            yield return new WaitForSeconds(HealInterval);
        }
        yield break;

    }

    #endregion

    #region 协程工具

    /// <summary>
    /// 停止火墙主协程。
    /// </summary>
    private void StopFireCoroutine()
    {
        if (fireCoro != null)
        {
            StopCoroutine(fireCoro);
            fireCoro = null;
        }
    }

    #endregion
}
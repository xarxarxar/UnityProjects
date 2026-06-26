using System.Collections;
using UnityEngine;

/// <summary>
/// 绊线保安英雄，负责发射墙面绊线并在命中角色后造成减速和伤害。
/// </summary>
public class BanXianBaoAnRole : BaseRole
{
    #region 常量

    private const string WallLayerName = "Wall";
    private const string IgnoreLayerYeLu = "YeLu";
    private const string RoleTag = "Role";
    private const float EmitterRayDistance = 100f;
    private const float LaserRayDistance = 100f;
    private const float LaserWallOffset = 0.01f;
    private const float EmitterMoveSpeed = 70.0f;
    private const float LaserDrawDuration = 0.1f;
    private const float HitFollowDuration = 1.5f;
    private const float TripSpeedMultiplier = 0.3f;
    private const float TripSlowDuration = 3f;
    private const int TripDamage = 10;

    #endregion

    #region Inspector 字段

    [Tooltip("绊线激光发射器")]
    public GameObject laserEmitter;

    [Tooltip("绊线激光接收器")]
    public GameObject laserReceiver;

    [Tooltip("绊线激光线段渲染器")]
    public LineRenderer laserLineRenderer;

    [Tooltip("绊线绊倒角色时播放的音效")]
    public AudioClip bandaorenAudio;

    #endregion

    #region 运行时状态

    private Color32 normalLaser = new Color32(0, 237, 255, 255);
    private Color32 hitLaser = new Color32(190, 74, 63, 255);
    private bool laserEmitterAttached = false;

    private Coroutine bigCoro = null;
    private Coroutine drawLaserOverTimeCoro = null;

    #endregion

    #region 大招入口

    /// <summary>
    /// 释放绊线保安大招。
    /// </summary>
    public override void Big()
    {
        ShootLaserEmitter();
    }

    #endregion

    #region 发射器流程

    /// <summary>
    /// 向目标方向发射激光发射器，命中墙体后开始布置绊线。
    /// </summary>
    private void ShootLaserEmitter()
    {
        Vector3 direction = (otherBaseRole.transform.position - transform.position).normalized;
        LayerMask wallLayerMask = LayerMask.GetMask(WallLayerName);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, EmitterRayDistance, wallLayerMask);

        if (hit.collider != null)
        {
            ActivateLaserDevices();
            laserEmitterAttached = true;
            DetachLaserDevices();
            StopBigCoroutine();

            bigCoro = StartCoroutine(MoveLaserEmitter(transform.position, hit.point, EmitterMoveSpeed, hit));
        }
    }

    /// <summary>
    /// 将激光发射器移动到墙体命中点。
    /// </summary>
    /// <param name="startPos">发射器起点。</param>
    /// <param name="targetPos">发射器目标点。</param>
    /// <param name="speed">发射器移动速度。</param>
    /// <param name="hit">最初命中的墙体射线结果。</param>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator MoveLaserEmitter(Vector3 startPos, Vector3 targetPos, float speed, RaycastHit2D hit)
    {
        float distance = Vector3.Distance(startPos, targetPos);
        float moved = 0f;

        while (moved < distance)
        {
            float step = speed * Time.deltaTime;
            moved += step;
            float t = Mathf.Clamp01(moved / distance);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            laserEmitter.transform.position = currentPos;
            laserReceiver.transform.position = currentPos;
            yield return null;
        }

        laserEmitter.transform.position = targetPos;
        laserReceiver.transform.position = targetPos;

        if (laserEmitterAttached)
        {
            LayerMask wallLayerMask = LayerMask.GetMask(WallLayerName);

            Vector3 laserDirection = hit.normal; // 关键：使用命中的法线作为激光方向
            RaycastHit2D laserHit = Physics2D.Raycast(laserEmitter.transform.position + laserDirection * LaserWallOffset, laserDirection, LaserRayDistance, wallLayerMask);

            if (laserHit.collider != null)
            {
                StartDrawLaserOverTime(laserEmitter.transform.position, laserHit.point);
            }
            else
            {
                StartDrawLaserOverTime(laserEmitter.transform.position, laserEmitter.transform.position + laserDirection * LaserRayDistance);
            }
        }
    }

    #endregion

    #region 绊线检测

    /// <summary>
    /// 按原逻辑延伸绊线，并持续检测第一个命中的角色。
    /// </summary>
    /// <param name="start">绊线起点。</param>
    /// <param name="end">绊线终点。</param>
    /// <param name="duration">绊线延伸时间。</param>
    /// <returns>Unity 协程迭代器。</returns>
    private IEnumerator DrawLaserOverTime(Vector3 start, Vector3 end, float duration)
    {
        float time = 0f;
        InitializeLaserLine(start);

        // 步骤 1：激光逐步延伸
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            Vector3 currentEnd = Vector3.Lerp(start, end, t);
            laserReceiver.transform.position = currentEnd;
            laserLineRenderer.SetPosition(1, currentEnd);
            yield return null;
        }

        // 步骤 2：激光延伸完成，设置终点
        laserLineRenderer.SetPosition(1, end); // 最终精确对齐

        while (true)
        {
            // 步骤 3：开始检测是否击中了 Role
            GameObject hitRole = FindFirstHitRole(start, end);

            // 步骤 4：如果击中了 Role，则形成折线
            if (hitRole != null)
            {
                float tmpTime = 0;
                laserLineRenderer.startColor = hitLaser;
                laserLineRenderer.endColor = hitLaser;

                BaseRole targetRole = hitRole.GetComponent<BaseRole>();
                VideoGameCombatUtility.SetSpeedThenBroadcast(targetRole, TripSpeedMultiplier, TripSlowDuration, $"{RoleName}的绊线对{targetRole.RoleName}造成了减速", roleColor);
                PlayAudio(bandaorenAudio);

                while (tmpTime < HitFollowDuration)
                {
                    UpdateHitLaserLine(start, hitRole.transform.position, end);
                    tmpTime += Time.deltaTime;
                    yield return null;
                }

                VideoGameCombatUtility.DamageThenBroadcast(targetRole, TripDamage, $"{RoleName}的绊线对{targetRole.RoleName}造成了10点伤害", roleColor);
                CloseLaserDevices();

                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 初始化绊线的显示状态。
    /// </summary>
    /// <param name="start">绊线起点。</param>
    private void InitializeLaserLine(Vector3 start)
    {
        laserLineRenderer.enabled = true;
        laserLineRenderer.positionCount = 2;
        laserLineRenderer.SetPosition(0, start);
        laserLineRenderer.SetPosition(1, start); // 初始长度为 0
        laserLineRenderer.startColor = normalLaser;
        laserLineRenderer.endColor = normalLaser;
    }

    /// <summary>
    /// 查找绊线当前命中的第一个非自己的角色。
    /// </summary>
    /// <param name="start">绊线起点。</param>
    /// <param name="end">绊线终点。</param>
    /// <returns>第一个被绊线命中的角色对象；未命中时返回 null。</returns>
    private GameObject FindFirstHitRole(Vector3 start, Vector3 end)
    {
        // LayerMask 排除自己
        int ignoreLayer = LayerMask.GetMask(IgnoreLayerYeLu);
        int layerMask = ~ignoreLayer;

        RaycastHit2D[] hits = Physics2D.RaycastAll(start, (end - start).normalized, Vector3.Distance(start, end), layerMask);

        foreach (RaycastHit2D h in hits)
        {
            if (h.collider != null && h.collider.CompareTag(RoleTag) && h.collider.gameObject != gameObject)
            {
                GameObject hitRole = h.collider.gameObject;
                Debug.Log($"击中了{hitRole.name}");
                return hitRole;
            }
        }

        return null;
    }

    /// <summary>
    /// 刷新命中后的三点折线。
    /// </summary>
    /// <param name="start">绊线起点。</param>
    /// <param name="middle">命中角色当前位置。</param>
    /// <param name="end">绊线终点。</param>
    private void UpdateHitLaserLine(Vector3 start, Vector3 middle, Vector3 end)
    {
        laserLineRenderer.positionCount = 3;
        laserLineRenderer.SetPosition(0, start);
        laserLineRenderer.SetPosition(1, middle);
        laserLineRenderer.SetPosition(2, end);
    }

    #endregion

    #region 设备与协程工具

    /// <summary>
    /// 启动绊线延伸协程。
    /// </summary>
    /// <param name="start">绊线起点。</param>
    /// <param name="end">绊线终点。</param>
    private void StartDrawLaserOverTime(Vector3 start, Vector3 end)
    {
        StopDrawLaserOverTimeCoroutine();
        drawLaserOverTimeCoro = StartCoroutine(DrawLaserOverTime(start, end, LaserDrawDuration));
    }

    /// <summary>
    /// 显示发射器和接收器。
    /// </summary>
    private void ActivateLaserDevices()
    {
        laserEmitter.SetActive(true);
        laserReceiver.SetActive(true);
    }

    /// <summary>
    /// 让发射器和接收器脱离角色父物体。
    /// </summary>
    private void DetachLaserDevices()
    {
        laserEmitter.transform.SetParent(null);
        laserReceiver.transform.SetParent(null);
    }

    /// <summary>
    /// 关闭绊线并把发射器和接收器挂回角色。
    /// </summary>
    private void CloseLaserDevices()
    {
        laserLineRenderer.enabled = false;
        laserEmitter.transform.SetParent(transform);
        laserReceiver.transform.SetParent(transform);
        laserEmitter.SetActive(false);
        laserReceiver.SetActive(false);
    }

    /// <summary>
    /// 停止发射器移动协程。
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
    /// 停止绊线延伸和检测协程。
    /// </summary>
    private void StopDrawLaserOverTimeCoroutine()
    {
        if (drawLaserOverTimeCoro != null)
        {
            StopCoroutine(drawLaserOverTimeCoro);
            drawLaserOverTimeCoro = null;
        }
    }

    #endregion
}
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 通用视频玩法子弹脚本，负责初始化移动、命中回调和回收。
/// </summary>
public class VideoGameBulletBase : MonoBehaviour
{
    #region 常量配置

    private const string RoleTag = "Role";
    private const string WallTag = "wall";
    private const string MissingRigidbodyWarning = "父物体没有 Rigidbody2D 组件！";

    #endregion

    #region 运行状态

    private Vector3 direction;
    private float speed = 50f;
    private BaseRole _baseRole;
    private string[] tags;

    #endregion

    #region 命中回调

    /// <summary>
    /// 子弹命中角色后触发。
    /// </summary>
    public UnityAction<BaseRole> OnBulletHitRole;

    /// <summary>
    /// 子弹命中墙后触发。
    /// </summary>
    public UnityAction OnBulletHitWall;

    #endregion

    #region 子弹流程

    /// <summary>
    /// 初始化子弹方向、角度、速度、起点和发射者。
    /// </summary>
    /// <param name="dir">子弹移动方向。</param>
    /// <param name="angle">子弹根物体 Z 轴角度。</param>
    /// <param name="speed">子弹移动速度。</param>
    /// <param name="startPos">子弹发射起点。</param>
    /// <param name="baseRole">发射子弹的角色。</param>
    /// <param name="tags">需要忽略的 Tag 列表。</param>
    public void Init(Vector3 dir,float angle,int speed,Vector3 startPos,
        BaseRole baseRole,string[] tags=null)
    {
        direction = dir.normalized;
        this.speed = speed;
        Transform bulletRoot = GetBulletRoot();
        bulletRoot.position = startPos;
        _baseRole = baseRole;
        this.tags = tags;

        SetBulletRootRotationAndActive(bulletRoot, angle);
        ApplyBulletVelocity(bulletRoot);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果该物体的 tag 在 tags 数组中，则忽略
        if (ShouldIgnoreTag(collision))
        {
            return;
        }

        if (TryHandleRoleHit(collision))
        {
            return;
        }

        if (collision.CompareTag(WallTag))
        {
            HandleWallHit();
        }
    }

    /// <summary>
    /// 回收子弹。
    /// </summary>
    public void Recycle()
    {
        StopAllCoroutines();
        GetBulletRoot().gameObject.SetActive(false);
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 获取子弹根物体。
    /// </summary>
    /// <returns>当前子弹脚本的父物体。</returns>
    private Transform GetBulletRoot()
    {
        return transform.parent;
    }

    /// <summary>
    /// 设置子弹根物体旋转并激活。
    /// </summary>
    /// <param name="bulletRoot">子弹根物体。</param>
    /// <param name="angle">子弹根物体 Z 轴角度。</param>
    private void SetBulletRootRotationAndActive(Transform bulletRoot, float angle)
    {
        bulletRoot.rotation = Quaternion.Euler(0, 0, angle);
        bulletRoot.gameObject.SetActive(true);
    }

    /// <summary>
    /// 给子弹根物体施加移动速度。
    /// </summary>
    /// <param name="bulletRoot">子弹根物体。</param>
    private void ApplyBulletVelocity(Transform bulletRoot)
    {
        // 设置父物体的速度
        Rigidbody2D rb = bulletRoot.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * this.speed;
        }
        else
        {
            Debug.LogWarning(MissingRigidbodyWarning);
        }
    }

    /// <summary>
    /// 尝试处理角色命中。
    /// </summary>
    /// <param name="collision">当前碰撞体。</param>
    /// <returns>如果碰撞体是非发射者角色，返回 true。</returns>
    private bool TryHandleRoleHit(Collider2D collision)
    {
        if (!collision.CompareTag(RoleTag) || collision.gameObject == _baseRole.gameObject)
        {
            return false;
        }

        BaseRole role = collision.GetComponent<BaseRole>();
        if (role != null)
        {
            OnBulletHitRole?.Invoke(role);
            // 命中角色后是否回收由外部回调决定，保留现有武器和英雄技能差异。
            //Recycle();
        }

        return true;
    }

    /// <summary>
    /// 处理墙体命中。
    /// </summary>
    private void HandleWallHit()
    {
        OnBulletHitWall?.Invoke();
        Recycle();
    }

    /// <summary>
    /// 判断当前碰撞体的 Tag 是否需要忽略。
    /// </summary>
    /// <param name="collision">当前碰撞体。</param>
    /// <returns>如果需要忽略，返回 true。</returns>
    private bool ShouldIgnoreTag(Collider2D collision)
    {
        if (tags == null)
        {
            return false;
        }

        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.CompareTag(tags[i]))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}

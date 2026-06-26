using UnityEngine;

/// <summary>
/// 厕所区域触发器，负责把进入区域的角色交给机制总控处理。
/// </summary>
[DisallowMultipleComponent]
public class ToiletArea : MonoBehaviour
{
    [SerializeField, Tooltip("上厕所机制总控；不填时会在第一次触发时自动查找。")]
    private ToiletMechanismManager mechanismManager;

    /// <summary>
    /// 手动设置上厕所机制总控。
    /// </summary>
    /// <param name="manager">负责处理厕所机制的总控组件。</param>
    public void SetMechanismManager(ToiletMechanismManager manager)
    {
        mechanismManager = manager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHandleRole(collision);
    }

    #region 内部流程

    private void TryHandleRole(Collider2D collision)
    {
        BaseRole role = GetRoleFromCollider(collision);
        if (role == null)
        {
            return;
        }

        ToiletMechanismManager manager = GetMechanismManager();
        if (manager == null)
        {
            return;
        }

        manager.TryEnterToilet(role);
    }

    private ToiletMechanismManager GetMechanismManager()
    {
        if (mechanismManager == null)
        {
            mechanismManager = FindObjectOfType<ToiletMechanismManager>();
        }

        return mechanismManager;
    }

    private static BaseRole GetRoleFromCollider(Collider2D collision)
    {
        BaseRole role = collision.GetComponent<BaseRole>();
        if (role == null)
        {
            role = collision.GetComponentInParent<BaseRole>();
        }

        return role;
    }

    #endregion
}

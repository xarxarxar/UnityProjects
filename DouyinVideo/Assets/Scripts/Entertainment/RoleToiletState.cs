using System.Collections;
using UnityEngine;

/// <summary>
/// 单个角色在“上厕所机制”中的运行时状态。
/// </summary>
[DisallowMultipleComponent]
public class RoleToiletState : MonoBehaviour
{
    [SerializeField, Tooltip("当前便意值，仅用于运行时观察。")]
    private int currentUrge;

    [SerializeField, Tooltip("是否正在厕所中强制停留。")]
    private bool isStayingInToilet;

    private BaseRole ownerRole;
    private Coroutine toiletStayCoroutine;

    public int CurrentUrge => currentUrge;
    public bool IsStayingInToilet => isStayingInToilet;

    /// <summary>
    /// 初始化角色状态组件。
    /// </summary>
    /// <param name="role">该状态所属的角色。</param>
    public void Initialize(BaseRole role)
    {
        ownerRole = role;
    }

    /// <summary>
    /// 增加便意值。
    /// </summary>
    /// <param name="value">本次增加的便意数值。</param>
    /// <param name="maxUrge">便意最大值。</param>
    public void IncreaseUrge(int value, int maxUrge)
    {
        if (isStayingInToilet || value <= 0)
        {
            return;
        }

        currentUrge = Mathf.Clamp(currentUrge + value, 0, maxUrge);
    }

    /// <summary>
    /// 对所属角色造成机制伤害。
    /// </summary>
    /// <param name="damage">造成的伤害数值。</param>
    public void ApplyDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        BaseRole role = GetOwnerRole();
        if (role == null)
        {
            return;
        }

        role.TakeDamage(damage);
    }

    /// <summary>
    /// 让角色进入厕所并在结束后清空便意。
    /// </summary>
    /// <param name="duration">强制停留时间。</param>
    public void BeginToiletStay(float duration)
    {
        BaseRole role = GetOwnerRole();
        if (role == null)
        {
            return;
        }

        if (toiletStayCoroutine != null)
        {
            StopCoroutine(toiletStayCoroutine);
        }

        toiletStayCoroutine = StartCoroutine(ToiletStayCoroutine(role, Mathf.Max(0.1f, duration)));
    }

    #region 内部流程

    /// <summary>
    /// 获取并缓存所属角色。
    /// </summary>
    /// <returns>当前物体上的角色组件。</returns>
    private BaseRole GetOwnerRole()
    {
        if (ownerRole == null)
        {
            ownerRole = GetComponent<BaseRole>();
        }

        return ownerRole;
    }

    private IEnumerator ToiletStayCoroutine(BaseRole role, float duration)
    {
        isStayingInToilet = true;
        role.SetSpeed(0, duration);

        yield return new WaitForSeconds(duration);

        currentUrge = 0;
        isStayingInToilet = false;
        toiletStayCoroutine = null;
    }

    private void OnDisable()
    {
        if (toiletStayCoroutine != null)
        {
            StopCoroutine(toiletStayCoroutine);
            toiletStayCoroutine = null;
        }

        isStayingInToilet = false;
    }

    #endregion
}

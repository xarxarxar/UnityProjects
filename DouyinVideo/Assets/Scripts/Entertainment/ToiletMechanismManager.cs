using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// “上厕所机制”的最小试点总控。
/// </summary>
[DisallowMultipleComponent]
public class ToiletMechanismManager : MonoBehaviour
{
    [Header("基础开关")]
    [SerializeField, Tooltip("是否启用上厕所娱乐机制，默认关闭。")]
    private bool enableToiletMechanism = false;

    [SerializeField, Tooltip("是否打印调试日志，默认关闭。")]
    private bool showDebugLog = false;

    [Header("便意规则")]
    [SerializeField, Tooltip("便意增长间隔。")]
    private float urgeIncreaseInterval = 2f;

    [SerializeField, Tooltip("每次增长的便意值。")]
    private int urgeIncreaseValue = 5;

    [SerializeField, Tooltip("便意最大值。")]
    private int maxUrge = 100;

    [Header("惩罚规则")]
    [SerializeField, Tooltip("便意达到最大值后的扣血间隔。")]
    private float damageInterval = 1f;

    [SerializeField, Tooltip("每次扣血数值。")]
    private int damagePerTick = 5;

    [SerializeField, Tooltip("便意满值扣血时是否播报。")]
    private bool broadcastDamageNews = false;

    [Header("厕所规则")]
    [SerializeField, Tooltip("进入厕所后的强制停留时间。")]
    private float stayDuration = 5f;

    [SerializeField, Tooltip("进入厕所时是否播报。")]
    private bool broadcastEnterNews = true;

    private readonly List<RoleToiletState> roleStates = new List<RoleToiletState>();
    private Coroutine urgeCoroutine;
    private Coroutine damageCoroutine;
    private WaitForSeconds urgeWait;
    private WaitForSeconds damageWait;

    private void Start()
    {
        if (!enableToiletMechanism)
        {
            return;
        }

        StartMechanism();
    }

    /// <summary>
    /// 尝试让角色进入厕所区域。
    /// </summary>
    /// <param name="role">进入厕所区域的角色。</param>
    public void TryEnterToilet(BaseRole role)
    {
        if (!CanRunMechanism() || role == null)
        {
            return;
        }

        RoleToiletState state = GetOrCreateState(role);
        if (state == null || state.IsStayingInToilet)
        {
            return;
        }

        state.BeginToiletStay(stayDuration);
        BroadcastNews($"{role.RoleName}冲进厕所", role.roleColor, broadcastEnterNews);
        LogDebug($"{role.RoleName}进入厕所，便意清零倒计时开始。");
    }

    /// <summary>
    /// 获取或创建角色的厕所机制状态。
    /// </summary>
    /// <param name="role">需要挂接状态的角色。</param>
    /// <returns>角色身上的厕所机制状态组件。</returns>
    public RoleToiletState GetOrCreateState(BaseRole role)
    {
        if (role == null)
        {
            return null;
        }

        RoleToiletState state = role.GetComponent<RoleToiletState>();
        if (state == null)
        {
            state = role.gameObject.AddComponent<RoleToiletState>();
        }

        state.Initialize(role);
        return state;
    }

    #region 内部流程

    private void StartMechanism()
    {
        urgeWait = new WaitForSeconds(Mathf.Max(0.1f, urgeIncreaseInterval));
        damageWait = new WaitForSeconds(Mathf.Max(0.1f, damageInterval));

        urgeCoroutine = StartCoroutine(UrgeLoop());
        damageCoroutine = StartCoroutine(DamageLoop());
    }

    private IEnumerator UrgeLoop()
    {
        yield return null;

        while (CanRunMechanism())
        {
            RefreshRoleStates();
            IncreaseUrgeForAllRoles();
            yield return urgeWait;
        }
    }

    private IEnumerator DamageLoop()
    {
        yield return null;

        while (CanRunMechanism())
        {
            RefreshRoleStates();
            DamageFullUrgeRoles();
            yield return damageWait;
        }
    }

    private void RefreshRoleStates()
    {
        roleStates.Clear();

        if (VideoGameManager.instance == null)
        {
            return;
        }

        List<BaseRole> roles = VideoGameManager.instance.circleRoles;
        for (int i = 0; i < roles.Count; i++)
        {
            RoleToiletState state = GetOrCreateState(roles[i]);
            if (state != null)
            {
                roleStates.Add(state);
            }
        }
    }

    private void IncreaseUrgeForAllRoles()
    {
        for (int i = 0; i < roleStates.Count; i++)
        {
            RoleToiletState state = roleStates[i];
            if (state == null || state.IsStayingInToilet)
            {
                continue;
            }

            state.IncreaseUrge(urgeIncreaseValue, maxUrge);
        }
    }

    private void DamageFullUrgeRoles()
    {
        for (int i = 0; i < roleStates.Count; i++)
        {
            RoleToiletState state = roleStates[i];
            if (state == null || state.IsStayingInToilet || state.CurrentUrge < maxUrge)
            {
                continue;
            }

            BaseRole role = state.GetComponent<BaseRole>();
            if (role == null)
            {
                continue;
            }

            BroadcastNews($"{role.RoleName}憋不住了，持续扣血", role.roleColor, broadcastDamageNews);
            state.ApplyDamage(damagePerTick);
        }
    }

    private bool CanRunMechanism()
    {
        return enableToiletMechanism
            && VideoGameManager.instance != null
            && !VideoGameManager.instance.gameEnd;
    }

    private void BroadcastNews(string news, Color32 color, bool shouldBroadcast)
    {
        if (!shouldBroadcast || Broadcast.instance == null)
        {
            return;
        }

        Broadcast.instance.BroadCastNews(news, color);
    }

    private void LogDebug(string message)
    {
        if (!showDebugLog)
        {
            return;
        }

        Debug.Log(message);
    }

    private void StopMechanismCoroutines()
    {
        if (urgeCoroutine != null)
        {
            StopCoroutine(urgeCoroutine);
            urgeCoroutine = null;
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private void OnDisable()
    {
        StopMechanismCoroutines();
        roleStates.Clear();
    }

    private void OnValidate()
    {
        urgeIncreaseInterval = Mathf.Max(0.1f, urgeIncreaseInterval);
        urgeIncreaseValue = Mathf.Max(1, urgeIncreaseValue);
        maxUrge = Mathf.Max(1, maxUrge);
        damageInterval = Mathf.Max(0.1f, damageInterval);
        damagePerTick = Mathf.Max(0, damagePerTick);
        stayDuration = Mathf.Max(0.1f, stayDuration);
    }

    #endregion
}

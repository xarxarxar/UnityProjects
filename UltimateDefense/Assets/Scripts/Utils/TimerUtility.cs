using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 延迟执行回调的工具类
/// </summary>
public class TimerUtility : MonoBehaviour
{
    #region 私有属性
    private static TimerUtility _instance;  // 单例实例
    #endregion

    #region 公开属性
    /// <summary>
    /// 单例公开属性
    /// </summary>
    public static TimerUtility Instance { get; private set; }
    #endregion

    #region public 成员方法
    /// <summary>
    /// 延迟 delay 秒后调用 callback 方法
    /// </summary>
    /// <param name="delay">延迟秒数</param>
    /// <param name="callback">回调方法</param>
    public void Timer(float delay, Action callback)
    {
        StartCoroutine(TimerCoroutine(delay, callback));
    }

    /// <summary>
    /// 重复执行某个动作，会跟随 owner 生命周期停止
    /// </summary>
    /// <param name="owner">执行协程的 MonoBehaviour</param>
    /// <param name="interval">每次执行间隔（秒）</param>
    /// <param name="action">要执行的动作</param>
    /// <param name="condition">继续执行的条件，返回 true 继续，false 停止</param>
    /// <param name="delay">延迟执行时间（秒）</param>
    public static void RepeatDoing(MonoBehaviour owner, float interval, UnityAction action, Func<bool> condition = null, float delay = 0f)
    {
        owner.StartCoroutine(RepeatDoingCoro(owner, interval, action, condition, delay));
    }

    private static IEnumerator RepeatDoingCoro(MonoBehaviour owner, float interval, UnityAction action, Func<bool> condition = null, float delay = 0f)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        while (owner != null && owner.gameObject.activeInHierarchy && (condition == null || condition()))
        {
            action?.Invoke();

            float timer = 0f;
            while (timer < interval)
            {
                // 如果 owner 被销毁或不活跃，直接退出
                if (owner == null || !owner.gameObject.activeInHierarchy)
                    yield break;

                // 可选：考虑游戏倍速
                timer += Time.deltaTime * (BattleManager.Instance?.GameSpeed.Value ?? 1f);

                yield return null;
            }
        }
    }

    #endregion

    #region 私有成员方法
    /// <summary>
    /// 单例初始化：如果 Instance == null，则 Instance = this; DontDestroyOnLoad(gameObject); 否则 Destroy(gameObject).
    /// </summary>
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); return; }
    }

    /// <summary>
    /// 定时协程，实现延迟调用 callback
    /// </summary>
    /// <param name="delay">延迟秒数</param>
    /// <param name="callback">回调方法</param>
    private IEnumerator TimerCoroutine(float delay, Action callback)
    {
        yield return WaitForGameSeconds(delay / BattleManager.Instance.GameSpeed.Value);
        callback?.Invoke();
        yield break;
    }


    /// <summary>
    /// 受GameSpeed影响的时间
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IEnumerator WaitForGameSeconds(float time)
    {
        float timer = 0f;

        while (timer < time)
        {
            if (!BattleManager.Instance.IsPaused.Value)
            {
                timer += Time.deltaTime * BattleManager.Instance.GameSpeed.Value;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Unity OnDestroy 回调，可 StopAllCoroutines 清理所有定时
    /// </summary>
    private void OnDestroy()
    {
        // StopAllCoroutines();
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("时间设置")]
    [SerializeField] private float _baseTimeScale = 1f;
    private float _currentSpeedMultiplier = 1f;
    private bool _isPaused = false;

    /// <summary>
    /// 获取当前帧的增量时间（已应用时间缩放）
    /// </summary>
    public float DeltaTime => Time.fixedDeltaTime * EffectiveTimeScale;

    /// <summary>
    /// 获取有效时间缩放（考虑暂停状态）
    /// </summary>
    public float EffectiveTimeScale => _isPaused ? 0 : _baseTimeScale * _currentSpeedMultiplier;

    /// <summary>
    /// 获取经过缩放的游戏时间（考虑暂停状态）
    /// </summary>
    public float EffectiveTime => Time.time * EffectiveTimeScale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 设置游戏速度倍率
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        _currentSpeedMultiplier = Mathf.Max(multiplier, 0);
        UpdateTimeScale();
    }

    /// <summary>
    /// 切换暂停状态
    /// </summary>
    public void TogglePause()
    {
        _isPaused = !_isPaused;
        UpdateTimeScale();
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = EffectiveTimeScale;
        Time.fixedDeltaTime = 0.02f * EffectiveTimeScale;
    }
}

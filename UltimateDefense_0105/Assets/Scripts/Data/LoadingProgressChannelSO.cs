using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 进入游戏时，加载的数据
/// </summary>
[CreateAssetMenu(menuName = "Event/LoadingProgressChannel")]
public class LoadingProgressChannelSO: ScriptableObject
{
    public event Action<float, string> OnProgressChanged;
    public event Action<bool> OnVisibilityChanged;

    private float lastProgress = 0f;
    private string lastMessage = "正在加载...";

    /// <summary>
    /// 触发进度事件
    /// </summary>
    /// <param name="progress">新的进度（0~100）。不填则保持原有进度。</param>
    /// <param name="message">要显示的消息。不填则保持原有消息。</param>
    /// <param name="addProgress">是否为“在原基础上增加”。</param>
    public void Raise(float? progress = null, string message = null, 
        bool addProgress = false)
    {
        OnVisibilityChanged?.Invoke(true);
        if (progress.HasValue)
        {
            if (addProgress)
                lastProgress += progress.Value;
            else
                lastProgress = progress.Value;

            // 限制范围在 0~100
            lastProgress = Mathf.Clamp(lastProgress, 0f, 100f);
        }

        if (message != null)
            lastMessage = message;

        OnProgressChanged?.Invoke(lastProgress, lastMessage);
    }

    /// <summary>
    /// 控制进度条隐藏
    /// </summary>
    /// <param name="visible"></param>
    public void SetInvisible()
    {
        OnVisibilityChanged?.Invoke(false);
    }
}

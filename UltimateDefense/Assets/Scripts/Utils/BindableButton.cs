using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class BindableButton : MonoBehaviour
{
    private Button button;

    public event Action onClick;

    [Header("点击音效 (可选)")]
    public AudioClip audioClip; // 点击音效

    public bool playSound=true;//是否播放默认点击音效

    //默认点击音效 (不修改则使用这个)
    private AudioClip defaultAudioClip=>AudioManager.Instance.GetAudioClip("按钮点击");

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // 使用外部指定的音效，否则用默认音效
        AudioClip clipToPlay = audioClip != null && playSound ? audioClip : defaultAudioClip;

        if (clipToPlay != null)
        {
            AudioManager.Instance.PlaySFX(clipToPlay);
        }

        // 触发事件
        onClick?.Invoke();
    }

    /// <summary>
    /// 添加点击事件监听
    /// </summary>
    public void AddListener(Action action)
    {
        onClick += action;
    }

    /// <summary>
    /// 移除点击事件监听
    /// </summary>
    public void RemoveListener(Action action)
    {
        onClick -= action;
    }

    /// <summary>
    /// 移除点击事件监听
    /// </summary>
    public void RemoveAllListeners()
    {
        onClick = null;
    }

    /// <summary>
    /// 当 RectTransform 被移除时（组件销毁时清理回调）
    /// </summary>
    public void OnRectTransformRemoved()
    {
        // 移除所有事件和监听，避免内存泄漏
        button.onClick.RemoveListener(OnButtonClicked);
        onClick = null;
    }
}

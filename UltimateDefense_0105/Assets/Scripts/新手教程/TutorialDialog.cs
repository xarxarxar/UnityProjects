using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 引入 DOTween 命名空间
using System;
using UnityEngine.Events; // 引入 System 命名空间以使用 Action

/// <summary>
/// 对话框的位置类型
/// </summary>
public enum DialogPositionType
{
    Bottom, // 屏幕下方
    Middle  // 屏幕中间
}

/// <summary>
/// 指引对话
/// </summary>
public class TutorialDialog : MonoBehaviour
{
    // --- 外部配置 ---
    [Header("对话框 UI 引用")]
    [Tooltip("场景中已存在的对话框 GameObject。它必须包含一个 RectTransform。")]
    public GameObject dialogBox; // 对话框 GameObject 引用

    [Header("位置配置")]
    [Tooltip("对话框位于下方时的 Y 偏移量。")]
    public float bottomYOffset = 100f; // 距离底部的偏移
    [Tooltip("对话框位于中间时的 Y 偏移量。")]
    public float middleYOffset = 0f; // 位于屏幕中心

    [Header("打字机效果配置")]
    [Tooltip("每个字符打出的时间间隔 (秒)。")]
    public float typingSpeed = 0.05f;

    // --- 内部引用 (缓存) ---
    private RectTransform dialogRect;
    public Text dialogText;

    [SerializeField]private GameObject tipText;//提示点击进行下一步的文本
    [SerializeField]private Button tipButton;//提示点击进行下一步的按钮

    // --- 初始化 ---
    void Awake()
    {
        if (dialogBox == null)
        {
            Debug.LogError("DialogBox 引用未设置。请在 Inspector 中拖入场景中的对话框 UI 对象。", this);
            enabled = false;
            return;
        }

        // 缓存 RectTransform
        dialogRect = dialogBox.GetComponent<RectTransform>();
        if (dialogRect == null)
        {
            Debug.LogError("对话框 UI 对象缺少 RectTransform 组件。", this);
            enabled = false;
            return;
        }

        // 初始化时隐藏对话框
        HideDialog();
    }


    /// <summary>
    /// 功能 1 & 2: 显示对话框、设置文本和位置，并以打字机效果显示文本。
    /// </summary>
    /// <param name="message">要显示的文本内容。</param>
    /// <param name="position">对话框显示的位置（Bottom 或 Middle）。</param>
    /// <param name="onComplete">打字机效果完成后的回调函数。</param>
    public void ShowDialog(string message, DialogPositionType position, UnityAction onComplete = null)
    {
        if (dialogBox == null || dialogRect == null || dialogText == null)
        {
            Debug.LogError("对话框组件未正确初始化，无法显示对话。", this);
            return;
        }

        // 停止任何正在运行的文本动画，以防重复调用
        dialogText.DOKill();

        // 2. 设置位置 (先设置位置确保对话框出现在正确位置)
        Vector2 targetPosition = Vector2.zero;

        if (position == DialogPositionType.Bottom)
        {
            // 锚点设置：底部并保持宽度拉伸 (X: 0到1, Y: 0到0)
            dialogRect.anchorMin = new Vector2(0f, 0f);
            dialogRect.anchorMax = new Vector2(1f, 0f);
            // 位置设置：从底部向上偏移 (X=0 保持水平居中)
            targetPosition = new Vector2(0f, bottomYOffset);
        }
        else if (position == DialogPositionType.Middle)
        {
            // 锚点设置：居中并保持宽度拉伸 (X: 0到1, Y: 0.5到0.5)
            dialogRect.anchorMin = new Vector2(0f, 0.5f);
            dialogRect.anchorMax = new Vector2(1f, 0.5f);
            // 位置设置：位于中心，可微调 Y 偏移 (X=0 保持水平居中)
            targetPosition = new Vector2(0f, middleYOffset);
        }

        // 应用最终位置
        // 注意：当锚点为水平拉伸时，anchoredPosition.x 应该为 0 才能保持居中。
        dialogRect.anchoredPosition = targetPosition;
        dialogBox.SetActive(true); // 显示对话框

        // 1. 使用 DOTween 实现打字机效果
        dialogText.text = ""; // 清空文本，准备打字机效果

        float duration = message.Length * typingSpeed; // 计算总持续时间

        HideTipText();
        dialogText.DOText(message, duration)
            .SetEase(Ease.Linear) // 打字机通常使用线性缓动
            .OnComplete(() =>
            {
                ShowTipText(onComplete);
            });
    }

    /// <summary>
    /// 功能 3: 隐藏并关闭当前对话框。
    /// </summary>
    public void HideDialog()
    {
        if (dialogBox != null)
        {
            // 停止任何正在进行的文本动画，防止在隐藏后继续修改 Text 组件
            dialogText?.DOKill();
            dialogBox.SetActive(false);
        }
    }

    /// <summary>
    /// 显示进行下一步的Text
    /// </summary>
    public void ShowTipText(UnityAction callback)
    {
        tipText.SetActive(true);
        tipButton.interactable = true;
        tipButton.onClick.RemoveAllListeners();
        tipButton.onClick.AddListener(callback);
    }

    /// <summary>
    /// 隐藏进行下一步的Text
    /// </summary>
    private void HideTipText()
    {
        tipText.SetActive(false);
        tipButton.interactable=false;
        tipButton.onClick.RemoveAllListeners();
    }
}
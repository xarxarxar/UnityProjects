using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 动态文字效果
/// </summary>
public class DynamicText : MonoBehaviour
{
    public static DynamicText instance;
    public Text dynamicText;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 文字从1.5倍缩放到1倍，停留指定秒数，支持设置文字颜色，返回完整 Sequence。
    /// </summary>
    /// <param name="text">要显示的文字</param>
    /// <param name="duration">缩放持续时间</param>
    /// <param name="waitSeconds">缩放后等待的秒数</param>
    /// <param name="color">（可选）文字颜色</param>
    /// <returns>完整 DOTween Sequence</returns>
    public Sequence ScaleTextToNormal(string text, float duration = 1.0f, float waitSeconds = 1.0f, Color? color = null)
    {
        dynamicText.text = text;

        // 如果传入了颜色，则设置
        if (color.HasValue)
        {
            dynamicText.color = color.Value;
        }

        // 设置初始缩放
        dynamicText.transform.localScale = Vector3.one * 1.5f;

        // 创建序列
        Sequence sequence = DOTween.Sequence();

        // 缩放动画
        sequence.Append(dynamicText.transform.DOScale(1f, duration));

        // 停留时间
        sequence.AppendInterval(waitSeconds);

        return sequence;
    }

}

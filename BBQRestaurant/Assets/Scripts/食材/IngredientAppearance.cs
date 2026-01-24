using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对食材的外观进行控制
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class IngredientAppearance : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;       // 原始颜色
    private Color cookedColor;         // 熟了的颜色
    private Color burnedColor;         // 烧焦颜色

    private float originalAlpha = 1f;  // 原始透明度

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;
        originalAlpha = sr.color.a;

        // 可调，这里只是示例
        cookedColor = new Color(1f, 0.85f, 0.5f, 1f); // 金黄色
        burnedColor = new Color(0.2f, 0.2f, 0.2f, 1f); // 深灰接近烧焦
    }

    /// <summary>
    /// 恢复成未改变的样子
    /// </summary>
    public void ResetAppearance()
    {
        sr.color = originalColor;
    }

    /// <summary>
    /// 设置透明度（用于虚影）
    /// </summary>
    public void SetAlpha(float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }

    /// <summary>
    /// 设置为“已熟”效果
    /// </summary>
    public void SetCooked()
    {
        sr.color = cookedColor;
    }

    /// <summary>
    /// 设置为“烧焦”效果
    /// </summary>
    public void SetBurned()
    {
        sr.color = burnedColor;
    }
}

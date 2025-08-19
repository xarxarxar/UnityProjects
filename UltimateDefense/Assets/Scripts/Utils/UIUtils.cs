using DG.Tweening;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class UIUtils : MonoBehaviour
{
    /// <summary>
    /// 播放数字跳动动画（只支持 UnityEngine.UI.Text）
    /// </summary>
    /// <param name="text">目标 Text</param>
    /// <param name="endValue">结束值</param>
    /// <param name="duration">动画时长（秒）</param>
    /// <param name="onComplete">动画完成回调</param>
    public static void PlayNumberAnimation(Text text, int endValue, float duration, UnityAction onComplete = null)
    {
        if (text == null) return;

        // 确保之前在这个对象上的 tween 都被干掉
        DOTween.Kill(text);

        // 当前数值（如果解析失败则默认为 0）
        int startValue = 0;
        int.TryParse(text.text, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out startValue);

        int currentValue = startValue;

        // 创建补间动画
        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            text.text = currentValue.ToString("N0"); // 带千位分隔符
        }, endValue, duration).SetTarget(text).OnComplete(() =>
        {
            // 确保最终数值正确
            text.text = endValue.ToString("N0");
            onComplete?.Invoke();
        });
    }
}

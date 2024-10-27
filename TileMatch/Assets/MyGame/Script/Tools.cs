using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Tools : MonoBehaviour
{
    // 函数参数：
    // targetObject：要复制并移动的UI物体
    // duration：移动和缩小的持续时间
    // callback：销毁后调用的回调函数
    public static void MoveAndShrinkUI(GameObject targetObject, float duration, System.Action callback)
    {
        // 复制UI物体
        GameObject newObject = Instantiate(targetObject, targetObject.transform.position, Quaternion.identity, targetObject.transform.parent);

        // 获取新物体的RectTransform
        RectTransform rectTransform = newObject.GetComponent<RectTransform>();

        // 获取当前的PosX
        float currentPosX = rectTransform.anchoredPosition.x;
        float targetPosX = -540f;  // 目标PosX

        // 设置目标缩放（缩小到0）
        Vector3 targetScale = Vector3.zero;

        // 创建 DOTween 动画序列
        Sequence sequence = DOTween.Sequence();

        // 修改PosX
        sequence.Append(DOTween.To(() => currentPosX, x => rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y), targetPosX, duration));

        // 同时缩小 `localScale`
        sequence.Join(rectTransform.DOScale(targetScale, duration));

        // 动画完成后销毁物体并调用回调函数
        sequence.OnComplete(() =>
        {
            Destroy(newObject);
            callback?.Invoke();  // 如果回调函数不为空，调用回调
        });
    }



    // 函数参数：
    // textComponent：要闪烁的TextMeshProUGUI组件
    // duration：闪烁的总持续时间
    public static void BlinkRedThreeTimes(TMP_Text textComponent, float duration)
    {
        // 将初始颜色设置为白色
        Color originalColor = Color.white;

        // 定义闪烁的颜色为红色
        Color redColor = Color.red;

        // 每次闪烁的时间（总时间的三分之一）
        float blinkTime = duration / 6f;  // 每个颜色变化分为红色 -> 原始颜色，所以总时长除以6

        // 创建一个序列
        Sequence sequence = DOTween.Sequence();

        // 添加闪红3次的动画
        for (int i = 0; i < 3; i++)
        {
            // 闪红
            sequence.Append(textComponent.DOColor(redColor, blinkTime));
            // 变回原始颜色
            sequence.Append(textComponent.DOColor(originalColor, blinkTime));
        }

        // 动画结束后，确保颜色恢复为原始颜色
        sequence.OnComplete(() => {
            textComponent.color = originalColor;
        });
    }

}

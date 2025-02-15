using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tip : MonoBehaviour
{
    private RectTransform uiElement;  // 你的UI元素（通常是RectTransform）
    private Vector3 targetPosition;  // 最终目标位置
    public float duration = 2f;     // 动画时长
    public string showString;//显示的文字
    public Text showText;//显示的文字

    void OnEnable()
    {
        uiElement = GetComponent<RectTransform>();
        targetPosition = uiElement.position+new Vector3(0,100,0);
        // 调用方法，开始动画
        AnimateUI();
    }

    void AnimateUI()
    {
        // 动画组合，UI元素向下移动、变小、变透明
        uiElement.DOMove(targetPosition, duration)        // 向目标位置移动
            .SetEase(Ease.OutQuad)                  // 设置缓动效果
            .OnStart(() => {
                showText.text = showString;
                uiElement.localScale =0.5f* Vector3.one;  // 确保开始时 UI 为正常大小
                
            })
            .OnComplete(() => {
                // 动画完成后可以执行的操作（例如销毁UI等）
                Destroy(gameObject); // 示例：销毁UI元素
            });

        uiElement.DOScale(1.0f, duration/2)                   // 变小到 0
           .SetEase(Ease.OutQuad);                  // 设置缓动效果
    }
}

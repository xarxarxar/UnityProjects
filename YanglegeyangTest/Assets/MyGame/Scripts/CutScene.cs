using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class CutScene : MonoBehaviour
{

    private void OnEnable()
    {
        // 获取屏幕的宽度
        float screenHeight = Screen.height;
        RectTransform rectTransform = GetComponent<RectTransform>();
        // 将图片的 RectTransform 移动到屏幕左边外部的位置
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);

        // 开始移动动画
        MoveImageAcrossScreen(screenHeight, rectTransform);
    }
    void MoveImageAcrossScreen(float screenHeight, RectTransform imageRectTransform)
    {
        // 计算目标位置（屏幕右侧外部的位置）
        float targetY = screenHeight;

        // 创建一个 Sequence
        Sequence sequence = DOTween.Sequence();

        // 添加一个等待的步骤
        sequence.AppendInterval(0.5f);

        // 添加移动的动画
        sequence.Append(// 使用 DOTween 从左侧移动到右侧
        imageRectTransform.DOAnchorPosY(targetY, 1.0f).SetEase(Ease.InSine)
            .OnComplete(() => { gameObject.SetActive(false); }));  // 一定时间内完成动画，使用Ease.InOutQuad缓动);

        // 启动动画序列
        sequence.Play();
    }



}

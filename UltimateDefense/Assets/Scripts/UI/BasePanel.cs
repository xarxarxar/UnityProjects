using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// panel的基础父类
/// </summary>
public class BasePanel : MonoBehaviour
{
    public Button CloseButton;//面板的关闭按钮

    public virtual void OnEnable()
    {
        // 先移除旧的监听，避免重复绑定
        CloseButton.onClick.RemoveListener(OnCloseButton);
        CloseButton.onClick.AddListener(OnCloseButton);

        // 初始缩放设为 0
        transform.localScale = Vector3.zero;

        // 动画缩放到 1（0.5 秒，使用 Ease.OutBack 有弹性感）
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }

    private void OnCloseButton()
    {
        // 动画缩放到 0，然后关闭面板
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }
}

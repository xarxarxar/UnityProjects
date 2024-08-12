using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // 初始缩放为 0
        rectTransform.localScale = Vector3.zero;
        // 使用 DOTween 执行放大动画
        rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine);
    }

    public void CloseButton()//关闭按钮
    {
        gameObject.SetActive(false);
    }

    

}

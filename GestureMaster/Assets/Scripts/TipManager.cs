using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipManager : MonoBehaviour
{
    public static TipManager instance;
    public GameObject tipPanel;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="tip"></param>
    public void ShowTip(string tip)
    {
        tipPanel.transform.Find("提示面板").Find("提示Text").GetComponent<Text>().text= tip;
        tipPanel.SetActive(true);
        PlayShowAndHide(tipPanel.transform.Find("提示面板").gameObject);
    }

    // 传入你想动画的 UI 对象
    private void PlayShowAndHide(GameObject uiObject, float moveDistance = 200f, float duration = 0.5f, float delay = 0.5f)
    {
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("该 UI 对象没有 RectTransform！");
            return;
        }

        // 初始位置
        Vector3 startPos = rectTransform.anchoredPosition;

        // 播放序列动画
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPosY(startPos.y + moveDistance, duration).SetEase(Ease.OutCubic))
           .AppendInterval(delay)
           .OnComplete(() => {
               uiObject.transform.parent.gameObject.SetActive(false);
               rectTransform.anchoredPosition = startPos; // 恢复初始位置
           });
    }
}


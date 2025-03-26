using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideFigner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveDistance = 100f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutSine;

    [SerializeField] private RectTransform targetRect;
    private Tween floatTween;

    private void OnEnable()
    {
        StartFloating();
    }

    private void OnDisable()
    {
        StopFloating();
    }


    void StartFloating()
    {
        floatTween = targetRect.DOLocalMoveY(moveDistance, duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative() // 相对当前位置移动
            .OnKill(() => floatTween = null); // 清理引用
    }

    void StopFloating()
    {
        // 立即停止并重置位置
        floatTween?.Kill(true);
        targetRect.localPosition = Vector3.zero;
    }

}

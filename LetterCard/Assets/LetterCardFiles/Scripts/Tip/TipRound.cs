using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipRound : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f; // 每秒旋转角度

    [SerializeField] private RectTransform _target;
    private Tween _rotationTween;


    private void OnEnable()
    {
        StartRotation();
    }

    private void OnDisable()
    {
        StopRotation();
    }

    void StartRotation()
    {
        // 计算单圈所需时间
        float duration = 360f / Mathf.Abs(rotationSpeed);
        // 确定旋转方向
        float direction = -1;

        _rotationTween = _target.DOLocalRotate(
                new Vector3(0, 0, 360f * direction),
                duration,
                RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .OnKill(() => _rotationTween = null);
    }

    void StopRotation()
    {
        _rotationTween?.Kill();
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public Image targetImage; // 在 Inspector 中绑定要旋转的 Image 组件
    private Tween rotationTween; // 用来控制旋转的 DoTween 动画

    private void OnEnable()
    {
        StartRotation();
    }

    // 开始旋转图片
    public void StartRotation(float duration = 2.5f)
    {
        if (rotationTween != null && rotationTween.IsPlaying())
            return; // 如果动画已经在播放，则不再重新开始

        rotationTween = targetImage.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear) // 线性缓动效果，使旋转匀速
            .SetLoops(-1, LoopType.Restart); // 无限循环旋转

        // 让图片在y轴方向上进行一个正弦波动的循环动画
        //rotationTween= targetImage.rectTransform.DOLocalMoveY(50, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}

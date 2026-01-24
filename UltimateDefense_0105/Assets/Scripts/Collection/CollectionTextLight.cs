using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionTextLight : MonoBehaviour
{
    public Text txt;//文字
    public Color32 color;
    public GameObject BackgroundLight;
    private Tween rotateTween;

    private void OnEnable()
    {
        // 背景光无限旋转
        if (BackgroundLight != null)
        {
            BackgroundLight.transform.localRotation = Quaternion.identity;

            // 先停止旧动画
            rotateTween?.Kill();

            // 创建新的无限旋转动画
            rotateTween = BackgroundLight.transform
                .DORotate(new Vector3(0, 0, -360), 4f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }
    }

    //设置该文字的文本和颜色
    public void SetCollectionText(char c, Color32 color)
    {
        txt.text = c.ToString();
        txt.color = color;
    }
}

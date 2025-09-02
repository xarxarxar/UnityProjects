using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public List<Image> images;              // 要飞行的 Image 列表
    [SerializeField] private Image target;
    [SerializeField] private Sprite newTargetSprite; // 飞行结束后切换的 sprite
    public float duration = 0.6f;           // 飞行时长
    public Vector2 horizontalOffsetRange = new Vector2(-100f, 100f);
    public Vector2 verticalOffsetRange = new Vector2(100f, 200f);

    private int completedCount = 0;

    private void Start()
    {
        StartFly();
    }

    public void StartFly()
    {
        completedCount = 0;

        // target 初始透明
        Color c = target.color;
        target.color = new Color(c.r, c.g, c.b, 0f);

        // target 在整个动画时长内渐显
        target.DOFade(1f, duration).SetEase(Ease.Linear);

        // 启动每个 Image 的飞行动画
        foreach (var img in images)
        {
            FlyImage(img);
        }
    }

    private void FlyImage(Image img)
    {
        RectTransform rect = img.rectTransform;

        Vector3 screenStart = rect.position;
        Vector3 screenEnd = target.transform.position;

        // 计算中间控制点（弧线弯曲点）
        Vector3 midPoint = (screenStart + screenEnd) / 2f;

        // 添加随机偏移，使每个轨迹略有不同
        float horizontalOffset = Random.Range(horizontalOffsetRange.x, horizontalOffsetRange.y);
        float verticalOffset = Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);
        midPoint += new Vector3(horizontalOffset, verticalOffset, 0f);

        // 设置路径（三点曲线）
        Vector3[] path = new Vector3[] { screenStart, midPoint, screenEnd };

        // 并行动画：路径移动 + 渐隐
        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad));

        //seq.Join(img.DOFade(0f, duration * 0.8f)); // 渐隐，稍早消失

        seq.OnComplete(() =>
        {
            //Destroy(img.gameObject); // 或者回收对象池
            img.DOFade(0f, 0.1f);
            completedCount++;

            // 等所有 images 完成
            if (completedCount == images.Count)
            {
                PlayTargetAnimation();
            }
        });
    }

    private void PlayTargetAnimation()
    {
        // 先把 rotation 归零，避免累计误差
        target.rectTransform.localRotation = Quaternion.identity;

        // 创建序列
        Sequence seq = DOTween.Sequence();

        // 摇三次（每次左右一次）
        for (int i = 0; i < 5; i++)
        {
            seq.Append(target.rectTransform.DORotate(new Vector3(0, 0, 20f), 0.1f));
            seq.Append(target.rectTransform.DORotate(new Vector3(0, 0, -20f), 0.1f));
        }

        // 最后归位并切换 sprite
        seq.AppendCallback(() =>
        {
            target.rectTransform.localRotation = Quaternion.identity;
            if (newTargetSprite != null)
                target.sprite = newTargetSprite;
        });
    }
}

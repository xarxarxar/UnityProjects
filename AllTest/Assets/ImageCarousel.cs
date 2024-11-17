using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 添加 DOTween 的命名空间

public class ImageCarousel : MonoBehaviour
{
    [SerializeField] private Image[] images; // 将需要轮播的所有图片拖拽到这个数组中
    [SerializeField] private float transitionDuration = 1f; // 图片切换过渡时间
    [SerializeField] private bool loop = true; // 是否循环播放

    private int currentImageIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        if (images.Length > 0)
        {
            ShowCurrentImage();
            StartCoroutine(StartTransition());
            InvokeRepeating("NextImage", transitionDuration, transitionDuration);
        }
    }

    IEnumerator StartTransition()
    {
        while (true)
        {
            if (!isTransitioning && images.Length > 1)
            {
                isTransitioning = true;

                // 当前图片淡出
                images[currentImageIndex].DOFade(0f, transitionDuration).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    images[currentImageIndex].gameObject.SetActive(false); // 隐藏当前图片
                    currentImageIndex = (currentImageIndex + 1) % images.Length; // 更新索引，循环播放

                    // 下一张图片淡入
                    images[currentImageIndex].gameObject.SetActive(true);
                    images[currentImageIndex].DOFade(1f, transitionDuration).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        isTransitioning = false;
                    });
                });
            }
            yield return null;
        }
    }

    void NextImage()
    {
        if (!loop && currentImageIndex == images.Length - 1) return; // 如果不循环且已经是最后一张图片，则停止

        StopAllCoroutines(); // 停止当前所有的协程以防止叠加
        StartCoroutine(StartTransition()); // 开始新的过渡动画
    }

    void ShowCurrentImage()
    {
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0); // 设置初始透明度为 0
        }
        images[currentImageIndex].gameObject.SetActive(true);
        images[currentImageIndex].color = new Color(images[currentImageIndex].color.r, images[currentImageIndex].color.g, images[currentImageIndex].color.b, 1); // 设置当前图片透明度为 1
    }
}

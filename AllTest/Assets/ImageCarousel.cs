using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // ��� DOTween �������ռ�

public class ImageCarousel : MonoBehaviour
{
    [SerializeField] private Image[] images; // ����Ҫ�ֲ�������ͼƬ��ק�����������
    [SerializeField] private float transitionDuration = 1f; // ͼƬ�л�����ʱ��
    [SerializeField] private bool loop = true; // �Ƿ�ѭ������

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

                // ��ǰͼƬ����
                images[currentImageIndex].DOFade(0f, transitionDuration).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    images[currentImageIndex].gameObject.SetActive(false); // ���ص�ǰͼƬ
                    currentImageIndex = (currentImageIndex + 1) % images.Length; // ����������ѭ������

                    // ��һ��ͼƬ����
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
        if (!loop && currentImageIndex == images.Length - 1) return; // �����ѭ�����Ѿ������һ��ͼƬ����ֹͣ

        StopAllCoroutines(); // ֹͣ��ǰ���е�Э���Է�ֹ����
        StartCoroutine(StartTransition()); // ��ʼ�µĹ��ɶ���
    }

    void ShowCurrentImage()
    {
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0); // ���ó�ʼ͸����Ϊ 0
        }
        images[currentImageIndex].gameObject.SetActive(true);
        images[currentImageIndex].color = new Color(images[currentImageIndex].color.r, images[currentImageIndex].color.g, images[currentImageIndex].color.b, 1); // ���õ�ǰͼƬ͸����Ϊ 1
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class CutScene : MonoBehaviour
{

    private void OnEnable()
    {
        // ��ȡ��Ļ�Ŀ��
        float screenHeight = Screen.height;
        RectTransform rectTransform = GetComponent<RectTransform>();
        // ��ͼƬ�� RectTransform �ƶ�����Ļ����ⲿ��λ��
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);

        // ��ʼ�ƶ�����
        MoveImageAcrossScreen(screenHeight, rectTransform);
    }
    void MoveImageAcrossScreen(float screenHeight, RectTransform imageRectTransform)
    {
        // ����Ŀ��λ�ã���Ļ�Ҳ��ⲿ��λ�ã�
        float targetY = screenHeight;

        // ����һ�� Sequence
        Sequence sequence = DOTween.Sequence();

        // ���һ���ȴ��Ĳ���
        sequence.AppendInterval(0.5f);

        // ����ƶ��Ķ���
        sequence.Append(// ʹ�� DOTween ������ƶ����Ҳ�
        imageRectTransform.DOAnchorPosY(targetY, 1.0f).SetEase(Ease.InSine)
            .OnComplete(() => { gameObject.SetActive(false); }));  // һ��ʱ������ɶ�����ʹ��Ease.InOutQuad����);

        // ������������
        sequence.Play();
    }



}

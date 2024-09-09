using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public Image targetImage; // �� Inspector �а�Ҫ��ת�� Image ���
    private Tween rotationTween; // ����������ת�� DoTween ����

    private void OnEnable()
    {
        StartRotation();
    }

    // ��ʼ��תͼƬ
    public void StartRotation(float duration = 2.5f)
    {
        if (rotationTween != null && rotationTween.IsPlaying())
            return; // ��������Ѿ��ڲ��ţ��������¿�ʼ

        rotationTween = targetImage.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear) // ���Ի���Ч����ʹ��ת����
            .SetLoops(-1, LoopType.Restart); // ����ѭ����ת

        // ��ͼƬ��y�᷽���Ͻ���һ�����Ҳ�����ѭ������
        //rotationTween= targetImage.rectTransform.DOLocalMoveY(50, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Tools : MonoBehaviour
{
    // ����������
    // targetObject��Ҫ���Ʋ��ƶ���UI����
    // duration���ƶ�����С�ĳ���ʱ��
    // callback�����ٺ���õĻص�����
    public static void MoveAndShrinkUI(GameObject targetObject, float duration, System.Action callback)
    {
        // ����UI����
        GameObject newObject = Instantiate(targetObject, targetObject.transform.position, Quaternion.identity, targetObject.transform.parent);

        // ����������ĸ�������ԭ����һ��
        //newObject.transform.SetParent(targetObject.transform.parent);

        // ��ȡ�������RectTransform
        RectTransform rectTransform = newObject.GetComponent<RectTransform>();

        // ��ȡ��ǰ��PosX
        float currentPosX = rectTransform.anchoredPosition.x;
        float targetPosX = -540f;  // Ŀ��PosX

        // ����Ŀ���Ⱥ͸߶ȣ���С��0��
        Vector2 targetSize = Vector2.zero;

        // ��ȡ��ǰ�Ŀ�Ⱥ͸߶�
        Vector2 currentSize = rectTransform.sizeDelta;

        // ���� DOTween ����
        Sequence sequence = DOTween.Sequence();

        // �޸�PosX
        sequence.Append(DOTween.To(() => currentPosX, x => rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y), targetPosX, duration));

        // ͬʱ��С��Ⱥ͸߶�
        sequence.Join(DOTween.To(() => currentSize, size => rectTransform.sizeDelta = size, targetSize, duration));

        // ������ɺ��������岢���ûص�����
        sequence.OnComplete(() =>
        {
            Destroy(newObject);
            callback?.Invoke();  // ����ص�������Ϊ�գ����ûص�
        });
    }


    // ����������
    // textComponent��Ҫ��˸��TextMeshProUGUI���
    // duration����˸���ܳ���ʱ��
    public static void BlinkRedThreeTimes(TMP_Text textComponent, float duration)
    {
        // ����ʼ��ɫ����Ϊ��ɫ
        Color originalColor = Color.white;

        // ������˸����ɫΪ��ɫ
        Color redColor = Color.red;

        // ÿ����˸��ʱ�䣨��ʱ�������֮һ��
        float blinkTime = duration / 6f;  // ÿ����ɫ�仯��Ϊ��ɫ -> ԭʼ��ɫ��������ʱ������6

        // ����һ������
        Sequence sequence = DOTween.Sequence();

        // �������3�εĶ���
        for (int i = 0; i < 3; i++)
        {
            // ����
            sequence.Append(textComponent.DOColor(redColor, blinkTime));
            // ���ԭʼ��ɫ
            sequence.Append(textComponent.DOColor(originalColor, blinkTime));
        }

        // ����������ȷ����ɫ�ָ�Ϊԭʼ��ɫ
        sequence.OnComplete(() => {
            textComponent.color = originalColor;
        });
    }

}

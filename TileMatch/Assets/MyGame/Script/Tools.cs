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

        // ��ȡ�������RectTransform
        RectTransform rectTransform = newObject.GetComponent<RectTransform>();

        // ��ȡ��ǰ��PosX
        float currentPosX = rectTransform.anchoredPosition.x;
        float targetPosX = -540f;  // Ŀ��PosX

        // ����Ŀ�����ţ���С��0��
        Vector3 targetScale = Vector3.zero;

        // ���� DOTween ��������
        Sequence sequence = DOTween.Sequence();

        // �޸�PosX
        sequence.Append(DOTween.To(() => currentPosX, x => rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y), targetPosX, duration));

        // ͬʱ��С `localScale`
        sequence.Join(rectTransform.DOScale(targetScale, duration));

        // ������ɺ��������岢���ûص�����
        sequence.OnComplete(() =>
        {
            Destroy(newObject);
            callback?.Invoke();  // ����ص�������Ϊ�գ����ûص�
        });
    }


    // ����������
    // targetObject��Ҫ���Ʋ��ƶ���UI����
    // targetGameObject�������ƶ���Ŀ��GameObject
    // duration���ƶ�����С�ĳ���ʱ��
    // callback�����ٺ���õĻص�����
    public static void MoveAndShrinkUI(GameObject targetObject, GameObject targetGameObject, float duration, System.Action callback)
    {
        // ����UI����
        GameObject newObject = Instantiate(targetObject, targetObject.transform.position, Quaternion.identity, targetObject.transform.parent);

        // ��ȡ�������RectTransform
        RectTransform rectTransform = newObject.GetComponent<RectTransform>();

        // ��ȡĿ��GameObject��λ��
        Vector2 targetPosition = targetGameObject.GetComponent<RectTransform>().anchoredPosition;

        // ��ȡ��ǰ��PosX
        float currentPosX = rectTransform.anchoredPosition.x;

        // ����Ŀ�����ţ���С��0��
        Vector3 targetScale = Vector3.zero;

        // ���� DOTween ��������
        Sequence sequence = DOTween.Sequence();

        // �޸�PosX��Ŀ��λ��
        sequence.Append(DOTween.To(() => currentPosX, x => rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y), targetPosition.x, duration));

        // ͬʱ��С `localScale`
        sequence.Join(rectTransform.DOScale(targetScale, duration));

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
        float blinkTime = duration / 4f;  // ÿ����ɫ�仯��Ϊ��ɫ -> ԭʼ��ɫ��������ʱ������6

        // ����һ������
        Sequence sequence = DOTween.Sequence();

        // �������3�εĶ���
        for (int i = 0; i < 2; i++)
        {
            // ���첢ͬʱ�Ŵ�
            sequence.Append(textComponent.DOColor(redColor, blinkTime));
            sequence.Join(textComponent.transform.DOScale(2.0f, blinkTime));  // �Ŵ�1.2��
                                                                              // ���ԭʼ��ɫ����С
            sequence.Append(textComponent.DOColor(originalColor, blinkTime));
            sequence.Join(textComponent.transform.DOScale(1f, blinkTime));  // �ָ���ԭʼ��С
        }

        // ����������ȷ����ɫ�����Żָ�Ϊԭʼ״̬
        sequence.OnComplete(() => {
            textComponent.color = originalColor;
            textComponent.transform.localScale = Vector3.one;  // ȷ�����Żָ�
        });
    }


}

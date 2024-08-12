using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // ��ʼ����Ϊ 0
        rectTransform.localScale = Vector3.zero;
        // ʹ�� DOTween ִ�зŴ󶯻�
        rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine);
    }

    public void CloseButton()//�رհ�ť
    {
        gameObject.SetActive(false);
    }

    

}

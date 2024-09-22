using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class CollectionCard : MonoBehaviour
{
    /// <summary>
    /// ��ʼ����Ƭ
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="count"></param>
    public void Init(Sprite sprite,int count)
    {
        SetSprite(sprite);
        SetCount(count);
    }

    //���ÿ�Ƭ��sprite
    private void SetSprite(Sprite sprite)
    {
        transform.Find("Icon").GetComponent<Image>().sprite = sprite;
    }

    //�����·�������
    private void SetCount(int count)
    {
        transform.Find("Amount").Find("Label-Title").GetComponent<TextMeshProUGUI>().text = count.ToString();
        SetRatio(count);
    }

    //���ø�card�Ļ�ñ���
    private void SetRatio(int count)
    {
        count = Mathf.Clamp(count, 0, 1000);
        transform.Find("Ratio").GetComponent<Image>().fillAmount = (1000 - count) / 1000;
    }
}

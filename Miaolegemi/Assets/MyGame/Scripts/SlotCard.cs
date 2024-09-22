using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotCard: MonoBehaviour
{
    public string type;//�ÿ��Ƶ�����

    /// <summary>
    /// ����slotcard
    /// </summary>
    public void HideSlotCard()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ��ʾslotcard
    /// </summary>
    public void ShowSlotCard()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ���ø�slotcard��sprite
    /// </summary>
    /// <param name="sprite"></param>
    public void SetSlotCardSprite(Sprite sprite)
    {
        transform.Find("Sprite").GetComponent<Image>().sprite = sprite;
    }
}

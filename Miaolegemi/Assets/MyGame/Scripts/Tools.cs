using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Tools : MonoBehaviour
{
    /// <summary>
    /// ͨ�÷���������UIԪ�صĴ�С
    /// </summary>
    /// <param name="uiObject">UIԪ��</param>
    /// <param name="size">��С��Vector2����</param>
    public static void SetUiSize(GameObject uiObject, Vector2 size)
    {
        if (TryGetRectTransform(uiObject, out RectTransform rectTransform))
        {
            rectTransform.sizeDelta = size;
        }
    }

    /// <summary>
    /// ͨ�÷���������UIԪ�صĿ�ȣ��߶ȱ��ֲ���
    /// </summary>
    /// <param name="uiObject">UIԪ��</param>
    /// <param name="newWidth">�µĿ��ֵ</param>
    public static void SetUiWidth(GameObject uiObject, float newWidth)
    {
        if (TryGetRectTransform(uiObject, out RectTransform rectTransform))
        {
            float originalHeight = rectTransform.sizeDelta.y;
            rectTransform.sizeDelta = new Vector2(newWidth, originalHeight);
        }
    }

    /// <summary>
    /// ͨ�÷���������UIԪ�صĸ߶ȣ���ȱ��ֲ���
    /// </summary>
    /// <param name="uiObject">UIԪ��</param>
    /// <param name="newHeight">�µĸ߶�ֵ</param>
    public static void SetUiHeight(GameObject uiObject, float newHeight)
    {
        if (TryGetRectTransform(uiObject, out RectTransform rectTransform))
        {
            float originalWidth = rectTransform.sizeDelta.x;
            rectTransform.sizeDelta = new Vector2(originalWidth, newHeight);
        }
    }

    /// <summary>
    /// ͨ�÷�������AԪ�صĴ�С����ΪBԪ�صĴ�С��һ��������A��B����Ϊͬһ����
    /// </summary>
    /// <param name="AuiObject">AԪ��</param>
    /// <param name="BuiObject">BԪ��</param>
    /// <param name="widthRatio">��ȱ���</param>
    /// <param name="heightRatio">�߶ȱ���</param>
    public static void SetUiSizeRatio(GameObject AuiObject, GameObject BuiObject, float widthRatio, float heightRatio)
    {
        if (TryGetRectTransform(AuiObject, out RectTransform rectTransformA) && TryGetRectTransform(BuiObject, out RectTransform rectTransformB))
        {
            float Bwidth = rectTransformB.rect.width;
            float Bheight = rectTransformB.rect.height;
            rectTransformA.sizeDelta = new Vector2(Bwidth * widthRatio, Bheight * heightRatio);
        }
    }


    /// <summary>
    /// ��ȡuiԪ�صĴ�С(����),����transform����ʾ����ֵ��������ʵ�ʴ�С
    /// </summary>
    /// <param name="uiObject">uiԪ��</param>
    /// <returns>��UIԪ�صĴ�С</returns>
    public static Vector2 GetUiSize(GameObject uiObject)
    {
        return uiObject.GetComponent<RectTransform>().rect.size;
    }


    /// <summary>
    /// ���Ի�ȡ GameObject �� RectTransform ���
    /// </summary>
    /// <param name="uiObject">UIԪ��</param>
    /// <param name="rectTransform">���ص� RectTransform ���</param>
    /// <returns>�Ƿ�ɹ���ȡ RectTransform ���</returns>
    private static bool TryGetRectTransform(GameObject uiObject, out RectTransform rectTransform)
    {
        rectTransform = uiObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError($"RectTransform component not found on {uiObject.name}.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// ɾ��ָ�����������������
    /// </summary>
    /// <param name="parent">������</param>
    public static void DestroyAllChildren(GameObject parent)
    {
        // ѭ�����������������������
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}

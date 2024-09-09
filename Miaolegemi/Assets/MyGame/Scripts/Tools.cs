using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Tools : MonoBehaviour
{
    /// <summary>
    /// 通用方法：设置UI元素的大小
    /// </summary>
    /// <param name="uiObject">UI元素</param>
    /// <param name="size">大小，Vector2类型</param>
    public static void SetUiSize(GameObject uiObject, Vector2 size)
    {
        if (TryGetRectTransform(uiObject, out RectTransform rectTransform))
        {
            rectTransform.sizeDelta = size;
        }
    }

    /// <summary>
    /// 通用方法：设置UI元素的宽度，高度保持不变
    /// </summary>
    /// <param name="uiObject">UI元素</param>
    /// <param name="newWidth">新的宽度值</param>
    public static void SetUiWidth(GameObject uiObject, float newWidth)
    {
        if (TryGetRectTransform(uiObject, out RectTransform rectTransform))
        {
            float originalHeight = rectTransform.sizeDelta.y;
            rectTransform.sizeDelta = new Vector2(newWidth, originalHeight);
        }
    }

    /// <summary>
    /// 通用方法：设置UI元素的高度，宽度保持不变
    /// </summary>
    /// <param name="uiObject">UI元素</param>
    /// <param name="newHeight">新的高度值</param>
    public static void SetUiHeight(GameObject uiObject, float newHeight)
    {
        if (TryGetRectTransform(uiObject, out RectTransform rectTransform))
        {
            float originalWidth = rectTransform.sizeDelta.x;
            rectTransform.sizeDelta = new Vector2(originalWidth, newHeight);
        }
    }

    /// <summary>
    /// 通用方法：将A元素的大小设置为B元素的大小的一定比例，A与B可以为同一物体
    /// </summary>
    /// <param name="AuiObject">A元素</param>
    /// <param name="BuiObject">B元素</param>
    /// <param name="widthRatio">宽度比例</param>
    /// <param name="heightRatio">高度比例</param>
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
    /// 获取ui元素的大小(长宽),不是transform上显示的数值，而是其实际大小
    /// </summary>
    /// <param name="uiObject">ui元素</param>
    /// <returns>该UI元素的大小</returns>
    public static Vector2 GetUiSize(GameObject uiObject)
    {
        return uiObject.GetComponent<RectTransform>().rect.size;
    }


    /// <summary>
    /// 尝试获取 GameObject 的 RectTransform 组件
    /// </summary>
    /// <param name="uiObject">UI元素</param>
    /// <param name="rectTransform">返回的 RectTransform 组件</param>
    /// <returns>是否成功获取 RectTransform 组件</returns>
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
    /// 删除指定物体的所有子物体
    /// </summary>
    /// <param name="parent">父物体</param>
    public static void DestroyAllChildren(GameObject parent)
    {
        // 循环遍历父物体的所有子物体
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}

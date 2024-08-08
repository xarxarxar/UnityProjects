using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public const int CardHeight = 100;
    public const int CardWidth = 100;
    public List<Card> allCards= new List<Card>();
    public Canvas canvas;

    public static CardManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(TestClick());
        for (int i = 0; i < allCards.Count; i++)
        {
            GenerateUpAndDownList(allCards[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateUpAndDownList(Card oneCard)
    {
        // 获取 oneCard 的 RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();
        Rect oneCardRect = oneCardTransform.rect;

        // 将 oneCardRect 从局部坐标系转换到世界坐标系
        Vector3[] oneCardWorldCorners = new Vector3[4];
        oneCardTransform.GetWorldCorners(oneCardWorldCorners);
        Rect oneCardWorldRect = new Rect(oneCardWorldCorners[0], oneCardWorldCorners[2] - oneCardWorldCorners[0]);

        for (int i = 0; i < allCards.Count; i++)
        {
            if (allCards[i].level == oneCard.level)
            {
                continue;
            }
            else if (allCards[i].level > oneCard.level)
            {
                // 获取其他卡片的 RectTransform
                RectTransform otherTransform = allCards[i].gameObject.GetComponent<RectTransform>();
                Rect otherRect = otherTransform.rect;

                // 将 otherRect 从局部坐标系转换到世界坐标系
                Vector3[] otherWorldCorners = new Vector3[4];
                otherTransform.GetWorldCorners(otherWorldCorners);
                Rect otherWorldRect = new Rect(otherWorldCorners[0], otherWorldCorners[2] - otherWorldCorners[0]);

                if (oneCardWorldRect.Overlaps(otherWorldRect))
                {
                    oneCard.downCards.Add(allCards[i]);
                }
            }
            else
            {
                // 获取其他卡片的 RectTransform
                RectTransform otherTransform = allCards[i].gameObject.GetComponent<RectTransform>();
                Rect otherRect = otherTransform.rect;

                // 将 otherRect 从局部坐标系转换到世界坐标系
                Vector3[] otherWorldCorners = new Vector3[4];
                otherTransform.GetWorldCorners(otherWorldCorners);
                Rect otherWorldRect = new Rect(otherWorldCorners[0], otherWorldCorners[2] - otherWorldCorners[0]);

                if (oneCardWorldRect.Overlaps(otherWorldRect))
                {
                    oneCard.upCards.Add(allCards[i]);
                }
            }
        }
    }

    public  bool IsClickable(Card oneCard)
    {
        Debug.Log("检测");
        // 获取 oneCard 的 RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();
        Rect oneCardRect = oneCardTransform.rect;

        // 将 oneCardRect 从局部坐标系转换到世界坐标系
        Vector3[] oneCardWorldCorners = new Vector3[4];
        oneCardTransform.GetWorldCorners(oneCardWorldCorners);
        Rect oneCardWorldRect = new Rect(oneCardWorldCorners[0], oneCardWorldCorners[2] - oneCardWorldCorners[0]);

        for (int i = 0; i < oneCard.upCards.Count; i++)
        {
            // 获取其他卡片的 RectTransform
            RectTransform otherTransform = oneCard.upCards[i].gameObject.GetComponent<RectTransform>();
            Rect otherRect = otherTransform.rect;

            // 将 otherRect 从局部坐标系转换到世界坐标系
            Vector3[] otherWorldCorners = new Vector3[4];
            otherTransform.GetWorldCorners(otherWorldCorners);
            Rect otherWorldRect = new Rect(otherWorldCorners[0], otherWorldCorners[2] - otherWorldCorners[0]);

            // 检查两个矩形是否重叠
            if (oneCardWorldRect.Overlaps(otherWorldRect) && oneCard.upCards[i].gameObject.activeSelf)
            {
                oneCard.Isclickable = false;
                return false;//可以点击
            }
        }

        // 如果没有任何矩形重叠，返回 false
        oneCard.Isclickable = true;
        return true;//不可以点击
    }

    bool Isclickable(Card oneCard)
    {
        RectTransform recttransform= oneCard.GetComponent<RectTransform>();
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, recttransform.position);
        if (RectTransformUtility.RectangleContainsScreenPoint(recttransform, screenPoint, canvas.worldCamera))
        {
            // UI 元素未被遮挡
            oneCard.Isclickable=true;
            return true;
        }
        else
        {
            // UI 元素被遮挡
            oneCard.Isclickable=false;
            return false;
        }
    }


    IEnumerator TestClick()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            for (int i = 0; i < allCards.Count; i++)
            {
                IsClickable(allCards[i]);
            }
        }
    }
}

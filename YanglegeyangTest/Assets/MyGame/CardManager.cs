using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<Card> allCards= new List<Card>();

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
        RandomItem(20);
    }


    void GenerateUpAndDownList(Card oneCard)
    {
        // 获取 oneCard 的 RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();

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
            else if (allCards[i].level < oneCard.level)
            {
                // 获取其他卡片的 RectTransform
                RectTransform otherTransform = allCards[i].gameObject.GetComponent<RectTransform>();

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
        // 获取 oneCard 的 RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();

        // 将 oneCardRect 从局部坐标系转换到世界坐标系
        Vector3[] oneCardWorldCorners = new Vector3[4];
        oneCardTransform.GetWorldCorners(oneCardWorldCorners);
        Rect oneCardWorldRect = new Rect(oneCardWorldCorners[0], oneCardWorldCorners[2] - oneCardWorldCorners[0]);

        for (int i = 0; i < oneCard.upCards.Count; i++)
        {
            if (oneCard.upCards[i].isUsed) continue;
            // 获取其他卡片的 RectTransform
            RectTransform otherTransform = oneCard.upCards[i].gameObject.GetComponent<RectTransform>();

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


    List<ItemContent> RandomItem(int count)
    {
        List<ItemContent>  finalList=new List<ItemContent>();
        LootBox coinBox = new LootBox();
        LootBox objectBox = new LootBox();
        LootBox catBox = new LootBox();

        for (int i=0;i<GenerateCard.coinItems.Count;i++)
        {
            coinBox.AddItem(GenerateCard.coinItems[i]);
        }

        for (int i = 0; i < GenerateCard.objectItems.Count; i++)
        {
            objectBox.AddItem(GenerateCard.objectItems[i]);
        }

        for (int i = 0; i < GenerateCard.catItems.Count; i++)
        {
            catBox.AddItem(GenerateCard.catItems[i]);
        }

        

        for (int i = 0; i < 4; i++)
        {
            ItemContent coinItem = coinBox.GetRandomItem();
            finalList.Add(coinItem);
            coinBox.RemoveItem(coinItem);
            Debug.Log("资源：" + coinItem.Name);

            ItemContent objectItem = objectBox.GetRandomItem();
            finalList.Add(objectItem);
            objectBox.RemoveItem(objectItem);
            Debug.Log("物品：" + objectItem.Name);

            ItemContent catItem = catBox.GetRandomItem();
            finalList.Add(catItem);
            catBox.RemoveItem(catItem);
            Debug.Log("碎片："+ catItem.Name);
        }
        return finalList;
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

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
        RandomItem(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateUpAndDownList(Card oneCard)
    {
        // ��ȡ oneCard �� RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();
        Rect oneCardRect = oneCardTransform.rect;

        // �� oneCardRect �Ӿֲ�����ϵת������������ϵ
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
                // ��ȡ������Ƭ�� RectTransform
                RectTransform otherTransform = allCards[i].gameObject.GetComponent<RectTransform>();
                Rect otherRect = otherTransform.rect;

                // �� otherRect �Ӿֲ�����ϵת������������ϵ
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
                // ��ȡ������Ƭ�� RectTransform
                RectTransform otherTransform = allCards[i].gameObject.GetComponent<RectTransform>();
                Rect otherRect = otherTransform.rect;

                // �� otherRect �Ӿֲ�����ϵת������������ϵ
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
        // ��ȡ oneCard �� RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();
        Rect oneCardRect = oneCardTransform.rect;

        // �� oneCardRect �Ӿֲ�����ϵת������������ϵ
        Vector3[] oneCardWorldCorners = new Vector3[4];
        oneCardTransform.GetWorldCorners(oneCardWorldCorners);
        Rect oneCardWorldRect = new Rect(oneCardWorldCorners[0], oneCardWorldCorners[2] - oneCardWorldCorners[0]);

        for (int i = 0; i < oneCard.upCards.Count; i++)
        {
            if (oneCard.upCards[i].isUsed) continue;
            // ��ȡ������Ƭ�� RectTransform
            RectTransform otherTransform = oneCard.upCards[i].gameObject.GetComponent<RectTransform>();
            Rect otherRect = otherTransform.rect;

            // �� otherRect �Ӿֲ�����ϵת������������ϵ
            Vector3[] otherWorldCorners = new Vector3[4];
            otherTransform.GetWorldCorners(otherWorldCorners);
            Rect otherWorldRect = new Rect(otherWorldCorners[0], otherWorldCorners[2] - otherWorldCorners[0]);

            // ������������Ƿ��ص�
            if (oneCardWorldRect.Overlaps(otherWorldRect) && oneCard.upCards[i].gameObject.activeSelf)
            {
                oneCard.Isclickable = false;
                return false;//���Ե��
            }
        }

        // ���û���κξ����ص������� false
        oneCard.Isclickable = true;
        return true;//�����Ե��
    }

    bool Isclickable(Card oneCard)
    {
        RectTransform recttransform= oneCard.GetComponent<RectTransform>();
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, recttransform.position);
        if (RectTransformUtility.RectangleContainsScreenPoint(recttransform, screenPoint, canvas.worldCamera))
        {
            // UI Ԫ��δ���ڵ�
            oneCard.Isclickable=true;
            return true;
        }
        else
        {
            // UI Ԫ�ر��ڵ�
            oneCard.Isclickable=false;
            return false;
        }
    }

    List<ItemContent> RandomItem(int count)
    {
        LootBox lootBox = new LootBox();
        lootBox.AddItem(new ItemContent("a",1));
        lootBox.AddItem(new ItemContent("b",2));
        lootBox.AddItem(new ItemContent("c",3));
        lootBox.AddItem(new ItemContent("d",4));
        lootBox.AddItem(new ItemContent("e",5));
        lootBox.AddItem(new ItemContent("f",60));
        lootBox.AddItem(new ItemContent("g",70));
        lootBox.AddItem(new ItemContent("h",80));
        lootBox.AddItem(new ItemContent("i",90));
        lootBox.AddItem(new ItemContent("j",100));

        List<ItemContent> outs= new List<ItemContent>();
        for (int i = 0; i < count; i++) 
        {
            ItemContent randomItem = lootBox.GetRandomItem();
            Debug.Log(randomItem.Name);
            outs.Add(randomItem);
            lootBox.RemoveItem(randomItem);
        }
        return outs;
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

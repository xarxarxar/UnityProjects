using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    //�������е����п���
    public static CardManager instance;//����
    public List<Card> allCards = new List<Card>();//�ڳ����е����п��ƣ�������������
    public GameObject allCardsParentsGameobject;//���п��Ƶĸ������ʵ��
    public GameObject singalCardsParentsGameobject;//�������Ƶ�prefab

    public HorizontalLayoutGroup layoutGroup;//�����еĺ����Ű����
    public GameObject seizeCard;//����������ռλ����ʱ���ƣ�͸����

    public GameObject backButton;//���ذ�ť

    //�����ݿ��л�ȡ�����п��ƣ���ʱ��list����
    public List<ItemContent> allItems= new List<ItemContent>();

    public bool isFailed = false;//�Ƿ��Ѿ�ʧ��

    public bool isSeccess = false;

    public event Action failedEvent;//ʧ�ܴ����¼�
    public event Action successEvent;//�ɹ������¼�
    private void Awake()//���õ���
    {
        instance = this;
    }

    private void OnEnable()
    {
        isFailed = false;
        isSeccess=false;
        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(ResolutionManager.CardLength , ResolutionManager.CardLength);
        backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f*ResolutionManager.CardLength , -0.5f*ResolutionManager.CardLength);
        //���ÿ��۵ĳ���
        layoutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(ResolutionManager.CardLength*7+10, ResolutionManager.CardLength + 10);
        InitializeCard();//��ʼ����Ƭ
        for (int i = 0; i < allCards.Count; i++)//Ϊ�����е����п������ɸ��Ǻͱ����ǵĿ����б�
        {
            GenerateUpAndDownList(allCards[i]);
            JudgeClickable(allCards[i]);
        }
    }

    private void OnDisable()
    {

        //���ٿ����е����п���
        for (int i = 0;i< layoutGroup.transform.childCount; i++)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }
        Destroy(allCardsParentsGameobject);//�������prefab
        allCards.Clear();//�������б�
    }

    //Ϊĳ�ſ������ɸ��Ǻͱ����ǵĿ����б�
    void GenerateUpAndDownList(Card oneCard)
    {
        // ��ȡ oneCard �� RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();

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
            else if (allCards[i].level < oneCard.level)
            {
                // ��ȡ������Ƭ�� RectTransform
                RectTransform otherTransform = allCards[i].gameObject.GetComponent<RectTransform>();

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

    //����ĳ���ƵĿɵ��״̬
    public void JudgeClickable(Card oneCard)
    {
        // ��ȡ oneCard �� RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();

        // �� oneCardRect �Ӿֲ�����ϵת������������ϵ
        Vector3[] oneCardWorldCorners = new Vector3[4];
        oneCardTransform.GetWorldCorners(oneCardWorldCorners);
        Rect oneCardWorldRect = new Rect(oneCardWorldCorners[0], oneCardWorldCorners[2] - oneCardWorldCorners[0]);

        //ֻ��Ҫ���������ſ��Ƹ��ǵ��Ŀ�������Ϳ��ԣ�����ȫ������
        for (int i = 0; i < oneCard.upCards.Count; i++)
        {
            if (oneCard.upCards[i].isUsed) continue;
            // ��ȡ������Ƭ�� RectTransform
            RectTransform otherTransform = oneCard.upCards[i].gameObject.GetComponent<RectTransform>();

            // �� otherRect �Ӿֲ�����ϵת������������ϵ
            Vector3[] otherWorldCorners = new Vector3[4];
            otherTransform.GetWorldCorners(otherWorldCorners);
            Rect otherWorldRect = new Rect(otherWorldCorners[0], otherWorldCorners[2] - otherWorldCorners[0]);

            // ������������Ƿ��ص�
            if (oneCardWorldRect.Overlaps(otherWorldRect) && !oneCard.upCards[i].isUsed)
            {
                oneCard.Isclickable = false;//�����Ե��
                return;
            }
        }

        // ���û���κξ����ص�������Ե��
        oneCard.Isclickable = true;//���Ե��
        return;
    }

    //��ȡĳ�ſ���Ӧ���ڿ����е�����λ��
    int GetIndexInLayout(Card oneCard)
    {
        for (int i = layoutGroup.transform.childCount - 1; i >= 0; i--)
        {
            if (layoutGroup.transform.GetChild(i).GetComponent<SlotCard>().type == oneCard.type)
            {
                return i + 1;
            }
        }
        return layoutGroup.transform.childCount;
    }

    //������Ƶ�ʱ�򣬽����ſ����ƶ�����������Ӧ��λ�ã����Ҹ��¿������������۵�λ��
    public void MoveCard(Card oneCard)
    {
        // �� childObject ����Ϊ����������һ��������
        oneCard.transform.SetSiblingIndex(oneCard.transform.parent.childCount - 1);
        // ʵ����Image�����ó�ʼλ��
        GameObject newImage = oneCard.gameObject;

        // Ԥ����Ŀ��λ�ã�
        // 1. ��ʵ����һ���յ�ռλ��������ʱ��ӵ�Layout Group��
        GameObject placeholder = Instantiate(seizeCard);
        placeholder.name= oneCard.type;
        int oneCardIndex = GetIndexInLayout(oneCard);
        placeholder.transform.SetParent(layoutGroup.transform);
        placeholder.transform.SetSiblingIndex(oneCardIndex);
        placeholder.GetComponent<SlotCard>().type = oneCard.type;
        placeholder.GetComponent<RectTransform>().sizeDelta= oneCard.GetComponent<RectTransform>().sizeDelta;
        //�ж��Ƿ�ʧ��
        List<GameObject> sameCards = JudgeSameCard();
        //����Ѿ�ʧ���˵Ļ����ͽ��������������嶼����Ϊ���ɵ��
        if (layoutGroup.transform.childCount >= 7 && sameCards.Count==0)//ʧ��
        {
            isFailed=true;
            failedEvent?.Invoke();
        }

        // 2. ǿ��ˢ�²��֣���ȡռλ����λ��
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        Vector3 targetPosition = placeholder.GetComponent<RectTransform>().position;

        // ִ�ж���
        RectTransform newImageRect = newImage.GetComponent<RectTransform>();
        newImageRect.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            //// ������ɺ󣬽�onecard����Ϣ���ƹ�ȥ����ɾ��oneCard
            placeholder.GetComponent<Image>().sprite=oneCard.GetComponent<Image>().sprite;
            placeholder.GetComponent<Image>().color=new Color32(255,255,255,255);
            
            allCards.Remove(oneCard);
            Destroy(oneCard.gameObject);
            for (int i = 0; i < sameCards.Count; i++)
            {
                Destroy(sameCards[i]);
                if (allCards.Count == 0)
                {
                    isSeccess = true;
                    Debug.Log("�ɹ���");
                    successEvent?.Invoke();
                }
            }
        });
    }

    //�ж��Ƿ�������������ͬ�ֿ��ƣ�����еĻ����Ƚ����������б����Ƴ����ȶ������֮����ɾ��
    List<GameObject> JudgeSameCard()
    {
        List<GameObject> sameCards = new List<GameObject>();
        int sameCount = 1;
        for (int i = 1; i < layoutGroup.transform.childCount; i++)
        {
            if (layoutGroup.transform.GetChild(i).GetComponent<SlotCard>().type 
                == layoutGroup.transform.GetChild(i -1).GetComponent<SlotCard>().type)//���Ƶ�������ͬ
            {
                sameCount++;
                if (sameCount == 3)
                {
                    GameObject card1 = layoutGroup.transform.GetChild(i).gameObject;
                    GameObject card2 = layoutGroup.transform.GetChild(i-1).gameObject;
                    GameObject card3 = layoutGroup.transform.GetChild(i-2).gameObject;
                    sameCards.Add(card1);
                    sameCards.Add(card2);
                    sameCards.Add(card3);
                    return sameCards;
                }
            }
            else
            {
                sameCount = 1;
            }
        }
        return sameCards;
    }

    //�ӿ���ѡȡcount��Ԫ��
    List<ItemContent> RandomItem(int count)
    {
        List<ItemContent> finalList = new List<ItemContent>();
        LootBox catBox = new LootBox();

        //Ϊ���catBox���Ԫ��
        for (int i = 0; i < allItems.Count; i++)
        {
            catBox.AddItem(allItems[i]);
        }
        
        //��catBox��ѡȡ10����Ƭ��֮��10����Ƭ��ÿ����Ƭ���䵽����
        for (int i = 0; i < count; i++)
        {
            ItemContent catItem = catBox.GetRandomItem();
            finalList.Add(catItem);
            catBox.RemoveItem(catItem);
        }
        return finalList;//����ѡȡ����Ƭ
    }

    void InitializeCard()
    {

        for(int i = 0; i < 49; i++)
        {
            GameObject singalCard= Instantiate(singalCardsParentsGameobject, allCardsParentsGameobject.transform);
            float cardWidth = ResolutionManager.CardLength;
            RectTransform rectTransform = singalCard.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cardWidth, cardWidth);
            rectTransform.anchoredPosition=new Vector2(-3* cardWidth,3* cardWidth) + new Vector2(i%7, -i/7)* cardWidth;//����λ��
            singalCard.GetComponent<Card>().level = 0;
        }

        for (int i = 0; i < 11; i++)
        {
            GameObject singalCard = Instantiate(singalCardsParentsGameobject, allCardsParentsGameobject.transform);
            float cardWidth = ResolutionManager.CardLength;
            RectTransform rectTransform = singalCard.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cardWidth, cardWidth);
            rectTransform.anchoredPosition = new Vector2(-2.5f * cardWidth, 2.5f * cardWidth) + new Vector2(i % 5, -i / 5) * cardWidth;//����λ��
            singalCard.GetComponent<Card>().level = 1;
        }



        //������п��Ƶ������б���
        for (int i = 0; i < allCardsParentsGameobject.transform.childCount; i++)
        {
            allCards.Add(allCardsParentsGameobject.transform.GetChild(i).GetComponent<Card>());
        }


        List<ItemContent> itemContents = RandomItem(10);//ѡȡʮ��Ԫ��

        List<ItemContent> totalItems = new List<ItemContent>();//���������е�item����cardѡ��

        for (int i = 0; i< itemContents.Count; i++)
        {
            for (int j = 0; j < allCards.Count/10; j++)
            {
                totalItems.Add(itemContents[i]);
            }
        }

        //Ϊ������Ƭ���Ԫ��
        for(int i = 0;i< allCards.Count; i++)
        {
            int index=UnityEngine. Random.Range(0, totalItems.Count);
            allCards[i].type = totalItems[index].Name;
            allCards[i].GetComponent<Image>().sprite= totalItems[index].cardSprite;
            totalItems.RemoveAt(index);
        }
    }


}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class CardManager : MonoBehaviour
{
    //管理场景中的所有卡牌
    public static CardManager instance;//单例
    public List<Card> allCards = new List<Card>();//在场景中的所有卡牌，不包括卡槽内
    public GameObject allCardsParentsGameobject;//所有卡牌的父物体的实例
    public GameObject singalCardsParentsGameobject;//单个卡牌的prefab
    List<ItemContent> itemContents = new List<ItemContent>();
    public int currentDiffu = 60;

    public int slotLength=7;//判定失败的卡槽个数
    public HorizontalLayoutGroup layoutGroup;//卡槽中的横向排版组件
    public GameObject seizeCard;//卡槽中用来占位的临时卡牌，透明的

    public GameObject backButton;//返回按钮

    //从数据库中获取的所有卡牌，暂时用list代替
    public List<ItemContent> allItems= new List<ItemContent>();

    public bool isFailed = false;//是否已经失败

    public bool isSeccess = false;

    public event Action failedEvent;//失败触发事件
    public event Action successEvent;//成功触发事件
    private void Awake()//设置单例
    {
        instance = this;
    }

    private void OnEnable()
    {
        ButtonManager.instance.addSlotButton.interactable = true;
        ButtonManager.instance.reliveButton.interactable = true;
        ButtonManager.instance.shuffleButton.interactable = true;
        allItems=CallWechat.instance.allItems;

        isFailed = false;
        isSeccess=false;
        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(ResolutionManager.CardLength , ResolutionManager.CardLength);
        backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f*ResolutionManager.CardLength , -0.5f*ResolutionManager.CardLength);
        //设置卡槽的长宽
        layoutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(ResolutionManager.CardLength*7+10, ResolutionManager.CardLength + 10);
        InitializeCard();//初始化卡片
        for (int i = 0; i < allCards.Count; i++)//为场景中的所有卡牌生成覆盖和被覆盖的卡牌列表
        {
            GenerateUpAndDownList(allCards[i]);
            JudgeClickable(allCards[i]);
        }
    }


    private void OnDisable()
    {

        //销毁卡槽中的所有卡牌
        for (int i = 0;i< layoutGroup.transform.childCount; i++)
        {
            Destroy(layoutGroup.transform.GetChild(i).gameObject);
        }
        // 遍历所有子物体
        foreach (Transform child in allCardsParentsGameobject.transform)
        {
            Destroy(child.gameObject); // 销毁每个子物体
        }
        //Destroy(allCardsParentsGameobject);//销毁这个prefab
        allCards.Clear();//清空这个列表
    }

    //为某张卡牌生成覆盖和被覆盖的卡牌列表
    public void GenerateUpAndDownList(Card oneCard)
    {
        //先清空两个数组
        oneCard.downCards.Clear();
        oneCard.upCards.Clear();

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

    //设置某张牌的可点击状态
    public void JudgeClickable(Card oneCard)
    {
        // 获取 oneCard 的 RectTransform
        RectTransform oneCardTransform = oneCard.gameObject.GetComponent<RectTransform>();

        // 将 oneCardRect 从局部坐标系转换到世界坐标系
        Vector3[] oneCardWorldCorners = new Vector3[4];
        oneCardTransform.GetWorldCorners(oneCardWorldCorners);
        Rect oneCardWorldRect = new Rect(oneCardWorldCorners[0], oneCardWorldCorners[2] - oneCardWorldCorners[0]);

        //只需要遍历将这张卡牌覆盖掉的卡牌数组就可以，不用全部遍历
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
            if (oneCardWorldRect.Overlaps(otherWorldRect) && !oneCard.upCards[i].isUsed)
            {
                oneCard.Isclickable = false;//不可以点击
                return;
            }
        }

        // 如果没有任何矩形重叠，则可以点击
        oneCard.Isclickable = true;//可以点击
        return;
    }

    //获取某张卡牌应该在卡槽中的索引位置
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

    //点击卡牌的时候，将那张卡牌移动到卡槽中相应的位置，并且更新卡槽中其他卡槽的位置
    public void MoveCard(Card oneCard)
    {
        // 将 childObject 设置为父物体的最后一个子物体,防止在移动过程中被别的卡牌遮挡，没有其他用途
        oneCard.transform.SetSiblingIndex(oneCard.transform.parent.childCount - 1);
        // 实例化Image并设置初始位置
        GameObject newImage = oneCard.gameObject;

        // 预计算目标位置：
        // 1. 先实例化一个空的占位符对象并临时添加到Layout Group中
        GameObject placeholder = Instantiate(seizeCard);
        placeholder.name= oneCard.type;
        int oneCardIndex = GetIndexInLayout(oneCard);
        placeholder.transform.SetParent(layoutGroup.transform);
        placeholder.transform.SetSiblingIndex(oneCardIndex);
        placeholder.GetComponent<SlotCard>().type = oneCard.type;
        placeholder.GetComponent<RectTransform>().sizeDelta= oneCard.GetComponent<RectTransform>().sizeDelta;
        //判断是否失败
        List<GameObject> sameCards = JudgeSameCard();
        //如果已经失败了的话，就将场景中所有物体都设置为不可点击
        if (layoutGroup.transform.childCount >= slotLength && sameCards.Count==0)//失败
        {
            isFailed=true;
            failedEvent?.Invoke();
        }

        // 2. 强制刷新布局，获取占位符的位置
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        Vector3 targetPosition = placeholder.GetComponent<RectTransform>().position;

        // 执行动画
        RectTransform newImageRect = newImage.GetComponent<RectTransform>();
        newImageRect.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            //// 动画完成后，将onecard的信息复制过去，并删除oneCard
            placeholder.GetComponent<Image>().sprite=oneCard.GetComponent<Image>().sprite;
            placeholder.GetComponent<Image>().color=new Color32(255,255,255,255);
            
            allCards.Remove(oneCard);
            Destroy(oneCard.gameObject);
            for (int i = 0; i < sameCards.Count; i++)
            {
                Destroy(sameCards[i]);
            }
            if (allCards.Count == 0)
            {
                isSeccess = true;
                Debug.Log("成功啦");
                successEvent?.Invoke();

                //上传到云数据库
                for (int j = 0; j < itemContents.Count; j++)
                {
                    for (int k = 0; k < CallWechat.instance.thisUserData.cardData.Count; k++)
                    {
                        //已经拥有该卡牌
                        if (CallWechat.instance.thisUserData.cardData[k].OwnCard == 1)
                        {
                            continue;
                        }
                        //否则就增加
                        if (CallWechat.instance.thisUserData.cardData[k].CardName == itemContents[j].Name)
                        {
                            CallWechat.instance.thisUserData.cardData[k].Count += currentDiffu / 10;
                        }
                        //如果碎片数量超过可以拥有的数量，则已经拥有
                        if (CallWechat.instance.thisUserData.cardData[k].Count >= CallWechat.instance.thisUserData.cardData[k].Level * 100)
                        {
                            CallWechat.instance.thisUserData.cardData[k].OwnCard = 1;
                        }
                    }
                }
                CallWechat.instance.CallSetUserData(CallWechat.instance.thisUserData);

            }
        });
    }

    //判断是否有三个相连的同种卡牌，如果有的话，先将这三个从列表中移除，等动画完毕之后再删除
    List<GameObject> JudgeSameCard()
    {
        List<GameObject> sameCards = new List<GameObject>();
        int sameCount = 1;
        for (int i = 1; i < layoutGroup.transform.childCount; i++)
        {
            if (layoutGroup.transform.GetChild(i).GetComponent<SlotCard>().type 
                == layoutGroup.transform.GetChild(i -1).GetComponent<SlotCard>().type)//卡牌的类型相同
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

    //从库中选取count个元素
    List<ItemContent> RandomItem(int count)
    {
        List<ItemContent> finalList = new List<ItemContent>();
        LootBox catBox = new LootBox();

        //为这个catBox添加元素
        for (int i = 0; i < allItems.Count; i++)
        {
            catBox.AddItem(allItems[i]);
        }
        
        //从catBox中选取10个碎片，之后10个碎片，每个碎片扩充到总体
        for (int i = 0; i < count; i++)
        {
            ItemContent catItem = catBox.GetRandomItem();
            finalList.Add(catItem);
            catBox.RemoveItem(catItem);
        }
        return finalList;//返回选取的碎片
    }

    void InitializeCard()
    {

        for(int i = 0; i < 49; i++)
        {
            GameObject singalCard= Instantiate(singalCardsParentsGameobject, allCardsParentsGameobject.transform);
            float cardWidth = ResolutionManager.CardLength;
            RectTransform rectTransform = singalCard.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cardWidth, cardWidth);
            rectTransform.anchoredPosition=new Vector2(-3* cardWidth,3* cardWidth) + new Vector2(i%7, -i/7)* cardWidth;//设置位置
            singalCard.GetComponent<Card>().level = 0;
        }

        for (int i = 0; i < 11; i++)
        {
            GameObject singalCard = Instantiate(singalCardsParentsGameobject, allCardsParentsGameobject.transform);
            float cardWidth = ResolutionManager.CardLength;
            RectTransform rectTransform = singalCard.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cardWidth, cardWidth);
            rectTransform.anchoredPosition = new Vector2(-2.5f * cardWidth, 2.5f * cardWidth) + new Vector2(i % 5, -i / 5) * cardWidth;//设置位置
            singalCard.GetComponent<Card>().level = 1;
        }



        //添加所有卡牌到卡牌列表中
        for (int i = 0; i < allCardsParentsGameobject.transform.childCount; i++)
        {
            allCards.Add(allCardsParentsGameobject.transform.GetChild(i).GetComponent<Card>());
        }


        itemContents = RandomItem(10);//选取十个元素

        List<ItemContent> totalItems = new List<ItemContent>();//场景中所有的item，供card选择

        for (int i = 0; i< itemContents.Count; i++)
        {
            for (int j = 0; j < currentDiffu / 10; j++)
            {
                totalItems.Add(itemContents[i]);
            }
        }

        Debug.Log($"itemContents is {itemContents.Count},totalItems is {totalItems.Count}");


        //为场景卡片填充元素
        for(int i = 0;i< allCards.Count; i++)
        {
            int index=UnityEngine. Random.Range(0, totalItems.Count);
            allCards[i].type = totalItems[index].Name;
            allCards[i].GetComponent<Image>().sprite= totalItems[index].cardSprite;
            totalItems.RemoveAt(index);

            //填充卡片字典，到时候该字典要从网络获取
            if (!ResourceManager.instance.imageDict.ContainsKey(allCards[i].type))
            {
                ResourceManager.instance.imageDict.Add(allCards[i].type, allCards[i].GetComponent<Image>().sprite);
            }
        }




    }


}

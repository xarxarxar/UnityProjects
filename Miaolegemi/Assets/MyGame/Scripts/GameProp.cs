using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// 游戏道具的实现方法
/// </summary>
public class GameProp : MonoBehaviour
{
    //public GameObject transparentSlots;//透明的用来存放复活时的卡牌的卡槽
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            AddSlot();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Shuffle();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            TransferCards();
        }
    }
    //增加一格卡槽
    public void AddSlot()
    {
        //设置卡槽的长宽
        if (CardManager.instance.slotLength >= 9) return;
        CardManager.instance.slotLength += 1;
        CardManager.instance.isFailed = false;
        CardManager.instance.layoutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(ResolutionManager.CardLength * CardManager.instance.slotLength + 10, ResolutionManager.CardLength + 10);
        
    }

    //洗牌
    public void Shuffle()
    {
        List<GameObject> cardGameObjects = new List<GameObject>();
        //遍历场景内所有卡牌
        for(int i = 0; i < CardManager.instance.allCards.Count; i++)
        {
            cardGameObjects.Add(CardManager.instance.allCards[i].gameObject);
        }

        //洗牌,从allSprites随机选取sprite赋给相应的Image组件
        // 洗牌逻辑
        for (int i = 0; i < CardManager.instance.allCards.Count; i++)
        {
            // 随机从allSprites列表中抽取一个sprite
            int randomIndex = Random.Range(0, cardGameObjects.Count);
            GameObject selectedObject = cardGameObjects[randomIndex];

            // 将选中的sprite赋值给相应卡牌的Image组件
            CardManager.instance.allCards[i].GetComponent<Image>().sprite = selectedObject.GetComponent<Image>().sprite;
            CardManager.instance.allCards[i].GetComponent<Card>().type = selectedObject.GetComponent<Card>().type;


            // 从allSprites列表中移除已分配的sprite
            cardGameObjects.RemoveAt(randomIndex);

            // 获取当前卡牌的RectTransform
            RectTransform cardRect = CardManager.instance.allCards[i].GetComponent<RectTransform>();
            // 使用DoTween让RectTransform的大小在0.5秒内从当前状态变为0再变回来
            cardRect.DOScale(Vector3.zero, 0.25f);
            cardRect.DORotate(new Vector3(0, 0, 180), 0.25f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                // 同时放大并恢复旋转
                cardRect.DOScale(Vector3.one, 0.25f);
                cardRect.DORotate(Vector3.zero, 0.25f);
            });
        }
    }

    //将卡槽内所有卡片转移到外部来
    public void TransferCards()
    {

        

        RectTransform slotRect = CardManager.instance.layoutGroup.GetComponent<RectTransform>();//获取卡槽

        if (slotRect.childCount == 0)//卡槽内没有卡片
        {
            return;
        }

        // 获取物体的高度                                                                                       
        float height = slotRect.rect.height*1.01f;

        // 计算目标位置
        Vector3 targetPosition = slotRect.localPosition - new Vector3(0, height, 0);



        for(int i = 0;i< slotRect.childCount; i++)
        {
            slotRect.GetChild(i).AddComponent<Card>().type= slotRect.GetChild(i).GetComponent<SlotCard>().type;
            slotRect.GetChild(i).GetComponent<Card>().isUsed= false;
            slotRect.GetChild(i).GetComponent<Card>().Isclickable= true;
            slotRect.GetChild(i).GetComponent<Image>().raycastTarget =true;
            Destroy(slotRect.GetChild(i).GetComponent<SlotCard>());
        }

        GameObject copySlot = Instantiate(CardManager.instance.layoutGroup.gameObject, CardManager.instance.layoutGroup.transform.parent);
        // 将 childObject 设置为父物体的第一个子物体,防止在移动过程中遮挡别的卡牌，没有其他用途
        copySlot.transform.SetSiblingIndex(0);
        copySlot.SetActive(false);
        copySlot.transform.localScale = Vector3.zero;
        // 遍历物体的所有子物体
        foreach (Transform child in copySlot.transform)
        {
            // 销毁子物体
            Destroy(child.gameObject);
        }
        foreach (Transform child in slotRect.transform)
        {
            CardManager.instance.allCards.Add(child.GetComponent<Card>());
        }
        CardManager.instance.layoutGroup = copySlot.GetComponent<HorizontalLayoutGroup>();

        // 使用 DoTween 平移物体
        DOTween.Sequence()
               .Append(slotRect.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad))
               .Join(slotRect.GetComponent<Image>().DOFade(0f, 0.5f)) // 透明度变为 0
               .OnComplete(() =>
        {
            copySlot.SetActive(true);
            copySlot.transform.DOScale(Vector3.one, 0.5f);
        });

        
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// ��Ϸ���ߵ�ʵ�ַ���
/// </summary>
public class GameProp : MonoBehaviour
{
    //public GameObject transparentSlots;//͸����������Ÿ���ʱ�Ŀ��ƵĿ���
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
    //����һ�񿨲�
    public void AddSlot()
    {
        //���ÿ��۵ĳ���
        if (CardManager.instance.slotLength >= 9) return;
        CardManager.instance.slotLength += 1;
        CardManager.instance.isFailed = false;
        CardManager.instance.layoutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(ResolutionManager.CardLength * CardManager.instance.slotLength + 10, ResolutionManager.CardLength + 10);
        
    }

    //ϴ��
    public void Shuffle()
    {
        List<GameObject> cardGameObjects = new List<GameObject>();
        //�������������п���
        for(int i = 0; i < CardManager.instance.allCards.Count; i++)
        {
            cardGameObjects.Add(CardManager.instance.allCards[i].gameObject);
        }

        //ϴ��,��allSprites���ѡȡsprite������Ӧ��Image���
        // ϴ���߼�
        for (int i = 0; i < CardManager.instance.allCards.Count; i++)
        {
            // �����allSprites�б��г�ȡһ��sprite
            int randomIndex = Random.Range(0, cardGameObjects.Count);
            GameObject selectedObject = cardGameObjects[randomIndex];

            // ��ѡ�е�sprite��ֵ����Ӧ���Ƶ�Image���
            CardManager.instance.allCards[i].GetComponent<Image>().sprite = selectedObject.GetComponent<Image>().sprite;
            CardManager.instance.allCards[i].GetComponent<Card>().type = selectedObject.GetComponent<Card>().type;


            // ��allSprites�б����Ƴ��ѷ����sprite
            cardGameObjects.RemoveAt(randomIndex);

            // ��ȡ��ǰ���Ƶ�RectTransform
            RectTransform cardRect = CardManager.instance.allCards[i].GetComponent<RectTransform>();
            // ʹ��DoTween��RectTransform�Ĵ�С��0.5���ڴӵ�ǰ״̬��Ϊ0�ٱ����
            cardRect.DOScale(Vector3.zero, 0.25f);
            cardRect.DORotate(new Vector3(0, 0, 180), 0.25f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                // ͬʱ�Ŵ󲢻ָ���ת
                cardRect.DOScale(Vector3.one, 0.25f);
                cardRect.DORotate(Vector3.zero, 0.25f);
            });
        }
    }

    //�����������п�Ƭת�Ƶ��ⲿ��
    public void TransferCards()
    {

        

        RectTransform slotRect = CardManager.instance.layoutGroup.GetComponent<RectTransform>();//��ȡ����

        if (slotRect.childCount == 0)//������û�п�Ƭ
        {
            return;
        }

        // ��ȡ����ĸ߶�                                                                                       
        float height = slotRect.rect.height*1.01f;

        // ����Ŀ��λ��
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
        // �� childObject ����Ϊ������ĵ�һ��������,��ֹ���ƶ��������ڵ���Ŀ��ƣ�û��������;
        copySlot.transform.SetSiblingIndex(0);
        copySlot.SetActive(false);
        copySlot.transform.localScale = Vector3.zero;
        // �������������������
        foreach (Transform child in copySlot.transform)
        {
            // ����������
            Destroy(child.gameObject);
        }
        foreach (Transform child in slotRect.transform)
        {
            CardManager.instance.allCards.Add(child.GetComponent<Card>());
        }
        CardManager.instance.layoutGroup = copySlot.GetComponent<HorizontalLayoutGroup>();

        // ʹ�� DoTween ƽ������
        DOTween.Sequence()
               .Append(slotRect.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad))
               .Join(slotRect.GetComponent<Image>().DOFade(0f, 0.5f)) // ͸���ȱ�Ϊ 0
               .OnComplete(() =>
        {
            copySlot.SetActive(true);
            copySlot.transform.DOScale(Vector3.one, 0.5f);
        });

        
    }

}
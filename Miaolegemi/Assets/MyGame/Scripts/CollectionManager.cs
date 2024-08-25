using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    public GameObject collectionCard;
    private void OnEnable()
    {
        AdaptWidthAndHeight();
        CallWechat.instance.GetCardData(GetUserdataSuccess);
    }

    private void OnDisable()
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }

    //����ӦUI
    private void AdaptWidthAndHeight()
    {
        float cellWidth = ResolutionManager.ScreenWidth / 2;
        float cellHeight = cellWidth /2;
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellWidth, cellHeight);
    }

    //��ȡ�������Ϣ�ɹ�֮����õĺ���
    private void GetUserdataSuccess(OnlineUserdata userdata)
    {
        CallWechat.instance.thisUserData.UserName = userdata.data.UserName;
        CallWechat.instance.thisUserData.UserID = userdata.data.UserID;
        CallWechat.instance.thisUserData.UserScore = userdata.data.UserScore;
        CallWechat.instance.thisUserData.cardData = userdata.data.cardData;
        for (int i = 0; i < CallWechat.instance.thisUserData.cardData.Count; i++)
        {
            GameObject temp =  Instantiate(collectionCard,transform);
            for (int j = 0; j < CallWechat.instance.allItems.Count;j++)
            {
                if (CallWechat.instance.allItems[j].Name == CallWechat.instance.thisUserData.cardData[i].CardName)
                {
                    //�滻sprite
                    temp.transform.Find("ShowImage").GetComponent<Image>().sprite = CallWechat.instance.allItems[j].cardSprite;
                    temp.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = CallWechat.instance.allItems[j].Name;

                    //�ж��Ƿ���ӵ��
                    if (CallWechat.instance.thisUserData.cardData[i].OwnCard == 1)
                    {
                        temp.transform.Find("ShowImage").GetComponent<Image>().color = Color.white;
                        temp.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = "��ӵ��";
                    }
                    else
                    {
                        temp.transform.Find("ShowImage").GetComponent<Image>().color = Color.grey;
                        temp.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = CallWechat.instance.thisUserData.cardData[i].Count.ToString();
                    }
                }

            }
            
        }
    }
}

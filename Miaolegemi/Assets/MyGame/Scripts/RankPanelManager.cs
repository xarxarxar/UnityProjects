using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankPanelManager : MonoBehaviour
{

    public GameObject RankInfoPrefab;//������Ԥ����
    public Transform RanInfoParent;//������������
    private void OnEnable()
    {
        ClearRankPrefab();//���������
        CallWechat.instance.GetRankInfo(RankUser);//��ȡ������Ϣ
    }

    private void OnDisable()
    {
        ClearRankPrefab();//���������
    }

    //����������
    public void RankUser(RankInfo rankInfo)//����
    {
        for (int i = 0; i < rankInfo.data.Count; i++)
        {
            int currentIndex = i;  // �� i ֵ���浽�ֲ�����

            GameObject tmpRank = Instantiate(RankInfoPrefab, RanInfoParent);
            tmpRank.transform.Find("RankText").GetComponent<Text>().text = (currentIndex + 1).ToString();


            ResourceManager.instance.LoadImageFromUrl(rankInfo.data[currentIndex].gamedata.AvatarURL, (texture2D) => {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                tmpRank.transform.Find("AvatarImage").GetComponent<Image>().sprite = sprite;
            });

            tmpRank.transform.Find("NickNameTMP").GetComponent<TextMeshProUGUI>().text = rankInfo.data[currentIndex].gamedata.UserNickName;

            int ownCount = 0;
            for (int j = 0; j < rankInfo.data[currentIndex].gamedata.cardData.Count; j++)
            {
                if (rankInfo.data[currentIndex].gamedata.cardData[j].OwnCard == 1) ownCount += 1;
            }
            tmpRank.transform.Find("UserCollectionText").GetComponent<Text>().text = $"�ղ�{ownCount}��";

        }
    }

    //�������������
    public void ClearRankPrefab()
    {
        // ʹ��ѭ��������������
        for (int i = RanInfoParent.childCount - 1; i >= 0; i--)
        {
            // ��ȡ������
            Transform child = RanInfoParent.GetChild(i);
            // ����������
            Destroy(child.gameObject);
        }
    }
}

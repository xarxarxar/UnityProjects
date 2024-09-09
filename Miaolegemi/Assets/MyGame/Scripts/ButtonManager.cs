using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public static ButtonManager instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject startScene;//��Ϸ��ʼ�ĳ���
    [Header("��ʼ��ť")]
    public GameObject gameScene;//��Ϸ������
    public GameObject cutScenes;//����������UIԪ��
    public void StartButton()
    {
        startScene.SetActive(false);
        cutScenes.SetActive(true);
        gameScene.SetActive(true);
    }

    [Header("�ֿⰴť")]
    public GameObject collectScene;//�ֿ�ĳ���
    public void CollectButton()
    {
        startScene.SetActive(false);
        collectScene.SetActive(true);
    }

    [Header("���ð�ť")]
    public GameObject settingScene;//�������
    public void SettingButton()
    {
        settingScene.SetActive(true);
    }

    [Header("������ս��ť")]
    public GameObject tipPanel;//�����Ƿ��������ս
    public void GiveUpButton()//������ս��ť
    {
        tipPanel.SetActive(true);
    }


    public void ConfrimGiveUp()//ȷ�Ϸ�����ť
    {
        tipPanel.SetActive(false);
        cutScenes.SetActive(true);
        settingScene.SetActive(false);
        gameScene.SetActive(false);
        startScene.SetActive(true);
    }
    public void ContinueButton()//������ս��ť
    {
        tipPanel.SetActive(false);
    }
    public void BackToStart()//�Ӳֿ���淵�ؿ�ʼ����
    {
        collectScene.SetActive(false);
        startScene.SetActive(true) ;
    }

    [Header("���߰�ť")]
    public  Button addSlotButton;
    public  Button shuffleButton;
    public  Button reliveButton;
    //���ӿ��۰�ť
    public void AddSlotButton()
    {
        GameProp.instance.AddSlot();//���ӿ���
        GameProp.instance.addSlotCount--;//���ӿ��۴�����1

        if (GameProp.instance.addSlotCount < 1)
        {
            addSlotButton.interactable = false;
        }
    }
    //ϴ�ư�ť
    public void ShuffleButton()
    {
        GameProp.instance.Shuffle();//ϴ��
        GameProp.instance.shuffleCount--;//ϴ�ƴ�����1

        if (GameProp.instance.shuffleCount < 1)
        {
            shuffleButton.interactable = false;
        }
    }
    //���ť
    public void ReliveButton()
    {
        GameProp.instance.TransferCards();//�Ƴ����еĿ��������
        GameProp.instance.reliveCount--;//��������1

        if (GameProp.instance.reliveCount < 1)
        {
            reliveButton.interactable = false;
        }
    }

    [Header("������ť")]
    public GameObject RankInfoPanel;//�������
    public GameObject RankInfoPrefab;//������Ԥ����
    public Transform RanInfoParent;//������������
    public void GetRankButton()
    {
        RankInfoPanel.SetActive(true);//���������
        //CallWechat.instance.GetRankInfo(RankUser);//��ȡ������Ϣ
        //CallWechat.instance.GetUserWechatIDandAvatar();
        //CallWechat.instance.GetUserInfo();
    }
    public void RankUser(RankInfo rankInfo)//����
    {
        for (int i = 0; i < rankInfo.data.Count; i++)
        {
            int currentIndex = i;  // �� i ֵ���浽�ֲ�����

            Debug.Log("RankUser:"+rankInfo.data[currentIndex].gamedata.AvatarURL);
            Debug.Log("RankUser:"+rankInfo.data[currentIndex].gamedata.UserNickName);

            GameObject tmpRank = Instantiate(RankInfoPrefab, RanInfoParent);
            tmpRank.transform.Find("RankText").GetComponent<Text>().text = (currentIndex + 1).ToString();

            Debug.Log("RankUser01:" + rankInfo.data[currentIndex].gamedata.AvatarURL);
            Debug.Log("RankUser01:" + rankInfo.data[currentIndex].gamedata.UserNickName);

            ResourceManager.instance.LoadImageFromUrl(rankInfo.data[currentIndex].gamedata.AvatarURL, (texture2D) => {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                tmpRank.transform.Find("AvatarImage").GetComponent<Image>().sprite = sprite;
            });
            Debug.Log("RankUser02:" + rankInfo.data[currentIndex].gamedata.AvatarURL);
            Debug.Log("RankUser02:" + rankInfo.data[currentIndex].gamedata.UserNickName);
            tmpRank.transform.Find("NickNameTMP").GetComponent<TextMeshProUGUI>().text = rankInfo.data[currentIndex].gamedata.UserNickName;

            int ownCount = 0;
            for (int j = 0; j < rankInfo.data[currentIndex].gamedata.cardData.Count; j++)
            {
                if (rankInfo.data[currentIndex].gamedata.cardData[j].OwnCard == 1) ownCount += 1;
            }
            tmpRank.transform.Find("UserCollectionText").GetComponent<Text>().text = $"�ղ�{ownCount}��";
            
        }
    }

    //�ر�������尴ť
    public void CloseRankPanelButton()
    {
        RankInfoPanel.SetActive(false);
    }

    [Header("������ť")]
    public GameObject SuccessPanel;//�ɹ����
    //�ɹ�֮���������ȷ����ť
    public void SuccessConfirmButton()
    {
        SuccessPanel.SetActive(false);
        gameScene.SetActive(false);
        startScene.SetActive(true);
    }
}

[System.Serializable]
public class RankInfo
{
    public List<RankUser> data;
}

[System.Serializable]
public class RankUser
{
    public string _id;
    public string openid;
    public GameData gamedata;
    public int ownCardCount;
}

[System.Serializable]
public class GameData
{
    public string AvatarURL;
    public string UserNickName;
    public int UserScore;
    public List<CardData> cardData;
    public myUserInfo userInfo;
}

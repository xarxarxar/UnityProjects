using System.Collections;
using System.Collections.Generic;
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

    public void GetRankButton()
    {
        CallWechat.instance.GetRankInfo(RankUser);//��ȡ������Ϣ
        //CallWechat.instance.GetUserWechatIDandAvatar();
        //CallWechat.instance.GetUserInfo();
    }
    public void RankUser(RankInfo rankInfo)//����
    {
        for (int i = 0; i < rankInfo.data.Count; i++)
        {
            Debug.Log(rankInfo.data[i].openid);
            Debug.Log(rankInfo.data[i].gamedata.UserName);
        }
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
    public string UserID;
    public string UserName;
    public int UserScore;
    public List<CardData> cardData;
    public myUserInfo userInfo;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData 
{
    public string UserName;
    public string UserID;
    public int UserScore;
    public int testName;
    public List<CardData> cardData =new List<CardData>();
    public UserData()
    {
        
    }
}
[System.Serializable]
public class CardData
{
    public string CardUID;//��ƬΨһ��ʶ��
    public string CardName;//��Ƭ����
    public int Count;//��Ƭ����
    public int Level;//��Ƭ�ȼ�
    public int OwnCard;//�ÿ�Ƭ�Ƿ��Ѿ��ϳɣ�Ҳ����˵�Ѿ�ӵ��
    public CardData(string cardUID, string cardName,int count,int level,int ownCard)
    {
        CardUID = cardUID;
        CardName = cardName;
        Count = count;
        Level = level;
        OwnCard = ownCard;
    }
}

[System.Serializable]
public class OnlineUserdata
{
    public Data data;
}

[System.Serializable]
public class Data
{
    public string UserName;
    public string UserID;
    public int UserScore;
    public int testName;
    public List<CardData> cardData = new List<CardData>();
    public UserInfo userInfo;
}

[System.Serializable]
public class UserInfo
{
    public string appId;
    public string openId;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData 
{
    public string UserNickName;
    public string AvatarURL;
    public int UserScore;
    public List<CardData> cardData =new List<CardData>();
    public UserData()
    {
        
    }
}
[System.Serializable]
public class CardData
{
    public string CardName;//��Ƭ����
    public int Count;//��Ƭ����
    public int Level;//��Ƭ�ȼ�
    public int OwnCard;//�ÿ�Ƭ�Ƿ��Ѿ��ϳɣ�Ҳ����˵�Ѿ�ӵ��
    public CardData( string cardName,int count,int level,int ownCard)
    {
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
    public string UserNickName;
    public string AvatarURL;
    public int UserScore;
    public List<CardData> cardData = new List<CardData>();
    public myUserInfo userInfo;
}

[System.Serializable]
public class myUserInfo
{
    public string appId;
    public string openId;
}

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
    public string CardUID;//碎片唯一标识符
    public string CardName;//碎片名称
    public int Count;//碎片个数
    public int Level;//碎片等级
    public int OwnCard;//该卡片是否已经合成，也就是说已经拥有
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

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
    public string CardName;//碎片名称
    public int Count;//碎片个数
    public int Level;//碎片等级
    public int OwnCard;//该卡片是否已经合成，也就是说已经拥有
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

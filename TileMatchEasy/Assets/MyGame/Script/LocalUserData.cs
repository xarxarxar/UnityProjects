using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 本地的玩家游戏数据
/// </summary>
[System.Serializable]
public class LocalUserData
{
    public int IsActived;//是否已经过了新手教程，0为否，1为是
    public int CoinCount;//用户金币
    public int MaxLifeCount;//最大生命值
    public int CurrentLiftCount;//当前生命值
    public int LifeDuration;//用户生命值恢复间隔
    public int MaxLevelCount;//玩家的最大关卡
    public long FreeCoinButtonLastTime;     //上一次领取免费金币的按钮的时间戳
    public long ShareLifeButtonLastTime;    //上一次分享获取爱心的按钮的时间戳
    public long SharePromptButtonLastTime;  //上一次分享获取提示道具的按钮的时间戳
    public long ShareShuffleButtonLastTime; //上一次分享获取洗牌道具的按钮的时间戳
    public long ShareUndoButtonLastTime;    //上一次分享获取撤回道具的按钮的时间戳
    public long ShareAddButtonLastTime;     //上一次分享获取增加卡槽的道具的按钮的时间戳
}

/// <summary>
/// 从微信数据库中获取的玩家数据
/// </summary>
[System.Serializable]
public class LocalUserDataContioner
{
    public LocalUserData data;
}
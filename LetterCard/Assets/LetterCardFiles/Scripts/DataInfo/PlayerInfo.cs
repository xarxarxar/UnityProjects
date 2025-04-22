using System;
using UnityEngine;

[Serializable]
public enum PlayerPlatform
{
    Unknown,
    Wechat,
    Douyin,
    Tiktok
}

[Serializable]
public class PlayerInfo
{
    // 玩家昵称
    public string playerName;

    // 玩家头像URL
    public string avatarUrl;

    // 玩家是否需要进行新手引导
    public bool needGuide;

    // 玩家最佳回合数
    public int maxRound;

    // 玩家最佳分数
    public int maxScore;

    // 音乐音量
    [Range(0f, 1f)]
    public float musicVolume;

    // 音效音量
    [Range(0f, 1f)]
    public float soundEffectVolume;

    /// <summary>
    /// 无参数构造函数（默认初始化）
    /// </summary>
    public PlayerInfo()
    {
        playerName = "游客";
        avatarUrl = "";
        needGuide = true;
        maxRound = 0;
        maxScore = 0;
        musicVolume = 0.5f;
        soundEffectVolume = 0.5f;
    }

    /// <summary>
    /// 带参数构造函数（初始化玩家信息）
    /// </summary>
    public PlayerInfo( string playerName, string avatarUrl, bool needGuide,int maxRound, int maxScore,float musicVolume,float soundEffectVolume)
    {
        this.playerName = playerName;
        this.avatarUrl = avatarUrl;
        this.needGuide = needGuide;
        this.maxRound = maxRound;
        this.maxScore = maxScore;
    }
}

[Serializable]
public class CloudResponse
{
    public PlayerInfo data; // 对应云函数返回的 "data" 字段

    public CloudResponse() 
    {
        data = new PlayerInfo();
    }
}

using System;

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
    // 玩家唯一ID，可以是微信ID、抖音ID等
    public string playerID;

    //public PlayerPlatform playerPlatform;

    // 玩家昵称
    public string playerName;

    // 玩家头像URL
    public string avatarUrl;

    // 玩家最佳回合数
    public int maxRound;

    // 玩家的金币数
    public int coinCount;

    // 玩家游戏设置
    //public GameSettings settings;

    //玩家残局信息,就是玩家自己上线之后继续玩
    //public EndGameInfo endGameInfo;

    //玩家挑战信息，就是分享给好友进行继续挑战
    //public PlayerChallenge playerChallenge;

    /// <summary>
    /// 无参数构造函数（默认初始化）
    /// </summary>
    public PlayerInfo()
    {
        playerID = "";
        //playerPlatform = PlayerPlatform.Unknown; // 默认平台类型
        playerName = "游客";
        avatarUrl = "";
        maxRound = 0;
        coinCount = 0;
        //settings = new GameSettings();
        //endGameInfo = new EndGameInfo();
        //playerChallenge = new PlayerChallenge();
    }

    /// <summary>
    /// 带参数构造函数（初始化玩家信息）
    /// </summary>
    public PlayerInfo(string playerID, PlayerPlatform playerPlatform, string playerName, string avatarUrl, int maxRound, int coinCount, GameSettings settings, EndGameInfo endGameInfo, PlayerChallenge playerChallenge)
    {
        this.playerID = playerID;
        //this.playerPlatform = playerPlatform;
        this.playerName = playerName;
        this.avatarUrl = avatarUrl;
        this.maxRound = maxRound;
        this.coinCount = coinCount;
        //this.settings = settings ?? new GameSettings(); // 避免传入null
        //this.endGameInfo = endGameInfo ?? new EndGameInfo();
        //this.playerChallenge = playerChallenge ?? new PlayerChallenge();
    }
}

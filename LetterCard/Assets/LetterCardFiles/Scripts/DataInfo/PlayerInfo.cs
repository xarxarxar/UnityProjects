using System;

[Serializable]
public class PlayerInfo
{
    // 玩家唯一ID，可以是微信ID、抖音ID等
    public string playerID;

    // 玩家昵称
    public string playerName;

    // 玩家头像URL
    public string avatarUrl;

    // 玩家当前的关卡
    public int level;

    // 玩家当前的分数（比如总分）
    public int score;


    // 玩家设置
    public GameSettings settings;

    // 其他扩展字段（如排行榜数据等）
    public string rank;

    // 构造函数（初始化玩家信息）
    public PlayerInfo(string playerID, string playerName, int level, int score, GameSettings settings, string rank)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.level = level;
        this.score = score;
        this.settings = settings;
        this.rank = rank;
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string UserName;
    public string AavtarUrl;
    public int CoinCount;//局外金币数量
    public int PassCount;//通过的次数
    public int UnlockCount;//解锁的科技index

    // 构造函数
    public PlayerInfo(string userName = null, string avatarUrl = null, int passCount = 0,int coinCount=0, int unlockCount=0)
    {
        UserName = string.IsNullOrEmpty(userName) ? "游客" : userName;
        AavtarUrl = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;//头像默认为空
        CoinCount = coinCount;//局外金币的数量
        PassCount = passCount; // 假设 0 是你的默认通关次数
        UnlockCount = unlockCount;
    }
}

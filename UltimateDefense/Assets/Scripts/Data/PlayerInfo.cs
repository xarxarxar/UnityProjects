[System.Serializable]
public class PlayerInfo
{
    public string UserName; //用户的昵称
    public string AavtarUrl;//用户的头像
    public int DiamondCount;//局外钻石数量
    public int CrownCount;  //局外王冠数量
    public int PassCount;   //通过的次数
    public int UnlockCount; //解锁的科技index

    // 构造函数
    public PlayerInfo(string userName = null, string avatarUrl = null, 
        int passCount = 0,int diamondCount=0, int crownCount=0,
        int unlockCount=0)
    {
        UserName = string.IsNullOrEmpty(userName) ? "游客" : userName;
        AavtarUrl = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;//头像默认为空
        DiamondCount = diamondCount;//局外钻石的数量
        CrownCount=crownCount;//局外王冠的数量
        PassCount = passCount; // 假设 0 是你的默认通关次数
        UnlockCount = unlockCount;
    }
}

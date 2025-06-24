[System.Serializable]
public class PlayerInfo
{
    public string UserName;
    public string AavtarUrl;
    public int PassCount;//通过的次数

    // 构造函数
    public PlayerInfo(string userName = null, string avatarUrl = null, int passCount = 0)
    {
        UserName = string.IsNullOrEmpty(userName) ? "游客" : userName;
        AavtarUrl = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;//头像默认为空
        PassCount = passCount == 0 ? 1 : passCount; // 假设 1 是你的默认通关次数
    }
}

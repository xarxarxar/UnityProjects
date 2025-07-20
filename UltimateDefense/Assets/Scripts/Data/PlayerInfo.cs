using System;

[System.Serializable]
public class PlayerInfo
{
    public Bindable<string> UserName=new Bindable<string>();        //用户的昵称
    public Bindable<string> AavtarUrl=new Bindable<string>();       //用户的头像
    public Bindable<int> DiamondCount=new Bindable<int>();          //局外钻石数量
    public Bindable<int> CrownCount = new Bindable<int>();          //局外王冠数量
    public Bindable<int> PassCount = new Bindable<int>();           //通过的次数
    public Bindable<int> UnlockCount = new Bindable<int>();         //解锁的科技index
    public Bindable<DateTime> LastLoginDate=new Bindable<DateTime>();// 上一次登录的日期
    public Bindable<int> TodayOnlineMinutes = new Bindable<int>();   // 今天在线时长（分钟）

    // 构造函数
    public PlayerInfo(string userName = null, string avatarUrl = null, 
        int passCount = 0,int diamondCount=0, int crownCount=0,
        int unlockCount=0, DateTime? lastLoginDate = null, int todayOnlineMinutes = 0)
    {
        UserName.Value = string.IsNullOrEmpty(userName) ? "游客" : userName;
        AavtarUrl.Value = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;//头像默认为空
        DiamondCount.Value = diamondCount;//局外钻石的数量
        CrownCount.Value = crownCount;//局外王冠的数量
        PassCount.Value = passCount; // 假设 0 是你的默认通关次数
        UnlockCount.Value = unlockCount;
        LastLoginDate.Value = lastLoginDate ?? DateTime.Now;
        TodayOnlineMinutes.Value = todayOnlineMinutes;
    }
}

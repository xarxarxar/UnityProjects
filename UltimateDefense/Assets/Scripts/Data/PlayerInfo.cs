using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInfo
{
    public string UserName;        //用户的昵称
    public string AavtarUrl;       //用户的头像
    public int DiamondCount;          //局外钻石数量
    public int CrownCount;          //局外王冠数量
    public int PassCount;           //通过的次数
    public int UnlockCount;         //解锁的科技index
    public DateTime LastLoginDate;// 上一次登录的日期，准确的说是上一次在线的时间，只用记录到天就可以
    public int TodayOnlineMinutes;   // 今天在线时长（分钟）
    public int TodayEnemyDieCount;   // 今天消灭敌人数量
    public int TodayWaveCount;   // 今天过了多少波次
    public int TodayFreshCount;   // 今天刷新了多少次刷新按钮
    public int TodayPassCount;   // 今天通关次数
    public int TodayShareCount;   // 今天分享次数
    public Dictionary<TowerType,int> TowerDatas = new Dictionary<TowerType, int>();//炮塔的等级数据
    public TowerType CurrentTowerType;//当前使用的炮塔
    // 在线奖励，每日任务，邀请有利这种每日刷新的奖励的领取情况,
    public Dictionary<string, bool> DailyRewardReceived = new Dictionary<string, bool>();
    public Dictionary<string, float> Config = new Dictionary<string, float>();//配置文件

    // 构造函数
    public PlayerInfo(string userName = null, string avatarUrl = null, 
        int passCount = 0,int diamondCount=0, int crownCount=0,
        int unlockCount=-1, DateTime? lastLoginDate = null, int todayOnlineMinutes = 0, 
        Dictionary<TowerType, int> towerDatas = null, TowerType currentTowerType =TowerType.Basic,
        Dictionary<string, bool> dailyRewardReceived =null, int todayEnemyDieCount=0,
        int todayWaveCount = 0, int todayFreshCount = 0,int todayPassCount=0,int todayShareCount = 0,
        Dictionary<string, float> config=null)
    {
        UserName = string.IsNullOrEmpty(userName) ? "游客" : userName;
        AavtarUrl = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;//头像默认为空
        DiamondCount = diamondCount;//局外钻石的数量
        CrownCount = crownCount;//局外王冠的数量
        PassCount = passCount; // 假设 0 是你的默认通关次数,这个是总的通关次数
        UnlockCount = unlockCount;
        LastLoginDate = lastLoginDate ?? DateTime.Now;
        TodayOnlineMinutes = todayOnlineMinutes;
        TodayEnemyDieCount= todayEnemyDieCount;
        TodayWaveCount = todayWaveCount;
        TodayFreshCount = todayFreshCount;
        TodayPassCount = todayPassCount;
        TodayShareCount = todayShareCount;
        CurrentTowerType = currentTowerType;


        if (towerDatas == null)
        {
            TowerDatas[TowerType.Basic] = 1;
            TowerDatas[TowerType.RapidFire] = 0;
            TowerDatas[TowerType.Ricochet] = 0;
            TowerDatas[TowerType.Spread] = 0;
            TowerDatas[TowerType.Sniper] = 0;
            TowerDatas[TowerType.Piercing] = 0;
        }
        else
        {
            TowerDatas.Clear();
            foreach (var kv in towerDatas)
            {
                TowerDatas[kv.Key] = kv.Value;
            }
        }
        if (dailyRewardReceived == null)
        {
            DailyRewardReceived.Clear();
        }
        else
        {
            DailyRewardReceived.Clear();
            foreach (var kv in dailyRewardReceived)
            {
                DailyRewardReceived[kv.Key] = kv.Value;
            }
        }

        if (config == null)
        {
            Config.Clear();
        }
        else
        {
            Config.Clear();
            foreach (var kv in config)
            {
                Config[kv.Key] = kv.Value;
            }
        }

    }
}

public class BindablePlayerInfo
{
    public Bindable<string> UserName=new Bindable<string>();        //用户的昵称
    public Bindable<string> AavtarUrl = new Bindable<string>();       //用户的头像
    public Bindable<int> DiamondCount = new Bindable<int>();          //局外钻石数量
    public Bindable<int> CrownCount = new Bindable<int>();          //局外王冠数量
    public Bindable<int> PassCount = new Bindable<int>();           //通过的次数
    public Bindable<int> UnlockCount = new Bindable<int>();         //解锁的科技index

    public Bindable<int> TodayOnlineMinutes = new Bindable<int>();   // 今天在线时长（分钟）
    public Bindable<int> TodayEnemyDieCount = new Bindable<int>();   // 今天消灭敌人数量
    public Bindable<int> TodayWaveCount = new Bindable<int>();   // 今天过了多少波次
    public Bindable<int> TodayFreshCount = new Bindable<int>();   // 今天刷新了多少次刷新按钮
    public Bindable<int> TodayPassCount = new Bindable<int>();   // 今天通关次数
    public Bindable<int> TodayShareCount = new Bindable<int>();   // 今天分享次数
    public Bindable<DateTime> LastLoginDate = new Bindable<DateTime>();// 上一次登录的日期，准确的说是上一次在线的时间，只用记录到天就可以
    public Bindable<TowerType> CurrentTowerType=new Bindable<TowerType>();//当前使用的炮塔
    public Dictionary<TowerType, int> TowerDatas;//防御塔的等级数据
    // 在线奖励，每日任务，邀请有利这种每日刷新的奖励的领取情况,
    public Dictionary<string,bool> DailyRewardReceived = new Dictionary<string, bool>();
    public Dictionary<string, float> Config = new Dictionary<string, float>();//配置文件

    // 构造函数
    public BindablePlayerInfo(string userName = null, string avatarUrl = null,
        int passCount = 0, int diamondCount = 0, int crownCount = 0,
        int unlockCount = -1, DateTime? lastLoginDate = null, int todayOnlineMinutes = 0,
        TowerType currentTowerType = TowerType.Basic, int todayEnemyDieCount = 0,
        int todayWaveCount = 0, int todayFreshCount = 0, int todayPassCount = 0,int todayShareCount=0)
    {
        UserName.Value = string.IsNullOrEmpty(userName) ? "游客" : userName;
        AavtarUrl.Value = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;//头像默认为空
        DiamondCount.Value = diamondCount;//局外钻石的数量
        CrownCount.Value = crownCount;//局外王冠的数量
        PassCount.Value = passCount; // 假设 0 是你的默认通关次数
        UnlockCount.Value = unlockCount;
        LastLoginDate.Value = lastLoginDate ?? DateTime.Now;
        TodayOnlineMinutes.Value = todayOnlineMinutes;
        TodayEnemyDieCount.Value = todayEnemyDieCount;
        TodayFreshCount.Value = todayFreshCount;
        TodayWaveCount.Value = todayWaveCount;
        TodayPassCount.Value = todayPassCount;
        TodayShareCount.Value = todayShareCount;

        CurrentTowerType.Value = currentTowerType;
        TowerDatas = new Dictionary<TowerType, int> {
            { TowerType.Basic,1},{ TowerType.RapidFire,0 },{ TowerType.Ricochet,0 },
            { TowerType.Spread,0 },{ TowerType.Sniper,0 },{ TowerType.Piercing,0 }};
    }

    /// <summary>
    /// 从PlayerInfo转到BindablePlayerInfo
    /// </summary>
    /// <param name="playerInfo"></param>
    public void CopyFromPlayerInfo(PlayerInfo playerInfo)
    {
        if (playerInfo == null)
        {
            UnityEngine.Debug.LogError("传入的 PlayerInfo 为 null");
            return;
        }
        UserName.Value = playerInfo.UserName;
        AavtarUrl.Value = playerInfo.AavtarUrl;
        DiamondCount.Value = playerInfo.DiamondCount;
        CrownCount.Value = playerInfo.CrownCount;
        PassCount.Value = playerInfo.PassCount;
        UnlockCount.Value = playerInfo.UnlockCount;
        LastLoginDate.Value = playerInfo.LastLoginDate;
        CurrentTowerType.Value =playerInfo.CurrentTowerType;
        TodayOnlineMinutes.Value = playerInfo.TodayOnlineMinutes;
        TodayEnemyDieCount.Value = playerInfo.TodayEnemyDieCount;
        TodayWaveCount.Value = playerInfo.TodayWaveCount;
        TodayFreshCount.Value = playerInfo.TodayFreshCount;
        TodayPassCount.Value = playerInfo.TodayPassCount;
        TodayShareCount.Value = playerInfo.TodayShareCount;

        TowerDatas.Clear();
        foreach (var kv in playerInfo.TowerDatas)
        {
            TowerDatas[kv.Key] = kv.Value;
        }
        DailyRewardReceived.Clear();
        foreach (var kv in playerInfo.DailyRewardReceived)
        {
            DailyRewardReceived[kv.Key] = kv.Value;
        }
        Config.Clear();
        foreach (var kv in playerInfo.Config)
        {
            Config[kv.Key] = kv.Value;
        }
    }

    /// <summary>
    /// 将BindablePlayerInfo转为PlayerInfo
    /// </summary>
    /// <returns></returns>
    public PlayerInfo ConvertToPlayerInfo()
    {
        return new PlayerInfo(
            userName: UserName.Value,
            avatarUrl: AavtarUrl.Value,
            passCount: PassCount.Value,
            diamondCount: DiamondCount.Value,
            crownCount: CrownCount.Value,
            unlockCount: UnlockCount.Value,
            lastLoginDate: LastLoginDate.Value,
            todayOnlineMinutes: TodayOnlineMinutes.Value,
            currentTowerType:CurrentTowerType.Value,
            towerDatas: TowerDatas,
            dailyRewardReceived:DailyRewardReceived,
            todayEnemyDieCount:TodayEnemyDieCount.Value,
            todayWaveCount:TodayWaveCount.Value,
            todayFreshCount:TodayFreshCount.Value,
            todayPassCount:TodayPassCount.Value,
            todayShareCount:TodayShareCount.Value,
            config:Config
        );
    }
}

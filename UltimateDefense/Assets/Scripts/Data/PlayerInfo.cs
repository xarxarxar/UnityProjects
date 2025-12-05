using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SkinInfo
{
    public int currentIndex;      // 当前使用的皮肤 index
    public List<int> unlockedSkinIds; // 已解锁皮肤 index 列表
}

[Serializable]
public class PlayerInfo
{
    // ① 用户基本信息
    public string UserName;
    public string AvatarUrl;

    // ② 资源
    public int Diamond;
    public int Crown;
    public int Medal;

    // ③ 对局统计（非每日，不清零）
    public int TotalPassCount;
    public int TotalBattleCount;

    // ④ 每日任务
    public DailyTaskProgress DailyTask = new DailyTaskProgress();

    // ⑤ 每月收集
    public Dictionary<int, int> MonthlyCollection = new Dictionary<int, int>();

    // ⑥ 炮塔 / 皮肤状态（新方案）
    public int CurrentTowerID;
    public Dictionary<int, ItemState> TowerStateMap = new Dictionary<int, ItemState>();
    public int CurrentTowerPlatformID;
    public Dictionary<int, ItemState> TowerPlatformStateMap = new Dictionary<int, ItemState>();

    // ⑦ 配置
    public Dictionary<string, float> Config = new Dictionary<string, float>();

    public int UnlockCount;
    public int BattleCountNoCollect;


    // -------------------------------------------------
    // 构造函数
    // -------------------------------------------------
    public PlayerInfo(
        string userName = "游客", string avatarUrl = "",
        int diamond = 0, int crown = 0, int medal = 0,
        int totalPass = 0, int totalBattle = 0,
        int unlockCount = 0, int battleCountNoCollect = 0,
        Dictionary<int, int> monthlyCollection = null,
        int currentTowerID = 0,int currentTowerPlatformID = 0,
        Dictionary<int, ItemState> towerStateMap = null, Dictionary<int, ItemState> towerPlatformStateMap = null,
        Dictionary<string, float> config = null
    )
    {
        UserName = userName;
        AvatarUrl = avatarUrl;

        Diamond = diamond;
        Crown = crown;
        Medal = medal;

        TotalPassCount = totalPass;
        TotalBattleCount = totalBattle;

        UnlockCount = unlockCount;
        BattleCountNoCollect = battleCountNoCollect;

        MonthlyCollection = monthlyCollection ?? new Dictionary<int, int>();

        CurrentTowerID = currentTowerID;
        TowerStateMap = towerStateMap ?? new Dictionary<int, ItemState>();

        CurrentTowerPlatformID=currentTowerPlatformID;
        TowerPlatformStateMap= towerPlatformStateMap?? new Dictionary<int, ItemState>();

        Config = config ?? new Dictionary<string, float>();

        DailyTask = new DailyTaskProgress();
    }
}

[System.Serializable]
public class BindablePlayerInfo
{
    // 1 基本信息
    public Bindable<string> UserName;
    public Bindable<string> AvatarUrl;

    // 2 资源
    public Bindable<int> Diamond;
    public Bindable<int> Crown;
    public Bindable<int> Medal;

    // 3 累计数据
    public Bindable<int> TotalPassCount;
    public Bindable<int> TotalBattleCount;

    // 4 每日任务
    public DailyTaskProgress DailyTask;

    // 5 每月收集
    public Dictionary<int, int> MonthlyCollection;

    // 6 炮塔系统
    public Bindable<int> CurrentTowerID;
    public Dictionary<int, ItemState> TowerStateMap;

    public Bindable<int> CurrentTowerPlatformID;
    public Dictionary<int, ItemState> TowerPlatformStateMap;

    // 7 配置
    public Dictionary<string, float> Config;

    public Bindable<int> UnlockCount;
    public Bindable<int> BattleCountNoCollect;


    // -------------------------------------------------
    // 构造函数
    // -------------------------------------------------
    public BindablePlayerInfo()
    {
        UserName = new Bindable<string>("游客");
        AvatarUrl = new Bindable<string>("");

        Diamond = new Bindable<int>(0);
        Crown = new Bindable<int>(0);
        Medal = new Bindable<int>(0);

        TotalPassCount = new Bindable<int>(0);
        TotalBattleCount = new Bindable<int>(0);

        UnlockCount = new Bindable<int>(0);
        BattleCountNoCollect = new Bindable<int>(0);

        DailyTask = new DailyTaskProgress();

        MonthlyCollection = new Dictionary<int, int>();
        TowerStateMap = new Dictionary<int, ItemState>();
        TowerPlatformStateMap=new Dictionary<int, ItemState>();

        CurrentTowerID = new Bindable<int>(0);
        CurrentTowerPlatformID = new Bindable<int>(0);

        Config = new Dictionary<string, float>();
    }


    // -------------------------------------------------
    // 将 PlayerInfo 数据复制到 BindablePlayerInfo（不触发事件）
    // -------------------------------------------------
    public void CopyFromPlayerInfo(PlayerInfo p)
    {
        UserName.SetSilent(p.UserName);
        AvatarUrl.SetSilent(p.AvatarUrl);

        Diamond.SetSilent(p.Diamond);
        Crown.SetSilent(p.Crown);
        Medal.SetSilent(p.Medal);

        TotalPassCount.SetSilent(p.TotalPassCount);
        TotalBattleCount.SetSilent(p.TotalBattleCount);

        UnlockCount.SetSilent(p.UnlockCount);
        BattleCountNoCollect.SetSilent(p.BattleCountNoCollect);

        DailyTask = p.DailyTask;

        MonthlyCollection = new Dictionary<int, int>(p.MonthlyCollection);
        TowerStateMap = new Dictionary<int, ItemState>(p.TowerStateMap);

        CurrentTowerID.SetSilent(p.CurrentTowerID);
        CurrentTowerPlatformID.SetSilent(p.CurrentTowerPlatformID);

        Config = new Dictionary<string, float>(p.Config);
    }


    // -------------------------------------------------
    // 转换回 PlayerInfo（用于保存 JSON）
    // -------------------------------------------------
    public PlayerInfo ConvertToPlayerInfo()
    {
        PlayerInfo p = new PlayerInfo();

        p.UserName = UserName.Value;
        p.AvatarUrl = AvatarUrl.Value;

        p.Diamond = Diamond.Value;
        p.Crown = Crown.Value;
        p.Medal = Medal.Value;

        p.TotalPassCount = TotalPassCount.Value;
        p.TotalBattleCount = TotalBattleCount.Value;

        p.UnlockCount = UnlockCount.Value;
        p.BattleCountNoCollect = BattleCountNoCollect.Value;

        p.DailyTask = DailyTask;

        p.MonthlyCollection = new Dictionary<int, int>(MonthlyCollection);
        p.TowerStateMap = new Dictionary<int, ItemState>(TowerStateMap);
        p.TowerPlatformStateMap=new Dictionary<int, ItemState>(TowerPlatformStateMap);

        p.CurrentTowerID = CurrentTowerID.Value;
        p.CurrentTowerPlatformID = CurrentTowerPlatformID.Value;

        p.Config = new Dictionary<string, float>(Config);

        return p;
    }
}

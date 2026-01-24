using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;



[Serializable]
public class PlayerInfo
{
    #region 私有字段
    // ====================1.用户基本信息====================
    [SerializeField] private string _userName="游客";
    [SerializeField] private string _avatarUrl=string.Empty;
    // ====================2.对局统计（非每日，不清零）====================
    [SerializeField] private int _totalPassCount=0;
    [SerializeField] private int _totalBattleCount = 0;
    // ====================3.资源（私有字段 + 公有属性）=============================
    [SerializeField] private int _diamond = 0;
    [SerializeField] private int _crown = 0;
    [SerializeField] private int _medal = 0;
    [SerializeField] private int _unlockCount = 0;//科技解锁的个数
    [SerializeField] private int _battleCountNoCollect = 0;//没有收集到字符的战斗次数
    // ====================4.每日任务====================
    [SerializeField] private DailyTaskProgress _dailyTask = new DailyTaskProgress();
    // ====================5.每月收集====================
    [SerializeField] private Dictionary<int, int> _monthlyCollection = new Dictionary<int, int>();
    // ====================6.炮塔 / 皮肤状态（新方案）====================
    [SerializeField] private int _currentTowerID = 0;
    [SerializeField] private Dictionary<int, ItemState> _towerStateMap = new Dictionary<int, ItemState>();
    [SerializeField] private int _currentTowerPlatformID=0;
    [SerializeField] private Dictionary<int, ItemState> _towerPlatformStateMap = new Dictionary<int, ItemState>();
    // ====================7.配置====================
    [SerializeField] private Dictionary<string, float> _config = new Dictionary<string, float>();
    [SerializeField] private Dictionary<int, List<int>> _blessCount = new Dictionary<int, List<int>>();//拥有的祝福数量
    #endregion

    #region 公共事件（数据变更通知：old -> new）
    //=============第一个参数为修改之前的值，第二个参数为修改之后的值
    /// <summary>
    /// 玩家名称发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<string, string> OnUserNameChanged;
    /// <summary>
    /// 玩家头像 URL 发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<string, string> OnAvatarUrlChanged;
    /// <summary>
    /// 通关次数发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnTotalPassCountChanged;
    /// <summary>
    /// 战斗次数发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnTotalBattleCountChanged;
    /// <summary>
    /// 钻石数量发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnDiamondChanged;
    /// <summary>
    /// 皇冠数量发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnCrownChanged;
    /// <summary>
    /// 勋章数量发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnMedalChanged;
    /// <summary>
    /// 科技解锁数量发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnUnlockCountChanged;
    /// <summary>
    /// 未收集字符的战斗次数发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnBattleCountNoCollectChanged;
    /// <summary>
    /// 当前使用的炮塔 ID 发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnCurrentTowerChanged;
    /// <summary>
    /// 当前使用的炮塔底座 ID 发生变化（旧值，新值）
    /// </summary>
    public event UnityAction<int, int> OnCurrentTowerPlatformChanged;

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerInfo()
    {
        _towerStateMap = new Dictionary<int, ItemState>();
        _towerPlatformStateMap = new Dictionary<int, ItemState>();
        _monthlyCollection = new Dictionary<int, int>();
        _dailyTask = new DailyTaskProgress();

        _dailyTask.OnDirty += SavePlayerInfoLocal;
        ItemState.OnStateChange += SavePlayerInfoLocal;
        SkinInfo.OnSkinfoChange += SavePlayerInfoLocal;
    }


    #region 公共只读属性（简单字段）
    // ====================公共只读属性（简单字段）=============================
    /// <summary>
    /// 玩家当前的显示名称
    /// </summary>
    public string UserName => _userName;

    /// <summary>
    /// 玩家头像的 URL 地址
    /// </summary>
    public string AvatarUrl => _avatarUrl;

    /// <summary>
    /// 玩家累计通关的关卡次数（非每日统计）
    /// </summary>
    public int TotalPassCount => _totalPassCount;

    /// <summary>
    /// 玩家累计参与的战斗次数（非每日统计）
    /// </summary>
    public int TotalBattleCount => _totalBattleCount;

    /// <summary>
    /// 玩家当前拥有的钻石数量
    /// </summary>
    public int Diamond => _diamond;

    /// <summary>
    /// 玩家当前拥有的皇冠数量
    /// </summary>
    public int Crown => _crown;

    /// <summary>
    /// 玩家当前拥有的勋章数量
    /// </summary>
    public int Medal => _medal;

    /// <summary>
    /// 玩家已解锁的科技数量
    /// </summary>
    public int UnlockCount => _unlockCount;

    /// <summary>
    /// 玩家未收集到字符的战斗次数
    /// </summary>
    public int BattleCountNoCollect => _battleCountNoCollect;

    /// <summary>
    /// 当前正在使用的炮塔 ID
    /// </summary>
    public int CurrentTowerID => _currentTowerID;

    /// <summary>
    /// 当前正在使用的炮塔底座 ID
    /// </summary>
    public int CurrentTowerPlatformID => _currentTowerPlatformID;

    /// <summary>
    /// 玩家的游戏配置
    /// </summary>
    public Dictionary<string,float> Config=>_config;

    /// <summary>
    /// 月收集任务
    /// </summary>
    public Dictionary<int, int> MonthlyCollection=> _monthlyCollection;

    /// <summary>
    /// 日常任务
    /// </summary>
    public DailyTaskProgress DailyTask=>_dailyTask;

    /// <summary>
    /// 炮塔数据
    /// </summary>
    public Dictionary<int,ItemState> TowerStateMap=> _towerStateMap;
    /// <summary>
    /// 炮塔底座数据
    /// </summary>
    public Dictionary<int,ItemState> TowerPlatformStateMap => _towerPlatformStateMap;
    /// <summary>
    /// 拥有的祝福数量
    /// </summary>
    public Dictionary<int, List<int>> BlessCount=>_blessCount;

    #endregion
    #region 修改接口（触发 MarkDirty）
    // ===================修改接口（触发 MarkDirty）==============================
    /// <summary>
    /// 设置玩家显示名称
    /// </summary>
    /// <param name="userName">新的玩家名称</param>
    public void SetUserName(string userName,bool save=true)
    {
        if (_userName == userName) return;

        string old = _userName;
        _userName = userName;

        OnUserNameChanged?.Invoke(old, _userName);
        if(save)
        {
            Debug.Log("SetUserName");
            SavePlayerInfoLocal();
        }
        
    }

    /// <summary>
    /// 设置玩家头像 URL
    /// </summary>
    /// <param name="avatarUrl">新的头像地址</param>
    public void SetAvatarUrl(string avatarUrl, bool save = true)
    {
        if (_avatarUrl == avatarUrl) return;

        string old = _avatarUrl;
        _avatarUrl = avatarUrl;

        OnAvatarUrlChanged?.Invoke(old, _avatarUrl);
        if (save)
        {
            Debug.Log("SetAvatarUrl");
            SavePlayerInfoLocal();
        }
            
    }

    /// <summary>
    /// 设置钻石数量（不会小于 0）
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetDiamond(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_diamond == value) return;

        int old = _diamond;
        _diamond = value;

        OnDiamondChanged?.Invoke(old, _diamond);
        if (save)
        {
            Debug.Log("SetDiamond");
            SavePlayerInfoLocal();
        }
            
    }

    /// <summary>
    /// 设置皇冠数量（不会小于 0）
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetCrown(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_crown == value) return;

        int old = _crown;
        _crown = value;

        OnCrownChanged?.Invoke(old, _crown);
        if (save)
        {
            Debug.Log("SetCrown");
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置勋章数量（不会小于 0）
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetMedal(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_medal == value) return;

        int old = _medal;
        _medal = value;

        OnMedalChanged?.Invoke(old, _medal);
        if (save)
        {
            Debug.Log("SetMedal");
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置通关次数
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetTotalPassCount(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_totalPassCount == value) return;

        int old = _totalPassCount;
        _totalPassCount = value;

        OnTotalPassCountChanged?.Invoke(old, _totalPassCount);
        if (save)
        {
            Debug.Log("SetTotalPassCount");
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置战斗次数
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetTotalBattleCount(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_totalBattleCount == value) return;

        int old = _totalBattleCount;
        _totalBattleCount = value;

        OnTotalBattleCountChanged?.Invoke(old, _totalBattleCount);
        if (save)
        {
            Debug.Log("SetTotalBattleCount");
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置科技解锁数量
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetUnlockCount(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_unlockCount == value) return;

        int old = _unlockCount;
        _unlockCount = value;

        OnUnlockCountChanged?.Invoke(old, _unlockCount);
        if (save)
        {
            Debug.Log("SetUnlockCount");
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置未收集字符的战斗次数
    /// </summary>
    /// <param name="value">设置的值</param>
    public void SetBattleCountNoCollect(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_battleCountNoCollect == value) return;

        int old = _battleCountNoCollect;
        _battleCountNoCollect = value;

        OnBattleCountNoCollectChanged?.Invoke(old, _battleCountNoCollect);
        if (save)
        {
            Debug.Log("SetBattleCountNoCollectt");
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置当前使用的炮塔 ID
    /// </summary>
    /// <param name="towerID">炮塔 ID</param>
    public void SetCurrentTower(int towerID, bool save = true)
    {
        if (_currentTowerID == towerID) return;

        int old = _currentTowerID;
        _currentTowerID = towerID;

        OnCurrentTowerChanged?.Invoke(old, _currentTowerID);
        if (save)
        {
            Debug.Log("SetCurrentTower");
            SavePlayerInfoLocal();
        }
    }




    /// <summary>
    /// 设置当前使用的炮塔底座 ID
    /// </summary>
    /// <param name="platformID">炮塔底座 ID</param>
    public void SetCurrentTowerPlatform(int platformID, bool save = true)
    {
        if (_currentTowerPlatformID == platformID) return;

        int old = _currentTowerPlatformID;
        _currentTowerPlatformID = platformID;

        OnCurrentTowerPlatformChanged?.Invoke(old, _currentTowerPlatformID);
        if (save)
        {
            Debug.Log("SetCurrentTowerPlatform");
            SavePlayerInfoLocal();
        }
    }
    #endregion

    //==================对于字典等复杂数据类型的访问和修改===============================
    /// <summary>
    /// 设置Config的值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetConfig(string key, float value, bool save = true)
    {
        if (_config.TryGetValue(key, out var old) && Mathf.Approximately(old, value))
            return;

        _config[key] = value;
        if (save)
        {
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置MonthlyCollection的值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetMonthlyCollection(int key, int value, bool save = true)
    {
        if (_monthlyCollection.TryGetValue(key, out var old) && Mathf.Approximately(old, value))
            return;

        _monthlyCollection[key] = value;
        if (save)
        {
            Debug.Log("SetMonthlyCollection");
            SavePlayerInfoLocal();
        }
    }
    /// <summary>
    /// 设置BlessCount的值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="save"></param>
    public void SetBlessCount(int key, List<int> value, bool save = true)
    {
        if (_blessCount.TryGetValue(key, out var old))
        {
            bool isEqual = value.Count == old.Count && value.SequenceEqual(old);
            if (isEqual) { return; }
        }
            
        _blessCount[key] = value;
        if (save)
        {
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 获取一个炮塔的ItemState
    /// </summary>
    /// <returns></returns>
    public bool GetTowerItemState(int towerID,out ItemState itemState)
    {
        return _towerStateMap.TryGetValue(towerID, out itemState);
    }


    /// <summary>
    /// 设置某个炮塔的itemstate
    /// </summary>
    /// <param name="towerID">炮塔ID</param>
    public void SetTowerItemState(int towerID, ItemState itemState, bool save = true)
    {
        if (!_towerStateMap.ContainsKey(towerID))
        {
            _towerStateMap[towerID] = new ItemState();
        }
        
        _towerStateMap[towerID].SetItemState(itemState,save);
        if (save)
        {
            SavePlayerInfoLocal();
        }
    }

    /// <summary>
    /// 设置某个炮塔底座的itemstate
    /// </summary>
    /// <param name="towerPlatformID">炮塔底座ID</param>
    public void SetTowerPlatformItemState(int towerPlatformID, ItemState itemState, bool save = true)
    {
        if (!_towerPlatformStateMap.ContainsKey(towerPlatformID))
        {
            _towerPlatformStateMap[towerPlatformID] = new ItemState();
        }
        Debug.Log($"添加底座的元素为,isUnlocked={itemState.IsUnlocked},_currentSkinID=={itemState.CurrentSkinID},pieceCount=={itemState.PieceCount},allskin的大小为{itemState.AllSkins.Count}");
        _towerPlatformStateMap[towerPlatformID].SetItemState(itemState,save);
        Debug.Log($"_towerPlatformStateMap==null吗{_towerPlatformStateMap == null}");
        if (save)
        {
            Debug.Log("SetTowerPlatformItemState");
            SavePlayerInfoLocal();
        }
    }


    // ==================内部：标记脏 & 存档===============================
    /// <summary>
    /// 保存在本地
    /// </summary>
    public void SavePlayerInfoLocal()
    {
        if(TowerStateMap == null)
        {
            Debug.Log($"在此时为null");
        }
        DataManager.Instance.UpdateNewestFinalPlayerInfp();
        //Debug.Log($"在更新newestFinalPlayerInfp之后,此时towerstatemap==null吗{TowerStateMap == null}");
        DataManager.Instance.SavePlayerInfoLocal();
        //Debug.Log($"在SavePlayerInfoLocal之后,此时towerstatemap==null吗{TowerStateMap == null}");
    }

    
}

//[System.Serializable]
//public class BindablePlayerInfo
//{
//    // 1 基本信息
//    public Bindable<string> UserName;
//    public Bindable<string> AvatarUrl;

//    // 2 资源
//    public Bindable<int> Diamond;
//    public Bindable<int> Crown;
//    public Bindable<int> Medal;

//    // 3 累计数据
//    public Bindable<int> TotalPassCount;
//    public Bindable<int> TotalBattleCount;

//    // 4 每日任务
//    public DailyTaskProgress DailyTask;

//    // 5 每月收集
//    public Dictionary<int, int> MonthlyCollection;

//    // 6 炮塔系统
//    public Bindable<int> CurrentTowerID;
//    public Dictionary<int, ItemState> TowerStateMap;

//    public Bindable<int> CurrentTowerPlatformID;
//    public Dictionary<int, ItemState> TowerPlatformStateMap;

//    // 7 配置
//    public Dictionary<string, float> Config;

//    public Bindable<int> UnlockCount;
//    public Bindable<int> BattleCountNoCollect;


//    // -------------------------------------------------
//    // 构造函数
//    // -------------------------------------------------
//    public BindablePlayerInfo()
//    {
//        UserName = new Bindable<string>("游客");
//        AvatarUrl = new Bindable<string>("");

//        Diamond = new Bindable<int>(0);
//        Crown = new Bindable<int>(0);
//        Medal = new Bindable<int>(0);

//        TotalPassCount = new Bindable<int>(0);
//        TotalBattleCount = new Bindable<int>(0);

//        UnlockCount = new Bindable<int>(0);
//        BattleCountNoCollect = new Bindable<int>(0);

//        DailyTask = new DailyTaskProgress();

//        MonthlyCollection = new Dictionary<int, int>();
//        TowerStateMap = new Dictionary<int, ItemState>();
//        TowerPlatformStateMap=new Dictionary<int, ItemState>();

//        CurrentTowerID = new Bindable<int>(0);
//        CurrentTowerPlatformID = new Bindable<int>(0);

//        Config = new Dictionary<string, float>();
//    }


//    // -------------------------------------------------
//    // 将 PlayerInfo 数据复制到 BindablePlayerInfo（不触发事件）
//    // -------------------------------------------------
//    public void CopyFromPlayerInfo(PlayerInfo p)
//    {
//        UserName.SetSilent(p.UserName);
//        AvatarUrl.SetSilent(p.AvatarUrl);

//        Diamond.SetSilent(p.Diamond);
//        Crown.SetSilent(p.Crown);
//        Medal.SetSilent(p.Medal);

//        TotalPassCount.SetSilent(p.TotalPassCount);
//        TotalBattleCount.SetSilent(p.TotalBattleCount);

//        UnlockCount.SetSilent(p.UnlockCount);
//        BattleCountNoCollect.SetSilent(p.BattleCountNoCollect);

//        DailyTask = p.DailyTask;

//        MonthlyCollection = new Dictionary<int, int>(p.MonthlyCollection);
//        TowerStateMap = new Dictionary<int, ItemState>(p.TowerStateMap);

//        CurrentTowerID.SetSilent(p.CurrentTowerID);
//        CurrentTowerPlatformID.SetSilent(p.CurrentTowerPlatformID);

//        Config = new Dictionary<string, float>(p.Config);
//    }


//    // -------------------------------------------------
//    // 转换回 PlayerInfo（用于保存 JSON）
//    // -------------------------------------------------
//    public PlayerInfo ConvertToPlayerInfo()
//    {
//        PlayerInfo p = new PlayerInfo();

//        p.UserName = UserName.Value;
//        p.AvatarUrl = AvatarUrl.Value;

//        p.Diamond = Diamond.Value;
//        p.Crown = Crown.Value;
//        p.Medal = Medal.Value;

//        p.TotalPassCount = TotalPassCount.Value;
//        p.TotalBattleCount = TotalBattleCount.Value;

//        p.UnlockCount = UnlockCount.Value;
//        p.BattleCountNoCollect = BattleCountNoCollect.Value;

//        p.DailyTask = DailyTask;

//        p.MonthlyCollection = new Dictionary<int, int>(MonthlyCollection);
//        p.TowerStateMap = new Dictionary<int, ItemState>(TowerStateMap);
//        p.TowerPlatformStateMap=new Dictionary<int, ItemState>(TowerPlatformStateMap);

//        p.CurrentTowerID = CurrentTowerID.Value;
//        p.CurrentTowerPlatformID = CurrentTowerPlatformID.Value;

//        p.Config = new Dictionary<string, float>(Config);

//        return p;
//    }
//}

/// <summary>
/// 超级简化版，用来存储玩家数据的类
/// </summary>
[System.Serializable]
public class SerializedPlayerInfo
{
    public string u;//username
    public string a;//avatarurl
    public int tpc;//totalPaccCount
    public int tbc;//totalBattleCount
    public int d;//diamond
    public int c;//crown
    public int m;//medal
    public int uc;//unlockCount
    public int bc;//BattleCountNoCollect
    public int ti;//currentTowerID
    public int tpi;//currentTowerPlatformID

    public int tu;//TowerUnlocked,每个ID的tower的解锁情况
    public List<int> tsu=new List<int>();//TowerSkinUnlocked,每个炮塔的每个皮肤的解锁情况
    //TowerStateMap,每个元素有>2个元素，依次为[CurrentSkinID,PieceCount,后面是AllSkins的PieceCount的数据]
    //比如[0,10,-1,4,7,23,56,4...]0代表CurrentSkinID为0，10代表该炮塔的PieceCount为10，后面的数据就是AllSkins第i位的PieceCount的数据
    public List<List<int>> ts=new List<List<int>>();//TowerStateMap

    //同上，不过是存储TowerPlatform的
    public int tpu;//每个ID的tower的解锁情况
    public List<int> tpsu=new List<int>();//每个炮塔的每个皮肤的解锁情况
    //每个元素有>2个元素，依次为[CurrentSkinID,PieceCount,后面是AllSkins的PieceCount的数据]
    //比如[0,10,-1,4,7,23,56,4...]0代表CurrentSkinID为0，10代表该炮塔底座的PieceCount为10，后面的数据就是AllSkins第i位的PieceCount的数据
    public List<List<int>> tps=new List<List<int>>();//TowerPlatformStateMap

    public List<int> mo=new List<int>();//字典MonthlyCollection
    public Dictionary<string, float> co = new Dictionary<string, float>();//config
    public List<int> dt = new List<int>();//dailyTask,前几个元素分别对应前几个属性，最后一个int是bool型数组
    public Dictionary<string, bool> rr= new Dictionary<string, bool>();//dailyTask的RewardReceived 
    public Dictionary<int,List<int>> b=new Dictionary<int, List<int>>();//BlessCount的缩写

    //List<bool>[0] <-> int 的 第 0 位（最低位）
    //最多支持 32 个 bool
    //超过 32 位自动截断 / 忽略
    //List<bool>转int
    public static int BoolListToInt(List<bool> list)
    {
        int result = 0;

        int count = Mathf.Min(list.Count, 32);

        for (int i = 0; i < count; i++)
        {
            if (list[i])
            {
                result |= (1 << i);
            }
        }

        return result;
    }
    //int转List<bool>
    public static List<bool> IntToBoolList(int value, int length)
    {
        length = Mathf.Clamp(length, 0, 32);

        var list = new List<bool>(length);

        for (int i = 0; i < length; i++)
        {
            bool bit = (value & (1 << i)) != 0;
            list.Add(bit);
        }

        return list;
    }

    public static SerializedPlayerInfo ChangePlayerInfo(PlayerInfo playerInfo)
    {
        if (playerInfo == null) return null;
        SerializedPlayerInfo serializedPlayerInfo = new SerializedPlayerInfo();
        serializedPlayerInfo.u= playerInfo.UserName;
        serializedPlayerInfo.a = playerInfo.AvatarUrl;
        serializedPlayerInfo.tpc = playerInfo.TotalPassCount;
        serializedPlayerInfo.tbc = playerInfo.TotalBattleCount;
        serializedPlayerInfo.d = playerInfo.Diamond;
        serializedPlayerInfo.c = playerInfo.Crown;
        serializedPlayerInfo.m = playerInfo.Medal;
        serializedPlayerInfo.uc = playerInfo.UnlockCount;
        serializedPlayerInfo.bc = playerInfo.BattleCountNoCollect;
        serializedPlayerInfo.ti = playerInfo.CurrentTowerID;
        serializedPlayerInfo.tpi = playerInfo.CurrentTowerPlatformID;

        //tower
        List<bool> tu = new List<bool>();
        List<int> tsu = new List<int>();
        List<List<int>> ts=new List<List<int>>();
        foreach (var item in playerInfo.TowerStateMap.OrderBy(kv => kv.Key))//按照key进行排序遍历
        {
            tu.Add(item.Value.IsUnlocked);

            List<int> ts_signle = new List<int>();
            ts_signle.Add(item.Value.CurrentSkinID);
            ts_signle.Add(item.Value.PieceCount);

            List<bool> tsu_bool = new List<bool>();
            foreach (var skinItem in item.Value.AllSkins.OrderBy(kv => kv.Key))
            {
                tsu_bool.Add(skinItem.Value.IsUnlocked);
                ts_signle.Add(skinItem.Value.PieceCount);
            }
            int tsu_bool_int = BoolListToInt(tsu_bool);
            tsu.Add(tsu_bool_int);
            ts.Add(ts_signle);
        }
        serializedPlayerInfo.tu = BoolListToInt(tu);
        serializedPlayerInfo.tsu = tsu;
        serializedPlayerInfo.ts = ts;

        //towerplatform
        List<bool> tpu = new List<bool>();
        List<int> tpsu = new List<int>();
        List<List<int>> tps = new List<List<int>>();
        foreach (var item in playerInfo.TowerPlatformStateMap.OrderBy(kv => kv.Key))//按照key进行排序遍历
        {
            tpu.Add(item.Value.IsUnlocked);

            List<int> ts_signle = new List<int>();
            ts_signle.Add(item.Value.CurrentSkinID);
            ts_signle.Add(item.Value.PieceCount);

            List<bool> tsu_bool = new List<bool>();
            foreach (var skinItem in item.Value.AllSkins.OrderBy(kv => kv.Key))
            {
                tsu_bool.Add(skinItem.Value.IsUnlocked);
                ts_signle.Add(skinItem.Value.PieceCount);
            }
            int tsu_bool_int = BoolListToInt(tsu_bool);
            tpsu.Add(tsu_bool_int);
            tps.Add(ts_signle);
        }
        serializedPlayerInfo.tpu = BoolListToInt(tpu);
        serializedPlayerInfo.tpsu = tpsu;
        serializedPlayerInfo.tps = tps;

        //MonthlyCollection
        List<int> mo=new List<int>();
        foreach(var item in playerInfo.MonthlyCollection.OrderBy(kv => kv.Key))
        {
            mo.Add(item.Value);
        }
        serializedPlayerInfo.mo = mo;
        serializedPlayerInfo.co = playerInfo.Config;

        List<int> dt=new List<int>();
        dt.Add(playerInfo.DailyTask.OnlineMinutes);
        dt.Add(playerInfo.DailyTask.EnemyKilled);
        dt.Add(playerInfo.DailyTask.WavePassed);
        dt.Add(playerInfo.DailyTask.FreshCount);
        dt.Add(playerInfo.DailyTask.PassCount);
        dt.Add(playerInfo.DailyTask.ShareCount);
        serializedPlayerInfo.dt = dt;
        serializedPlayerInfo.rr=playerInfo.DailyTask.RewardReceived;
        serializedPlayerInfo.b=playerInfo.BlessCount;

        return serializedPlayerInfo;
    }

    public static void ChangeSerializedPlayerInfo(SerializedPlayerInfo serializedPlayerInfo,PlayerInfo playerInfo)
    {
        Debug.Log("将serializedPlayerInfo转换为PlayerInfo");
        if (serializedPlayerInfo == null) return;
        playerInfo.SetUserName(serializedPlayerInfo.u,false);
        playerInfo.SetAvatarUrl(serializedPlayerInfo.a, false);
        playerInfo.SetTotalPassCount(serializedPlayerInfo.tpc, false);
        playerInfo.SetTotalBattleCount(serializedPlayerInfo.tbc, false);
        playerInfo.SetDiamond(serializedPlayerInfo.d, false);
        playerInfo.SetCrown(serializedPlayerInfo.c, false);
        playerInfo.SetMedal(serializedPlayerInfo.m, false);
        playerInfo.SetUnlockCount(serializedPlayerInfo.uc, false);
        playerInfo.SetBattleCountNoCollect(serializedPlayerInfo.bc, false);
        playerInfo.SetCurrentTower(serializedPlayerInfo.ti);
        playerInfo.SetCurrentTowerPlatform(serializedPlayerInfo.tpi, false);

        List<bool> towerItemStateUnlocked = IntToBoolList(serializedPlayerInfo.tu, serializedPlayerInfo.tsu.Count);
        for (int i = 0; i < serializedPlayerInfo.tsu.Count; i++)//serializedPlayerInfo.tsu.Count有这么多个炮塔
        {
            //该炮塔的皮肤解锁情况
            List<bool> itemSkinUnlocked = IntToBoolList(serializedPlayerInfo.tsu[i], serializedPlayerInfo.ts[i].Count - 2);

            ItemState itemState = new ItemState();
            itemState.SetIsUnlocked(towerItemStateUnlocked[i], false); 
            itemState.SetCurrentSkinID(serializedPlayerInfo.ts[i][0], false);
            itemState.SetPieceCount(serializedPlayerInfo.ts[i][1], false);
            for (int j = 0; j < serializedPlayerInfo.ts[i].Count - 2; j++)//有这么多个皮肤
            {
                SkinInfo skinInfo = new SkinInfo();
                skinInfo.SetIsUnlocked(itemSkinUnlocked[j], false);
                skinInfo.SetPieceCount(serializedPlayerInfo.ts[i][j + 2], false);//-1代表无穷多个
                itemState.SetSkins(j, skinInfo, false);
            }
            playerInfo.SetTowerItemState(i, itemState, false);
        }

        List<bool> towerPlatformItemStateUnlocked = IntToBoolList(serializedPlayerInfo.tpu, serializedPlayerInfo.tpsu.Count);
        for (int i = 0; i < serializedPlayerInfo.tpsu.Count; i++)//serializedPlayerInfo.tsu.Count有这么多个炮塔
        {
            //该炮塔的皮肤解锁情况
            List<bool> itemSkinUnlocked = IntToBoolList(serializedPlayerInfo.tpsu[i], serializedPlayerInfo.tps[i].Count - 2);

            ItemState itemState = new ItemState();
            itemState.SetIsUnlocked(towerPlatformItemStateUnlocked[i], false);
            itemState.SetCurrentSkinID(serializedPlayerInfo.tps[i][0], false);
            itemState.SetPieceCount(serializedPlayerInfo.tps[i][1], false);
            for (int j = 0; j < serializedPlayerInfo.tps[i].Count - 2; j++)//有这么多个皮肤
            {
                SkinInfo skinInfo = new SkinInfo();
                skinInfo.SetIsUnlocked(itemSkinUnlocked[j], false);
                skinInfo.SetPieceCount(serializedPlayerInfo.tps[i][j + 2], false);//-1代表无穷多个
                itemState.SetSkins(j, skinInfo, false);
            }
            playerInfo.SetTowerPlatformItemState(i, itemState, false);
        }

        //MonthlyCollection
        for (int i = 0; i < serializedPlayerInfo.mo.Count; i++)
        {
            playerInfo.SetMonthlyCollection(i, serializedPlayerInfo.mo[i], false);
        }

        //config
        foreach (var kv in serializedPlayerInfo.co)
        {
            playerInfo.SetConfig(kv.Key, kv.Value, false);
        }
        //dailyTask
        playerInfo.DailyTask.SetOnlineMinutes(serializedPlayerInfo.dt[0], false);
        playerInfo.DailyTask.SetEnemyKilled(serializedPlayerInfo.dt[1], false);
        playerInfo.DailyTask.SetWavePassed(serializedPlayerInfo.dt[2], false);
        playerInfo.DailyTask.SetFreshCount(serializedPlayerInfo.dt[3], false);
        playerInfo.DailyTask.SetPassCount(serializedPlayerInfo.dt[4], false);
        playerInfo.DailyTask.SetShareCount(serializedPlayerInfo.dt[5], false);

        //rewardReceived
        foreach(var kv in serializedPlayerInfo.rr)
        {
            playerInfo.DailyTask.SetRewardReceived(kv.Key,kv.Value,false);
        }
        //BlessCount
        foreach (var kv in serializedPlayerInfo.b)
        {
            playerInfo.SetBlessCount(kv.Key, kv.Value, false);
        }
    }


    public static long DateTimeToUnixSeconds(DateTime dt)
    {
        // 明确告诉系统：这是 UTC 时间
        var utc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        return new DateTimeOffset(utc).ToUnixTimeSeconds();
    }

    public static DateTime UnixSecondsToDateTime(long seconds)
    {
        return DateTimeOffset
            .FromUnixTimeSeconds(seconds)
            .UtcDateTime;
    }

    

}

[System.Serializable]
public class FinalSavePlayerInfo 
{
    public string v;//version
    public long s;//savetime的时间戳
    public SerializedPlayerInfo p;//playerinfo
}

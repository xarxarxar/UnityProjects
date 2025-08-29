using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.Collections;
using System;
using System.Linq;

public class DataManager : ManagerBase<DataManager>
{
    [SerializeField] private BindablePlayerInfo _playerInfo=new BindablePlayerInfo();//全局的玩家信息

    public static event UnityAction<int> OnPassCountChanged;//通关次数变化
    public static event UnityAction OnDataLoaded;//数据加载完毕

    private static string SaveFolder => Path.Combine(Application.dataPath, "../Saves");
    private static string SaveFilePath => Path.Combine(SaveFolder, "player_info.json");
    /// <summary>
    /// 玩家全局信息
    /// </summary>
    public BindablePlayerInfo PlayerInfo { get => _playerInfo;}


    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.OutBattle;//局外Manager
    }

    #region 公共方法
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        BattleManager.OnEndBattle += OnEndBattle;
    }

    /// <summary>
    /// 保存 PlayerInfo 到本地 json 文件（使用 Newtonsoft）
    /// </summary>
    public void SavePlayerInfo()
    {
        SavePlayerInfoLocal();//本地的测试
    }

    /// <summary>
    /// 加载 PlayerInfo（使用 Newtonsoft）
    /// </summary>
    public void LoadPlayerInfo()
    {
        StartCoroutine(LoadPlayerInfoLocal(() =>
        {
            OnDataLoaded?.Invoke();
        })) ;//用于测试
    }

    
    #endregion

    #region 私有方法
    //结束挑战
    private void OnEndBattle(bool success)
    {
        if(success)
        {
            _playerInfo.PassCount.Value++;
            OnPassCountChanged?.Invoke(_playerInfo.PassCount.Value);
        }
    }

    //保存本地，用于测试
    private void SavePlayerInfoLocal()
    {
        var jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            // NullValueHandling = NullValueHandling.Ignore, // 可选
        };

        PlayerInfo tmpPlayerInfo = _playerInfo.ConvertToPlayerInfo();//保存的是PlayerInfo格式
        string json = JsonConvert.SerializeObject(tmpPlayerInfo, jsonSettings);

        // 确保保存路径目录存在
        string directory = Path.GetDirectoryName(SaveFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"[保存成功] PlayerInfo 保存到：{SaveFilePath}\n{json}");
    }

    //加载本地，用于测试
    private IEnumerator LoadPlayerInfoLocal(UnityAction callback)
    {
        // 模拟耗时加载（比如显示 loading 动画）
        yield return new WaitForSeconds(1.0f);

        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            PlayerInfo tmpPlayerInfo= JsonConvert.DeserializeObject<PlayerInfo>(json);
            _playerInfo.CopyFromPlayerInfo(tmpPlayerInfo);//从PlayerInfo转为BindablePlayerInfo
            JudgeTheSameDay();//判断是否是同一天
        }
        else
        {
            SavePlayerInfoLocal();//保存一个
        }
        callback?.Invoke();
    }

    //判断是否是同一天
    private void JudgeTheSameDay()
    {
        DateTime now = DateTime.Now;
        if (now.Date != _playerInfo.LastLoginDate.Value.Date)
        {
            // 不同一天
            RefreshDailyPlayerInfo();
        }
    }

    /// <summary>
    /// 刷新日结玩家数据，登录时检刷新，或者是过了凌晨自动刷新
    /// </summary>
    public void RefreshDailyPlayerInfo()
    {
        _playerInfo.LastLoginDate.Value = DateTime.Now;
        _playerInfo.TodayOnlineMinutes.Value = 0;
        _playerInfo.TodayEnemyDieCount.Value = 0;
        _playerInfo.TodayWaveCount.Value = 0;
        _playerInfo.TodayFreshCount.Value = 0;
        _playerInfo.TodayPassCount.Value = 0;
        _playerInfo.TodayShareCount.Value = 0;

        foreach (var key in _playerInfo.DailyRewardReceived.Keys.ToList())
        {
            _playerInfo.DailyRewardReceived[key] = false;
        }
        SavePlayerInfo();
    }
    #endregion
}

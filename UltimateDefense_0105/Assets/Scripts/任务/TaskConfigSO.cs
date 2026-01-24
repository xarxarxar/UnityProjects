using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TaskConfig", menuName = "GameConfig/TaskConfig")]
public class TaskConfigSO : ScriptableObject
{
    public TaskData[] tasks;
}

[System.Serializable]
public class TaskData
{
    public string id;                // 唯一任务ID
    public TaskResetType resetType;  // 重置方式（每日 / 每月 / 永久 / 活动）
    public TaskType taskType;        // 任务行为类型（击杀、在线等）
    public int requiredValue;        // 要达成的数值
    //public Reward reward;            // 奖励
    public DateRange activeDate;     // 活动任务可选
}

public enum TaskResetType
{
    Daily,      // 每日任务
    Monthly,    // 每月任务
    Permanent,  // 成就任务（永不重置）
    Activity    // 活动任务（指定活动期间）
}

public enum TaskType
{
    OnlineMinutes,
    KillEnemy,
    PassWave,
    ShareToFriend,
    ClearOnce,
    // 未来继续扩展更多任务类型
}

[System.Serializable]
public class DateRange
{
    public string startDate;  // 格式：YYYY-MM-DD
    public string endDate;    // 格式：YYYY-MM-DD
}


[System.Serializable]
public class DailyTaskProgress
{
    public event UnityAction OnDirty;

    // yyyy-MM-dd // 用 yyyy-MM-dd 判断是否需要自动重置

    private int _onlineMinutes = 0;
    private int _enemyKilled = 0;
    private int _wavePassed = 0;
    private int _freshCount = 0;
    private int _passCount = 0;
    private int _shareCount = 0;
    private Dictionary<string,bool> _rewardReceived=new Dictionary<string, bool>();

    public event UnityAction<int, int> OnOnlineMinutesChanged;
    public event UnityAction<int, int> OnEnemyKilledChanged;
    public event UnityAction<int, int> OnWavePassedChanged;
    public event UnityAction<int, int> OnFreshCountChanged;
    public event UnityAction<int, int> OnPassCountChanged;
    public event UnityAction<int, int> OnShareCountChanged;

    #region 公共只读属性
    public int OnlineMinutes => _onlineMinutes;
    public int EnemyKilled => _enemyKilled;
    public int WavePassed => _wavePassed;
    public int FreshCount => _freshCount;
    public int PassCount => _passCount;
    public int ShareCount => _shareCount;
    #endregion

    // 每日任务奖励领取情况
    public Dictionary<string, bool> RewardReceived => _rewardReceived;

    public DailyTaskProgress()
    {
        _rewardReceived = new Dictionary<string, bool>();
    }

    /// <summary>
    /// 设置今日在线时长
    /// </summary>
    /// <param name="value"></param>
    public void SetOnlineMinutes(int value,bool save=true)
    {
        value = Mathf.Max(0, value);
        if (_onlineMinutes == value) return;

        int old = _onlineMinutes;
        _onlineMinutes = value;

        OnOnlineMinutesChanged?.Invoke(old, _onlineMinutes);
        if(save)
        OnDirty?.Invoke();
    }

    /// <summary>
    /// 设置今日杀敌数量
    /// </summary>
    /// <param name="value"></param>
    public void SetEnemyKilled(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_enemyKilled == value) return;

        int old = _onlineMinutes;
        _enemyKilled = value;

        OnEnemyKilledChanged?.Invoke(old, _enemyKilled);
        if (save)
            OnDirty?.Invoke();
    }

    /// <summary>
    /// 设置今日通过的波次
    /// </summary>
    /// <param name="value"></param>
    public void SetWavePassed(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_wavePassed == value) return;

        int old = _wavePassed;
        _wavePassed = value;

        OnWavePassedChanged?.Invoke(old, _wavePassed);
        if (save)
            OnDirty?.Invoke();
    }

    /// <summary>
    /// 设置今日刷新商店的次数
    /// </summary>
    /// <param name="value"></param>
    public void SetFreshCount(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_freshCount == value) return;

        int old = _freshCount;
        _freshCount = value;

        OnFreshCountChanged?.Invoke(old, _freshCount);
        if (save)
            OnDirty?.Invoke();
    }

    /// <summary>
    /// 设置今日通关次数
    /// </summary>
    /// <param name="value"></param>
    public void SetPassCount(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_passCount == value) return;

        int old = _passCount;
        _passCount = value;

        OnPassCountChanged?.Invoke(old, _passCount);
        if (save)
            OnDirty?.Invoke();
    }

    /// <summary>
    /// 设置今日分享次数
    /// </summary>
    /// <param name="value"></param>
    public void SetShareCount(int value, bool save = true)
    {
        value = Mathf.Max(0, value);
        if (_shareCount == value) return;

        int old = _shareCount;
        _shareCount = value;

        OnShareCountChanged?.Invoke(old, _shareCount);
        if (save)
            OnDirty?.Invoke();
    }

    /// <summary>
    /// 设置RewardReceived
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetRewardReceived(string key,bool value, bool save = true)
    {
        _rewardReceived[key] = value;
        if (save)
            OnDirty?.Invoke();
    }

    /// <summary>
    /// 重置每日任务的情况
    /// </summary>
    public void ResetDailyTask()
    {
        _onlineMinutes= 0;
        _enemyKilled = 0;
        _wavePassed = 0;
        _freshCount = 0;
        _passCount = 0;
        _shareCount = 0;
        _rewardReceived.Clear();
        OnDirty?.Invoke();
    }
}

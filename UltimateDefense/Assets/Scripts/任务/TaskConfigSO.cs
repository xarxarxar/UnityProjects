using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering;

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
    public Bindable<string> LastRefreshDate = new Bindable<string>(""); // yyyy-MM-dd // 用 yyyy-MM-dd 判断是否需要自动重置

    public Bindable<int> OnlineMinutes = new Bindable<int>(0);
    public Bindable<int> EnemyKilled = new Bindable<int>(0);
    public Bindable<int> WavePassed = new Bindable<int>(0);
    public Bindable<int> FreshCount = new Bindable<int>(0);
    public Bindable<int> PassCount = new Bindable<int>(0);
    public Bindable<int> ShareCount = new Bindable<int>(0);

    // 每日任务奖励领取情况
    public Dictionary<string, bool> RewardReceived = new Dictionary<string, bool>();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharePanel : BasePanel
{
    [SerializeField] private BindableButton _checkInButton;//分享按钮
    public List<TaskReward> taskRewards = new List<TaskReward>();//所有任务
    int[] _needTimes = new int[] { 1, 2, 3,4,5};//需要分享的次数
    public override void Init()
    {
        DataManager.Instance.PlayerInfo.TodayShareCount.OnValueChanged += (times) =>
        {
            for (int i = 0; i < taskRewards.Count; i++)
            {
                taskRewards[i].UpdateStatus(times);
            }
        };
    }

    protected override void InitPanel()
    {
        for (int i = 0; i < taskRewards.Count; i++)
        {
            string description = $"分享{_needTimes[i]}次";
            if (!DataManager.Instance.PlayerInfo.DailyRewardReceived.ContainsKey(description))
            {
                DataManager.Instance.PlayerInfo.DailyRewardReceived[description] = false;
            }
            taskRewards[i].Init(RewardType.Diamond, _needTimes[i]*10,
                    description, _needTimes[i],
                    DataManager.Instance.PlayerInfo.TodayShareCount.Value,
                    DataManager.Instance.PlayerInfo.DailyRewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyRewardReceived[taskTag] = true;
                        DataManager.Instance.SavePlayerInfo();
                    });
        }
    }
}

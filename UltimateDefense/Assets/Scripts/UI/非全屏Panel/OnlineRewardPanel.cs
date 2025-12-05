using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class OnlineRewardPanel : BasePanel
{
    public List<TaskReward> taskRewards=new List<TaskReward>();//所有任务
    int[] _needMinutes = new int[]{5,10,20,30,60 };


    public override void Init()
    {
        UnityEngine.Debug.Log("OnlineRewardPanel初始化");
        DataManager.Instance.PlayerInfo.DailyTask.OnlineMinutes.OnValueChanged += (minutes) =>
        {
            for (int i = 0; i < taskRewards.Count; i++)
            {
                taskRewards[i].UpdateStatus(minutes);
            }
        };
    }

    protected override void InitPanel()
    {
        for(int i = 0; i < taskRewards.Count; i++)
        {
            string description = $"在线{_needMinutes[i]}分钟";
            if (!DataManager.Instance.PlayerInfo.DailyTask.RewardReceived.ContainsKey(description))
            {
                DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[description] = false;
            }
            if (i== taskRewards.Count - 1)
            {
                taskRewards[i].Init(RewardType.Crown, 1, description,
                    _needMinutes[i], DataManager.Instance.PlayerInfo.DailyTask.OnlineMinutes.Value,
                    DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[taskTag]=true;
                        DataManager.Instance.SavePlayerInfo();
                    });
            }
            else
            {
                taskRewards[i].Init(RewardType.Diamond, _needMinutes[i]/2,
                    description, _needMinutes[i],
                    DataManager.Instance.PlayerInfo.DailyTask.OnlineMinutes.Value,
                    DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[taskTag] = true;
                        DataManager.Instance.SavePlayerInfo();
                    });
            }
        }
    }

}

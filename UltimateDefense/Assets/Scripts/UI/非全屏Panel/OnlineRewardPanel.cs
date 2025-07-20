using System.Collections.Generic;
using UnityEngine;

public class OnlineRewardPanel : BasePanel
{
    public List<TaskReward> taskRewards=new List<TaskReward>();//所有任务
    int[] _needMinutes = new int[]{1,5,10,30,60 };

    private int _todayOnlineMinutes = 0;

    public override void Init()
    {
        
        _todayOnlineMinutes = 0;
        DataManager.Instance.PlayerInfo.TodayOnlineMinutes.OnValueChanged += (minutes) =>
        {
            for (int i = 0; i < taskRewards.Count; i++)
            {
                taskRewards[i].UpdateStatus(minutes);
            }
        };
    }

    protected override void InitPanel()
    {
        Debug.Log($"_todayOnlineMinutes is {_todayOnlineMinutes}");
        for(int i = 0; i < taskRewards.Count; i++)
        {
            if(i== taskRewards.Count - 1)
            {
                taskRewards[i].Init(RewardType.Crown, 1, $"在线{_needMinutes[i]}分钟",
                    -1, DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value);
            }
            else
            {
                taskRewards[i].Init(RewardType.Diamond, _needMinutes[i], 
                    $"在线{_needMinutes[i]}分钟",_needMinutes[i],
                    DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value);
            }
        }
    }

}

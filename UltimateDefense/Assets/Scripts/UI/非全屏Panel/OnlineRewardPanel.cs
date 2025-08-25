using System.Collections.Generic;

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
        for(int i = 0; i < taskRewards.Count; i++)
        {
            string description = $"在线{_needMinutes[i]}分钟";
            if (!DataManager.Instance.PlayerInfo.DailyRewardReceived.ContainsKey(description))
            {
                DataManager.Instance.PlayerInfo.DailyRewardReceived[description] = false;
            }
            if (i== taskRewards.Count - 1)
            {
                taskRewards[i].Init(RewardType.Crown, 1, description,
                    _needMinutes[i], DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value,
                    DataManager.Instance.PlayerInfo.DailyRewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyRewardReceived[taskTag]=true;
                        DataManager.Instance.SavePlayerInfo();
                    });
            }
            else
            {
                taskRewards[i].Init(RewardType.Diamond, _needMinutes[i],
                    description, _needMinutes[i],
                    DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value,
                    DataManager.Instance.PlayerInfo.DailyRewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyRewardReceived[taskTag] = true;
                        DataManager.Instance.SavePlayerInfo();
                    });
            }
        }
    }

}

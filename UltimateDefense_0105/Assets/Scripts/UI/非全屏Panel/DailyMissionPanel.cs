using System.Collections.Generic;

public class DailyMissionPanel : BasePanel
{
    public List<TaskReward> taskRewards = new List<TaskReward>();//所有任务
    int[] _needValues = new int[] { 1000, 100, 100, 1 };
    int[] _rewardValues = new int[] { 5, 10, 15, 1 };
    int[] _currentValues = new int[] { 0,0,0,0};

    private int EnemyKilled => DataManager.Instance.PlayerInfo.DailyTask.EnemyKilled;
    private int WavePassed => DataManager.Instance.PlayerInfo.DailyTask.WavePassed;
    private int FreshCount => DataManager.Instance.PlayerInfo.DailyTask.FreshCount;
    private int PassCount => DataManager.Instance.PlayerInfo.DailyTask.PassCount;

    string[] _descriptions = new string[] 
    {
        "累计刷新增益100次",
        "累计消灭1000个敌人",
        "累计完成100个回合",
        //"银行累计存入1000元",
        "通关1次"
    };

    public override void Init()
    {
        BattleManager.OnEndBattle += (isSuccess) =>
        {
            DataManager.Instance.PlayerInfo.DailyTask.SetEnemyKilled(EnemyKilled+ EnemyManager.Instance.SignleEnemyDieCount.Value);
            DataManager.Instance.PlayerInfo.DailyTask.SetWavePassed(WavePassed+ WaveManager.Instance.CurrentRound);
            DataManager.Instance.PlayerInfo.DailyTask.SetFreshCount(FreshCount+ UpgradeManager.Instance.RefreshCount.Value);

            taskRewards[0].UpdateStatus(taskRewards[0].CurrentValue + 1);
            taskRewards[1].UpdateStatus(taskRewards[1].CurrentValue + 1);
            taskRewards[2].UpdateStatus(taskRewards[2].CurrentValue + 1);
            if (isSuccess) 
            {
                DataManager.Instance.PlayerInfo.DailyTask.SetPassCount(PassCount+1);
                _currentValues[3] = DataManager.Instance.PlayerInfo.DailyTask.PassCount;
                taskRewards[3].UpdateStatus(taskRewards[3].CurrentValue + 1);
            }
        };
    }

    //初始化面板状态，当打开的时候初始化
    protected override void InitPanel()
    {
        _currentValues[0] = DataManager.Instance.PlayerInfo.DailyTask.EnemyKilled;
        _currentValues[1] = DataManager.Instance.PlayerInfo.DailyTask.FreshCount;
        _currentValues[2] = DataManager.Instance.PlayerInfo.DailyTask.WavePassed;
        _currentValues[3] = DataManager.Instance.PlayerInfo.DailyTask.PassCount;
        for (int i = 0; i < taskRewards.Count; i++)
        {
            string description = _descriptions[i];
            if (!DataManager.Instance.PlayerInfo.DailyTask.RewardReceived.TryGetValue(description, out var received))
            {
                DataManager.Instance.PlayerInfo.DailyTask.SetRewardReceived(description, false);
            }
            if (i == taskRewards.Count - 1)
            {
                taskRewards[i].Init(RewardType.Crown, _rewardValues[i], description, _needValues[i],
                    _currentValues[i], DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyTask.SetRewardReceived(taskTag, true);
                    });
            }
            else
            {
                taskRewards[i].Init(RewardType.Diamond, _rewardValues[i], description, _needValues[i], 
                    _currentValues[i], DataManager.Instance.PlayerInfo.DailyTask.RewardReceived[description],
                    (taskTag) =>
                    {
                        DataManager.Instance.PlayerInfo.DailyTask.SetRewardReceived(taskTag, true);
                    });
            }
        }
    }
}

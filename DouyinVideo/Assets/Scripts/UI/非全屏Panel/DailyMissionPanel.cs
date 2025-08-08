using System.Collections.Generic;
using System.Diagnostics;

public class DailyMissionPanel : BasePanel
{
    public List<TaskReward> taskRewards = new List<TaskReward>();//所有任务
    int[] _needValues = new int[] { 1000, 100, 100, 1000, 1 };
    int[] _rewardValues = new int[] { 10, 10, 10, 10, 1 };
    int[] _currentValues = new int[] { 0,0,0,0,0};
    string[] _descriptions = new string[] 
    { 
        "累计消灭1000个敌人",
        "累计完成100个回合",
        "累计刷新增益100次",
        "银行累计存入1000元",
        "通关1次"
    };


    private int _todayEnemyDieCount = 0;
    private int _todayWaveChangeCount = 0;
    private int _todayRefreshBuffCount = 0;


    public override void Init()
    {
        Enemy.OnEnemyDie += (enemy) =>
        {
            _currentValues[0]++;
            taskRewards[0].UpdateStatus(taskRewards[0].CurrentValue + 1);
        };
        WaveManager.OnWaveChanged += (value) =>
        {
            _currentValues[1]++;
            taskRewards[1].UpdateStatus(taskRewards[1].CurrentValue + 1);
        };
        UpgradeUI.OnRefreshBuff += () =>
        {
            _currentValues[2]++;
            taskRewards[2].UpdateStatus(taskRewards[2].CurrentValue + 1);
        };
        
    }

    //初始化面板状态，当打开的时候初始化
    protected override void InitPanel()
    {
        for (int i = 0; i < taskRewards.Count; i++)
        {
            if (i == taskRewards.Count - 1)
            {
                taskRewards[i].Init(RewardType.Crown, 1, _descriptions[i], _needValues[i], _currentValues[i]);
            }
            else
            {
                taskRewards[i].Init(RewardType.Diamond, _needValues[i], _descriptions[i], _needValues[i], _currentValues[i]);
            }
        }
    }
}

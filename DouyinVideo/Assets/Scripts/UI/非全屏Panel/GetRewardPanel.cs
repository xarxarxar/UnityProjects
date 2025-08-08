using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRewardPanel : BasePanel
{
    [SerializeField]private GameObject _blackBackground;  
    [SerializeField] private RewardStruct _rewardStructPrefab;//奖励的Prefab
    [SerializeField] private Transform _rewardParent; // 用于放置奖励的父物体（如 ScrollView Content）

    private List<RewardStruct> rewardInstances = new List<RewardStruct>();

    protected override void InitPanel()
    {
        _blackBackground.SetActive(true);
    }

    /// <summary>
    /// 设置获得的奖励内容和数量（支持传入一个或多个）
    /// </summary>
    /// <param name="rewards">一个或多个奖励结构</param>
    public void SetReward(params (RewardType type, int count)[] rewards)
    {
        // 清理旧实例（如果有）
        foreach (var reward in rewardInstances)
        {
            Destroy(reward.gameObject);
        }
        rewardInstances.Clear();

        // 创建新的奖励实例，赋值传入的 type 和 count
        foreach (var (type, count) in rewards) // 解构元组
        {
            RewardStruct newReward = Instantiate(_rewardStructPrefab, _rewardParent);
            newReward.Init(type, count);
            rewardInstances.Add(newReward);
        }
    }



    public override void OnEnd()
    {
        _blackBackground.SetActive(false);

        // 清理旧实例（如果有）
        foreach (var reward in rewardInstances)
        {
            Destroy(reward.gameObject);
        }
        rewardInstances.Clear();
    }
}

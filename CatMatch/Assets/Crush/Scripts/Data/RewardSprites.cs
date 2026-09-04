using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardDict : SerializableDictionary<RewardType, Sprite> { }


[CreateAssetMenu(fileName = "RewardSprites", menuName = "CrushDatas/RewardSprites")]
public class RewardSprites : ScriptableObject
{
    public RewardDict rewardSprites;

    public Sprite GetRewardSprite(RewardType rewardType)
    {
        return rewardSprites.ContainsKey(rewardType) ? rewardSprites[rewardType] : null;
    }
}

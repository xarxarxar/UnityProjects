using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理奖励的类型
/// </summary>
public class RewardManager : ManagerBase<RewardManager>,IManager
{
    [SerializeField] private Sprite _coinSprite;    //金币图标
    [SerializeField] private Sprite _diamondSprite; //钻石图标
    [SerializeField] private Sprite _crownSprite;  //王冠图标

    public static Dictionary<RewardType, Sprite> IconMap;

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;
    }

    public override void Init()
    {
        IconMap = new Dictionary<RewardType, Sprite>
        {
            { RewardType.Coin, _coinSprite },
            { RewardType.Diamond, _diamondSprite },
            { RewardType.Crown, _crownSprite }
        };
    }
}

/// <summary>
/// 奖励的类型
/// </summary>
public enum RewardType
{
    /// <summary>
    /// 局内金币
    /// </summary>
    Coin,
    /// <summary>
    /// 局外钻石
    /// </summary>
    Diamond,
    /// <summary>
    /// 局外王冠
    /// </summary>
    Crown
}

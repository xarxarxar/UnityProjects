using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Watermelon;

public class UserControl : MonoBehaviour
{
    public static LocalUserData localUserData;
    /// <summary>
    /// 同步玩家数据
    /// </summary>
    public static void AsyncUserLocalData()
    {
        //从微信获取数据
        TestWechat.GetUserData((WXlocalUserData) =>
        {
            localUserData = WXlocalUserData;//同步微信数据
        });


    }

    /// <summary>
    /// 是否跳过引导
    /// </summary>
    private static void JumpTour(bool isActived)
    {

    }

    /// <summary>
    /// 设置用户金币
    /// </summary>
    private static void SetPlayerCoin(int count)
    {
        CurrenciesController.Set(CurrencyType.Coins, count);
    }

    /// <summary>
    /// 设置用户最大生命值
    /// </summary>
    private static void SetPlayerMaxLife(int count)
    {
        LivesManager.SetMaxLife(count);
    }

    /// <summary>
    /// 设置用户当前生命值
    /// </summary>
    private static void SetPlayerCurrentLife(int count)
    {
        LivesManager.SetCurrentLife(count);
    }

    /// <summary>
    /// 设置用户生命值恢复间隔
    /// </summary>
    private static void SetPlayerLifeInterval(int timeInterval)
    {
        LivesManager.SetCurrentInterval(timeInterval);
    }

    /// <summary>
    /// 设置用户最大关卡数
    /// </summary>
    private static void SetPlayerMaxLevel(int level)
    {
        LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
        levelSave.MaxReachedLevelIndex = level;
    }
}

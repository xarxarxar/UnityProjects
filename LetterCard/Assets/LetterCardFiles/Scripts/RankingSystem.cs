using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 排行系统的抽象类，所有具体平台的排名系统都需要继承此类并实现其方法。
// 该类定义了排名系统的基本接口，其他平台如微信、抖音等具体实现将继承该类。
public abstract class RankingSystem
{
    // 提交玩家的得分，通常在玩家完成关卡或任务后调用。
    // level 参数表示玩家所在的关卡。
    // score 参数表示玩家在该关卡中获得的得分。
    public abstract void SubmitScore(int level, int score);

    // 获取全服排名信息，返回一个回调函数，回调中包含排名数据。
    // callback 参数是一个 Action 类型的委托，它接受一个包含排名数据的列表（List<RankData>）。
    // 排名数据包括玩家的排名、得分等信息，具体可以根据实际需求进行定义。
    public abstract void GetGlobalRanking(Action<List<RankData>> callback);

    // 获取好友排名信息，返回一个回调函数，回调中包含排名数据。
    // callback 参数是一个 Action 类型的委托，它接受一个包含排名数据的列表（List<RankData>）。
    // 排名数据包括玩家在好友圈中的排名、得分等信息，具体可以根据实际需求进行定义。
    public abstract void GetFriendRanking(Action<List<RankData>> callback);
}

// 微信排名系统实现类，继承自 RankingSystem。
// 该类实现了微信平台下的排名系统接口，例如提交分数和获取排名数据。
public class WeChatRanking : RankingSystem
{
    // 在这里实现微信平台的提交得分功能。
    // 通常通过微信SDK或者自定义API提交玩家的得分和关卡信息。
    public override void SubmitScore(int level, int score)
    {
        // 微信平台的实现代码
        // 例如调用微信SDK的接口提交玩家的得分
        Debug.Log($"微信平台提交得分：关卡 {level}，得分 {score}");
    }

    // 获取全服排名数据
    // 在微信平台上，通常会通过微信API来获取全服玩家的排名信息。
    public override void GetGlobalRanking(Action<List<RankData>> callback)
    {
        // 模拟获取全服排名数据
        List<RankData> rankingData = new List<RankData>
        {
            new RankData { playerName = "Player1", score = 100 },
            new RankData { playerName = "Player2", score = 90 }
        };

        // 调用回调函数，返回全服排名数据
        callback(rankingData);
    }

    // 获取好友排名数据
    // 在微信平台上，可以通过微信API获取与玩家好友的排名数据。
    public override void GetFriendRanking(Action<List<RankData>> callback)
    {
        // 模拟获取好友排名数据
        List<RankData> friendRankingData = new List<RankData>
        {
            new RankData { playerName = "Friend1", score = 80 },
            new RankData { playerName = "Friend2", score = 75 }
        };

        // 调用回调函数，返回好友排名数据
        callback(friendRankingData);
    }
}

// 抖音排名系统实现类，继承自 RankingSystem。
// 该类实现了抖音平台下的排名系统接口，例如提交分数和获取排名数据。
public class DouyinRanking : RankingSystem
{
    // 在这里实现抖音平台的提交得分功能。
    // 通常通过抖音SDK或者自定义API提交玩家的得分和关卡信息。
    public override void SubmitScore(int level, int score)
    {
        // 抖音平台的实现代码
        // 例如调用抖音SDK的接口提交玩家的得分
        Debug.Log($"抖音平台提交得分：关卡 {level}，得分 {score}");
    }

    // 获取全服排名数据
    // 在抖音平台上，通常会通过抖音API来获取全服玩家的排名信息。
    public override void GetGlobalRanking(Action<List<RankData>> callback)
    {
        // 模拟获取全服排名数据
        List<RankData> rankingData = new List<RankData>
        {
            new RankData { playerName = "Player1", score = 100 },
            new RankData { playerName = "Player2", score = 90 }
        };

        // 调用回调函数，返回全服排名数据
        callback(rankingData);
    }

    // 获取好友排名数据
    // 在抖音平台上，可以通过抖音API获取与玩家好友的排名数据。
    public override void GetFriendRanking(Action<List<RankData>> callback)
    {
        // 模拟获取好友排名数据
        List<RankData> friendRankingData = new List<RankData>
        {
            new RankData { playerName = "Friend1", score = 80 },
            new RankData { playerName = "Friend2", score = 75 }
        };

        // 调用回调函数，返回好友排名数据
        callback(friendRankingData);
    }
}

// 排名数据类，存储排名信息。
public class RankData
{
    // 玩家姓名
    public string playerName;

    // 玩家得分
    public int score;
}


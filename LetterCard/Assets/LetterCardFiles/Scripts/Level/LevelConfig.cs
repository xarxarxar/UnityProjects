using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

// 这个脚本定义了一个关卡配置类，可以用作ScriptableObject。
// ScriptableObject可以作为数据资源存储在项目中，方便多次使用和编辑。
[Serializable]
public class LevelConfig
{
    public int levelId;
    public string levelName;
    public int rounds;
    public int targetScore;
    [Range(0, 1)] public float specialCardProbability;
    public int maxNormalCards;
    public int maxSpecialCards;
}



// 特殊任务类，定义了一个关卡中的具体任务
public class SpecialMission
{
    // 任务类型，可能有不同类型的任务，任务类型用MissionType枚举表示
    //public MissionType type;

    // 目标字母，任务可能要求玩家在某些回合内使用特定的字母
    // 例如，可能要求使用字母"a"、"b"等，多个字母用逗号分隔
    public string targetLetters;

    // 任务要求的颜色，任务可能要求玩家在特定回合内出牌时使用特定颜色的卡牌
    // 这个字段使用ColorType枚举来定义颜色
    //public ColorType[] requiredColors;

    // 完成此任务后，玩家将获得的额外奖励分数
    public int bonusScore;
}

public enum MissionType
{
    SingleRoundColorSet,  //单回合颜色收集
    TotalLetterUsage,     //累计字母使用
    ComboChain,           //连击组合
    SpecialCardUsage      //特殊卡使用
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public List<SpecialMission> specialMissions = new List<SpecialMission>();
}



// 特殊任务类，定义了一个关卡中的具体任务
[System.Serializable]
public class SpecialMission
{
    // 任务类型，可能有不同类型的任务，任务类型用MissionType枚举表示
    public MissionType missionType;

    // 目标字母，任务可能要求玩家在某些回合内使用特定的字母
    // 例如，可能要求使用字母"a"、"b"等，多个字母用逗号分隔
    public string targetLetters;

    // 任务要求的颜色，任务可能要求玩家在特定回合内出牌时使用特定颜色的卡牌
    // 这个字段使用ColorType枚举来定义颜色
    //public ColorType[] requiredColors;

    // 完成此任务后，玩家将获得的额外奖励分数倍率
    public int bonusScore;

    public char requiredColor;    // 需要的颜色类型

    [SerializeField]
    public List<LetterColorPair> mixLetterColor = new List<LetterColorPair>();

    // 条件说明（编辑器用）
    public string conditionDescription;
}

public enum MissionType
{
    SpecificCombination,   // 指定字母组合
    SpecificColor,         // 指定颜色组合
    MixLetterAndColor,     // 混合字母颜色
    WordDictionary         // 单词字典
}

// 替换原有的元组定义，解决Unity序列化问题
[System.Serializable]
public class LetterColorPair
{
    public char letter;
    public char color;
}

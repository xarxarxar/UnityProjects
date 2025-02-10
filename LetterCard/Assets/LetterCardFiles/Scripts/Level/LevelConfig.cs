using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

// 这个脚本定义了一个关卡配置类，可以用作ScriptableObject。
// ScriptableObject可以作为数据资源存储在项目中，方便多次使用和编辑。
// 使用[CreateAssetMenu]特性，允许我们在Unity编辑器中通过右键点击来创建LevelConfig实例。
[CreateAssetMenu(fileName = "New Level Config", menuName = "Game/Level Config", order = 1)]
public class LevelConfig : ScriptableObject
{
    // 关卡ID，标识不同的关卡
    public int levelID;

    // 目标分数，玩家完成该关卡需要达到的分数
    public int targetScore;

    // 最大回合数，每个关卡最多允许多少回合
    public int maxRounds;

    // 每回合最多允许抽取的卡牌数量
    public int drawPerRound;

    // 特殊任务列表，关卡中可能有多个特殊任务，玩家需要完成这些任务来获得额外奖励
    // SpecialMission类可以定义任务类型、目标字母等信息
    public List<SpecialMission> specialMissions;

    // 有效组合列表，包含所有在此关卡中可以被认为有效的字母组合
    // ValidCombination类可以定义哪些字母组合在当前关卡有效
    //public List<ValidCombination> validCombinations;

    // 可用的特殊卡牌，玩家在此关卡中可以使用的特殊卡牌
    // SpecialCard类可以定义特殊卡牌的类型和功能
    //public List<SpecialCard> availableSpecialCards;
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

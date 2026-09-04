using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;


[System.Serializable]
public class EnemyData
{
    public Element element;
    public int star;
    public int level;

    public int common;
    public int rare;
    public int epic;
    public int legend;
}

[System.Serializable]
public class BossData
{
    public Element element;
    public int star;

    public bool immune;
}

[System.Serializable]
public class StageData
{
    public int stage;

    public EnemyData enemyData;
    public BossData bossData;
    public List<RewardData> rewardDatas;
}

[CreateAssetMenu(fileName = "StageDatas", menuName = "CrushDatas/StageDatas")]
public class StageDatas : ScriptableObject
{
    public List<StageData> stageDatas;

    public StageData GetStageData(int stage)
    {
        var stageData = stageDatas.FirstOrDefault(md => md.stage == stage);
        if (stageData == null) stageData = stageDatas[UnityEngine.Random.Range(0, stageDatas.Count)];
        return stageData;
    }
}


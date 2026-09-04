
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class LevelHelper
{
    public static BigInteger ExpNeedForNextLevel(BigInteger curLevel, BigInteger nextLevel)
    {
        var lv1 = nextLevel * (nextLevel - 1);
        var lv2 = curLevel * (curLevel - 1);

        var exp = (lv1 - lv2) * 2 + (nextLevel - curLevel) * 10;

        return exp;
    }

    public static BigInteger ExpWhenStash(BigInteger curLevel)
    {
        var exp = 2 * curLevel * (curLevel + 4);

        return exp;
    }

    public static List<BeastTeamInfo> GetStageEnemies(int stage)
    {
        List<BeastTeamInfo> datas = new List<BeastTeamInfo>();
        var beastIds = Enum.GetValues(typeof(BeastId)).OfType<BeastId>().ToList();

        var stageData = GameData.Instance.stageDatas.GetStageData(stage);

        var enemyData = stageData.enemyData;
        var common = enemyData.common;
        var rare = enemyData.rare;
        var epic = enemyData.epic;
        var legend = enemyData.legend;

        for (int k = 0; k < beastIds.Count; k++)
        {
            var beastId = beastIds[k];
            var beast = BeastPrefs.Instance.GetBeastPref(beastId);
            var beastBase = beast.GetComponent<BeastBase>();
            if (beastBase.element == enemyData.element)
            {
                // Debug.Log($"element:{beastBase.element} common:{common} rare:{rare} epic:{epic} legend:{legend}");
                if (beastBase.rarity == Rarity.Common)
                {
                    Debug.Log("common:" + common);
                    if (common > 0) common--;
                    else continue;
                    Debug.Log("common 2:" + common);
                }
                else if (beastBase.rarity == Rarity.Rare)
                {
                    if (rare > 0) rare--;
                    else continue;
                }
                else if (beastBase.rarity == Rarity.Epic)
                {
                    if (epic > 0) epic--;
                    else continue;
                }
                else if (beastBase.rarity == Rarity.Legend)
                {
                    if (legend > 0) legend--;
                    else continue;
                }

                Debug.Log($"element 2:{beastBase.element} common:{common} rare:{rare} epic:{epic} legend:{legend}");

                datas.Add(new BeastTeamInfo()
                {
                    beastId = beastId,
                    curStar = enemyData.star,
                    curLevel = enemyData.level
                });

                beastIds.Remove(beastId);
                k--;
            }
        }
        Debug.Log("Enemies Data:" + datas.Count);

        return datas;
    }

}
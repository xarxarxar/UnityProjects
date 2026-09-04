
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using BigIntCSharp = System.Numerics.BigInteger;

public class TeamBuffHelper
{
    public static int GenKey(Element buffForElement, BeastClass buffForClass, int teamBuff)
    {
        int tmp = 0;
        if (buffForElement != Element.None)
            tmp = (int)buffForElement;
        else if (buffForClass != BeastClass.None)
        {
            var elementLength = Enum.GetNames(typeof(Element)).Length;
            tmp = elementLength + (int)buffForClass;
        }

        return tmp * 1000 + teamBuff;
    }

    public static void RevertFromKey(int key, out Element buffFor, out BeastClass beastClass, out TeamBuff teamBuff)
    {
        beastClass = BeastClass.None;
        buffFor = Element.None;

        var elementLength = Enum.GetNames(typeof(Element)).Length;
        var vl = key / 1000;
        if (vl > elementLength)
        {
            beastClass = (BeastClass)(vl - elementLength);
        }
        else
        {
            buffFor = (Element)(key / 1000);
        }

        teamBuff = (TeamBuff)(key % 1000);
    }

    private BigIntCSharp GetAverageHP(List<BeastTeamInfo> beastTeamInfos, Dictionary<int, float> myTeamBuffData, Dictionary<int, float> enemyTeamBuffData, bool isMyTeam)
    {
        var main = isMyTeam ? myTeamBuffData : enemyTeamBuffData;
        var sub = isMyTeam ? enemyTeamBuffData : myTeamBuffData;
        BigIntCSharp res;

        for (int k = 0; k < beastTeamInfos.Count; k++)
        {
            var beastTeamInfo = beastTeamInfos[k];
            var beast = BeastPrefs.Instance.GetBeastPref(beastTeamInfo.beastId).GetComponent<BeastBase>();
            var beastData = beastTeamInfos.FirstOrDefault(dt => dt.beastId == beast.beastId);
            var newHp = StatHelper.GetBaseAtribute(beast.hp, beastData.curStar, beastData.curLevel, out _, out _);

            var perHp = 0f;
            foreach (var data in main)
            {
                var key = data.Key;
                TeamBuffHelper.RevertFromKey(key, out Element buffFor, out BeastClass buffForClass, out TeamBuff buffType);
                if (buffFor == beast.element || buffFor == Element.All || buffForClass == beast.beastClass)
                {
                    switch (buffType)
                    {
                        case TeamBuff.Hp:
                            perHp += data.Value;
                            break;
                    }
                }
            }

            foreach (var data in sub)
            {
                var key = data.Key;
                TeamBuffHelper.RevertFromKey(key, out Element buffFor, out BeastClass buffForClass, out TeamBuff buffType);
                if (buffFor == beast.element || buffFor == Element.All || buffForClass == beast.beastClass)
                {
                    switch (buffType)
                    {
                        case TeamBuff.EnemyHp:
                            perHp += data.Value;
                            break;
                    }
                }
            }

            newHp = BigIntegerHelper.BigMultiplyFloat(newHp, (1 + perHp * 0.01f));
            res += newHp;
            Debug.Log("GetAverageHP " + beast.beastId + " : " + newHp);
        }
        res = res / beastTeamInfos.Count;
        Debug.Log("GetAverageHP isMyTeam " + isMyTeam + " : " + res);
        return res;
    }
}
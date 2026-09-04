
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MergeBeast;
using UnityEngine;

public struct StatsInfo
{
    public string name;
    // public int baseValue;
    // public BigInteger moreValue;
    // public BigInteger totalValue;
    // public double morePercent;

    public string moreValueStr;
    public string totalValueStr;
    public string morePercentStr;
}

[System.Serializable]
public class BeastTeamInfo
{
    public BeastId beastId;

    public int curStar;
    public BigInteger curMedal;

    public BigInteger curLevel;

    public GameObject beastItemViewGo;
    public int fakeLane=-1;

    public string ToData()
    {
        return (int)beastId + "|" + curStar + "|" + curMedal + "|" + curLevel;
    }

    public static BeastTeamInfo FromData(string data)
    {
        var dts = data.Split('|');

        return new BeastTeamInfo()
        {
            beastId = (BeastId)int.Parse(dts[0]),
            curStar = int.Parse(dts[1]),
            curMedal = BigInteger.Parse(dts[2]),
            curLevel = BigInteger.Parse(dts[3]),
        };
    }

    public override string ToString()
    {
        return beastId + " curStar:" + curStar + " curMedal:" + curMedal + " curLevel:" + curLevel;
    }

    public Dictionary<int, StatsInfo> CalculateStats(BeastBase beastBase)
    {
        Dictionary<int, StatsInfo> res = new Dictionary<int, StatsInfo>();
        var statNames = Enum.GetNames(typeof(StatNames));

        // BigInteger moreValue, totalValue, morePercentBigInt;
        // double morePercent = 0d;

        for (int i = 0; i < statNames.Length; i++)
        {
            // totalValue = 0;
            // moreValue = 0;
            // morePercent = 0d;
            // morePercentBigInt = 0;

            BigInteger newBase = 0;
            BigInteger moreValue = 0;

            var st = new StatsInfo()
            {
                morePercentStr = "",
                totalValueStr = "",
                moreValueStr = ""
            };
            AbilityModel abilityModel;

            switch ((StatNames)i)
            {
                case StatNames.Atk:// int hiển thị int

                    StatHelper.GetBaseAtribute(beastBase.atk, curStar, curLevel, out newBase, out moreValue);
                    st = BigIntToBigInt(newBase + moreValue, moreValue);
                    break;
                case StatNames.Def:// int hiển thị int
                    StatHelper.GetBaseAtribute(beastBase.def, curStar, curLevel, out newBase, out moreValue);
                    st = BigIntToBigInt(newBase + moreValue, moreValue);
                    break;
                case StatNames.Hp:// int hiển thị int
                    StatHelper.GetBaseAtribute(beastBase.hp, curStar, curLevel, out newBase, out moreValue);
                    st = BigIntToBigInt(newBase + moreValue, moreValue);
                    break;
                case StatNames.CritRate: // % hiển thị ra %
                    st = PercentToPercent(beastBase.critRate, 0);
                    break;
                case StatNames.CriteDamage:// float hiển thị ra %
                    st = FloatToPercent(beastBase.critFactor, 0);
                    break;
                case StatNames.AtkPerSec:// float hiển thị float
                    st = FloatToFloat(beastBase.attackRate, 0);
                    break;
                case StatNames.AtkRange:// float hiển thị float
                    st = FloatToFloat(beastBase.attackRange, 0);
                    break;
                case StatNames.MoveSpeed:// float hiển thị float
                    st = FloatToFloat(beastBase.speed, 0);
                    break;
                case StatNames.EffectResistance:
                    break;
                case StatNames.FrenzyChance:
                    break;
                case StatNames.DodgeRate:// % hiển thị ra %
                    st = PercentToPercent(beastBase.doge, 0);
                    break;
                case StatNames.StunChance:// số int nhưng hiển thị ra %
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Stun);
                    if (abilityModel != null)
                        st = PercentToPercent(abilityModel.chance, 0);
                    break;
                case StatNames.StunTime:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Stun);
                    if (abilityModel != null)
                        st = FloatToFloat(abilityModel.time, 0);
                    break;
                case StatNames.AoERadius:
                    break;
                case StatNames.AoEDmg:
                    break;
                case StatNames.UltimateAtk:
                    break;
                case StatNames.KnightShieldHP:
                    break;
                case StatNames.BonusLoot:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.ExtraLoot);
                    if (abilityModel != null)
                        st = FloatToFloat(abilityModel.value, 0);
                    break;
                case StatNames.FreezeChance:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Freeze);
                    if (abilityModel != null)
                        st = PercentToPercent(abilityModel.chance, 0);
                    abilityModel = beastBase.GetAbilityModel(AbilityType.FreezeExplode);
                    if (abilityModel != null)
                        st = PercentToPercent(abilityModel.chance, 0);
                    break;
                case StatNames.FreezeTime:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Freeze);
                    if (abilityModel != null)
                        st = FloatToFloat(abilityModel.time, 0);
                    abilityModel = beastBase.GetAbilityModel(AbilityType.FreezeExplode);
                    if (abilityModel != null)
                        st = FloatToFloat(abilityModel.time, 0);
                    break;
                case StatNames.FreezeExplotionDmg:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.FreezeExplode);
                    if (abilityModel != null)
                        st = PercentToPercent(abilityModel.value, 0);
                    break;
                case StatNames.BurnChance:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Burn);
                    if (abilityModel != null)
                        st = PercentToPercent(abilityModel.chance, 0);
                    break;
                case StatNames.BurnTime:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Burn);
                    if (abilityModel != null)
                        st = FloatToFloat(abilityModel.time, 0);
                    break;
                case StatNames.BurnDmg:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Burn);
                    if (abilityModel != null)
                        st = FloatToPercent(abilityModel.value, 0);
                    break;
                case StatNames.PoisonChance:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Poison);
                    if (abilityModel != null)
                        st = PercentToPercent(abilityModel.chance, 0);
                    break;
                case StatNames.PoisonTime:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Poison);
                    if (abilityModel != null)
                        st = FloatToFloat(abilityModel.time, 0);
                    break;
                case StatNames.PoisonDmg:
                    abilityModel = beastBase.GetAbilityModel(AbilityType.Poison);
                    if (abilityModel != null)
                        st = FloatToPercent(abilityModel.value, 0);
                    break;
            }

            st.name = ((StatNames)i).ToString();
            res.Add(i, st);
        }
        return res;
    }

    private StatsInfo BigIntToBigInt(BigInteger totalValue, BigInteger moreValue)
    {
        var st = new StatsInfo();
        st.totalValueStr = Utils.FormatNumber(totalValue);
        st.moreValueStr = moreValue == 0 ? "" : Utils.FormatNumber(moreValue);
        int div = (int)(moreValue * 100_00 / totalValue);
        float morePercent = div / 100;
        st.morePercentStr = Helper.FloatPercentFormat(morePercent);
        return st;
    }

    private StatsInfo PercentToPercent(float totalValue, float moreValue)
    {
        var st = new StatsInfo();
        st.totalValueStr = Helper.FloatPercentFormat(totalValue);
        st.moreValueStr = Helper.FloatPercentFormat(moreValue);
        float morePercent = totalValue > 0 ? moreValue / totalValue : 0;
        st.morePercentStr = Helper.FloatPercentFormat(moreValue * 100);
        return st;
    }

    private StatsInfo FloatToPercent(float totalValue, float moreValue)
    {
        var st = new StatsInfo();
        st.totalValueStr = Helper.FloatPercentFormat(totalValue * 100);
        st.moreValueStr = Helper.FloatPercentFormat(moreValue * 100);
        var moreVl = totalValue > 0 ? moreValue / totalValue : 0;
        st.morePercentStr = Helper.FloatPercentFormat(moreValue * 100);
        return st;
    }

    private StatsInfo FloatToFloat(float totalValue, float moreValue)
    {
        var st = new StatsInfo();
        st.totalValueStr = Helper.FloatFormat(totalValue, true);
        st.moreValueStr = Helper.FloatFormat(moreValue, false);
        var moreVl = totalValue > 0 ? moreValue / totalValue : 0;
        st.morePercentStr = Helper.FloatPercentFormat(moreVl);
        return st;
    }
}
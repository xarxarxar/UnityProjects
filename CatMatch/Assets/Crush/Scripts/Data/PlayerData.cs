
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Numerics;
using System;
using Observer;

public class MyShop
{
    public string date;
    public List<int> beastIds;

    public int chestPremium;
    public int chestWater;
    public int chestFire;
    public int chestGrass;

    public int atlantusStock;
    public int furiosaStock;
    public int groovineStock;
    public int darkHunterStock;
    public int merlinusStock;
    public int scudStock;

    public int natureStock;
    public int vikingStock;
    public int navarreStock;
    public int assassinStock;
    public int iceStock;
    public int solarStock;
    public int valkenStock;
    public int voodooStock;

    public int chestPremiumBuy;
    public int chestWaterBuy;
    public int chestFireBuy;
    public int chestGrassBuy;

    public int atlantusStockBuy;
    public int furiosaStockBuy;
    public int groovineStockBuy;
    public int darkHunterStockBuy;
    public int merlinusStockBuy;
    public int scudStockBuy;

    public int natureStockBuy;
    public int vikingStockBuy;
    public int navarreStockBuy;
    public int assassinStockBuy;
    public int iceStockBuy;
    public int solarStockBuy;
    public int valkenStockBuy;
    public int voodooStockBuy;

    public int chestPremiumUse;
    public int chestWaterUse;
    public int chestFireUse;
    public int chestGrassUse;

    public int atlantusStockUse;
    public int furiosaStockUse;
    public int groovineStockUse;
    public int darkHunterStockUse;
    public int merlinusStockUse;
    public int scudStockUse;

    public int natureStockUse;
    public int vikingStockUse;
    public int navarreStockUse;
    public int assassinStockUse;
    public int iceStockUse;
    public int solarStockUse;
    public int valkenStockUse;
    public int voodooStockUse;

    public int shopItemDayth;

    public bool tutorial;

    public void Reset()
    {
        // chestPremium = chestWater = chestFire = chestGrass = 0;
        // atlantusStock = furiosaStock = groovineStock = 0;
        // natureStock = vikingStock = navarreStock = 0;
        // assassinStock = iceStock = solarStock = 0;
        // valkenStock = voodooStock = 0;

        chestPremiumBuy = chestWaterBuy = chestFireBuy = chestGrassBuy = 0;
        atlantusStockBuy = furiosaStockBuy = groovineStockBuy = 0;
        natureStockBuy = vikingStockBuy = navarreStockBuy = 0;
        assassinStockBuy = iceStockBuy = solarStockBuy = 0;
        valkenStockBuy = voodooStockBuy = 0;

        // chestPremiumUse = chestWaterUse = chestFireUse = chestGrassUse = 0;
        // atlantusStockUse = furiosaStockUse = groovineStockUse = 0;
        // natureStockUse = vikingStockUse = navarreStockUse = 0;
        // assassinStockUse = iceStockUse = solarStockUse = 0;
        // valkenStockUse = voodooStockUse = 0;
    }
}

public class PlayerData : ScriptableObject
{
    public static readonly PlayerData instance = new PlayerData();
    public int MaxStagePerRegion = 10;
    public int MaxRegion = 4;// region từ 0
    public int GemFirstWin = 10;

    private int maxStage;
    public int MaxStage
    {
        get { return maxStage; }
        set
        {
            maxStage = value;
            PlayerPrefs.SetInt(CrushStringHelper.MaxStage, maxStage);
            //PlayerPrefs.Save();
        }
    }

    public int CurStage;

    private BigInteger curExp;
    public BigInteger CurExp
    {
        get { return curExp; }
        set
        {
            curExp = value;
            PlayerPrefs.SetString(CrushStringHelper.CurExp, curExp.ToString());
            //PlayerPrefs.Save();
        }
    }

    private int currentTeam;
    public int CurrentTeam
    {
        get { return currentTeam; }
        set
        {
            currentTeam = value;
            PlayerPrefs.SetInt(CrushStringHelper.CurrentTeam, currentTeam);
            //PlayerPrefs.Save();
        }
    }

    private int openSlotAmount;
    public int OpenSlotAmount
    {
        get { return openSlotAmount; }
        set
        {
            openSlotAmount = value;
            PlayerPrefs.SetInt(CrushStringHelper.OpenSlotAmount, openSlotAmount);
            //PlayerPrefs.Save();
        }
    }

    private int playStageAmount;
    public int PlayStageAmount
    {
        get { return playStageAmount; }
        set
        {
            if (value >= 10) value = 10;

            playStageAmount = value;

            PlayerPrefs.SetInt(CrushStringHelper.PlayStageAmount, playStageAmount);
            EventDispatcher.Instance.PostEvent(EventID.PlayAmountChange, playStageAmount);
            //PlayerPrefs.Save();
        }
    }

    private int sweepAmount;
    public int SweepAmount
    {
        get { return sweepAmount; }
        set
        {
            sweepAmount = value;
            PlayerPrefs.SetInt(CrushStringHelper.SweepAmount, sweepAmount);
            EventDispatcher.Instance.PostEvent(EventID.SweepChange, sweepAmount);
            //PlayerPrefs.Save();
        }
    }

    private int tileTicketAmount;
    public int TileTicketAmount
    {
        get { return tileTicketAmount; }
        set
        {
            tileTicketAmount = value;
            PlayerPrefs.SetInt(CrushStringHelper.TileTicketAmount, tileTicketAmount);
            EventDispatcher.Instance.PostEvent(EventID.TileTicketChange, tileTicketAmount);
            //PlayerPrefs.Save();
        }
    }

    public String FirstWinStages
    {
        get { return PlayerPrefs.GetString(CrushStringHelper.FirstWinStages, ""); }
        set
        {
            PlayerPrefs.SetString(CrushStringHelper.FirstWinStages, value);
        }
    }

    public MyShop MyShop
    {
        get
        {
            var ms = PlayerPrefs.GetString(CrushStringHelper.MyShop, "");
            MyShop myShop = null;
            if (ms != "")
            {
                myShop = JsonUtility.FromJson<MyShop>(ms);
            }

            return myShop;
        }
        set
        {
            PlayerPrefs.SetString(CrushStringHelper.MyShop, JsonUtility.ToJson(value));
        }
    }

    private readonly Dictionary<int, List<BeastId>> teamBeast;

    private Dictionary<BeastId, BeastTeamInfo> beastDatas { get; set; } = new Dictionary<BeastId, BeastTeamInfo>();

    private readonly Dictionary<int, StageModel> myStagePair;

    public PlayerData()
    {
        teamBeast = new Dictionary<int, List<BeastId>>();
        myStagePair = new Dictionary<int, StageModel>();

        LoadBeastDatas();
        LoadTeams();
        LoadMyStages();

        openSlotAmount = PlayerPrefs.GetInt(CrushStringHelper.OpenSlotAmount, 5);
        currentTeam = PlayerPrefs.GetInt(CrushStringHelper.CurrentTeam, 1);
        var expStr = PlayerPrefs.GetString(CrushStringHelper.CurExp);

        if (!string.IsNullOrEmpty(expStr))
            curExp = BigInteger.Parse(expStr);
        else curExp = 0;

        playStageAmount = PlayerPrefs.GetInt(CrushStringHelper.PlayStageAmount, 10);
        sweepAmount = PlayerPrefs.GetInt(CrushStringHelper.SweepAmount, 5);
        maxStage = PlayerPrefs.GetInt(CrushStringHelper.MaxStage, 1);
        tileTicketAmount = PlayerPrefs.GetInt(CrushStringHelper.TileTicketAmount, 200);

        Debug.Log("curExp:" + curExp);

        // fake
        if (beastDatas.Count == 0)
        {
            // var bns = Enum.GetNames(typeof(BeastId));

            // for (int i = 0; i < bns.Length; i++)
            // {
            //     var bId = (BeastId)i;
            //     if(bId == BeastId.Onyx) continue;

            //     AddBeastData((BeastId)i, UnityEngine.Random.Range(0, 500));
            // }
            // AddBeastData(BeastId.Chaos, 80);
        }

        // maxStage = 13;
        // for (int k = 1; k <= 12; k++)
        // {
        //     AddStage(new StageModel() { numStar = UnityEngine.Random.Range(1, 4), stage = k }, true);
        // }
    }

    public List<BeastTeamInfo> GetBeastTeamInfos()
    {
        return beastDatas.Values.ToList();
    }

    public BeastTeamInfo GetBeastData(BeastId beastId)
    {
        return beastDatas.ContainsKey(beastId) ? beastDatas[beastId] : null;
    }

    private void AddBeastData(BeastTeamInfo beastTeamInfo)
    {
        if (beastDatas.ContainsKey(beastTeamInfo.beastId))
            beastDatas[beastTeamInfo.beastId] = beastTeamInfo;
        else
            beastDatas.Add(beastTeamInfo.beastId, beastTeamInfo);

        SaveBeastDatas();
        AddBeastToAllTeam(beastTeamInfo);
    }

    public BigInteger AddBeastData(BeastId beastId, int medalAmount, bool yourBuild = false)
    {
        var beastData = GetBeastData(beastId);
        if (beastData == null)
        {
            beastData = new BeastTeamInfo()
            {
                beastId = beastId,
                curMedal = medalAmount
            };
        }
        else
        {
            beastData.curMedal += medalAmount;
        }
        // Debug.Log("beastId:" + beastId + " medal:" + beastData.curMedal);

        if (!yourBuild)
        {
            for (int k = 0; k < Constant.medalByStars.Count; k++)
            {
                if (beastData.curStar < k + 1)
                {
                    if (beastData.curMedal >= Constant.medalByStars[k])
                    {
                        beastData.curStar = k + 1;
                        beastData.curMedal -= Constant.medalByStars[k];
                    }
                    else break;
                }
            }
        }

        // if (beastData.curStar == 0 && beastData.curMedal >= Constant.medalByStars[0])
        // {
        //     beastData.curStar = 1;
        //     beastData.curMedal -= Constant.medalByStars[0];
        // }
        AddBeastData(beastData);

        return beastData.curMedal;
    }

    public void Evolve(BeastId beastId)
    {
        var beastData = GetBeastData(beastId);
        if (beastData == null) return;

        for (int k = 0; k < Constant.medalByStars.Count; k++)
        {
            if (beastData.curStar < k + 1)
            {
                if (beastData.curMedal >= Constant.medalByStars[k])
                {
                    beastData.curStar = k + 1;
                    beastData.curMedal -= Constant.medalByStars[0];
                }
                else break;
            }
        }
    }

    private void AddBeastToAllTeam(BeastTeamInfo beastTeamInfo)
    {
        if (beastTeamInfo.curStar >= 1)
        {
            for (int k = 0; k < teamBeast.Count; k++)
            {
                var team = teamBeast.ElementAt(k);
                var slot = team.Value.Count;
                if (slot < openSlotAmount && !team.Value.Contains(beastTeamInfo.beastId))
                {
                    team.Value.Add(beastTeamInfo.beastId);
                    SaveTeams();
                }
            }
        }
    }

    public void SaveBeastDatas()
    {
        var beastDatas = this.beastDatas.Values.ToList();

        PlayerPrefs.SetInt(CrushStringHelper.BeastDataCount, beastDatas.Count);
        for (int i = 0; i < beastDatas.Count; i++)
        {
            // Debug.Log("SaveBeastDatas:" + beastDatas[i].ToData());
            PlayerPrefs.SetString(CrushStringHelper.BeastData + i, beastDatas[i].ToData());
        }
        //PlayerPrefs.Save();
    }

    private void LoadBeastDatas()
    {
        var count = PlayerPrefs.GetInt(CrushStringHelper.BeastDataCount);
        for (int i = 0; i < count; i++)
        {
            var json = PlayerPrefs.GetString(CrushStringHelper.BeastData + i);
            // Debug.Log("LoadBeastDatas:" + json);

            var beastData = BeastTeamInfo.FromData(json);
            beastDatas.Add(beastData.beastId, beastData);
        }
    }

    public Dictionary<int, List<BeastId>> GetTeams()
    {
        return teamBeast;
    }

    public void LoadTeams()
    {
        var count = PlayerPrefs.GetInt(CrushStringHelper.Team);
        for (int i = 0; i < count; i++)
        {
            var key = PlayerPrefs.GetInt(CrushStringHelper.TeamKey + i);
            var vl = PlayerPrefs.GetString(CrushStringHelper.TeamValue + i);

            // Debug.Log("key:" + key + " vl:" + vl);

            if (string.IsNullOrEmpty(vl))
                teamBeast.Add(key, new List<BeastId>());
            else
            {
                var values = vl.Split('|').Select(abc => (BeastId)(int.Parse(abc))).ToList();
                teamBeast.Add(key, values);
            }
        }
        if (teamBeast.Count() == 0)
        {
            teamBeast.Add(1, new List<BeastId>());
            teamBeast.Add(2, new List<BeastId>());
        }
    }

    public void SaveTeams()
    {
        PlayerPrefs.SetInt(CrushStringHelper.Team, teamBeast.Count);
        for (int i = 0; i < teamBeast.Count; i++)
        {
            var team = teamBeast.ElementAt(i);

            PlayerPrefs.SetInt(CrushStringHelper.TeamKey + i, team.Key);

            var values = team.Value.Select(abc => (int)abc);

            // Debug.Log("string:" + string.Join("|", values));

            PlayerPrefs.SetString(CrushStringHelper.TeamValue + i, string.Join("|", values));
        }
        //PlayerPrefs.Save();
    }

    public List<BeastTeamInfo> GetCurTeam()
    {
        List<BeastTeamInfo> res = new List<BeastTeamInfo>();
        if (teamBeast.ContainsKey(currentTeam))
        {
            var beastIds = teamBeast[currentTeam];
            for (int k = 0; k < beastIds.Count; k++)
            {
                Debug.Log("GetCurTeam:" + beastIds[k]);

                res.Add(beastDatas[beastIds[k]]);
            }
        }

        return res;
    }

    public List<BeastTeamInfo> GetBeastPlayerTeam(int team)
    {
        List<BeastTeamInfo> res = new List<BeastTeamInfo>();
        if (teamBeast.ContainsKey(team))
        {
            var beastIds = teamBeast[team];
            for (int k = 0; k < beastIds.Count; k++)
            {
                Debug.Log("GetBeastPlayerTeam:" + beastIds[k]);

                res.Add(beastDatas[beastIds[k]]);
            }
        }

        return res;
    }

    public Dictionary<int, StageModel> GetMyStagePair()
    {
        return myStagePair;
    }

    public void AddStage(StageModel missionModel, bool save)
    {
        if (myStagePair.ContainsKey(missionModel.stage))
        {
            myStagePair[missionModel.stage].numStar = myStagePair[missionModel.stage].numStar > missionModel.numStar ?
                myStagePair[missionModel.stage].numStar : missionModel.numStar;
        }
        else
            myStagePair.Add(missionModel.stage, missionModel);

        foreach (var ms in myStagePair)
        {
            // Debug.Log("AddStage:" + ms.Value.stage);
        }

        if (save) SaveMyStages();
    }

    public void SaveMyStages()
    {
        var _myMissions = this.myStagePair.Values.ToList();

        PlayerPrefs.SetInt(CrushStringHelper.MyStageCount, _myMissions.Count);
        for (int i = 0; i < _myMissions.Count; i++)
        {
            var st1 = JsonUtility.ToJson(_myMissions[i]);
            // Debug.Log("SaveMyStages:" + st1);

            PlayerPrefs.SetString(CrushStringHelper.MyStage + i, st1);
        }
        //PlayerPrefs.Save();
    }

    private void LoadMyStages()
    {
        var count = PlayerPrefs.GetInt(CrushStringHelper.MyStageCount);
        for (int i = 0; i < count; i++)
        {
            var st1 = PlayerPrefs.GetString(CrushStringHelper.MyStage + i);
            // Debug.Log("LoadMyStages:" + st1);

            var missonModel = JsonUtility.FromJson<StageModel>(st1);
            myStagePair.Add(missonModel.stage, missonModel);
        }
    }
}
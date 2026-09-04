using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MergeBeast;
using UnityEngine;

public class RewardHelper
{
    public static Dictionary<RewardType, BeastId> rewardBeastPair = new Dictionary<RewardType, BeastId>()
    {

        {RewardType.Atlantus, BeastId.Atlantus}, {RewardType.BlackBeard, BeastId.BlackBeard}, {RewardType.BugonautArcher, BeastId.BugonautArcher},
        {RewardType.Chaos, BeastId.Chaos},
        {RewardType.GrooVine, BeastId.GrooVine}, {RewardType.IceKnight, BeastId.IceKnight}, {RewardType.Kage, BeastId.Kage},
        {RewardType.Magmus, BeastId.Magmus}, {RewardType.MechaValken, BeastId.MechaValken}, {RewardType.Neko, BeastId.Neko},
        {RewardType.RobinHood, BeastId.RobinHood}, {RewardType.Spike, BeastId.Spike}, {RewardType.Thorn, BeastId.Thorn},
        {RewardType.VulcanArcher, BeastId.VulcanArcher},
        {RewardType.BlueFish, BeastId.BlueFish}, {RewardType.Valkyrie, BeastId.Valkyrie}, {RewardType.Pirato, BeastId.Pirato},
        {RewardType.Akwa, BeastId.Akwa}, {RewardType.Misty, BeastId.Misty}, {RewardType.Scud, BeastId.Scud},
        {RewardType.MonkeyKing, BeastId.MonkeyKing}, {RewardType.LeafBlade, BeastId.LeafBlade}, {RewardType.Furiosa, BeastId.Furiosa},
         {RewardType.SiegFried, BeastId.SiegFried}, {RewardType.OneEye, BeastId.OneEye}, {RewardType.FrostQueen, BeastId.FrostQueen},
          {RewardType.Onyx, BeastId.Onyx}, {RewardType.Merlinus, BeastId.Merlinus}, {RewardType.Circle, BeastId.Circle},
          {RewardType.DarkHunter, BeastId.DarkHunter}, {RewardType.Vlad, BeastId.Vlad}, {RewardType.Sorrow, BeastId.Sorrow},
    };

    public static void ReceiveRewards(RewardData data, bool yourBuild)
    {
        try
        {
            ReceiveRewards(data.rewardType, data.amount, yourBuild);
        }
        catch (UnityException ex)
        {
            Debug.LogError(ex.StackTrace);
        }
    }

    public static void ReceiveRewards(RewardModel data, bool yourBuild)
    {
        try
        {
            ReceiveRewards(data.rewardType, data.amount, yourBuild);
        }
        catch (UnityException ex)
        {
            Debug.LogError(ex.StackTrace);
        }
    }

    private static void ReceiveRewards(RewardType rewardType, BigInteger amount, bool yourBuild)
    {
        if ((int)rewardType <= (int)RewardType.Sorrow)
        {
            // var beastId = rewardBeastPair[rewardType];
            PlayerData.instance.AddBeastData((BeastId)((int)(rewardType)), (int)amount, yourBuild);
        }
        else
        {
            switch (rewardType)
            {
                case RewardType.Gold://soul
                    MergeBeast.UIManager.Instance?.UpdateMoneyCoin(amount, true);
                    break;
                case RewardType.Gem:
                    MergeBeast.Utils.AddRubyCoin((int)amount);
                    break;
                case RewardType.PlayAmount: //Play ticket
                    PlayerData.instance.PlayStageAmount += (int)amount;
                    break;
                case RewardType.SweepAmount://sweep ticket
                    PlayerData.instance.SweepAmount += (int)amount;
                    break;
                case RewardType.AutoMerge:
                    MergeBeast.GameManager.Instance?.AddBoostToInventory((int)EnumDefine.BOOST.AUTO_MERGE_1, (int)amount);
                    break;
                case RewardType.BoostDPS:
                    // BoostManager.Instance?.AddBoost(EnumDefine.BOOST.DAMAGE_BOOST_1);
                    MergeBeast.GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.DAMAGE_BOOST_1, (int)amount);
                    break;
                case RewardType.BoostTime:
                    MergeBeast.GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.SPAWN_FASTER_1, (int)amount);
                    // BoostManager.Instance?.AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_TIME);
                    break;
                case RewardType.PackSoul:
                    // BoostManager.Instance?.AddBoost(EnumDefine.BOOST.PACK_SOUL);
                    break;
                case RewardType.Undo:
                    amount += PlayerPrefs.GetInt(StringDefine.UNDO_AMOUNT, 10);
                    PlayerPrefs.SetInt(StringDefine.UNDO_AMOUNT, (int)amount);
                    break;
                case RewardType.Shuffle:
                    amount += PlayerPrefs.GetInt(StringDefine.SHUFFLE_AMOUNT, 10);
                    PlayerPrefs.SetInt(StringDefine.SHUFFLE_AMOUNT, (int)amount);
                    break;
                case RewardType.Commit:
                    CPlayer.AddCommission((int)amount);
                    break;
                case RewardType.Revive:
                    CPlayer.AddRevival((int)amount);
                    break;
                case RewardType.MergeMedal:
                    MergeBeast.Utils.AddMedalMerge((int)amount);
                    break;
                case RewardType.TileTicket:
                    PlayerData.instance.TileTicketAmount += (int)amount;
                    break;
                case RewardType.EXP:
                    PlayerData.instance.CurExp += amount;
                    break;
            }
        }
    }

    public static List<RewardModel> GetStageRewards(int stage, List<BeastTeamInfo> enemyMonsterDatas)
    {
        var rewardModels = new List<RewardModel>();
        var beastMedalRewards = new List<RewardModel>();

        // var curStage = PlayerData.instance.CurStage;
        // var stageData = GameData.Instance.stageDatas.GetStageData(curStage);
        // GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.AUTO_MERGE_1, value);// add auto merge
        // MergeBeast.UIManager.Instance.UpdateMoneyCoin();// add soul
        // MergeBeast.Utils.AddMedalMerge()// add medal merge

        var rewardModel = new RewardModel() { amount = BigInteger.Pow(2, stage) * 10, rewardType = RewardType.Gold };
        rewardModels.Add(rewardModel);
        ReceiveRewards(rewardModel, false);

        rewardModel = new RewardModel() { amount = 10, rewardType = RewardType.TileTicket };
        rewardModels.Add(rewardModel);
        ReceiveRewards(rewardModel, false);

        // MergeBeast.UIManager.Instance.UpdateMoneyCoin(rewardModel.amount, true);

        int am = (int)(500 * Mathf.Pow(1.05f, stage - 1));
        Debug.Log("GetStageRewards exp:" + am + " stage:" + stage);
        rewardModel = new RewardModel() { amount = am, rewardType = RewardType.EXP };
        rewardModels.Add(rewardModel);
        ReceiveRewards(rewardModel, false);

        var firstWinStages = PlayerData.instance.FirstWinStages;
        var fws = firstWinStages.Split('|').ToList();
        if (!fws.Contains(stage.ToString()))
        {
            PlayerData.instance.FirstWinStages = firstWinStages + "|" + stage;
            Debug.Log("GetStageRewards  PlayerData.instance.FirstWinStages:" + PlayerData.instance.FirstWinStages);

            rewardModel = new RewardModel() { amount = PlayerData.instance.GemFirstWin, rewardType = RewardType.Gem };
            rewardModels.Add(rewardModel);
            ReceiveRewards(rewardModel, false);

            if (stage == 1)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.RobinHood };
                rewardModels.Add(rewardModel);
                beastMedalRewards.Add(rewardModel);
            }
            else if (stage == 5)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.Pirato };
                rewardModels.Add(rewardModel);
                beastMedalRewards.Add(rewardModel);
            }
            else if (stage == 10)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.MonkeyKing };
                rewardModels.Add(rewardModel);
                beastMedalRewards.Add(rewardModel);
            }
            else if (stage == 15)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.Merlinus };
                rewardModels.Add(rewardModel);
                beastMedalRewards.Add(rewardModel);
            }
        }

        var bns = Enum.GetNames(typeof(BeastId));
        List<BeastId> commonBeastIds = new List<BeastId>();
        List<BeastId> allBeastIds = new List<BeastId>();
        for (int i = 0; i < bns.Length; i++)
        {
            var bId = (BeastId)i;
            var beastPref = BeastPrefs.Instance.GetBeastPref(bId);
            var bb = beastPref.GetComponent<BeastBase>();
            if (bb.rarity == Rarity.Common)
            {
                commonBeastIds.Add(bb.beastId);
            }
            allBeastIds.Add(bb.beastId);
        }

        var cm = commonBeastIds[UnityEngine.Random.Range(0, commonBeastIds.Count)];
        var prevReward = beastMedalRewards.FirstOrDefault(rw => rw.rewardType == (RewardType)((int)cm));
        if (prevReward != null) prevReward.amount += 1;
        else
        {
            rewardModel = new RewardModel() { amount = 1, rewardType = (RewardType)((int)cm) };
            rewardModels.Add(rewardModel);
            beastMedalRewards.Add(rewardModel);
        }

        if (stage >= 15)
        {
            var rd = allBeastIds[UnityEngine.Random.Range(0, allBeastIds.Count)];
            prevReward = beastMedalRewards.FirstOrDefault(rw => rw.rewardType == (RewardType)((int)rd));
            if (prevReward != null) prevReward.amount += 1;
            else
            {
                rewardModel = new RewardModel() { amount = 1, rewardType = (RewardType)((int)rd) };
                rewardModels.Add(rewardModel);
                beastMedalRewards.Add(rewardModel);
            }
        }
        for (int i = 0; i < beastMedalRewards.Count; i++)
        {
            var beastMedalReward = beastMedalRewards[i];
            if (stage == 1 || stage == 5 || stage == 10 || stage == 15)
                ReceiveRewards(beastMedalReward, false);
            else ReceiveRewards(beastMedalReward, true);
        }

        return rewardModels;
    }

    public static List<RewardModel> GetCanStageRewards(int stage)
    {
        var rewardModels = new List<RewardModel>();

        var rewardModel = new RewardModel() { amount = BigInteger.Pow(2, stage) * 10, rewardType = RewardType.Gold };
        rewardModels.Add(rewardModel);

        rewardModel = new RewardModel() { amount = 10, rewardType = RewardType.TileTicket };
        rewardModels.Add(rewardModel);

        int expAmount = (int)(500 * Mathf.Pow(1.05f, stage - 1));
        Debug.Log("GetStageRewards exp:" + expAmount + " stage:" + stage);
        rewardModel = new RewardModel() { amount = expAmount, rewardType = RewardType.EXP };
        rewardModels.Add(rewardModel);

        var firstWinStages = PlayerData.instance.FirstWinStages;
        var fws = firstWinStages.Split('|').ToList();
        if (!fws.Contains(stage.ToString()))
        {
            rewardModel = new RewardModel() { amount = PlayerData.instance.GemFirstWin, rewardType = RewardType.Gem };
            rewardModels.Add(rewardModel);

            if (stage == 1)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.RobinHood };
                rewardModels.Add(rewardModel);
            }
            else if (stage == 5)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.Pirato };
                rewardModels.Add(rewardModel);
            }
            else if (stage == 10)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.MonkeyKing };
                rewardModels.Add(rewardModel);
            }
            else if (stage == 15)
            {
                rewardModel = new RewardModel() { amount = 80, rewardType = RewardType.Merlinus };
                rewardModels.Add(rewardModel);
            }
        }


        rewardModel = new RewardModel() { amount = 1, rewardType = RewardType.CommonMedalMonster };
        rewardModels.Add(rewardModel);

        if (stage >= 15)
        {
            rewardModel = new RewardModel() { amount = 1, rewardType = RewardType.RandomMedalMonster };
            rewardModels.Add(rewardModel);
        }

        return rewardModels;
    }


    public static int GetTotalStarOfChapter(int region)
    {
        var maxStageOfRegion = (region + 1) * PlayerData.instance.MaxStagePerRegion;
        var minStageOfRegion = region * PlayerData.instance.MaxStagePerRegion + 1;

        var myStagePair = PlayerData.instance.GetMyStagePair();
        var totalStar = 0;
        var max = maxStageOfRegion > PlayerData.instance.MaxStage ? PlayerData.instance.MaxStage : maxStageOfRegion;
        for (int k = minStageOfRegion; k <= max; k++)
        {
            if (myStagePair.ContainsKey(k))
            {
                totalStar += myStagePair[k].numStar;
            }
        }
        return totalStar;
    }

    public static bool CheckHasChapterRewards()
    {
        int maxStage = PlayerData.instance.MaxStage;
        int currentRegion = (maxStage - 1) / PlayerData.instance.MaxStagePerRegion;
        var maxChapter = currentRegion + 1;

        var receivedMileStones = PlayerPrefs.GetString(CrushStringHelper.LastReceiveMileStoneReward, "");
        var revMS = receivedMileStones.Split('|');

        var canRevMileStones = new List<string>();
        for (int k = 1; k <= maxChapter; k++)
        {
            var myTotalStar = GetTotalStarOfChapter(k - 1);
            var chapterData = GameData.Instance.chapterDatas.GetChapterData(k);

            if (chapterData == null) continue;

            for (int l = 0; l < chapterData.milestones.Count; l++)
            {
                if (myTotalStar >= chapterData.milestones[l].starAmount)
                {
                    var ms = (k * 100 + l + 1);
                    canRevMileStones.Add(ms.ToString());
                }
            }
        }

        if (canRevMileStones.Count == 0) return false;

        return canRevMileStones.Exists(cr => !receivedMileStones.Contains(cr));
    }
}
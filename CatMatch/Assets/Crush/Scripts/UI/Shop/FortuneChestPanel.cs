using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeBeast;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FortuneChestPanel : MonoBehaviour
{
    public List<Button> animRects;
    public Material material;
    public BuyDialog buyDialog;
    public ConfirmDialog confirmDialog;
    public BeastInfoHomePanel beastInfoHomePanel;
    public PanelReceive2 panelReceive2;

    void OnEnable()
    {
        for (int k = 0; k < animRects.Count; k++)
        {
            var com = animRects[k].GetComponent<SkeletonGraphic>();
            Destroy(com);
        }
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(0.1f);

        var myShop = PlayerData.instance.MyShop;
        for (int k = 0; k < animRects.Count; k++)
        {
            var skeletonDataAsset = SkeletonDatas.Instance.skeletonBeastPair[(BeastId)myShop.beastIds[k]].skeletonDataAsset;
            var skeletonGraphic = SkeletonGraphic.AddSkeletonGraphicComponent(animRects[k].gameObject, skeletonDataAsset, material);
            skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
            if (k == 0)
                skeletonGraphic.gameObject.transform.localScale = new Vector3(-1, 1, 1);
            if (k == 1 || k == 2)
                skeletonGraphic.gameObject.transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
            if (k == 3)
                skeletonGraphic.gameObject.transform.localScale = new Vector3(-2f, 2f, 2f);

            Debug.Log("myShop:" + myShop.beastIds[k]);

            var beastId = (BeastId)myShop.beastIds[k];

            animRects[k].onClick.RemoveAllListeners();
            animRects[k].onClick.AddListener(() => ShowBeastInfo(beastId));
        }
    }

    void ShowBeastInfo(BeastId beastId)
    {
        beastInfoHomePanel?.gameObject.SetActive(true);

        var myShop = PlayerData.instance.MyShop;
        var beastIds = myShop.beastIds;
        var datas = new List<BeastTeamInfo>();

        BeastTeamInfo data = null;
        for (int k = 0; k < beastIds.Count; k++)
        {
            var _beastId = (BeastId)beastIds[k];
            var beastTeamInfo = PlayerData.instance.GetBeastData(_beastId);
            if (beastTeamInfo == null)
            {
                beastTeamInfo = new BeastTeamInfo()
                {
                    beastId = _beastId
                };
            }
            if (beastId == _beastId) data = beastTeamInfo;
            datas.Add(beastTeamInfo);
        }

        beastInfoHomePanel.Init(null, data, datas, EnableType.InTeamEditTeam);
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        ShopPanel.instance.closeBtn.gameObject.SetActive(true);
    }

    public void OnBuy()
    {
        buyDialog.gameObject.SetActive(true);

        int price = 200;

        Action yesAction = () =>
        {
            buyDialog.gameObject.SetActive(false);

            var money = Helper.GetMoney(CurrencyType.Gem);
            if (money >= price)
            {
                Helper.AddMoney(CurrencyType.Gem, -price);

                var myShop = PlayerData.instance.MyShop;
                Dictionary<BeastId, int> medals = new Dictionary<BeastId, int>(){
                    {(BeastId)myShop.beastIds[0], 50}, {(BeastId)myShop.beastIds[1], 50},
                    {(BeastId)myShop.beastIds[2], 30},{(BeastId)myShop.beastIds[3], 30}
                };
                Dictionary<BeastId, Rarity> rarity = new Dictionary<BeastId, Rarity>(){
                    {(BeastId)myShop.beastIds[0], Rarity.Common}, {(BeastId)myShop.beastIds[1], Rarity.Rare},
                    {(BeastId)myShop.beastIds[2], Rarity.Rare}, {(BeastId)myShop.beastIds[3], Rarity.Epic}
                };
                List<RewardModel> rewardModels = new List<RewardModel>();
                KeyValuePair<RewardType, BeastId> rewardBeast;

                var beastIds = myShop.beastIds.ToList();
                beastIds.RemoveAt(2);

                //take 1
                var beastId = beastIds[UnityEngine.Random.Range(0, beastIds.Count)];
                var medal = medals[(BeastId)beastId];
                PlayerData.instance.AddBeastData((BeastId)beastId, medal, true);
                Debug.Log("Beast 1:" + (BeastId)beastId + " medal:" + medal);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => (int)p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == (int)BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = medal });

                //take 2
                beastIds.Remove(beastId);
                beastId = beastIds[UnityEngine.Random.Range(0, beastIds.Count)];
                medal = medals[(BeastId)beastId];
                PlayerData.instance.AddBeastData((BeastId)beastId, medal, true);
                Debug.Log("Beast 2:" + (BeastId)beastId + " medal:" + medal);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => (int)p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == (int)BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = medal });

                //take 3
                var rd = UnityEngine.Random.Range(0, 2);
                beastIds.Remove(beastId);
                beastId = beastIds[0];
                medal = medals[(BeastId)beastId];
                var ra = rarity[(BeastId)beastId];
                if (ra == Rarity.Common)
                {
                    GetBeastIds(Element.None, out var commons, out var rares, out var epics);
                    commons.Remove((BeastId)beastId);
                    if (rd == 1) beastId = (int)commons[UnityEngine.Random.Range(0, commons.Count)];
                    PlayerData.instance.AddBeastData((BeastId)beastId, medal, true);
                    Debug.Log("Beast 31:" + (BeastId)beastId + " medal:" + medal);

                    rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => (int)p.Value == beastId);
                    if (rewardBeast.Key != RewardType.Atlantus || beastId == (int)BeastId.Atlantus)
                        rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = medal });
                }
                if (ra == Rarity.Rare)
                {
                    GetBeastIds(Element.None, out var commons, out var rares, out var epics, AbilityType.Crusher);
                    rares.Remove((BeastId)beastId);
                    if (rd == 1) beastId = (int)rares[UnityEngine.Random.Range(0, rares.Count)];
                    PlayerData.instance.AddBeastData((BeastId)beastId, medal, true);
                    Debug.Log("Beast 32:" + (BeastId)beastId + " medal:" + medal);

                    rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => (int)p.Value == beastId);
                    if (rewardBeast.Key != RewardType.Atlantus || beastId == (int)BeastId.Atlantus)
                        rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = medal });
                }
                if (ra == Rarity.Epic)
                {
                    GetBeastIds(Element.None, out var commons, out var rares, out var epics);
                    epics.Remove((BeastId)beastId);
                    if (rd == 1) beastId = (int)epics[UnityEngine.Random.Range(0, epics.Count)];
                    PlayerData.instance.AddBeastData((BeastId)beastId, medal, true);
                    Debug.Log("Beast 33:" + (BeastId)beastId + " medal:" + medal);

                    rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => (int)p.Value == beastId);
                    if (rewardBeast.Key != RewardType.Atlantus || beastId == (int)BeastId.Atlantus)
                        rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = medal });
                }

                // take 4
                rd = UnityEngine.Random.Range(0, 2);
                beastId = myShop.beastIds[2];
                medal = medals[(BeastId)beastId];
                if (rd == 1)
                {
                    GetBeastIds(Element.None, out var commons, out var rares, out var epics, AbilityType.Crusher);
                    rares.Remove((BeastId)beastId);
                    beastId = (int)rares[UnityEngine.Random.Range(0, rares.Count)];
                }
                PlayerData.instance.AddBeastData((BeastId)beastId, medal, true);
                Debug.Log("Beast4:" + (BeastId)beastId + " medal:" + medal);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => (int)p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == (int)BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = medal });

                panelReceive2.gameObject.SetActive(true);
                panelReceive2.Init(rewardModels);
            }
            else
            {
                confirmDialog.SetUp("宝石不足", "你的货币不足，无法使用");
                confirmDialog.gameObject.SetActive(true);
            }
        };
        Action noAction = () => { buyDialog.gameObject.SetActive(false); };
        buyDialog.SetUp("购买幸运宝箱", "你确定要购买此礼包吗？", MergeBeast.Utils.FormatNumber(price), yesAction, noAction,
            CurrencyType.Gem);
    }

    void GetBeastIds(Element element, out List<BeastId> commons, out List<BeastId> rares, out List<BeastId> epics, AbilityType ignore = AbilityType.None)
    {
        var names = Enum.GetNames(typeof(BeastId));
        commons = new List<BeastId>();
        rares = new List<BeastId>();
        epics = new List<BeastId>();

        for (int k = 0; k < names.Length; k++)
        {
            var bid = (BeastId)k;
            var pref = BeastPrefs.Instance.GetBeastPref(bid);
            var bb = pref.GetComponent<BeastBase>();
            if (bb.element == element || element == Element.None)
            {
                if (ignore != AbilityType.None)
                {
                    var am = bb.abilityModels.FirstOrDefault(ab => ab.abilityType == ignore);
                    if (am != null) continue;
                }
                var rarity = bb.rarity;
                if (rarity == Rarity.Common) commons.Add(bid);
                if (rarity == Rarity.Rare) rares.Add(bid);
                if (rarity == Rarity.Epic) epics.Add(bid);
            }
        }
    }
}

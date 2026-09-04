using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeBeast;
using Observer;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public static ShopPanel instance;

    public Transform container;
    public GameObject fortuenPref;
    public GameObject chestPref;
    public GameObject shopItemPref;

    public ShopDatas shopDatas;
    public RectTransform closeBtn;
    public FortuneChestPanel fortuneChestPanel;
    public BeastInfoHomePanel beastInfoHomePanel;

    public Button chestBtn;
    public Button itemBtn;
    public Sprite selectedImg;
    public Sprite notSelectImg;
    public BuyDialog buyDialog;
    public ConfirmDialog confirmDialog;

    public Text curGemTxt;
    public Text curCarrotTxt;

    private bool onChestScrollView;

    public PanelReceive2 panelReceive2;

    public Button premeiumChestBtn;

    void Awake()
    {
        if (instance == null) instance = this;

        this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => GemChange(param));
        this.RegisterListener(EventID.TileTicketChange, (sender, param) => TileTicketChange(param));
        this.RegisterListener(EventID.BeastEvolve, (sender, param) => BeastEvolve(param));
        this.RegisterListener(EventID.BeastLvlUp, (sender, param) => BeastLvlUp(param));
    }

    private void BeastLvlUp(object param)
    {
        // if (!gameObject.activeSelf) return;
        // if (!onChestScrollView)
        //     Refresh(false);
    }

    private void BeastEvolve(object param)
    {
        if (!gameObject.activeSelf) return;
        if (!onChestScrollView)
        {
            var beastId = (BeastId)((int)param);
            var pair = ShopItemView.pairs.FirstOrDefault(a => a.Value == beastId);

            var myShop = PlayerData.instance.MyShop;

            var count = container.childCount;
            var datas = shopDatas.shopItemDatas.shopItemDatas;
            var data = datas.FirstOrDefault(dt => dt.shopItemType == pair.Key);
            var chestTypeLength = Enum.GetNames(typeof(ChestType)).Length;

            for (int k = 0; k < count; k++)
            {
                var child = container.GetChild(k);
                var shopItemView = child.GetComponent<ShopItemView>();
                if (shopItemView != null)
                {
                    Debug.Log("shopItemView:" + shopItemView.beastId);
                    if (shopItemView.beastId == beastId)
                    {
                        var chestModel = new ChestModel()
                        {
                            amount = data.amount,
                            currencyType = data.currencyType,
                            des = data.des,
                            name = data.name,
                            price = data.price,
                            icon = data.icon,
                            curStock = data.maxStock - GetMyStock(myShop, ChestType.None, data.shopItemType),
                            maxStock = data.maxStock,
                            typeInt = (int)data.shopItemType + chestTypeLength,
                            quantity = data.amount
                        };

                        shopItemView.SetUp(chestModel);
                    }
                    else
                    {
                        if (shopItemView.chestModel.price > Helper.GetMoney(shopItemView.chestModel.currencyType))
                        {
                            Debug.Log("we shar ma");
                            shopItemView.buyBtn.interactable = false;
                        }
                    }
                }
            }
        }
    }

    private void TileTicketChange(object param)
    {
        curCarrotTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.TileTicketAmount);
    }

    private void GemChange(object param)
    {
        curGemTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());
    }

    void Start()
    {
        instance = this;
    }

    void OnEnable()
    {
        curGemTxt.text = MergeBeast.Utils.FormatNumber(Helper.GetMoney(CurrencyType.Gem));
        curCarrotTxt.text = MergeBeast.Utils.FormatNumber(Helper.GetMoney(CurrencyType.Tile));
        Refresh(true);
    }

    public void Refresh(bool chest)
    {
        chestBtn.image.sprite = chest ? selectedImg : notSelectImg;
        itemBtn.image.sprite = !chest ? selectedImg : notSelectImg;
        while (container.childCount > 0)
        {
            var gameOb = container.GetChild(0).gameObject;
            var chestItemView = gameOb.GetComponent<ChestItemView>();
            if (chestItemView != null && chestItemView.beastRectTFs != null)
            {
                for (int k = 0; k < chestItemView.beastRectTFs.Count; k++)
                {
                    var skg = chestItemView.beastRectTFs[k].GetComponent<SkeletonGraphic>();
                    if (skg != null)
                        Destroy(skg);
                }
            }
            ObjectPool.Instance.ReleaseObject(gameOb);
        }
        onChestScrollView = chest;
        StartCoroutine(Run(chest));
    }

    IEnumerator Run(bool chest)
    {
        yield return new WaitForSeconds(0.1f);

        var myShop = PlayerData.instance.MyShop;
        if (myShop == null)
        {
            myShop = new MyShop();
            myShop.date = DateTime.Now.ToString("dd/MM/yyyy");
            myShop.beastIds = GetBeastIds();
            myShop.shopItemDayth = (int)ShopItemType.NatureRune;
            myShop.chestPremium += 1;
            myShop.tutorial = true;

            PlayerPrefs.SetString(CrushStringHelper.MyShop, JsonUtility.ToJson(myShop));
        }
        else if (!string.IsNullOrEmpty(myShop.date))
        {
            var curDate = DateTime.Now.ToString("dd/MM/yyyy");
            if (myShop.date != curDate)
            {
                myShop.date = curDate;
                myShop.beastIds = GetBeastIds();
                myShop.Reset();
                myShop.shopItemDayth++;
                if (myShop.shopItemDayth > Enum.GetNames(typeof(ShopItemType)).Length)
                {
                    myShop.shopItemDayth = (int)ShopItemType.NatureRune;
                }
                PlayerData.instance.MyShop = myShop;
            }
        }

        if (chest) RefreshChest(myShop);
        else RefreshItem(myShop);
    }

    void RefreshChest(MyShop myShop)
    {
        var chestDatas = shopDatas.chestDatas.chestDatas;

        Debug.Log("wtf:" + container.childCount);

        var chestTypeLength = Enum.GetNames(typeof(ChestType)).Length;

        for (int k = 0; k < chestDatas.Count; k++)
        {
            // fake thôi, phải bỏ đi khi anim hết bug
            var chestData = chestDatas[k];
            Debug.Log("chestData:" + chestData.chestType + " days:" + string.IsNullOrEmpty(chestData.days));

            if (!string.IsNullOrEmpty(chestData.days) && !chestData.days.Contains(((int)DateTime.Now.DayOfWeek).ToString()))
            {
                continue;
            }
            if (chestData.chestType == ChestType.MagicMedal) continue;

            var chestModel = new ChestModel()
            {
                amount = chestData.amount,
                chestType = chestData.chestType,
                currencyType = chestData.currencyType,
                des = chestData.des,
                name = chestData.name,
                price = chestData.price,
                icon = chestData.icon,
                curStock = chestData.maxStock - GetBuyAmount(myShop, chestData.chestType, ShopItemType.None),
                maxStock = chestData.maxStock,
                typeInt = (int)chestData.chestType,
                quantity = chestData.quantity,
                useAmount = GetUseAmount(myShop, chestData.chestType, ShopItemType.None),
                buyAmount = GetBuyAmount(myShop, chestData.chestType, ShopItemType.None),
                freeAmount = GetMyStock(myShop, chestData.chestType, ShopItemType.None) - GetUseAmount(myShop, chestData.chestType, ShopItemType.None),
            };
            GameObject go = null;

            if (chestData.chestType == ChestType.Fortune)
            {
                go = ObjectPool.Instance.GetGameObject(fortuenPref, Vector3.zero, Quaternion.identity);
                chestModel.beastIds = new List<BeastId>() { };
                for (int l = 0; l < myShop.beastIds.Count; l++)
                {
                    chestModel.beastIds.Add((BeastId)myShop.beastIds[l]);
                }
            }
            else
            {
                go = ObjectPool.Instance.GetGameObject(chestPref, Vector3.zero, Quaternion.identity);
            }

            go.transform.SetParent(container);
            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

            var shopItemView = go.GetComponent<ChestItemView>();
            shopItemView.SetUp(chestModel);

            if ((chestModel.maxStock == 0 || chestModel.maxStock == -1 || chestModel.curStock > 0)
                    && (
                         (chestModel.currencyType == CurrencyType.Tile && chestModel.price <= Helper.GetMoney(CurrencyType.Tile))
                         || chestModel.currencyType != CurrencyType.Tile
                        )
                    )
            {
                shopItemView.buyBtn.interactable = true;
                shopItemView.buyBtn.onClick.RemoveAllListeners();
                shopItemView.buyBtn.onClick.AddListener(() => OnBuy(true, chestModel, shopItemView.gameObject));
            }
            else shopItemView.buyBtn.interactable = false;
            if (k == 1)
            {
                premeiumChestBtn = shopItemView.buyBtn;
                Debug.Log("ShopPanel premeiumChestBtn");
            }
        }
    }

    void RefreshItem(MyShop myShop)
    {
        var datas = shopDatas.shopItemDatas.shopItemDatas;

        Debug.Log("wtf:" + container.childCount);

        var chestTypeLength = Enum.GetNames(typeof(ChestType)).Length;

        Debug.Log("myshop:" + JsonUtility.ToJson(PlayerData.instance.MyShop));

        for (int k = 0; k < datas.Count; k++)
        {
            var data = datas[k];
            bool br = false;
            if (data.shopItemType >= ShopItemType.CommonRune)
            {
                break;
            }

            if (data.shopItemType > ShopItemType.EpicRune)
            {
                data = datas[myShop.shopItemDayth - 1];
                br = true;
            }
            var chestModel = new ChestModel()
            {
                amount = data.amount,
                currencyType = data.currencyType,
                des = data.des,
                name = data.name,
                price = data.price,
                icon = data.icon,
                curStock = data.maxStock - GetBuyAmount(myShop, ChestType.None, data.shopItemType),
                maxStock = data.maxStock,
                typeInt = (int)data.shopItemType + chestTypeLength,
                quantity = data.amount,
                useAmount = GetUseAmount(myShop, ChestType.None, data.shopItemType),
                buyAmount = GetBuyAmount(myShop, ChestType.None, data.shopItemType),
                freeAmount = GetMyStock(myShop, ChestType.None, data.shopItemType) - GetUseAmount(myShop, ChestType.None, data.shopItemType),
            };
            GameObject go = null;

            if (data.shopItemType <= ShopItemType.Scud)
            {
                go = ObjectPool.Instance.GetGameObject(shopItemPref, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(container);
                go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
                var shopItemView = go.GetComponent<ShopItemView>();
                shopItemView.SetUp(chestModel);

                shopItemView.iconBtn.onClick.RemoveAllListeners();
                shopItemView.iconBtn.onClick.AddListener(() => ShowBeastInfo(data));

                if ((chestModel.maxStock == 0 || chestModel.maxStock == -1 || chestModel.curStock > 0)
                    && (
                         (chestModel.currencyType == CurrencyType.Tile && chestModel.price <= Helper.GetMoney(CurrencyType.Tile))
                         || chestModel.currencyType != CurrencyType.Tile
                        )
                    )
                {
                    shopItemView.buyBtn.interactable = true;
                    shopItemView.buyBtn.onClick.RemoveAllListeners();
                    shopItemView.buyBtn.onClick.AddListener(() => OnBuy(false, chestModel, shopItemView.gameObject));
                }
                else shopItemView.buyBtn.interactable = false;
            }
            else
            {
                go = ObjectPool.Instance.GetGameObject(chestPref, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(container);
                go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
                var shopItemView = go.GetComponent<ChestItemView>();
                shopItemView.SetUp(chestModel);

                if (chestModel.maxStock == 0 || chestModel.maxStock == -1 || chestModel.curStock > 0)
                {
                    shopItemView.buyBtn.interactable = true;
                    shopItemView.buyBtn.onClick.RemoveAllListeners();
                    shopItemView.buyBtn.onClick.AddListener(() => OnBuy(true, chestModel, shopItemView.gameObject));
                }
                else shopItemView.buyBtn.interactable = false;
            }
            if (br) break;
        }
    }

    void ShowBeastInfo(ShopItemData shopItemData)
    {
        // beastInfoHomePanel?.gameObject.SetActive(true);
        Dictionary<ShopItemType, BeastId> pair = new Dictionary<ShopItemType, BeastId>()
        {
            {ShopItemType.DarkHunter, BeastId.DarkHunter}, {ShopItemType.Merlinus, BeastId.Merlinus},
            {ShopItemType.Atlantus, BeastId.Atlantus}, {ShopItemType.Furiosa, BeastId.Furiosa},
            {ShopItemType.Groovine, BeastId.GrooVine}, {ShopItemType.Scud, BeastId.Scud}
        };

        ShowBeastInfo(pair[shopItemData.shopItemType], false);

        // var beastTeamInfo = PlayerData.instance.GetBeastData(pair[shopItemData.shopItemType]);
        // if (beastTeamInfo == null)
        // {
        //     beastTeamInfo = new BeastTeamInfo()
        //     {
        //         beastId = pair[shopItemData.shopItemType]
        //     };
        // }
        // var datas = new List<BeastTeamInfo>() { beastTeamInfo };

        // beastInfoHomePanel.Init(null, beastTeamInfo, datas, EnableType.InTeamEditTeam);
    }

    void ShowBeastInfo(BeastId beastId, bool evolve = true)
    {
        beastInfoHomePanel?.gameObject.SetActive(true);

        if (evolve)
            PlayerData.instance.Evolve(beastId);

        var beastTeamInfo = PlayerData.instance.GetBeastData(beastId);
        if (beastTeamInfo == null)
        {
            beastTeamInfo = new BeastTeamInfo()
            {
                beastId = beastId
            };
        }
        var datas = new List<BeastTeamInfo>() { beastTeamInfo };

        beastInfoHomePanel.Init(null, beastTeamInfo, datas, EnableType.InTeamEditTeam);
    }

    void OnBuy(bool chest, ChestModel chestModel, GameObject go)
    {
        Debug.Log("OnBuyyyy");
        var length = Enum.GetNames(typeof(ChestType)).Length;
        int price = chestModel.price;

        if (chestModel.typeInt == (int)ChestType.Fortune)
        {
            fortuneChestPanel.gameObject.SetActive(true);
            closeBtn.gameObject.SetActive(false);
        }
        else
        {

            if (chestModel.freeAmount > 0)
            {
                if (chestModel.typeInt > length)
                {
                    var vl = chestModel.typeInt - length;
                    SaveMyStock(null, ChestType.None, (ShopItemType)vl, chestModel.amount, chestModel, go, chest, false);
                }
                else SaveMyStock(null, (ChestType)chestModel.typeInt, ShopItemType.None, chestModel.amount, chestModel, go, chest, false);

                UpdateStateBuyBtn();
            }
            else
            {
                if (chestModel.maxStock > 0 && chestModel.curStock == 0)
                {
                    confirmDialog.SetUp("已售罄", "今日库存已售完");
                    confirmDialog.gameObject.SetActive(true);
                    return;
                }

                if (chestModel.currencyType == CurrencyType.Tile)
                {
                    CheckMoney();
                    UpdateStateBuyBtn();
                }
                else
                {
                    buyDialog.gameObject.SetActive(true);
                    Action yesAction = () =>
                    {
                        buyDialog.gameObject.SetActive(false);

                        CheckMoney();
                        UpdateStateBuyBtn();
                    };
                    Action noAction = () => { buyDialog.gameObject.SetActive(false); };
                    buyDialog.SetUp("购买 " + chestModel.name, "你确定要购买此礼包吗？", MergeBeast.Utils.FormatNumber(price), yesAction, noAction,
                    chestModel.currencyType);
                }
            }


        }

        void CheckMoney()
        {
            var money = Helper.GetMoney(chestModel.currencyType);
            if (money >= price)
            {
                Helper.AddMoney(chestModel.currencyType, -price);

                // Debug.Log("lengthhhhhhhhhhh:" + length + " chestModel.typeInt:" + chestModel.typeInt);

                if (chestModel.typeInt > length)
                {
                    var vl = chestModel.typeInt - length;
                    SaveMyStock(null, ChestType.None, (ShopItemType)vl, chestModel.amount, chestModel, go, chest);
                }
                else SaveMyStock(null, (ChestType)chestModel.typeInt, ShopItemType.None, chestModel.amount, chestModel, go, chest);
            }
            else
            {
                confirmDialog.SetUp("宝石不足", "你的货币不足，无法使用");
                confirmDialog.gameObject.SetActive(true);
            }
        }

        void UpdateStateBuyBtn()
        {
            if (chest)
            {
                Debug.Log("chestModel.maxStock:" + chestModel.maxStock + "  chestModel.curStock:" + chestModel.curStock);

                var shopItemView = go.GetComponent<ChestItemView>();
                if ((chestModel.maxStock > 0 && chestModel.curStock == 0) ||
                    (chestModel.currencyType == CurrencyType.Tile && chestModel.price > Helper.GetMoney(CurrencyType.Tile)))
                {
                    shopItemView.buyBtn.interactable = false;
                }
            }
            else
            {
                var shopItemView = go.GetComponent<ShopItemView>();
                if (chestModel.maxStock > 0 && chestModel.curStock == 0)
                {
                    shopItemView.buyBtn.interactable = false;
                }

                for (int k = 0; k < container.childCount; k++)
                {
                    var child = container.GetChild(k);
                    var shopItemViewOther = child.GetComponent<ShopItemView>();
                    if (shopItemViewOther != null && shopItemViewOther.chestModel.price > Helper.GetMoney(shopItemViewOther.chestModel.currencyType))
                    {
                        Debug.Log("we shar ma");
                        shopItemViewOther.buyBtn.interactable = false;
                    }
                }
            }
        }
    }

    List<int> GetBeastIds()
    {
        var names = Enum.GetNames(typeof(BeastId));
        List<BeastId> commons = new List<BeastId>();
        List<BeastId> rares = new List<BeastId>();
        List<BeastId> epics = new List<BeastId>();

        for (int k = 0; k < names.Length; k++)
        {
            var bid = (BeastId)k;
            var pref = BeastPrefs.Instance.GetBeastPref(bid);
            var rarity = pref.GetComponent<BeastBase>().rarity;
            if (rarity == Rarity.Common) commons.Add(bid);
            if (rarity == Rarity.Rare) rares.Add(bid);
            if (rarity == Rarity.Epic) epics.Add(bid);
        }

        var res = new List<int>();

        var id = commons[UnityEngine.Random.Range(0, commons.Count)];
        res.Add((int)id);
        id = rares[UnityEngine.Random.Range(0, rares.Count)];
        rares.Remove(id);
        res.Add((int)id);
        id = rares[UnityEngine.Random.Range(0, rares.Count)];
        res.Add((int)id);
        id = epics[UnityEngine.Random.Range(0, epics.Count)];
        res.Add((int)id);

        return res;
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

    int GetMyStock(MyShop myShop, ChestType chestType, ShopItemType shopItemType)
    {
        if (myShop == null) myShop = PlayerData.instance.MyShop;

        switch (chestType)
        {
            case ChestType.Premium:
                return myShop.chestPremium;
            case ChestType.MonsterFire:
                return myShop.chestFire;
            case ChestType.MonsterGrass:
                return myShop.chestGrass;
            case ChestType.MonsterWater:
                return myShop.chestWater;
        }

        switch (shopItemType)
        {
            case ShopItemType.DarkHunter:
                return myShop.darkHunterStock;
            case ShopItemType.Merlinus:
                return myShop.merlinusStock;
            case ShopItemType.Atlantus:
                return myShop.atlantusStock;
            case ShopItemType.Furiosa:
                return myShop.furiosaStock;
            case ShopItemType.Groovine:
                return myShop.groovineStock;
            case ShopItemType.Scud:
                return myShop.scudStock;

            case ShopItemType.NatureRune:
                return myShop.natureStock;
            case ShopItemType.VikingRune:
                return myShop.vikingStock;
            case ShopItemType.Navarre:
                return myShop.navarreStock;
            case ShopItemType.Assassin:
                return myShop.assassinStock;
            case ShopItemType.Ice:
                return myShop.iceStock;
            case ShopItemType.Solar:
                return myShop.solarStock;
            case ShopItemType.Valken:
                return myShop.valkenStock;
            case ShopItemType.Voodoo:
                return myShop.voodooStock;
        }

        return 0;
    }

    int GetUseAmount(MyShop myShop, ChestType chestType, ShopItemType shopItemType)
    {
        if (myShop == null) myShop = PlayerData.instance.MyShop;

        switch (chestType)
        {
            case ChestType.Premium:
                return myShop.chestPremiumUse;
            case ChestType.MonsterFire:
                return myShop.chestFireUse;
            case ChestType.MonsterGrass:
                return myShop.chestGrassUse;
            case ChestType.MonsterWater:
                return myShop.chestWaterUse;
        }

        switch (shopItemType)
        {
            case ShopItemType.DarkHunter:
                return myShop.darkHunterStockUse;
            case ShopItemType.Merlinus:
                return myShop.merlinusStockUse;
            case ShopItemType.Atlantus:
                return myShop.atlantusStockUse;
            case ShopItemType.Furiosa:
                return myShop.furiosaStockUse;
            case ShopItemType.Groovine:
                return myShop.groovineStockUse;
            case ShopItemType.Scud:
                return myShop.scudStockUse;

            case ShopItemType.NatureRune:
                return myShop.natureStockUse;
            case ShopItemType.VikingRune:
                return myShop.vikingStockUse;
            case ShopItemType.Navarre:
                return myShop.navarreStockUse;
            case ShopItemType.Assassin:
                return myShop.assassinStockUse;
            case ShopItemType.Ice:
                return myShop.iceStockUse;
            case ShopItemType.Solar:
                return myShop.solarStockUse;
            case ShopItemType.Valken:
                return myShop.valkenStockUse;
            case ShopItemType.Voodoo:
                return myShop.voodooStockUse;
        }

        return 0;
    }

    int GetBuyAmount(MyShop myShop, ChestType chestType, ShopItemType shopItemType)
    {
        if (myShop == null) myShop = PlayerData.instance.MyShop;

        switch (chestType)
        {
            case ChestType.Premium:
                return myShop.chestPremiumBuy;
            case ChestType.MonsterFire:
                return myShop.chestFireBuy;
            case ChestType.MonsterGrass:
                return myShop.chestGrassBuy;
            case ChestType.MonsterWater:
                return myShop.chestWaterBuy;
        }

        switch (shopItemType)
        {
            case ShopItemType.DarkHunter:
                return myShop.darkHunterStockBuy;
            case ShopItemType.Merlinus:
                return myShop.merlinusStockBuy;
            case ShopItemType.Atlantus:
                return myShop.atlantusStockBuy;
            case ShopItemType.Furiosa:
                return myShop.furiosaStockBuy;
            case ShopItemType.Groovine:
                return myShop.groovineStockBuy;
            case ShopItemType.Scud:
                return myShop.scudStockBuy;

            case ShopItemType.NatureRune:
                return myShop.natureStockBuy;
            case ShopItemType.VikingRune:
                return myShop.vikingStockBuy; ;
            case ShopItemType.Navarre:
                return myShop.navarreStockBuy;
            case ShopItemType.Assassin:
                return myShop.assassinStockBuy;
            case ShopItemType.Ice:
                return myShop.iceStockBuy;
            case ShopItemType.Solar:
                return myShop.solarStockBuy;
            case ShopItemType.Valken:
                return myShop.valkenStockBuy;
            case ShopItemType.Voodoo:
                return myShop.voodooStockBuy;
        }

        return 0;
    }

    void SaveMyStock(MyShop myShop, ChestType chestType, ShopItemType shopItemType, int amount, ChestModel chestModel, GameObject itemViewGO, bool isChest, bool isBuy = true)
    {
        if (myShop == null) myShop = PlayerData.instance.MyShop;

        List<BeastId> commons, rares, epics;
        BeastId beastId;
        var rd = UnityEngine.Random.Range(0, 2);

        KeyValuePair<RewardType, BeastId> rewardBeast;
        List<RewardModel> rewardModels = new List<RewardModel>();
        switch (chestType)
        {
            case ChestType.MonsterFire:
                if (isBuy)
                {
                    myShop.chestFireBuy += 1;
                    myShop.chestFire += 1;
                }
                myShop.chestFireUse += 1;

                GetBeastIds(Element.Fire, out commons, out rares, out epics);
                if (rd == 0)
                {
                    rares.Remove(BeastId.Furiosa);
                    beastId = rares[UnityEngine.Random.Range(0, rares.Count)];
                }
                else
                {
                    epics.Remove(BeastId.Furiosa);
                    beastId = epics[UnityEngine.Random.Range(0, epics.Count)];
                }
                PlayerData.instance.AddBeastData(beastId, amount, true);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = amount });
                panelReceive2.gameObject.SetActive(true);
                panelReceive2.Init(rewardModels);
                break;
            case ChestType.MonsterGrass:
                if (isBuy)
                {
                    myShop.chestGrassBuy += 1;
                    myShop.chestGrass += 1;
                }
                myShop.chestGrassUse += 1;

                GetBeastIds(Element.Grass, out commons, out rares, out epics);
                if (rd == 0)
                {
                    rares.Remove(BeastId.GrooVine);
                    beastId = rares[UnityEngine.Random.Range(0, rares.Count)];
                }
                else
                {
                    epics.Remove(BeastId.GrooVine);
                    beastId = epics[UnityEngine.Random.Range(0, epics.Count)];
                }
                PlayerData.instance.AddBeastData(beastId, amount, true);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = amount });
                panelReceive2.gameObject.SetActive(true);
                panelReceive2.Init(rewardModels);
                break;
            case ChestType.MonsterWater:
                if (isBuy)
                {
                    myShop.chestWaterBuy += 1;
                    myShop.chestWater += 1;
                }
                myShop.chestWaterUse += 1;

                GetBeastIds(Element.Water, out commons, out rares, out epics);
                if (rd == 0)
                {
                    rares.Remove(BeastId.Atlantus);
                    beastId = rares[UnityEngine.Random.Range(0, rares.Count)];
                }
                else
                {
                    epics.Remove(BeastId.Atlantus);
                    beastId = epics[UnityEngine.Random.Range(0, epics.Count)];
                }
                PlayerData.instance.AddBeastData(beastId, amount, true);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = amount });
                panelReceive2.gameObject.SetActive(true);
                panelReceive2.Init(rewardModels);
                break;
            case ChestType.Silver:
                GetBeastIds(Element.None, out commons, out rares, out epics);
                beastId = commons[UnityEngine.Random.Range(0, commons.Count)];
                PlayerData.instance.AddBeastData(beastId, amount, true);

                rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == beastId);
                if (rewardBeast.Key != RewardType.Atlantus || beastId == BeastId.Atlantus)
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = amount });
                panelReceive2.gameObject.SetActive(true);
                panelReceive2.Init(rewardModels);
                break;
            case ChestType.Premium:
                if (isBuy)
                {
                    myShop.chestPremiumBuy += 1;
                    myShop.chestPremium += 1;
                }
                myShop.chestPremiumUse += 1;

                if (myShop.tutorial)
                {
                    myShop.tutorial = false;
                    rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == BeastId.Chaos);
                    rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = amount + 10 });
                    PlayerData.instance.AddBeastData(BeastId.Chaos, amount + 10, false);
                }
                else
                {
                    GetBeastIds(Element.None, out commons, out rares, out epics, AbilityType.Crusher);
                    beastId = rares[UnityEngine.Random.Range(0, rares.Count)];
                    PlayerData.instance.AddBeastData(beastId, amount, true);

                    rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == beastId);
                    if (rewardBeast.Key != RewardType.Atlantus || beastId == BeastId.Atlantus)
                        rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = amount });
                    Debug.Log("Rewards rewardBeast:" + (RewardType)rewardBeast.Key + " beastId:" + beastId);

                    PlayerData.instance.AddBeastData(beastId, amount, true);
                    rares.Remove(beastId);
                    beastId = rares[UnityEngine.Random.Range(0, rares.Count)];
                    PlayerData.instance.AddBeastData(beastId, 10, true);

                    rewardBeast = RewardHelper.rewardBeastPair.FirstOrDefault(p => p.Value == beastId);
                    if (rewardBeast.Key != RewardType.Atlantus || beastId == BeastId.Atlantus)
                        rewardModels.Add(new RewardModel() { rewardType = rewardBeast.Key, amount = 10 });
                    Debug.Log("Rewards rewardBeast 2:" + (RewardType)rewardBeast.Key + " beastId:" + beastId);
                }

                Debug.Log("Rewards:" + rewardModels);
                panelReceive2.gameObject.SetActive(true);
                panelReceive2.Init(rewardModels);
                break;
        }

        System.Numerics.BigInteger medal = 0;
        switch (shopItemType)
        {
            case ShopItemType.Atlantus:
                if (isBuy)
                {
                    myShop.atlantusStockBuy += 1;
                    myShop.atlantusStock += 1;
                }
                myShop.atlantusStockUse += 1;

                medal = PlayerData.instance.AddBeastData(BeastId.Atlantus, amount, true);
                var atlan = PlayerData.instance.GetBeastData(BeastId.Atlantus);
                if (atlan.curStar <= 0 && medal >= Constant.medalByStars[0])
                    ShowBeastInfo(BeastId.Atlantus);
                break;
            case ShopItemType.Furiosa:
                if (isBuy)
                {
                    myShop.furiosaStockBuy += 1;
                    myShop.furiosaStock += 1;
                }
                myShop.furiosaStockUse += 1;

                medal = PlayerData.instance.AddBeastData(BeastId.Furiosa, amount, true);
                var furi = PlayerData.instance.GetBeastData(BeastId.Furiosa);
                if (furi.curStar <= 0 && medal >= Constant.medalByStars[0])
                    ShowBeastInfo(BeastId.Furiosa);
                break;
            case ShopItemType.Groovine:
                if (isBuy)
                {
                    myShop.groovineStockBuy += 1;
                    myShop.groovineStock += 1;
                }
                myShop.groovineStockUse += 1;

                medal = PlayerData.instance.AddBeastData(BeastId.GrooVine, amount, true);
                var groo = PlayerData.instance.GetBeastData(BeastId.GrooVine);
                if (groo.curStar <= 0 && medal >= Constant.medalByStars[0])
                    ShowBeastInfo(BeastId.GrooVine);
                break;
            case ShopItemType.DarkHunter:
                if (isBuy)
                {
                    myShop.darkHunterStockBuy += 1;
                    myShop.darkHunterStock += 1;
                }
                myShop.darkHunterStockUse += 1;

                medal = PlayerData.instance.AddBeastData(BeastId.DarkHunter, amount, true);
                var dark = PlayerData.instance.GetBeastData(BeastId.DarkHunter);
                if (dark.curStar <= 0 && medal >= Constant.medalByStars[0])
                    ShowBeastInfo(BeastId.DarkHunter);
                break;
            case ShopItemType.Merlinus:
                if (isBuy)
                {
                    myShop.merlinusStockBuy += 1;
                    myShop.merlinusStock += 1;
                }
                myShop.merlinusStockUse += 1;

                medal = PlayerData.instance.AddBeastData(BeastId.Merlinus, amount, true);
                var mer = PlayerData.instance.GetBeastData(BeastId.Merlinus);
                if (mer.curStar <= 0 && medal >= Constant.medalByStars[0])
                    ShowBeastInfo(BeastId.Merlinus);
                break;
            case ShopItemType.Scud:
                if (isBuy)
                {
                    myShop.scudStockBuy += 1;
                    myShop.scudStock += 1;
                }
                myShop.scudStockUse += 1;

                medal = PlayerData.instance.AddBeastData(BeastId.Scud, amount, true);
                var scu = PlayerData.instance.GetBeastData(BeastId.Scud);
                if (scu.curStar <= 0 && medal >= Constant.medalByStars[0])
                    ShowBeastInfo(BeastId.Scud);
                break;
            case ShopItemType.SweepTicket:
                Helper.AddMoney(CurrencyType.SweepAmount, amount);
                break;
            case ShopItemType.NatureRune:
                myShop.natureStock += 1;
                break;
            case ShopItemType.VikingRune:
                myShop.vikingStock += 1;
                break;
            case ShopItemType.Navarre:
                myShop.navarreStock += 1;
                break;
            case ShopItemType.Assassin:
                myShop.assassinStock += 1;
                break;
            case ShopItemType.Ice:
                myShop.iceStock += 1;
                break;
            case ShopItemType.Solar:
                myShop.solarStock += 1;
                break;
            case ShopItemType.Valken:
                myShop.valkenStock += 1;
                break;
            case ShopItemType.Voodoo:
                myShop.voodooStock += 1;
                break;
        }
        PlayerData.instance.MyShop = myShop;

        chestModel.curStock = chestModel.maxStock - GetBuyAmount(myShop, chestType, shopItemType);
        Debug.Log("Buy chestModel.curStock:" + chestModel.curStock + " GetBuyAmount:" + GetBuyAmount(myShop, chestType, shopItemType));
        chestModel.useAmount = GetUseAmount(myShop, chestType, shopItemType);
        chestModel.buyAmount = GetBuyAmount(myShop, chestType, shopItemType);
        chestModel.freeAmount = GetMyStock(myShop, chestType, shopItemType) - GetUseAmount(myShop, chestType, shopItemType);

        if (isChest)
        {
            var shopItemView = itemViewGO.GetComponent<ChestItemView>();
            shopItemView.SetUp(chestModel);
        }
        else
        {
            // chestModel.curStock = chestModel.maxStock - GetBuyAmount(myShop, ChestType.None, shopItemType);
            // chestModel.useAmount = GetUseAmount(myShop, ChestType.None, shopItemType);
            // chestModel.buyAmount = GetBuyAmount(myShop, ChestType.None, shopItemType);
            // chestModel.freeAmount = GetMyStock(myShop, ChestType.None, shopItemType) - GetUseAmount(myShop, ChestType.None, shopItemType);

            var shopItemView = itemViewGO.GetComponent<ShopItemView>();
            shopItemView.SetUp(chestModel);
        }
    }

    public void TutorialBuy(int child)
    {
        var go = container.transform.GetChild(child);

        var shopItemView = go.GetComponent<ChestItemView>();
        OnBuy(true, shopItemView.mChestModel, shopItemView.gameObject);
    }
}
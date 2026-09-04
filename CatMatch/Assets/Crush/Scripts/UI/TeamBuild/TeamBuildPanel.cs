using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeBeast;
using UnityEngine;
using UnityEngine.UI;
using Observer;

public class TeamBuildPanel : BaseScreen, ITeamBuffTouch
{
    public static TeamBuildPanel Instance;

    private const int TEAM_1 = 1;
    private const int TEAM_3 = 3;

    public Transform beastInTeamParent;
    public GameObject beastInTeamPref;
    public BeastInfoHomePanel beastInfoHomePanel;

    public Transform teamBuffParent;
    public GameObject teamBuffPef;

    public List<Image> teamOnImgs;
    public List<Image> teamOffImgs;
    public Image moreTeamImg;
    public DragChecker dragChecker;

    [SerializeField] private ScrollRect scrollMonsterList;
    [SerializeField] private GameObject monsterInListPref;
    [SerializeField] private Transform monsterListParent;
    [SerializeField] private Text curGemTxt;

    public BuyDialog buyDialog;
    public ConfirmDialog confirmDialog;

    public RectTransform beastActionDialog;
    public RectTransform teamBuffInfoDialog;

    private Dictionary<int, List<BeastTeamInfo>> beastTeamInfoByTeamId = new Dictionary<int, List<BeastTeamInfo>>();

    public static int emptySlotAmout;

    private bool onDraggingListMonster;

    public bool endRefreshBeastInTeam;

    void Awake()
    {
        Instance = this;

        ActionHelper.BeastEvolve += (beastTeamInfo) => CalTeamBuffs(beastTeamInfo);
        this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => GemChange(param));
    }

    private void GemChange(object param)
    {
        curGemTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());
    }

    void OnEnable()
    {
        // AdsManager.Instance?.HideBanner();

        beastTeamInfoByTeamId.Clear();
        curGemTxt.text = MergeBeast.Utils.FormatNumber(Helper.GetMoney(CurrencyType.Gem));

        ShowButtons();

        GenMonsterList();

        GenBeastInTeam();
    }

    void OnDisable()
    {
        // AdsManager.Instance.RequestBanner();
    }

    public void CalTeamBuffs(BeastTeamInfo data)
    {
        if (data != null && data.curStar < 4) return;

        while (teamBuffParent.childCount > 0)
        {
            var go = teamBuffParent.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        var beastInTeamByTeamId = PlayerData.instance.GetTeams();
        var beastIds = beastInTeamByTeamId[PlayerData.instance.CurrentTeam];
        var beastTeamInfos = PlayerData.instance.GetBeastTeamInfos().Where(bif => beastIds.Contains(bif.beastId)).ToList();

        Dictionary<int, float> teamBuffData = new Dictionary<int, float>();

        foreach (var beastTeamInfo in beastTeamInfos)
        {
            if (beastTeamInfo.curStar < 4) continue;

            var beastAllInfo = BeastPrefs.Instance.GetBeastAllInfo(beastTeamInfo.beastId);
            var beastBase = beastAllInfo.pref.GetComponent<BeastBase>();

            // Debug.Log(beastBase.teamBuff + " " + beastBase.buffForElement);

            if (beastBase.teamBuff != TeamBuff.None)
            {
                var key = TeamBuffHelper.GenKey(beastBase.buffForElement, beastBase.buffForClass, (int)beastBase.teamBuff);
                // Debug.Log("key:" + key);

                if (teamBuffData.ContainsKey(key))
                    teamBuffData[key] += beastBase.teamBuffValue;
                else
                    teamBuffData.Add(key, beastBase.teamBuffValue);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            var go = ObjectPool.Instance.GetGameObject(teamBuffPef, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            go.transform.SetParent(teamBuffParent);
            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

            int key = -1;
            Element buffFor = Element.None;
            TeamBuff teamBuff = TeamBuff.None;
            BeastClass _buffForClass = BeastClass.None;
            float value = 0f;
            if (teamBuffData.Count > i)
            {
                key = teamBuffData.ElementAt(i).Key;
                value = teamBuffData.ElementAt(i).Value;
                TeamBuffHelper.RevertFromKey(key, out buffFor, out _buffForClass, out teamBuff);
            }
            go.GetComponent<TeamBuffItemView>().SetUp(this, TeamBuffSprites.Instance.GetIcon(key),
            new TeamBuffModel() { buffForElement = buffFor, teamBuff = teamBuff, value = value, buffForClass = _buffForClass });
        }
    }

    private void ShowButtons()
    {
        for (int k = 0; k < teamOffImgs.Count; k++)
        {
            teamOffImgs[k].gameObject.SetActive(k != PlayerData.instance.CurrentTeam - 1);
            teamOnImgs[k].gameObject.SetActive(k == PlayerData.instance.CurrentTeam - 1);
            if (k == 2)
            {
                if (!PlayerData.instance.GetTeams().ContainsKey(TEAM_3))
                {
                    teamOffImgs[k].gameObject.SetActive(false);
                    teamOnImgs[k].gameObject.SetActive(false);
                }
                else moreTeamImg.gameObject.SetActive(false);
            }
        }
    }

    public void GenBeastInTeam()
    {
        while (beastInTeamParent.childCount > 0)
        {
            var go = beastInTeamParent.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        PlayerData.instance.SaveTeams();

        var beastInTeamByTeamId = PlayerData.instance.GetTeams();
        var beastIds = beastInTeamByTeamId[PlayerData.instance.CurrentTeam];

        var beastTeamInfos = new List<BeastTeamInfo>();
        for (int i = 0; i < beastIds.Count; i++)
        {
            var beastTeamInfo = PlayerData.instance.GetBeastData(beastIds[i]);
            beastTeamInfos.Add(beastTeamInfo);
        }

        for (int i = 0; i < beastIds.Count; i++)
        {
            var beastId = beastIds[i];

            var beastTeamInfo = PlayerData.instance.GetBeastData(beastId);

            var go = ObjectPool.Instance.GetGameObject(beastInTeamPref, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);

            go.transform.SetParent(beastInTeamParent);

            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

            beastTeamInfo.beastItemViewGo = go;
            Action actionShort = () => RemoveBeastFromTeam(beastTeamInfo);
            Action actionLong =
                () => ShowActionDialogFromMonsterInTeam(go.transform.position, beastInTeamParent, beastTeamInfo, beastTeamInfos, EnableType.InTeamEditTeam);

            var btiv = go.GetComponent<BeastInTeamItemView>();
            btiv.SetUp(BeastInTeamSlotType.BeastInfo, beastTeamInfo, "", actionShort, actionLong);
        }

        emptySlotAmout = PlayerData.instance.OpenSlotAmount - beastIds.Count;

        Debug.Log("emptySlotAmout:" + emptySlotAmout);

        if (emptySlotAmout > 0)
        {
            for (int k = 0; k < emptySlotAmout; k++)
            {
                var go = ObjectPool.Instance.GetGameObject(beastInTeamPref, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
                go.transform.SetParent(beastInTeamParent);
                go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

                var btiv = go.GetComponent<BeastInTeamItemView>();
                btiv.SetUp(BeastInTeamSlotType.Empty, null, "", null, null);
            }
        }

        //buy slot
        if (PlayerData.instance.OpenSlotAmount < 10)
        {
            var go = ObjectPool.Instance.GetGameObject(beastInTeamPref, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            go.transform.SetParent(beastInTeamParent);
            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            var gem = MergeBeast.Utils.FormatNumber(Constant.slotPrices[PlayerData.instance.OpenSlotAmount + 1]);

            Action actionLong = () => ShowDialogBuySlot(Constant.slotPrices[PlayerData.instance.OpenSlotAmount + 1]);
            Action actionShort = () => ShowDialogBuySlot(Constant.slotPrices[PlayerData.instance.OpenSlotAmount + 1]);

            var btiv = go.GetComponent<BeastInTeamItemView>();
            btiv.SetUp(BeastInTeamSlotType.Lock, null, gem, actionShort, actionLong);
        }

        CalTeamBuffs(null);

        endRefreshBeastInTeam = true;
    }

    private void ShowDialogBuySlot(int price)
    {
        buyDialog.gameObject.SetActive(true);
        Action yesAction = () =>
        {
            buyDialog.gameObject.SetActive(false);

            var money = Helper.GetMoney(CurrencyType.Gem);
            if (money >= price)
            {
                Helper.AddMoney(CurrencyType.Gem, -price);
                PlayerData.instance.OpenSlotAmount++;
                GenBeastInTeam();
            }
            else
            {
                confirmDialog.SetUp("宝石不足", "你的货币不足，无法使用");
                confirmDialog.gameObject.SetActive(true);
            }

            buyDialog.gameObject.SetActive(false);
        };
        Action noAction = () => { buyDialog.gameObject.SetActive(false); };
        var gem = MergeBeast.Utils.FormatNumber(Constant.slotPrices[PlayerData.instance.OpenSlotAmount + 1]);
        buyDialog.SetUp("购买槽位", "你确定要购买该槽位吗？", gem, yesAction, noAction);
    }

    public bool AddBeastToTeam(BeastTeamInfo beastTeamInfo)
    {
        if (emptySlotAmout <= 0 || beastTeamInfo.curStar <= 0) return false;

        ObjectPool.Instance.ReleaseObject(beastTeamInfo.beastItemViewGo);

        var beastInTeamByTeamId = PlayerData.instance.GetTeams();

        if (beastInTeamByTeamId[PlayerData.instance.CurrentTeam].Contains(beastTeamInfo.beastId))
            throw new Exception();

        beastInTeamByTeamId[PlayerData.instance.CurrentTeam].Add(beastTeamInfo.beastId);

        GenBeastInTeam();

        beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam].Remove(beastTeamInfo);

        return true;
    }

    public void UpdateDataBeastInCurTeam(BeastTeamInfo beastTeamInfo)
    {
        beastTeamInfo.beastItemViewGo.GetComponent<BeastInTeamItemView>().UpdateData(beastTeamInfo);
    }

    private void RemoveBeastFromTeam(BeastTeamInfo beastTeamInfo)
    {
        // Stash(beastTeamInfo);

        ObjectPool.Instance.ReleaseObject(beastTeamInfo.beastItemViewGo);

        var beastInTeamByTeamId = PlayerData.instance.GetTeams();
        beastInTeamByTeamId[PlayerData.instance.CurrentTeam].Remove(beastTeamInfo.beastId);

        GenBeastInTeam();

        beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam].Add(beastTeamInfo);
        GenMonster(beastTeamInfo, beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam]);
    }

    private void Stash(BeastTeamInfo beastTeamInfo)
    {
        var expRetain = LevelHelper.ExpWhenStash(beastTeamInfo.curLevel);
        PlayerData.instance.CurExp += expRetain;
        beastTeamInfo.curLevel = 0;
        PlayerData.instance.SaveBeastDatas();
    }

    public void GenMonsterList()
    {
        scrollMonsterList.verticalNormalizedPosition = 0f;

        while (monsterListParent.childCount > 0)
        {
            var go = monsterListParent.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        var beastInTeamByTeamId = PlayerData.instance.GetTeams();
        var beastIds = beastInTeamByTeamId[PlayerData.instance.CurrentTeam];
        beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam] = PlayerData.instance.GetBeastTeamInfos().Where(bif => !beastIds.Contains(bif.beastId)).ToList();

        for (int k = 0; k < beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam].Count(); k++)
        {
            var beastTeamInfo = beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam].ElementAt(k);
            GenMonster(beastTeamInfo, beastTeamInfoByTeamId[PlayerData.instance.CurrentTeam]);
        }
    }

    // private IEnumerator OnValueChange()
    // {
    //     float time = 0f;
    //     while (true)
    //     {
    //         yield return null;
    //         if (scrollMonsterList.velocity != Vector2.zero)
    //         {
    //             onDraggingListMonster = true;
    //             time = 0.2f;
    //             Debug.Log("onDraggingListMonster:" + scrollMonsterList.velocity);
    //         }
    //         else
    //         {
    //             time -= Time.deltaTime;
    //             if (onDraggingListMonster && time <= 0)
    //             {
    //                 Debug.Log("onDraggingListMonster:" + DateTime.Now);
    //                 onDraggingListMonster = false;
    //                 time = 0f;
    //             }
    //         }
    //     }
    // }

    private void GenMonster(BeastTeamInfo beastTeamInfo, List<BeastTeamInfo> beastTeamInfos)
    {
        var go = ObjectPool.Instance.GetGameObject(monsterInListPref, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
        go.transform.SetParent(monsterListParent);
        go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

        beastTeamInfo.beastItemViewGo = go;
        Action actionLong =
         () => ShowActionDialogFromMonsterList(go.transform.position, monsterListParent, beastTeamInfo, beastTeamInfos, EnableType.FromListEditTeam);

        Action actionShort = () =>
        {
            if (dragChecker.onDrag) return;

            bool ss = AddBeastToTeam(beastTeamInfo);
            if (!ss) actionLong.Invoke();
        };

        var btiv = go.GetComponent<BeastTeamItemView>();
        btiv.SetUp(beastTeamInfo, actionShort, actionLong);
    }

    private void ShowActionDialogFromMonsterList(Vector3 pos, Transform parentTeam, BeastTeamInfo beastTeamInfo, List<BeastTeamInfo> datas, EnableType enableType)
    {
        Debug.Log("possssssss:" + pos);

        beastActionDialog.pivot = new Vector3(pos.x > 0 ? 1 : 0, pos.y > 0 ? 1 : 0);
        beastActionDialog.transform.position = new Vector3(pos.x, pos.y, 1);

        Action action2 = () => ShowPanelInfo(parentTeam, beastTeamInfo, datas, EnableType.FromListEditTeam);
        Action action1 = () =>
        {
            var ss = AddBeastToTeam(beastTeamInfo);
            if (!ss) action2.Invoke();
        };

        beastActionDialog.GetComponent<BeastActionDialog>()
            .SetUp(emptySlotAmout > 0 && beastTeamInfo.curStar > 0, "加入", "查看信息", action1, action2);

        // beastActionDialog.gameObject.SetActive(true);

        beastActionDialog.parent.gameObject.SetActive(true);
    }

    private void ShowActionDialogFromMonsterInTeam(Vector3 pos, Transform parentTeam, BeastTeamInfo beastTeamInfo, List<BeastTeamInfo> datas, EnableType enableType)
    {
        beastActionDialog.pivot = new Vector3(pos.x > 0 ? 1 : 0, pos.y > 0 ? 1 : 0);
        beastActionDialog.transform.position = new Vector3(pos.x, pos.y, 1);

        Action action1 = () => RemoveBeastFromTeam(beastTeamInfo);
        Action action2 = () => ShowPanelInfo(parentTeam, beastTeamInfo, datas, enableType);

        // beastActionDialog.gameObject.SetActive(true);
        beastActionDialog.GetComponent<BeastActionDialog>()
            .SetUp(true, "放入仓库", "查看信息", action1, action2);

        beastActionDialog.parent.gameObject.SetActive(true);
    }

    private void ShowPanelInfo(Transform parentTeam, BeastTeamInfo beastTeamInfo, List<BeastTeamInfo> datas, EnableType enableType)
    {
        if (beastInfoHomePanel.gameObject.activeSelf) return;

        beastInfoHomePanel?.gameObject.SetActive(true);
        beastInfoHomePanel.Init(parentTeam, beastTeamInfo, datas, enableType);
    }

    public void OnClickTeam(int team)
    {
        if (team == TEAM_3 && !PlayerData.instance.GetTeams().ContainsKey(TEAM_3))
        {
            //buy ok
            ShowDialogBuyTeam3();
            return;

        }

        PlayerData.instance.CurrentTeam = team;

        ShowButtons();

        GenMonsterList();

        GenBeastInTeam();
    }

    private void ShowDialogBuyTeam3()
    {
        buyDialog.gameObject.SetActive(true);
        Action yesAction = () =>
        {
            buyDialog.gameObject.SetActive(false);

            var money = Helper.GetMoney(CurrencyType.Gem);
            var price = 100;
            if (money >= price)
            {
                Helper.AddMoney(CurrencyType.Gem, -price);

                PlayerData.instance.GetTeams().Add(TEAM_3, new List<BeastId>());
                PlayerData.instance.SaveTeams();

                PlayerData.instance.CurrentTeam = TEAM_3;
                ShowButtons();
                GenMonsterList();
                GenBeastInTeam();
            }
            else
            {
                confirmDialog.SetUp("宝石不足", "你的货币不足，无法使用");
                confirmDialog.gameObject.SetActive(true);
            }
        };
        Action noAction = () => { buyDialog.gameObject.SetActive(false); };
        var gem = MergeBeast.Utils.FormatNumber(Constant.teamSlotPrices[PlayerData.instance.GetTeams().Count + 1]);
        buyDialog.SetUp("购买队伍", "你确定要购买该队伍槽位吗？", gem, yesAction, noAction);
    }

    public void Confirm()
    {
        ScreenManager.Instance.DeActiveScreen();
    }

    public void Confirm2()
    {
        gameObject.SetActive(false);
        this.PostEvent(EventID.OnConfirmEditTeam, null);
    }

    public void OnTouch(Vector3 pos, string title, string des)
    {
        teamBuffInfoDialog.pivot = new Vector3(pos.x > 0 ? 1 : 0, 0);
        teamBuffInfoDialog.transform.position = new Vector3(pos.x, pos.y + 0.4f, 1);
        teamBuffInfoDialog.gameObject.SetActive(true);
        teamBuffInfoDialog.GetComponent<TeamBuffInfoDialog>()
          .SetUp(title, des);
    }

    public void ExitTouch(Vector3 position)
    {
        teamBuffInfoDialog.gameObject.SetActive(false);
    }

    public void OnClickFillBtn()
    {
        beastActionDialog.parent.gameObject.SetActive(false);
    }

    public void OnClickBeastInTeam(int child)
    {
        var go = beastInTeamParent.transform.GetChild(child);
        var missionItemView = go.GetComponent<BeastInTeamItemView>();
        missionItemView.longTouchAction?.Invoke();
    }

    public Vector3 GetBeastInTeamPos(int child)
    {
        var go = beastInTeamParent.transform.GetChild(child);
        var missionItemView = go.GetComponent<BeastInTeamItemView>();
        return missionItemView.transform.position;
    }
}

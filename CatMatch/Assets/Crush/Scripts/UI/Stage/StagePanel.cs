using System;
using System.Collections;
using System.Collections.Generic;
using MergeBeast;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Observer;
using System.Linq;
using UnityEngine.Events;

public class StagePanel : MonoBehaviour
{
    public static StagePanel instance;

    public Transform missionParent;
    public GameObject missionPref;

    public List<Button> selectTeamButtons;
    public Sprite teamOnSp;
    public Sprite teamOffSp;
    public Sprite moreTeamSp;

    public ScrollRect scrollRect;
    public BuyDialog buyDialog;
    public TeamBuildPanel teamBuildPanel;
    public BuyCurrencyDialog buyCurrencyDialog;
    public ConfirmDialog confirmDialog;
    public PanelWin panelWin;
    public ShopPanel shopPanel;

    public Text chapterNumberTxt;
    public Text chapterNameTxt;

    public Text gemValueTxt;
    public Text playAmountTxt;
    public Text sweepAmountTxt;
    public Text tileValueTxt;

    public Button nextChapter;
    public Button prevChapter;
    public Sprite nextChapterOn;
    public Sprite nextChapterOff;
    public Sprite prevChapterOn;
    public Sprite prevChapterOff;

    public PreparePanel preparePanel;
    public StageInfoPanel stageInfoPanel;

    private int currentRegion;
    private int timeSec;
    public bool endRefresh;

    void Awake()
    {
        instance = this;
        this.RegisterListener(EventID.OnConfirmEditTeam, (sender, param) => ConfirmEditTeam());
        this.RegisterListener(EventID.EndGameCrush, (sender, param) => Init());

        this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => GemChange(param));
        this.RegisterListener(EventID.TileTicketChange, (sender, param) => TileTicketChange(param));
        this.RegisterListener(EventID.SweepChange, (sender, param) => SweepChange(param));
        this.RegisterListener(EventID.PlayAmountChange, (sender, param) => PlayAmountChange(param));
    }

    private void TileTicketChange(object param)
    {
        tileValueTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.TileTicketAmount);
    }

    private void GemChange(object param)
    {
        gemValueTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());
    }

    private void PlayAmountChange(object param)
    {
        playAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.PlayStageAmount);
    }

    private void SweepChange(object param)
    {
        sweepAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.SweepAmount);
    }

    public void Init(int region = -1)
    {
        // scrRect.sizeDelta = new Vector2(scrRect.sizeDelta.x, bottomRect.position.y - )
        // scrRect.position = topRect.position;

        if (region != -1)
        {
            currentRegion = region;
        }
        else
        {
            var canNextRegion = (currentRegion + 1);
            var maxStageOfRegion = canNextRegion * PlayerData.instance.MaxStagePerRegion;
            if (maxStageOfRegion + 1 == PlayerData.instance.MaxStage && canNextRegion <= PlayerData.instance.MaxRegion)
            {
                currentRegion += 1;
            }
        }

        var chapterData = GameData.Instance.chapterDatas.GetChapterData(currentRegion);
        chapterNameTxt.text = chapterData.title;
        chapterNumberTxt.text = string.Format("章节：{0}", chapterData.chapter.ToString());

        RefreshStages();

        playAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.PlayStageAmount);
        gemValueTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());
        sweepAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.SweepAmount);
        tileValueTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.TileTicketAmount);

        int maxRegion = (PlayerData.instance.MaxStage - 1) / PlayerData.instance.MaxStagePerRegion;
        if (maxRegion >= PlayerData.instance.MaxRegion)
            maxRegion = PlayerData.instance.MaxRegion;

        nextChapter.image.sprite = currentRegion < maxRegion ? nextChapterOn : nextChapterOff;
        nextChapter.interactable = currentRegion < maxRegion;
        prevChapter.image.sprite = currentRegion > 0 ? prevChapterOn : prevChapterOff;
        prevChapter.interactable = currentRegion > 0;

        ShowTeamButtons();
    }

    private void RefreshStages()
    {
        while (missionParent.childCount > 0)
        {
            var go = missionParent.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        // scrollRect.verticalNormalizedPosition = 1;

        int firstStage = currentRegion * PlayerData.instance.MaxStagePerRegion + 1;

        int playMissionAmount = PlayerData.instance.PlayStageAmount;
        int sweepAmount = PlayerData.instance.SweepAmount;

        var myStagePair = PlayerData.instance.GetMyStagePair();

        // Debug.Log(sweepAmount);
        var maxStageOfRegion = (currentRegion + 1) * PlayerData.instance.MaxStagePerRegion;
        var max = maxStageOfRegion > PlayerData.instance.MaxStage ? PlayerData.instance.MaxStage : maxStageOfRegion;

        Debug.Log("firstStage:" + firstStage + " maxStageOfRegion:" + maxStageOfRegion + "  PlayerData.instance.MaxStage:" + PlayerData.instance.MaxStage + " max:" + max);

        for (int k = firstStage; k <= max; k++)
        {
            var go = ObjectPool.Instance.GetGameObject(missionPref, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(missionParent);
            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

            var myStage = myStagePair.ContainsKey(k) ? myStagePair[k] : null;

            var stage = GameData.Instance.stageDatas.GetStageData(k);
            var goldRW = stage.rewardDatas.FirstOrDefault(rd => rd.rewardType == RewardType.Gold);
            var firstWinStages = PlayerData.instance.FirstWinStages;

            var gold = System.Numerics.BigInteger.Pow(2, k) * 10;

            var missionModel = new StageModel()
            {
                stage = k,
                numStar = myStage != null ? myStage.numStar : 0,
                onBattle = playMissionAmount > 0,
                onSweep = sweepAmount > 0 && (myStage != null && myStage.numStar == 3) && playMissionAmount > 0,
                gem = firstWinStages.Contains(k.ToString()) ? "" : PlayerData.instance.GemFirstWin.ToString(),
                soul = goldRW != null ? Helper.FormatNumber(goldRW.amount + gold) : Helper.FormatNumber(gold)
            };
            var missionItemView = go.GetComponent<StageItemView>();
            missionItemView.SetUp(missionModel);

            missionItemView.battleBtn.onClick.RemoveAllListeners();
            missionItemView.sweepBtn.onClick.RemoveAllListeners();
            missionItemView.infoBtn.onClick.RemoveAllListeners();

            missionItemView.battleBtn?.onClick.AddListener(() => { OnClickBattleBtn(missionModel); });
            missionItemView.sweepBtn?.onClick.AddListener(() => { OnClickSweepBtn(missionModel); });
            missionItemView.infoBtn?.onClick.AddListener(() => { OnClickStageInfoBtn(missionModel); });

            // if (k == maxStageOfRegion && myStage != null && myStage.numStar > 0)
            // {
            //     go = ObjectPool.Instance.GetGameObject(missionPref, Vector3.zero, Quaternion.identity);
            //     go.transform.SetParent(missionParent);
            //     go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            //     var missionItemView2 = go.GetComponent<StageItemView>();
            //     missionItemView2.SetUp(null);
            //     missionItemView2.nextChapter.onClick.RemoveAllListeners();
            //     missionItemView2.nextChapter.onClick.AddListener(() => { Init(currentRegion + 1); });
            // }
        }

        // scrollRect.verticalNormalizedPosition = 1 ;
        StartCoroutine(EndFrame());
        endRefresh = true;
        if (max == 2
            && PlayerPrefs.GetInt(CrushStringHelper.TutMap, 0) == 1)
        {
            PlayerPrefs.SetInt(CrushStringHelper.TutMap, 2);
            TutorialMap.Instance.OnClickJoinBattle(1);
        }
    }

    private void OnClickStageInfoBtn(StageModel missionModel)
    {
        stageInfoPanel.gameObject.SetActive(true);
        stageInfoPanel.Init(missionModel.stage);
    }

    IEnumerator EndFrame()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }

    void OnEnable()
    {
        var lastOnLineTimeStr = PlayerPrefs.GetString(CrushStringHelper.LastOnLineDateTime, "");
        DateTime lastOnlineTime = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(lastOnLineTimeStr))
        {
            timeSec = PlayerPrefs.GetInt(CrushStringHelper.LastTimeSecAgain);
            lastOnlineTime = DateTime.Parse(lastOnLineTimeStr);
            timeSec += (int)(DateTime.UtcNow - lastOnlineTime).TotalSeconds;
        }
        Debug.Log("lastOnLineTimeStr:" + lastOnLineTimeStr + " lastOnlineTime:" + lastOnlineTime + " now:"
            + DateTime.UtcNow + " sec:" + (int)(DateTime.UtcNow - lastOnlineTime).TotalSeconds);
        Debug.Log("timeSec:" + timeSec);
        StartCoroutine(RunTimeIE());
    }

    void OnDisable()
    {
        // if (UnbiasedTime.Instance == null) return;
        var lastOnLineTimeStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("lastOnLineTimeStr:" + lastOnLineTimeStr);
        Debug.Log("timeSec:" + timeSec);

        PlayerPrefs.SetString(CrushStringHelper.LastOnLineDateTime, lastOnLineTimeStr);
        PlayerPrefs.SetInt(CrushStringHelper.LastTimeSecAgain, timeSec);
    }

    void OnApplicationQuit()
    {
        Debug.Log("StagePanel OnApplicationQuit");
        var lastOnLineTimeStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log("lastOnLineTimeStr:" + lastOnLineTimeStr);
        Debug.Log("timeSec:" + timeSec);

        PlayerPrefs.SetString(CrushStringHelper.LastOnLineDateTime, lastOnLineTimeStr);
        PlayerPrefs.SetInt(CrushStringHelper.LastTimeSecAgain, timeSec);
    }

    IEnumerator RunTimeIE()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeSec += 1;

            int amount = timeSec / 600;
            if (amount >= 1)
            {
                Debug.Log("amount:" + amount);

                timeSec -= amount * 600;

                PlayerData.instance.PlayStageAmount += amount;
                playAmountTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.PlayStageAmount);
            }
        }
    }

    void UpdateStageItemView()
    {
        // int childCount = missionParent.childCount;
        // for(int k = 0; k < childCount; k++)
        // {
        //     var stageItemView = missionParent.GetChild(k).GetComponent<StageItemView>();
        //     stageItemView.SetUp(playMissionAmount, sweepAmount);
        // }
    }

    public void OnClickBattleBtn(StageModel missionModel)
    {
        if (PlayerData.instance.PlayStageAmount <= 0)
        {
            var price = 0;
            var purchasedAmount = PlayerPrefs.GetInt(CrushStringHelper.BuyPlayTicketAmount, 0);
            var nextPurchasedAmount = purchasedAmount + 1;
            if (nextPurchasedAmount > Constant.buyPlayTicketAmount.Count())
            {
                price = Constant.buyPlayTicketAmount[Constant.buyPlayTicketAmount.Count()];
            }
            else
            {
                price = Constant.buyPlayTicketAmount[nextPurchasedAmount];
            }

            Action yes = () =>
            {
                buyCurrencyDialog.gameObject.SetActive(false);

                if (Utils.GetCurrentRubyMoney() >= price)
                // if(false)
                {
                    MergeBeast.Utils.AddRubyCoin(-price);
                    gemValueTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());

                    PlayerPrefs.SetInt(CrushStringHelper.BuyPlayTicketAmount, nextPurchasedAmount);
                    PlayerData.instance.PlayStageAmount += 10;
                    StartBattle(missionModel.stage);
                }
                else
                {
                    confirmDialog.SetUp("宝石不足", "你的宝石不足");
                    confirmDialog.gameObject.SetActive(true);
                    // ScreenManager.Instance.ShowNoti(err);
                }
            };
            Action no = () =>
            {
                buyCurrencyDialog.gameObject.SetActive(false);
            };

            buyCurrencyDialog.Setup(CurrencyType.PlayAmount, price, 5, purchasedAmount, yes, no);
            buyCurrencyDialog.gameObject.SetActive(true);
            return;
        }
        StartBattle(missionModel.stage);
    }

    private void StartBattle(int stage)
    {
        // var curTeam = PlayerData.instance.GetCurTeam();
        // if (curTeam.Count <= 0)
        // {
        //     confirmDialog.SetUp("No monster", "No monster in team");
        //     confirmDialog.gameObject.SetActive(true);
        //     return;
        // }

        PlayerData.instance.CurStage = stage;
        // PlayerData.instance.PlayStageAmount -= 1;

        // Debug.Log("stage:" + PlayerData.instance.CurStage);

        // Scene crush = SceneManager.GetSceneByName(StringDefine.SCENE_CRUSH);
        // if (!crush.isLoaded)
        // {
        //     SceneManager.LoadScene(StringDefine.SCENE_CRUSH, LoadSceneMode.Additive);
        // }
        preparePanel.gameObject.SetActive(true);
    }

    public void OnClickSweepBtn(StageModel missionModel)
    {
        if (PlayerData.instance.SweepAmount <= 0 || PlayerData.instance.PlayStageAmount <= 0) return;

        PlayerData.instance.SweepAmount -= 1;
        PlayerData.instance.PlayStageAmount -= 1;

        panelWin.gameObject.SetActive(true);

        var enemyMonsterDatas = LevelHelper.GetStageEnemies(missionModel.stage);
        panelWin.UpdateEndGame3Star(RewardHelper.GetStageRewards(missionModel.stage, enemyMonsterDatas));

        RefreshStages();
    }

    public void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        // this.PostEvent(EventID.BackToMap, null);
        MergeBeast.MapController._Instance?.ShowRegions();
    }

    private void ConfirmEditTeam()
    {
        ShowTeamButtons();
    }

    private void ShowTeamButtons()
    {
        for (int k = 0; k < selectTeamButtons.Count; k++)
        {
            selectTeamButtons[k].image.sprite = k != PlayerData.instance.CurrentTeam - 1 ? teamOffSp : teamOnSp;
            if (k == 2)
            {
                if (!PlayerData.instance.GetTeams().ContainsKey(3))
                {
                    selectTeamButtons[k].GetComponentInChildren<Text>().text = "";
                    selectTeamButtons[k].image.sprite = moreTeamSp;
                }
                else
                    selectTeamButtons[k].GetComponentInChildren<Text>().text = "队伍3";
            }
        }
    }

    public void OnClickTeam(int team)
    {
        if (team == 3 && !PlayerData.instance.GetTeams().ContainsKey(3))
        {
            //buy ok
            ShowDialogBuyTeam3();
            return;

        }

        PlayerData.instance.CurrentTeam = team;

        ShowTeamButtons();
    }

    private void ShowDialogBuyTeam3()
    {
        buyDialog.gameObject.SetActive(true);

        int price = Constant.teamSlotPrices[PlayerData.instance.GetTeams().Count + 1];

        Action yesAction = () =>
        {
            if (Utils.GetCurrentRubyMoney() >= price)
            {
                MergeBeast.Utils.AddRubyCoin(-price);
                gemValueTxt.text = MergeBeast.Utils.FormatNumber(MergeBeast.Utils.GetCurrentRubyMoney());

                PlayerData.instance.GetTeams().Add(3, new List<BeastId>());
                PlayerData.instance.SaveTeams();
                ShowTeamButtons();
            }
            else
            {
                confirmDialog.SetUp("宝石不足", "你的宝石不足");
                confirmDialog.gameObject.SetActive(true);
            }

            buyDialog.gameObject.SetActive(false);
        };
        Action noAction = () => { buyDialog.gameObject.SetActive(false); };
        buyDialog.SetUp("购买队伍", "你确定要购买该队伍槽位吗？", MergeBeast.Utils.FormatNumber(price), yesAction, noAction);
    }


    public void OnClickEditTeam()
    {
        teamBuildPanel?.gameObject.SetActive(true);
    }

    public void OnClickNextChapter()
    {
        Init(currentRegion + 1);
    }

    public void OnClickPrevChapter()
    {
        Init(currentRegion - 1);
    }

    public void OnClickShop()
    {
        shopPanel.gameObject.SetActive(true);
    }

    public void OnClickBattleBtnTut(int child)
    {
        var go = missionParent.transform.GetChild(child);
        var missionItemView = go.GetComponent<StageItemView>();
        OnClickBattleBtn(missionItemView.stageModel);
    }

    public Button GetBattleBtnTut(int child)
    {
        var go = missionParent.transform.GetChild(child);
        var missionItemView = go.GetComponent<StageItemView>();
        return missionItemView.battleBtn;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using MergeBeast;
using Observer;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMap : TutBase
{
    public static TutorialMap Instance;

    public Button regionBtn;
    public Button shopBtn;
    public Transform neoOnReceivePanelTf;
    public Button closeShopBtn;

    public Button joinBattleBtn;
    public Button startBattleBtn;
    public Button editTeamBtn;
    public Button levelUpBtn;
    public Button closeLevelUpPanelBtn;
    public Button confirmBuildTeamBtn;

    private int levelUpMonsterTh = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("iiiiiiiiii");
        }
        this.RegisterListener(EventID.OnClickReceiveShopTut, (sender, param) => ClickCloseShopPanel());
        // this.RegisterListener(EventID.OnClickPremiumTut, (sender, param) => OnClickPremiumTut());
    }

    protected override void Start()
    {
        base.Start();
        // ShowFocus(Vector3.zero, () => { });
        // ShowText("hahaaaaaa", () =>
        // {
        //     HideText();
        // });
    }

    void OnEnable()
    {
        OnClickShopBtn();
    }

    public void OnClickShopBtn()
    {
        var tutMap = PlayerPrefs.GetInt(CrushStringHelper.TutMap, 0);
        Debug.Log("OnClickShopBtn tutMap:" + tutMap);
        if (tutMap == 0)
        {
            StartCoroutine(OnClickShopBtnIE());
            PlayerPrefs.SetInt(CrushStringHelper.TutMap, 1);
        }

        IEnumerator OnClickShopBtnIE()
        {
            yield return new WaitForSeconds(0.5f);
            ShowFocus(shopBtn.transform.position, () =>
            {
                MapController._Instance.OnClickShop();
                ClickPremiumChestBtn();
            });
        }
    }

    private void ClickPremiumChestBtn()
    {
        Debug.Log("OnClickShopTut");

        StartCoroutine(OnClickShopTutIE());
        IEnumerator OnClickShopTutIE()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                if (ShopPanel.instance.premeiumChestBtn != null)
                {
                    Debug.Log("psssssssss:" + ShopPanel.instance.premeiumChestBtn.transform.position);

                    ShowFocus(ShopPanel.instance.premeiumChestBtn.transform.position, () =>
                    {
                        ShopPanel.instance.TutorialBuy(1);
                        HideFocus();
                        ResetAllEffect();
                        ClickReceivePanel();
                    });
                    break;
                }
            }
        }
    }

    void ClickReceivePanel()
    {
        ShowOnlyHand(neoOnReceivePanelTf.position);
        // ShowFocus(neoOnReceivePanelTf.position, () =>
        // {
        //     ClickCloseShopPanel();
        // ShopPanel.instance.TutorialBuy(0);
        // HideFocus();
        // ResetAllEffect();
        // });
    }

    private void ClickCloseShopPanel()
    {
        var tutMap = PlayerPrefs.GetInt(CrushStringHelper.TutMap, 0);
        if (tutMap == 1)
        {
            ShowFocus(closeShopBtn.transform.position, () =>
            {
                ShopPanel.instance.gameObject.SetActive(false);
                ClickRegionBtn();
            });
        }
    }

    void ClickRegionBtn()
    {
        ShowFocus(regionBtn.transform.position, () =>
        {
            MapController._Instance.OnClickRegion(0);
            OnClickJoinBattle(0);
        });
    }

    public void OnClickJoinBattle(int child)
    {
        StartCoroutine(OnClickJoinBattleIE());
        IEnumerator OnClickJoinBattleIE()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (StagePanel.instance.endRefresh)
                {
                    ShowFocus(StagePanel.instance.GetBattleBtnTut(child).transform.position, () =>
                    {
                        StagePanel.instance.OnClickBattleBtnTut(child);
                        var tutMap = PlayerPrefs.GetInt(CrushStringHelper.TutMap, 0);
                        if (tutMap == 1)
                            OnClickStartBattle();
                        else if (tutMap == 2)
                            OnClickEditTeam();
                    });
                    break;
                }
            }
        }
    }

    void OnClickStartBattle()
    {
        ShowFocus(startBattleBtn.transform.position, () =>
        {
            PreparePanel.instance.OnClickBattle();
            if (levelUpMonsterTh > 1)
            {
                HideFocus();
                ResetAllEffect();
            }
        });
    }

    // turn 2
    void OnClickEditTeam()
    {
        ShowFocus(editTeamBtn.transform.position, () =>
        {
            PreparePanel.instance.OnClickEditTeam();

            OnClickBeastInTeam(0);
        });
    }

    void OnClickBeastInTeam(int child)
    {
        StartCoroutine(OnClickBeastInTeamIE());
        IEnumerator OnClickBeastInTeamIE()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (TeamBuildPanel.Instance.endRefreshBeastInTeam)
                {
                    ShowFocus(TeamBuildPanel.Instance.GetBeastInTeamPos(child), () =>
                    {
                        TeamBuildPanel.Instance.OnClickBeastInTeam(child);
                        OnClickBeastInfoButton();
                    });
                    break;
                }
            }
        }
    }

    void OnClickBeastInfoButton()
    {
        StartCoroutine(OnClickBeastInfoButtonIE());
        IEnumerator OnClickBeastInfoButtonIE()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (BeastActionDialog.Instance.enabled)
                {
                    ShowFocus(BeastActionDialog.Instance.txt2.transform.position, () =>
                    {
                        BeastActionDialog.Instance.Action2();
                        // BeastActionDialog.Instance.gameObject.SetActive(false);

                        levelUpMonsterTh++;
                        OnClickLevelUp();
                    });
                    break;
                }
            }
        }
    }

    void OnClickLevelUp()
    {
        ShowFocus(levelUpBtn.transform.position, () =>
        {
            var level = BeastInfoHomePanel.Instance.LevelUp();
            if (levelUpMonsterTh == 1)
            {
                if (level == 10)
                {
                    OnCloseLevelUpPanel();
                }
            }
            else if(level == 5)
                 OnCloseLevelUpPanel();
        });
    }

    void OnCloseLevelUpPanel()
    {
        ShowFocus(closeLevelUpPanelBtn.transform.position, () =>
        {
            BeastInfoHomePanel.Instance.Close();
            if (levelUpMonsterTh == 1) OnClickBeastInTeam(1);
            else
            {
                ClickConfirmBuildTeam();
            }
        });
    }

    void ClickConfirmBuildTeam()
    {
        ShowFocus(confirmBuildTeamBtn.transform.position, () =>
       {
           TeamBuildPanel.Instance.Confirm2();
           OnClickStartBattle();
       });
    }
}

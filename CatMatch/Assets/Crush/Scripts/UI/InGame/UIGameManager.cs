using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameManager : MonoBehaviour
{
    public static UIGameManager instance;

    // public GameObject panelEndGame;
    public PanelInfo panelInfo;
    public PanelLoading panelLoading;
    public PanelRetreat panelRetreat;
    public PanelWin panelWin;
    public PanelLose panelLose;
    public PanelStats panelStats;

    public Text timeTxt;

    private List<BeastInfoItem> beastInfoItemsAtEndGame;

    void Start()
    {
        instance = this;

        panelLoading.StartLoading();
    }

    public void StartGame()
    {
        panelRetreat.gameObject.SetActive(false);
        panelWin.gameObject.SetActive(false);
        panelStats.gameObject.SetActive(false);
        panelLose.gameObject.SetActive(false);

        panelInfo.RestartGame();
    }

    public void SetTimeText(string time)
    {
        timeTxt.text = time;
    }

    public void EndGame(bool isWin, List<BeastInfoItem> beastInfoItems, List<RewardModel> rewardModels)
    {
        beastInfoItemsAtEndGame = beastInfoItems;
        if (isWin)
        {
            panelWin.gameObject.SetActive(true);
            panelWin.UpdateEndGame(beastInfoItems, rewardModels);
        }
        else
        {
            panelLose.gameObject.SetActive(true);
            panelLose.UpdateBeastInfoItems(beastInfoItems);
        }
    }

    public void RestartGame()
    {
        GameManager.instance.StartGame();
    }

    public void UpdateBeastInfoItems(List<BeastInfoItem> beastInfoItems)
    {
        panelInfo.beastScrollView.UpdateBeastInfoItems(beastInfoItems);
    }

    public void InitSkill3(BeastBase beast)
    {
        panelInfo.InitSkill3(beast);
    }

    public void Retreat()
    {
        if (!GameManager.instance.IsPlaying()) return;

        Time.timeScale = 0;
        panelRetreat.gameObject.SetActive(true);
    }

    public void Stats()
    {
        panelStats.gameObject.SetActive(true);
        panelStats.BeastInfoItemsAtEndGameCache(beastInfoItemsAtEndGame);
    }
}

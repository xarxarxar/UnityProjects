using System;
using System.Collections;
using System.Collections.Generic;
using MergeBeast;
using Observer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreparePanel : MonoBehaviour
{
    public static PreparePanel instance;

    public RectTransform enemyContainer;
    public RectTransform myContainer;
    public TeamBuildPanel teamBuildPanel;
    public ConfirmDialog confirmDialog;

    public RectTransform myRect;
    public RectTransform enemyRect;
    public RectTransform vs;

    public Button nextTeamBtn;
    public Button prevTeamBtn;
    public Text teamNameTxt;

    public Text enemyPowerTxt;
    public Text playerPowerTxt;

    void Awake()
    {
        instance = this;

        this.RegisterListener(EventID.OnConfirmEditTeam, (sender, param) => ConfirmEditTeam());
    }

    private void ConfirmEditTeam()
    {
        Refresh(PlayerData.instance.CurrentTeam);
    }

    private void OnEnable()
    {
        myRect.position = vs.position;
        enemyRect.position = vs.position;

        Refresh(PlayerData.instance.CurrentTeam);
    }

    void Refresh(int team)
    {
        System.Numerics.BigInteger powerEnemy;

        var enemyMonsterDatas = LevelHelper.GetStageEnemies(PlayerData.instance.CurStage);
        for (int k = 0; k < enemyContainer.childCount; k++)
        {
            var child = enemyContainer.GetChild(k);
            var prepare = child.GetComponent<PrepareItemView>();

            if (enemyMonsterDatas.Count > k)
            {
                var data = enemyMonsterDatas[k];
                var beast = BeastPrefs.Instance.GetBeastPref(data.beastId);
                var icon = beast.GetComponent<BeastBase>().icons[0];
                prepare.SetUp(PrepareSlotType.None, icon, data.curLevel.ToString(), data.curStar.ToString());

                powerEnemy += Power(data);
            }
            else
            {
                prepare.SetUp(PrepareSlotType.Empty, null, "", "");
            }
        }

        System.Numerics.BigInteger powerPlayer;
        var myMonsterDatas = PlayerData.instance.GetBeastPlayerTeam(team);
        for (int k = 0; k < myContainer.childCount; k++)
        {
            var child = myContainer.GetChild(k);
            var prepare = child.GetComponent<PrepareItemView>();

            if (myMonsterDatas.Count > k)
            {
                var data = myMonsterDatas[k];
                var beast = BeastPrefs.Instance.GetBeastPref(data.beastId);
                var icon = beast.GetComponent<BeastBase>().icons[0];
                prepare.SetUp(PrepareSlotType.None, icon, data.curLevel.ToString(), data.curStar.ToString());
                powerPlayer += Power(data);
            }
            else
            {
                if (PlayerData.instance.OpenSlotAmount > k)
                {
                    prepare.SetUp(PrepareSlotType.DontHaveBeast, null, "", "");
                }
                else
                    prepare.SetUp(PrepareSlotType.Lock, null, "", "");
            }
        }

        teamNameTxt.text = "队伍 " + PlayerData.instance.CurrentTeam;

        var ct = PlayerData.instance.CurrentTeam;
        prevTeamBtn.interactable = ct > 1;
        nextTeamBtn.interactable = PlayerData.instance.GetTeams().ContainsKey(ct + 1);

        prevTeamBtn.onClick.RemoveAllListeners();
        prevTeamBtn.onClick.AddListener(OnclickPrevTeamBtn);
        nextTeamBtn.onClick.RemoveAllListeners();
        nextTeamBtn.onClick.AddListener(OnclickNextTeamBtn);

        enemyPowerTxt.text = Helper.FormatNumber(powerEnemy);
        playerPowerTxt.text = Helper.FormatNumber(powerPlayer);
    }

    void OnclickPrevTeamBtn()
    {
        PlayerData.instance.CurrentTeam -= 1;
        Refresh(PlayerData.instance.CurrentTeam);
    }

    void OnclickNextTeamBtn()
    {
        PlayerData.instance.CurrentTeam += 1;
        Refresh(PlayerData.instance.CurrentTeam);
    }

    public void OnClickBattle()
    {
        var curTeam = PlayerData.instance.GetCurTeam();
        if (curTeam.Count <= 0)
        {
            confirmDialog.SetUp("没有怪物", "队伍中没有怪物");
            confirmDialog.gameObject.SetActive(true);
            return;
        }

        Debug.Log("stage:" + PlayerData.instance.CurStage);

        Scene crush = SceneManager.GetSceneByName(StringDefine.SCENE_CRUSH);
        if (!crush.isLoaded)
        {
            SceneManager.LoadScene(StringDefine.SCENE_CRUSH, LoadSceneMode.Additive);
        }
        StartCoroutine(RunOff());
    }

    IEnumerator RunOff()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    public void OnClickEditTeam()
    {
        teamBuildPanel?.gameObject.SetActive(true);
    }

    System.Numerics.BigInteger Power(BeastTeamInfo beastTeamInfo)
    {
        var m1 = 200 * Mathf.Pow(2, beastTeamInfo.curStar - 1);
        var m2 = Mathf.Pow(1.5f, 0);
        var m3 = BigIntegerHelper.Pow2((decimal)1.05f, beastTeamInfo.curLevel);

        Debug.Log("m1:"+m1 + " m2:" + m2 + " m3:" + m3.numerator + " :" + m3.denominator);

        return BigIntegerHelper.BigMultiplyFloat(m3.numerator, m1 * m2) / m3.denominator;
    }
}

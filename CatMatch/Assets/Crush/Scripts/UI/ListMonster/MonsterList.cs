using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeBeast;
using Observer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonsterList : MonoBehaviour
{
    public ShopPanel shopPanel;
    [SerializeField] private Transform _parentTeam;
    [SerializeField] private ScrollRect _scrollTeam;
    [SerializeField] private GameObject _prfItemTeam;
    [SerializeField] private BeastInfoHomePanel beastInfoHomePanel;
    public DragChecker dragChecker;

    void Awake()
    {
        this.RegisterListener(EventID.BackToMonsterList, (sender, param) => BackToMonsterList());
    }

    private void BackToMonsterList()
    {
        if(gameObject.activeSelf)
        {
            SpawnHomeTeam();
        }
    }

    void OnEnable()
    {
        SpawnHomeTeam();
    }

    public void SpawnHomeTeam()
    {
        _scrollTeam.verticalNormalizedPosition = 0f;

        while (_parentTeam.childCount > 0)
        {
            var go = _parentTeam.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        var beastTeamInfos = PlayerData.instance.GetBeastTeamInfos();

        for (int k = 0; k < beastTeamInfos.Count(); k++)
        {
            var itemData = beastTeamInfos.ElementAt(k);

            var go = ObjectPool.Instance.GetGameObject(_prfItemTeam, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);

            go.transform.SetParent(_parentTeam);

            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

            itemData.beastItemViewGo = go;

            Action action = () => ShowPanelInfo(_parentTeam, itemData, beastTeamInfos);
            Action action2 = () => ShowPanelInfo(_parentTeam, itemData, beastTeamInfos);

            var btiv = go.GetComponent<BeastTeamItemView>();
            btiv.SetUp(itemData, action, action2);
        }
    }

    private void ShowPanelInfo(Transform _parentTeam, BeastTeamInfo beastTeamInfo, List<BeastTeamInfo> datas)
    {
        if (dragChecker.onDrag) return;

        beastInfoHomePanel?.gameObject.SetActive(true);
        beastInfoHomePanel.Init(_parentTeam, beastTeamInfo, datas, EnableType.FromListMonster);
    }

    public void ClickTeamBuild()
    {
        ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.TEAM_BUILD);
    }

    public void OnClickShop()
    {
        shopPanel.gameObject.SetActive(true);
    }

    public void LoadBattle()
    {
        Scene scene = SceneManager.GetSceneByName(StringDefine.SCENE_MAP);
        if (scene.IsValid())
        {
            SceneManager.SetActiveScene(scene);
        }
        else
        {
            SceneManager.LoadScene(StringDefine.SCENE_MAP, LoadSceneMode.Additive);
        }
    }
}

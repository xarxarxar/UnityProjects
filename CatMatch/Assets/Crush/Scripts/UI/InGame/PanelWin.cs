using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeBeast;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Observer;

public class PanelWin : MonoBehaviour
{
    [SerializeField] private ScrollViewResult scrollViewResult;
    [SerializeField] private Text lostMonsterTxt;
    [SerializeField] private Text rewardTxt;
    [SerializeField] private Button statsBtn;

    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite starOff;
    [SerializeField] private Sprite starOn;
    [SerializeField] private Transform rewardContainer;
    [SerializeField] private GameObject rewardPref;

    private int type;

    public void Ok()
    {
        if (type == 1) // from in game
        {
            ObjectPool.Instance.ReleaseAll();

            Scene crush = SceneManager.GetSceneByName(StringDefine.SCENE_CRUSH);
            if (crush.isLoaded)
            {
                SceneManager.UnloadSceneAsync(crush);
            }
            this.PostEvent(EventID.EndGameCrush, null);
            // AudioManager.instance.StopAll();
        }
        else // on stage list
        {
            gameObject.SetActive(false);
        }
    }

    public void Stats()
    {
        UIGameManager.instance.Stats();
    }

    void Enable()
    {
        rewardTxt.text = LangManager.Instance.Get("Reward");
    }

    public void UpdateEndGame(List<BeastInfoItem> beastInfoItems, List<RewardModel> rewardModels)
    {
        type = 1;

        PlayerData.instance.PlayStageAmount -= 1;

        UpdateBeastInfoItems(beastInfoItems);

        while (rewardContainer.childCount > 0)
        {
            var go = rewardContainer.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        foreach (var rw in rewardModels)
        {
            var go = ObjectPool.Instance.GetGameObject(rewardPref, Vector3.zero, Quaternion.identity);

            go.transform.SetParent(rewardContainer);

            go.transform.localScale = new Vector3(1, 1, 1);

            go.GetComponent<RewardItemView>().SetUp(rw);
        }
    }

    public void UpdateEndGame3Star(List<RewardModel> rewardModels)
    {
        type = 2;

        statsBtn.gameObject.SetActive(false);

        lostMonsterTxt.text = string.Format(LangManager.Instance.Get("LostMonster"), 0);
        ApplyStar(3);

        while (rewardContainer.childCount > 0)
        {
            var go = rewardContainer.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }

        foreach (var rw in rewardModels)
        {
            var go = ObjectPool.Instance.GetGameObject(rewardPref, Vector3.zero, Quaternion.identity);

            go.transform.SetParent(rewardContainer);

            go.transform.localScale = new Vector3(1, 1, 1);

            go.GetComponent<RewardItemView>().SetUp(rw);
        }

        while (scrollViewResult.contentPanel.childCount > 0)
        {
            var go = scrollViewResult.contentPanel.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }
    }

    private void UpdateBeastInfoItems(List<BeastInfoItem> beastInfoItems)
    {
        var cnt = beastInfoItems.Where(bi => bi.curTeam == Team.My && bi.isDie).Count();
        lostMonsterTxt.text = string.Format(LangManager.Instance.Get("LostMonster"), cnt);

        var star = 0;
        if (cnt == 0)
        {
            star = 3;
        }
        else if (cnt < 3)
        {
            star = 2;
        }
        else if (cnt >= 3)
        {
            star = 1;
        }
        ApplyStar(star);

        scrollViewResult.UpdateBeastInfoItems(beastInfoItems, true);
    }

    private void ApplyStar(int numStar)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < numStar)
            {
                stars[i].sprite = starOn;
            }
            else
            {
                stars[i].sprite = starOff;
            }
        }
    }
}

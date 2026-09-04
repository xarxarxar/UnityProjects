using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MergeBeast;
using Observer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelLose : MonoBehaviour
{
    [SerializeField] private ScrollViewResult scrollViewResult;
    [SerializeField] private Text lostMonsterTxt;
    [SerializeField] private Text damagePerMonsterTxt;

    public void Ok()
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

    public void Stats()
    {
        UIGameManager.instance.Stats();
    }

    void Enable()
    {
        damagePerMonsterTxt.text = LangManager.Instance.Get("DamagePerMonster");
    }

    public void UpdateBeastInfoItems(List<BeastInfoItem> beastInfoItems)
    {
        var cnt = beastInfoItems.Where(bi => bi.curTeam == Team.My && bi.isDie).Count();
        lostMonsterTxt.text = string.Format(LangManager.Instance.Get("LostMonster"), cnt);

        scrollViewResult.UpdateBeastInfoItems(beastInfoItems, false);
    }
}

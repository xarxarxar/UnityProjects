using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using Observer;
using MergeBeast;
using UnityEngine.SceneManagement;

public class TutorialController : TutBase
{
    public static TutorialController Instance;

    public Transform worldBtn;
    public Transform worldBtnTarget;
    public Transform goToBattleBtn;
    bool startBattle = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        // this.RegisterListener(EventID.OnClickBattleAtMain, (sender, param) => OnClickBattleAtMain());
    }

    private void OnClickBattleAtMain()
    {
        ShowFocus(goToBattleBtn.position, () =>
        {
            HideHand();
            StartCoroutine(OnClickCrushBtn());
        });
    }

    private IEnumerator OnClickCrushBtn()
    {
        if (startBattle) yield break;
        startBattle = true;

        Scene scene = SceneManager.GetSceneByName(StringDefine.SCENE_MAP);
        if (scene.IsValid())
        {
            SceneManager.SetActiveScene(scene);
            HideFocus();
            ResetAllEffect();
        }
        else
        {
            var asyncLoad = SceneManager.LoadSceneAsync(StringDefine.SCENE_MAP, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            HideFocus();
            ResetAllEffect();
        }
    }

    protected override void Start()
    {
        base.Start();

        int curBeast = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST);
        var curData = MergeBeast.GameManager.Instance.GetBeast(curBeast);
        var tutMain = PlayerPrefs.GetInt(CrushStringHelper.TutMain, 0);
        if (tutMain == 1 || curData.Level > 5) worldBtnTarget.gameObject.SetActive(true);
        // ShowFocus(Vector3.zero, () => { });
        // ShowText("hahaaaaaa", () =>
        // {
        //     HideText();
        // });

        // StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(5f);
        worldBtn.gameObject.SetActive(false);
        StartCoroutine(ButtonWorldShowIE());
    }

    public void ButtonWorldShow()
    {
        PlayerPrefs.SetInt(CrushStringHelper.TutMain, 1);
        StartCoroutine(ButtonWorldShowIE());
    }

    IEnumerator ButtonWorldShowIE()
    {
        ShowText("");

        worldBtn.gameObject.SetActive(true);
        Vector3 source = new Vector3(0, 0, 0);
        Vector3 target = new Vector3(2, 2, 2);
        while (true)
        {
            yield return null;
            source = Vector3.Lerp(source, target, Time.deltaTime * 5f);
            var dis = Vector2.Distance(target, source);
            worldBtn.localScale = source;

            // Debug.Log("ButtonWorldToTarget worldBtn.position:" + a + " worldBtnTarget.position:" + target);

            if (dis <= 0.05f)
            {
                worldBtn.localScale = target;
                break;
            }
        }
        // Debug.Log("ButtonWorldToTarget end");

        EndButtonWorldShow();
    }

    void EndButtonWorldShow()
    {
        ShowText("点击此处继续！", () =>
        {
            // HideText();
            ShowText("");
            StartCoroutine(ButtonWorldToTarget());
        });

        // messageTxt.transform.parent.gameObject.SetActive(true);
        // messageTxt.gameObject.SetActive(false);
        // continueTxt.gameObject.SetActive(true);

        // _canNextAction = true;
        // _actionNextTut = () =>
        // {
        //     messageTxt.gameObject.SetActive(false);
        //     continueTxt.gameObject.SetActive(false);

        //     StartCoroutine(ButtonWorldToTarget());
        // };
    }

    IEnumerator ButtonWorldToTarget()
    {
        Vector3 sourceSc = worldBtn.localScale;
        Vector3 targetSc = new Vector3(1, 1, 1);

        Vector3 sourcePos = worldBtn.position;
        Vector3 targetPos = worldBtnTarget.position;
        while (true)
        {
            yield return null;
            sourcePos = Vector3.Lerp(sourcePos, targetPos, Time.deltaTime * 7f);
            sourceSc = Vector3.Lerp(sourceSc, targetSc, Time.deltaTime * 7f);

            worldBtn.position = sourcePos;
            worldBtn.localScale = sourceSc;

            // Debug.Log("ButtonWorldToTarget worldBtn.position:" + a + " worldBtnTarget.position:" + target);
            var dis = Vector2.Distance(sourcePos, targetPos);

            if (dis <= 0.05f)
            {
                worldBtn.position = targetPos;
                worldBtn.localScale = targetSc;
                break;
            }
        }
        worldBtn.gameObject.SetActive(false);
        worldBtnTarget.gameObject.SetActive(true);

        HideText();
        ShowFocus(worldBtnTarget.transform.position, ShowBattle);
    }

    void ShowBattle()
    {
        // this.PostEvent(EventID.OnClickBattleAtMain);
        ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.BATTLE);
        OnClickBattleAtMain();
    }
}

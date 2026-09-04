using System.Collections;
using System.Collections.Generic;
using MergeBeast;
using Observer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewTut : MonoBehaviour
{
    public Animator mHandAnim;

    public RectTransform mMask;
    public Button mMaskBtn;

    public Transform worldBtn;
    public Transform worldBtnTarget;
    public Transform goToBattleBtn;

    public Text messageTxt;
    public Text continueTxt;

    private bool _canNextAction;
    private UnityAction _actionNextTut;

    void Awake()
    {
        _canNextAction = false;

        this.RegisterListener(EventID.OnClickBattleAtMain, (sender, param) => OnClickBattleAtMain());

        mHandAnim.gameObject.SetActive(false);
        mMask.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        StartCoroutine(ButtonWorldShow());
    }

    private void OnClickBattleAtMain()
    {
        mHandAnim.gameObject.SetActive(true);
        mHandAnim.transform.position = goToBattleBtn.position;
        mHandAnim.Play("Tutorial-Spawn");

        ShowMask(goToBattleBtn.position, OnClickCrushBtn);
    }

    public void OnClickNextTutorial()
    {
        if (!_canNextAction) return;
        _canNextAction = false;
        _actionNextTut?.Invoke();
    }

    IEnumerator ButtonWorldShow()
    {
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
        messageTxt.transform.parent.gameObject.SetActive(true);
        messageTxt.gameObject.SetActive(false);
        continueTxt.gameObject.SetActive(true);

        _canNextAction = true;
        _actionNextTut = () =>
        {
            messageTxt.gameObject.SetActive(false);
            continueTxt.gameObject.SetActive(false);

            StartCoroutine(ButtonWorldToTarget());
        };
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

        messageTxt.transform.parent.gameObject.SetActive(false);

        ShowMask(worldBtnTarget.position, ShowBattle);

        mHandAnim.gameObject.SetActive(true);
        mHandAnim.transform.position = worldBtn.position;
        mHandAnim.Play("Tutorial-Spawn");
    }

    void ShowBattle()
    {
        this.PostEvent(EventID.OnClickBattleAtMain);
        ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.BATTLE);
    }

    private void OnClickCrushBtn()
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

    void ShowMask(Vector3 position, UnityAction action)
    {
        mMask.gameObject.SetActive(true);
        mMask.transform.position = position;
        mMaskBtn.onClick.RemoveAllListeners();
        mMaskBtn.onClick.AddListener(action);
    }
}

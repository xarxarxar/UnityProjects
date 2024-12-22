using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneType {
    None,
    Home,
    Play,
    Result
}

public class PlayManager : Singleton<PlayManager>
{
    public SceneType sceneType = SceneType.None;
    [HideInInspector] public bool isLoadScene = false;
    [HideInInspector] public int score;
    [HideInInspector] public int turn;
    [HideInInspector] public bool isSaveGameStart = false;
    [HideInInspector] public bool isFade = false;
    [HideInInspector] public int countPlay;
    [HideInInspector] public int countBreakeBrick;
    [HideInInspector] public int countAllClear;
    [HideInInspector] public int countLuckyBonus;
    [HideInInspector] public int countHighestCombo;
    [HideInInspector] public bool isPopupOn = false;

    [HideInInspector] public PanelBase panelBase;
    [HideInInspector] public CommonUI commonUI;
    public Scene scene;


    private CtrBase _ctrBase;

    public CtrBase currentBase
    {
        get { return _ctrBase; }
        set { _ctrBase = value; }
    }


    private void Awake()
    {
        //Check current scene
        SceneCheck();

        //If there is no common UI, it is created dynamically.
        if (!commonUI)
        {
            GameObject obj = (GameObject) Instantiate(Resources.Load("CommonUI"), Vector3.zero, Quaternion.identity);
            commonUI = obj.GetComponent<CommonUI>();
            commonUI.transform.SetParent(transform, false);
            commonUI.GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }


    /// <summary>
    /// Screen Touch On OFF
    /// </summary>
    public bool IsTouch
    {
        set { commonUI.eventSystem.SetActive(value); }
    }




#if UNITY_IOS
    [HideInInspector] public bool isHaptic = false;

    /// <summary>
    /// In case of IOS, check whether the value of haptic use is started.
    /// </summary>
    private void Start () {
        //isHaptic = iOSHapticFeedback.Instance.IsSupported();
    }
#endif

    /// <summary>
    /// SceneLoad
    /// </summary>
    public void LoadScene(string sceneName, bool isRefresh = false)
    {
        if (isLoadScene) return;
        isLoadScene = true;
        Time.timeScale = 1;
        StartCoroutine(LoadSceneCo(sceneName, isRefresh));
    }

    IEnumerator LoadSceneCo(string sceneName, bool isRefresh = false)
    {

        //Remove all PoolObjects loaded in the scene.
        PoolManager.DestroyAllInactive(false);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!async.isDone)
        {
            if (async.progress < 0.9f)
            {
                //Loading
                async.allowSceneActivation = false;
            }
            else
            {
                //Loading End
                yield return true;
                async.allowSceneActivation = true;


            }

            yield return null;
        }

        SceneCheck();

        //If the camera sound is playing after the result popup, it stops and the coroutine stops.
        if (cameraCoroutine != null)
        {
            isCameraSound = false;
            StopCoroutine(cameraCoroutine);
            cameraCoroutine = null;
        }

        isLoadScene = false;
        Time.timeScale = 1;

        //After loading the scene, set the CommonUI canvs WorldCamera as the camera of the current scene.
        commonUI.GetComponent<Canvas>().worldCamera = Camera.main;

        IsTouch = true;
    }


    /// <summary>
    /// After loading the scene, check which scene is the current scene.
    /// </summary>
    void SceneCheck()
    {
        string scenename = SceneManager.GetActiveScene().name;
        if (scenename == Data.scene_home)
        {
            sceneType = SceneType.Home;
        }
        else if (scenename == Data.scene_play)
        {
            sceneType = SceneType.Play;
        }
        else if (scenename == Data.scene_result)
        {
            sceneType = SceneType.Result;
        }
    }


    /// <summary>
    /// Get the name of the current scene.
    /// </summary>
    public string GetSceneName()
    {
        scene = SceneManager.GetActiveScene();
        return scene.name;
    }



    /// <summary>
    /// Play sound when combo effect
    /// </summary>
    public void CameraSound()
    {
        StartCoroutine(CameraSoundCo());
    }

    IEnumerator CameraSoundCo()
    {
        int num = Random.Range(5, 6);
        for (int i = 0; i < num; i++)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_gameover_sfx_cameraflash);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }


    /// <summary>
    /// 결과팝업에서 카메라 셔터 사운드 재생
    /// </summary>
    Coroutine cameraCoroutine;

    bool isCameraSound = false;

    public void CameraSoundLoop()
    {
        isCameraSound = true;
        cameraCoroutine = StartCoroutine(CameraSoundLoopCo());
    }

    IEnumerator CameraSoundLoopCo()
    {
        while (isCameraSound)
        {
            SoundManager.Instance.PlayEffect(SoundList.sound_gameover_sfx_cameraflash);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }


    /// <summary>
    ///Call when using Game Center and Google Ranking without using own ranking
    ///**The existing ranking panel should be deleted.**
    /// </summary>
    public void ShowLeaderboardUI()
    {
        if (Social.localUser.authenticated == true)
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Social.ShowLeaderboardUI();
                }
                else
                {

                }
            });
        }
    }

    /// <summary>
    /// Send game center and Google ranking scores
    /// </summary>
    public void ReportScore(int score)
    {
#if UNITY_IOS
        if (Social.localUser.authenticated == true) {
            Social.ReportScore(score, "HighScore", (result) => { });
        } else {
            Social.localUser.Authenticate((bool success) => {
                if (success) {
                    Social.ReportScore(score, "HighScore", (result) => { });
                } else {
                }
            });
        }

#elif UNITY_ANDROID
        // if (Social.localUser.authenticated == true) {
        //     Social.ReportScore(score, GPGSIds.leaderboard_high_score, (bool success) => { });
        // } else {
        //     Social.localUser.Authenticate((bool success) => {
        //         if (success) {
        //             Social.ReportScore(score, GPGSIds.leaderboard_high_score, (bool successs) => { });
        //         } else {
        //             CommonUI.instance.SetToast("Failed to authenticate");
        //         }
        //     });
        // }
#endif
    }


    /// <summary>
    /// Send game center and Google ranking turn points
    /// </summary>
    public void ReportTurn(int turn)
    {
#if UNITY_IOS
        if (Social.localUser.authenticated == true) {
            Social.ReportScore(turn, "HighTurn", (result) => { });
        } else {
            Social.localUser.Authenticate((bool success) => {
                if (success) {
                    Social.ReportScore(turn, "HighTurn", (result) => { });
                } else {
                    commonUI.SetToast("Failed to authenticate", 3);
                }
            });
        }
#elif UNITY_ANDROID
        // if (Social.localUser.authenticated == true) {
        //     Social.ReportScore(turn, GPGSIds.leaderboard_longest_played_turns, (bool result) => { });
        // } else {
        //     Social.localUser.Authenticate((bool success) => {
        //         if (success) {
        //             Social.ReportScore(turn, GPGSIds.leaderboard_longest_played_turns, (bool result) => { });
        //         } else {
        //             commonUI.SetToast("Failed to authenticate");
        //         }
        //     });
        // }
#endif
    }
}



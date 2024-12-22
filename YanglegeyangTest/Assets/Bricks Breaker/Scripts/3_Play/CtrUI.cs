using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CtrUI : CtrBase
{

    static CtrUI _instance;

    public static CtrUI instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CtrUI>();
            }

            return _instance;
        }
    }


    public PopupPause _PopupPause;
    public PopupContinue _PopupContinue;
    public TextMeshProUGUI textBallCount;
    public GameObject btnReturnBall;
    public TextMeshProUGUI textTurn;

    public TextMeshProUGUI textScore;

    public Sprite[] spriteCombo;

    public ComboEffectText _ComboEffectText;
    public ButtonRocket _ButtonRocket;

    public void SetTurn(int num)
    {
        textTurn.text = num.ToString();
    }

    public void AllClear()
    {
        _ComboEffectText.AllClear();

        //Counting GameData
        PlayManager.Instance.countAllClear++;
    }

    bool isLucky = false;

    public void LuckyBonus()
    {
        if (isLucky) return;
        isLucky = true;
        _ComboEffectText.Lucky();

        //Counting GameData
        PlayManager.Instance.countLuckyBonus++;
    }

    /// <summary>
    /// Reset for next turn
    /// </summary>
    public void NextTurnReady()
    {
        isLucky = false;
        _ButtonRocket.CheckRocketCoolTime();

    }


    private void Awake()
    {
        _PopupPause.UIReset();
        _PopupContinue.UIReset();
        _ComboEffectText.UIReset();

        SetReturnBallButton(false);
        SetResolutionScreen();
    }

    public Camera mainCamera;


    int width;
    int heigh;
    float screenRatio;

    public void SetResolutionScreen()
    {
        float screenRatio = (1.0f * Screen.width) / (1.0f * Screen.height);


#if UNITY_ANDROID
        /*
        //10:16
        width = 1200;
        heigh = 1920;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>10:16</color> : " + screenRatio);

        //10:16
        width = 1600;
        heigh = 2560;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>10:16</color> : " + screenRatio);

        //10:16
        width = 800;
        heigh = 1280;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>10:16</color> : " + screenRatio);


        //9:16
        width = 640;
        heigh = 1136;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:16</color> : " + screenRatio);

        //9:16
        width = 720;
        heigh = 1280;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:16</color> : " + screenRatio);

        //9:16
        width = 750;
        heigh = 1334;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:16</color> : " + screenRatio);

        //9:16
        width = 1440;
        heigh = 2560;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:16</color> : " + screenRatio);

        //9:16
        width = 1080;
        heigh = 1920;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:16</color> : " + screenRatio);

        //9:18
        width = 1440;
        heigh = 2880;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:18</color> : " + screenRatio);

        //9:18.5
        width = 1440;
        heigh = 2960;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:18.5</color> : " + screenRatio);

        //9:19
        width = 1440;
        heigh = 3040;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:19</color> : " + screenRatio);

        //9:19.5
        width = 1440;
        heigh = 3120;
        screenRatio = (1.0f * width) / (1.0f * heigh);
        Debug.Log("<color=red>9:19:.5</color> : " + screenRatio);
    */

        if (screenRatio < 0.47f) {
            //9:19.5
            //Debug.Log("<color=red>9:19.5</color> : " + screenRatio);
            mainCamera.orthographicSize = 7.8f;
        } else if (screenRatio > 0.47f && screenRatio < 0.48f) {
            //9:19
            //Debug.Log("<color=red>9:19</color> : " + screenRatio);
            mainCamera.orthographicSize = 7.6f;
        } else if (screenRatio > 0.48f && screenRatio < 0.495f) {
           //Debug.Log("<color=red>9:18.5</color> : " + screenRatio);
            //9:18.5
            mainCamera.orthographicSize = 7.4f;
        } else if (screenRatio > 0.495f && screenRatio < 0.55f) {
            //Debug.Log("<color=red>9:18</color> : " + screenRatio);
            //9:18
            mainCamera.orthographicSize = 7.2f;
        } else {
            //Debug.Log("<color=red>9:16</color> : " + screenRatio);
            //9:16
            mainCamera.orthographicSize = 6.4f;
        }
#else


        if (screenRatio > 0.6f && screenRatio < 0.7f)
        {
            //Debug.Log("3:2 iPhones models 4 and earlier");
        }
        else if (screenRatio > 0.5f && screenRatio < 0.6f)
        {
            //Debug.Log("16:9 iPhones models 5, SE, 8+");
        }
        else if (screenRatio > 0.4f && screenRatio < 0.5f)
        {
            //Debug.Log("19.5:9 iPhones - models X, Xs, Xr, Xsmax");
            mainCamera.orthographicSize = 7.8f;
        }
        else
        {
            //Debug.Log("Find Not iPhones Size");
        }
#endif
    }


    public void SetBallCount(int ballCount)
    {
        textBallCount.text = string.Format("x{0}", ballCount);
    }

    public void Click_Pause()
    {
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
        _PopupPause.Open();
    }


    public void SetReturnBallButton(bool value)
    {
        btnReturnBall.transform.DOKill();

        if (value)
        {
            btnReturnBall.transform.DOScale(1f, 0.1f).SetEase(Ease.OutCubic);
        }
        else
        {
            btnReturnBall.transform.DOScale(0f, 0f);
        }
    }

    public void Click_ReturnBall()
    {
        if (Player.instance.isReturnBall) return;
        Player.instance.isReturnBall = true;

        SoundManager.Instance.PlayEffect(SoundList.sound_play_common_sfx_ballcollect);
        Player.instance.ReturnBall();
    }







    bool isScoreAnim = false;

    public void AddScore(int num)
    {
        PlayManager.Instance.score += num;

        StartCoroutine(ScoreAnimCo(num));
    }

    IEnumerator ScoreAnimCo(int num)
    {
        isScoreAnim = true;
        int bScore = PlayManager.Instance.score - num;
        int score = PlayManager.Instance.score;

        DOTween.To(() => bScore, x => score = x, score, 0.5f).SetEase(Ease.OutCubic)
            .OnComplete(() => { isScoreAnim = false; });

        while (isScoreAnim)
        {
            textScore.text = Utility.ChangeThousandsSeparator(score);
            yield return null;
        }
    }



#if UNITY_ANDROID
    private void Update () {
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKey(KeyCode.Escape)) {
                if (_PopupPause.isOn) {
                    _PopupPause.Close();
                } else {
                    _PopupPause.Open();
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (_PopupPause.isOn) {
                _PopupPause.Close();
            } else {
                _PopupPause.Open();
            }
            //Application.Quit();
        }
#endif
    }
#endif
}




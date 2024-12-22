using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Advertisements;

public class CtrResult : MonoBehaviour
{

    public GameObject buttonHome;
    public GameObject buttonPlay;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textBestScore;
    public TextMeshProUGUI textTurn;
    public GameObject objRibon;

    int score = 0;
    int bestScore = 0;
    int turn = 0;

    bool isReplay = false;

    void Awake()
    {


        //Counting GameData
        PlayManager.Instance.countPlay++;
        GameData.SaveData();

        objRibon.transform.DOScale(0f, 0f);
        buttonHome.transform.DOScale(0f, 0f);
        buttonPlay.transform.DOScale(0f, 0f);
        coinObject.transform.DOScale(0f, 0f);
        levelupText.DOFade(0f, 0f);
        slideExpBar.value = GameData.PlayerExp / GameData.PlayerMaxExp;
        textLv.text = GameData.PlayerLevel.ToString();

        bestScore = GameData.BestScore;
        score = GameData.Score = PlayManager.Instance.score;
        turn = GameData.Turn = PlayManager.Instance.turn;

        //Turn 반영
        textTurn.text = string.Format("Turn {0}", PlayManager.Instance.turn);


        textBestScore.text = Utility.ChangeThousandsSeparator(bestScore);


    }

    void Start()
    {
        PlayManager.Instance.commonUI._CoinGem.Hide();
        SoundManager.Instance.PlayEffect(SoundList.sound_gameover_sfx_default);
        this.gameObject.SetActive(true);
        //Show results
        StartCoroutine(ShowResultCo());
    }

    IEnumerator ShowResultCo()
    {
        isShow = true;


        float time = 0.5f;
        int playScore = PlayManager.Instance.score;
        DOTween.To(() => score, x => score = x, playScore, time).SetEase(Ease.Linear);

        SoundManager.Instance.PlayEffectLoop(SoundList.sound_result_sfx_score);
        while (time > 0)
        {
            time -= Time.deltaTime;
            textScore.text = Utility.ChangeThousandsSeparator(score);
            yield return null;
        }

        textScore.text = Utility.ChangeThousandsSeparator(playScore);

        //When I hit the record
        if (playScore > bestScore)
        {
            //Sound that scores go up
            SoundManager.Instance.PlayEffectLoop(SoundList.sound_result_sfx_score);

            //Camera Shutter Sound Playback (Loop)
            PlayManager.Instance.CameraSoundLoop();

            //Score animation time
            time = 0.5f;

            //Highest Record Breaking Ribbon Animation
            objRibon.SetActive(true);

            //Highest score update sound playback
            SoundManager.Instance.PlayEffect(SoundList.sound_gameover_sfx_newhighscore);
            objRibon.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.2f);
            //Score count animation
            DOTween.To(() => bestScore, x => bestScore = x, playScore, time).SetEase(Ease.Linear);

            while (time > 0)
            {
                time -= Time.deltaTime;
                textBestScore.text = Utility.ChangeThousandsSeparator(bestScore);
                yield return null;
            }


            //PlayerPrefs saves highscores
            GameData.BestScore = score;
            GameData.BestTurn = turn;
        }

        SoundManager.Instance.StopEffectLoop();

        //Show ads
        SetADs();

        textBestScore.text = Utility.ChangeThousandsSeparator(score);

        if (PlayManager.Instance.countBreakeBrick > 0)
        {
            float exp_vTurn = 4f;
            float exp_vScore = 0.04f;
            float exp_vLucky = 9f;
            float exp_vAllClear = 9f;

            float coin_vTurn = 1f;
            float coin_vScore = 0.002f;
            float coin_vLucky = 5f;
            float coin_vAllClear = 5f;

            float sTurn = PlayManager.Instance.turn;
            float sScore = PlayManager.Instance.score;
            float sLucky = PlayManager.Instance.countLuckyBonus;
            float sAllClear = PlayManager.Instance.countAllClear;


            float getExp = (exp_vTurn * sTurn) + (exp_vScore * sScore) +
                           (exp_vLucky * sLucky) + (exp_vAllClear * sAllClear);




            float getCoin = (coin_vTurn * sTurn) + (coin_vScore * sScore) +
                            (coin_vLucky * sLucky) + (coin_vAllClear * sAllClear);


            GetExp(getExp);
            GetCoin((int) getCoin);
        }


        while (isExpAnimation)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        buttonHome.transform.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.1f);
        buttonPlay.transform.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);

        //PlayManager.Instance.ReportScore(PlayManager.Instance.score);
        //PlayManager.Instance.ReportTurn(PlayManager.Instance.turn);

        isShow = false;

    }


    public GameObject coinObject;
    public TextMeshProUGUI textCoin;

    public void GetCoin(int value = 0)
    {
        textCoin.text = Utility.ChangeThousandsSeparator(value);
        GameData.Coin += value;
        coinObject.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }


    #region - Exp

    public bool sliderAnim = false;
    public TextMeshProUGUI levelupText;
    public Slider slideExpBar;
    public TextMeshProUGUI textLv;
    public bool isExpAnimation = false;


    bool isShow = false;

    public void GetExp(float addexp = 10)
    {

        Debug.Log("addexp : " + addexp);
        //Start experience bar animation
        if (sliderAnim) return;
        sliderAnim = true;


        //Acquisition Experience + Player Experience
        float resultExp = GameData.PlayerExp + addexp;

        //if (!World.SoundEffectMute) {
        //    gageAudio.Play();
        //}

        if (resultExp >= GameData.PlayerMaxExp)
        {
            //Player Experience + Acquisition Experience> If it is greater than the total player experience
            float rExp = resultExp - GameData.PlayerMaxExp;

            slideExpBar.DOValue(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                sliderAnim = false;

                //LevelUp
                LevelUp();
                //SoundManager.Instance.PlayEffect(Sound.sound_result_sfx_levelup);

                //Regain the remaining experience

                if (rExp == 0)
                {
                    //Debug.Log("There is no experience after leveling up, so proceed to the next");
                    isExpAnimation = false;
                }
                else
                {
                    GetExp(rExp);
                }

                //gageAudio.Stop();

            });
            return;
        }
        else if (resultExp < GameData.PlayerMaxExp)
        {
            //Player Experience + Acquisition Experience <If less than the total experience of the player

            slideExpBar.DOValue((float) resultExp / (float) GameData.PlayerMaxExp, 0.5f).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    sliderAnim = false;
                    isExpAnimation = false;
                });
            GameData.PlayerExp = GameData.PlayerExp + addexp;
        }
    }

    private void LevelUp()
    {
        //Mission.AccountLevelCount += 1;

        SoundManager.Instance.PlayEffect(SoundList.sound_result_sfx_levelup);

        if (!levelupText.gameObject.activeSelf)
        {
            levelupText.gameObject.SetActive(true);
            levelupText.DOFade(1f, 0.25f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            //levelupFx.SetActive(true);
        }

        GameData.PlayerLevel++;
        GameData.PlayerExp = 0;

        //Apply UI level up
        slideExpBar.value = 0;
        textLv.text = GameData.PlayerLevel.ToString();
    }

    #endregion





    void SetADs()
    {
        //광고제거가 False상태일때
        if (Random.Range(0, 100) < 25)
        {
            GameData.RandomADs = true;
            //ADManager.Instance.ShowInterstitialAd();
        }
    }



    //클릭 리플레이
    public void ClickReplay()
    {
        PlayManager.Instance.IsTouch = false;
        //ADManager.Instance.ShowRewardedVideo(result =>
        //{
        //    if (result == ShowResult.Finished)
        //    {
        //        RePlay();
        //    }

        //    PlayManager.Instance.IsTouch = true;
        //});
    }

    void RePlay()
    {
        PlayManager.Instance.isSaveGameStart = true;
        PlayManager.Instance.LoadScene(Data.scene_play);
    }


    //클릭 홈
    public void Click_Home()
    {
        PlayManager.Instance.IsTouch = false;
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
        PlayManager.Instance.LoadScene(Data.scene_home);
    }


    //클릭 플레이
    public void Click_Play()
    {
        PlayManager.Instance.IsTouch = false;
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
        PlayManager.Instance.LoadScene(Data.scene_play);
    }

#if UNITY_ANDROID
    private void Update () {
        if (Input.GetKey(KeyCode.Escape)) {
            if (isShow) return;
            Click_Home();
        }
    }
#endif
}

using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
#endif



public class CtrHome : CtrBase
{
    //IOS Haptic Button
    public GameObject settingObjHaptic;
    
    
    public RectTransform panelSetting;
    public Image settingDim;
    bool isOpenSetting = false;
    
    public CanvasGroup canvasGroupHome;
    public MyInfo _MyInfo;
    float posOpenY = -180f;
    float posCloseY = -930f;
    bool isPlay = false;
    public GameObject lockScreen;

    //Panels
    public PanelStart panelStart;
    public PanelMyPage panelMyPage;
    public PanelRanking panelRanking;
    public PanelBallSkin panelBallSkin;
    public PanelShop panelShop;
    public PopupBuy _PopupBuy;

    public void Start()
    {
        SetData();
    }

    void SetData()
    {
        SetHomeUI(false, null, 0f);

        //화폐 설정
        PlayManager.Instance.commonUI._CoinGem.SetCoin();
        PlayManager.Instance.commonUI._CoinGem.SetGem();
        
        if (GameData.NickName == "")
        {
            Instantiate(Resources.Load("Popup/Popup_InputName"));
        }
        else
        {
            //todo 랭킹데이터 가져오기
            // RankingManager.Instance.GetRankingData();
        }

        //랭킹패널 초기화
        panelRanking.UIReset();

        //스타트패널 초기화
        panelStart.UIReset();

        //마이페이지 초기화
        panelMyPage.UIReset();

        //볼패널 초기화
        panelBallSkin.UIReset();

        //패널상점 초기화
        panelShop.UIReset();

        _PopupBuy.UIReset();


#if UNITY_IOS
        //리뷰하기 체크
        if (!GameData.ReviewSuccess) {
            if (GameData.ReviewCount != 0 && GameData.ReviewCount % 10 == 0) {
                GameData.ReviewSuccess = true;
                Device.RequestStoreReview();
            } else {
                GameData.ReviewCount += 1;
            }
        } else {
            if (GameData.ReviewCount != 0 && GameData.ReviewCount % 1000 == 0) {
                Device.RequestStoreReview();
            } else {
                GameData.ReviewCount += 1;
            }
        }
#endif


#if UNITY_ANDROID
        // 안드로이드 일경우 햅틱기능 감추기
        settingObjHaptic.gameObject.SetActive(false);
#endif

        //배경음악재생
        // SoundManager.Instance.PlayBGM(SoundList.sound_home_bgm);

        if (GameData.NickName != "")
        {
            //내정보 설정
            SetMyInfoMyPage();
        }
    }


    public void SetMyInfoMyPage()
    {
        StartCoroutine(SetMyInfoMyPageCo());
    }

    //내정보들 설정
    IEnumerator SetMyInfoMyPageCo()
    {
        // while (!RankingManager.Instance.isDataLoadComplete) {
        //     yield return null;
        // }

        yield return null;
        _MyInfo.SetMyInfo();
        panelMyPage.SetMyPage();

        SetHomeUI(true, null);
    }

    //todo 클릭 마이페이지
    public void Click_MyPage()
    {
        PlayManager.Instance.commonUI._CoinGem.Hide();
        panelMyPage.Open();
    }


    // public TextMeshProUGUI textBestScore;
    //
    // //베스트 스코어 보여주기 애니메이션
    // IEnumerator ShowBestScoreCo()
    // {
    //
    //     int score = 0;
    //     float time = 0.5f;
    //
    //     DOTween.To(() => score, x => score = x, GameData.BestScore, time).SetEase(Ease.Linear);
    //
    //     while (time > 0)
    //     {
    //         time -= Time.deltaTime;
    //         textBestScore.text = Data.ChangeCountFormat(score);
    //         yield return null;
    //     }
    //
    //     textBestScore.text = Data.ChangeCountFormat(GameData.BestScore);
    // }



    #region //클릭 플레이

    public void ClickPlay()
    {
        panelStart.SetPanelStart();
    }

    #endregion


    #region //클릭 볼

    public void Click_Ball()
    {
        panelBallSkin.SetData();
        panelBallSkin.Open();
    }

    #endregion


    #region //클릭 랭킹

    public void Click_Ranking()
    {
        PlayManager.Instance.commonUI._CoinGem.Hide();
        panelRanking.Open();
    }

    #endregion

    #region //클릭 설정

    public void Click_Settings()
    {
        if (!isOpenSetting)
        {
            settingDim.gameObject.SetActive(true);
            settingDim.DOFade(1f, 0.2f);


            isOpenSetting = true;
            panelSetting.DOAnchorPosY(posOpenY, 0.2f).SetEase(Ease.InOutCubic);
            SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);

            SetHomeUI(false);
        }
        else
        {
            isOpenSetting = false;
            panelSetting.DOAnchorPosY(posCloseY, 0.2f).SetEase(Ease.OutCubic);
            SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_close);

            settingDim.DOFade(0f, 0.2f).OnComplete(() => { settingDim.gameObject.SetActive(false); });

            SetHomeUI(true);
        }

        lockScreen.SetActive(isOpenSetting);
    }

    #endregion


    //클릭 구매복원
    public void Click_Restore () {
        // IAPManager.instance.RestorePurchases();
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }


    #region //클릭 리뷰

    public void Click_Review()
    {
        Application.OpenURL(Data.url_app);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    #endregion


    #region //클릭 기타게임

    public void Click_OtherGames()
    {
        Application.OpenURL(Data.url_othergame);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    #endregion

    #region //클릭 페이스북

    public void Click_Facebook()
    {
        Application.OpenURL(Data.url_facebook);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    #endregion


    #region //클릭 상점

    public void Click_Shop()
    {
        panelShop.Open();
    }

    #endregion


    public void SetHomeUI(bool value, CanvasGroup targetGroup, float t = 0.25f)
    {
        float time = t;
        if (!value)
        {
            //홈 패널 숨기기
            canvasGroupHome.DOKill();

            canvasGroupHome.DOFade(0f, time).SetEase(Ease.OutSine);
            canvasGroupHome.transform.DOScale(1.15f, time).SetEase(Ease.OutSine).OnComplete(() =>
            {
                canvasGroupHome.gameObject.SetActive(false);

                if (targetGroup != null)
                {
                    targetGroup.gameObject.SetActive(true);
                    targetGroup.DOKill();
                    targetGroup.DOFade(1f, time).SetEase(Ease.OutCubic);
                    targetGroup.transform.DOScale(1f, time).SetEase(Ease.OutCubic);


                    if (targetGroup.GetComponent<PanelRanking>() != null)
                    {
                        targetGroup.GetComponent<PanelRanking>().ShowRanking();
                    }
                }
            });
        }
        else
        {
            PlayManager.Instance.commonUI._CoinGem.Show();

            if (targetGroup != null)
            {
                targetGroup.DOKill();
                targetGroup.DOFade(0f, time).SetEase(Ease.OutCubic);
                targetGroup.transform.DOScale(0.95f, time).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    targetGroup.gameObject.SetActive(false);
                    //홈 패널 보이기
                    canvasGroupHome.DOKill();
                    canvasGroupHome.gameObject.SetActive(true);
                    canvasGroupHome.DOFade(1f, time).SetEase(Ease.OutCubic);
                    canvasGroupHome.transform.DOScale(1f, time).SetEase(Ease.OutBack);
                });
            }
            else
            {
                //홈 패널 보이기
                canvasGroupHome.DOKill();
                canvasGroupHome.gameObject.SetActive(true);
                canvasGroupHome.DOFade(1f, time).SetEase(Ease.OutCubic);
                canvasGroupHome.transform.DOScale(1f, time).SetEase(Ease.OutBack);
            }
        }
    }


    public void SetHomeUI(bool value)
    {
        if (!value)
        {
            //홈 패널 숨기기
            canvasGroupHome.DOKill();
            canvasGroupHome.transform.DOScale(0.9f, 0.25f).SetEase(Ease.OutCubic);
            PlayManager.Instance.commonUI._CoinGem.SetSmallSize();


        }
        else
        {
            canvasGroupHome.DOKill();
            canvasGroupHome.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutCubic);

            PlayManager.Instance.commonUI._CoinGem.SetLargeSize();
        }
    }

   


#if UNITY_ANDROID
    private void Update () {
        if (Input.GetKey(KeyCode.Escape)) {
            if (PlayManager.Instance.panelBase != null) return;

            if (PlayManager.Instance.isPopupOn) return;
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Instantiate(Resources.Load("Popup/Popup_Exit"));
            }
        }
    }
#endif

}

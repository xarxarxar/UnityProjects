using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Advertisements;


public class PanelStart : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI textTurnScore;
    public Image replayLight;
    public GameObject iconAds;

    /// <summary>
    /// Initialize all UI.
    /// </summary>
    public void UIReset()
    {
        canvasGroup.DOFade(0f, 0f);
        transform.DOScale(0.9f, 0f);
    }

    public void SetPanelStart()
    {

        if (GameData.Save_Turn < 50)
        {
            Click_Play();
            return;
        }

        //if(World.NoAds) {
        //    iconAds.SetActive(false);
        //}

        replayLight.DOKill();
        replayLight.DOFade(1f, 0f);
        replayLight.DOFade(0f, 0.5f).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);

        //Record 50% local save
        GameData.Save_Turn = GameData.Turn / 2;
        GameData.Save_Score = GameData.Score / 2;
        textTurnScore.text = string.Format("<size=18>TURN</size> {0} <size=18>SCORE</size> {1}", GameData.Save_Turn,
            GameData.Save_Score);

        this.gameObject.SetActive(true);
        transform.DOKill();
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.OutCubic);

        (PlayManager.Instance.currentBase as CtrHome).SetHomeUI(false);
    }



    public void Close()
    {
        if (isClick) return;
        isClick = true;
        canvasGroup.DOKill();

        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_close);
        canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            isClick = false;
            this.gameObject.SetActive(false);
            transform.DOKill();
            transform.DOScale(0.9f, 0f);

        });

        (PlayManager.Instance.currentBase as CtrHome).SetHomeUI(true);
    }


    bool isClick = false;

    public void Click_Replay()
    {
        if (isClick) return;
        isClick = true;

        //ADManager.Instance.ShowRewardedVideo(result =>
        //{
        //    if (result == ShowResult.Finished)
        //    {
        //        RePlay();
        //    }
        //    else
        //    {
        //        isClick = false;
        //    }
        //});

    }


    void RePlay()
    {
        PlayManager.Instance.isSaveGameStart = true;
        Click_Play();
    }

    public void Click_Play()
    {
        PlayManager.Instance.LoadScene(Data.scene_play);

        SoundManager.Instance.PlayEffect(SoundList.sound_home_btn_start);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_in);
    }
}

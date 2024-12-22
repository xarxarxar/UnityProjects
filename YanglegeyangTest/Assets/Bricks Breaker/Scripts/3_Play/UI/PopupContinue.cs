using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Advertisements;


public class PopupContinue : MonoBehaviour
{

    public TextMeshProUGUI textScore;
    public CanvasGroup canvasGroup;
    public GameObject btnNoThanks;
    public GameObject popupBg;

    public GameObject iconAds;
    public TextMeshProUGUI textBest;
    public TextMeshProUGUI textTurn;

    public GameObject buttonContinue;

    /// <summary>
    /// Initialize all UI.
    /// </summary>
    public void UIReset()
    {
        canvasGroup.DOFade(0f, 0f);
        btnNoThanks.transform.DOScale(0f, 0f);
        popupBg.transform.DOScale(0f, 0f);

    }

    public void Open()
    {
        SoundManager.Instance.PlayEffect(SoundList.sound_continue_sfx_default);
        iconAds.SetActive(false);

        CtrGame.instance.isGameOver = true;

        textTurn.text = $"Turn {CtrGame.instance.turnCount}";
        // textBest.text = World.ChangeCountFormat(RankingManager.Instance.rankingMyScore.score);


        textScore.text = 0.ToString();
        this.gameObject.SetActive(true);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.15f).SetEase(Ease.OutCubic);
        popupBg.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        StartCoroutine(ScoreAnimCo());
    }

    IEnumerator ScoreAnimCo()
    {
        int score = 0;
        int targetScore = PlayManager.Instance.score;

        float time = 0.5f;
        DOTween.To(() => score, x => score = x, targetScore, time).SetEase(Ease.Linear);

        SoundManager.Instance.PlayEffectLoop(SoundList.sound_result_sfx_score);

        while (time > 0)
        {
            time -= Time.deltaTime;
            textScore.text = Utility.ChangeThousandsSeparator(score);
            yield return null;
        }

        SoundManager.Instance.StopEffectLoop();
        textScore.text = Utility.ChangeThousandsSeparator(targetScore);

        yield return new WaitForSeconds(1f);

        btnNoThanks.transform.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);


    }

    public void Close()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.15f).SetEase(Ease.OutCubic).OnComplete(() => { this.gameObject.SetActive(false); });
    }


    bool isClick = false;

    public void Click_Continue()
    {
        if (isClick) return;
        isClick = true;

        //ADManager.Instance.ShowRewardedVideo(result =>
        //{
        //    if (result == ShowResult.Finished)
        //    {
        //        CtrGame.instance.Continue();
        //    }
        //    else
        //    {
        //        isClick = false;
        //    }

        //    buttonContinue.SetActive(false);
        //});
    }

    public void Click_NoThanks()
    {
        PlayManager.Instance.LoadScene(Data.scene_result);
    }
}

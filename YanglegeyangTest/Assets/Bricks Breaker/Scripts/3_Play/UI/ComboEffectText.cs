using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Spine.Unity;

public class ComboEffectText : MonoBehaviour {

    public SkeletonGraphic animAllClear;
    public TextMeshProUGUI textAllClear;
    public bool isAllClear = false;
    public int allClearCount = 0;
    public ParticleSystem fxShot;

    /// <summary>
    /// Initialize all UI.
    /// </summary>
    public void UIReset () {
        textAllClear.transform.parent.transform.DOScale(0f, 0f);
        imageLucky.transform.DOScale(0f, 0f);
    }

    public void AllClear () {
        allClearCount += 1;
        isAllClear = true;
        this.gameObject.SetActive(true);
        animAllClear.gameObject.SetActive(true);

        if (CtrGame.instance.isAllClear) {
            //Every clear second
            if (allClearCount == 2) {
                SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_allclearbonus2);
            } else {
                SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_allclearbonus1);
            }

            animAllClear.Skeleton.SetSkin("x2");
            animAllClear.Skeleton.SetSlotsToSetupPose();
            animAllClear.LateUpdate();

        } else {
            //Basic All Clear Animation
            animAllClear.Skeleton.SetSkin("None");
            animAllClear.Skeleton.SetSlotsToSetupPose();
            animAllClear.LateUpdate();
        }

        StartCoroutine(AllClearCo());
    }


    IEnumerator AllClearCo () {
        CtrUI.instance.Click_ReturnBall();

        while (isLucky) {
            yield return null;
        }

        Vibrate();

        CtrGame.instance.tiltCamera.Shaking(0.1f);


        textAllClear.gameObject.SetActive(true);
        textAllClear.DOFade(1f, 0f);

        int score;
        if (!CtrGame.instance.isAllClear) {
            score = 100;
        } else {
            score = 200;
        }

        CtrGame.instance.turnScore += (score);

        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_lucky);

        textAllClear.text = string.Format("+{0}", Utility.ChangeThousandsSeparator(score));
        animAllClear.AnimationState.SetAnimation(0, "animation", false);

        textAllClear.transform.parent.transform.DOScale(0f, 0f);
        textAllClear.transform.parent.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            textAllClear.DOFade(0f, 0.25f).SetEase(Ease.OutCubic).SetDelay(1.5f);

        });

        yield return new WaitForSeconds(1.8f);
        animAllClear.gameObject.SetActive(false);
        textAllClear.gameObject.SetActive(false);
        textAllClear.DOFade(1f, 0f);
        textAllClear.transform.parent.transform.DOScale(0f, 0f);

    }

    public SkeletonGraphic animLucky;
    public CanvasGroup imageLucky;
    bool isLucky = false;

    public void Lucky () {
        isLucky = true;
        this.gameObject.SetActive(true);
        animLucky.gameObject.SetActive(true);
        StartCoroutine(LuckyCo());
    }


    IEnumerator LuckyCo () {
        Vibrate();

        yield return null;
        int score = 200;
        CtrGame.instance.turnScore += score;
        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_allclearbonus0);

        fxShot.Play();
        PlayManager.Instance.CameraSound();

        imageLucky.DOFade(1f, 0f);
        imageLucky.transform.DOScale(0f, 0f);
        imageLucky.gameObject.SetActive(true);
        imageLucky.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            imageLucky.DOFade(0f, 0.25f).SetEase(Ease.OutCubic).SetDelay(1.25f);
        });


        animLucky.AnimationState.SetAnimation(0, "animation", false);
        yield return new WaitForSeconds(1.5f);
        isLucky = false;
        animLucky.gameObject.SetActive(false);
        imageLucky.gameObject.SetActive(false);
        imageLucky.DOFade(1f, 0f);
    }


    //Haptic vibration in case of IOS (All Clear, Lucky Bonus)
    public void Vibrate () {
#if UNITY_IOS
        if (PlayManager.Instance.isHaptic) {
            //iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactHeavy);
            //0 : small 1 : light 2 : midium 3 : heavy 4 : success 5 : warring 6 : falure 7 : onoff 
        }
#endif
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class PopupPause : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI textBest;
    public Switch buttonSwitch;
    public bool isOn = false;

    /// <summary>
    /// Initialize all UI.
    /// </summary>
public void UIReset () {
        canvasGroup.DOFade(0f, 0f).SetUpdate(true);

#if UNITY_ANDROID
        buttonSwitch.transform.parent.gameObject.SetActive(false);
#else
        buttonSwitch.SetSwitch();
#endif

    }

    private int c = 1;
    public void Open () {
        if (isOn) return;
        isOn = true;
        Time.timeScale = 0;


        textBest.text = Utility.ChangeThousandsSeparator(GameData.BestScore);

        this.gameObject.SetActive(true);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.15f).SetEase(Ease.OutCubic).SetUpdate(true);

        textMusic.text = $"Music {GameData.BgmCount + 1}";


    }

    public void Close () {
        if (!isOn) return;
        isOn = false;
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);

        Time.timeScale = 1;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.15f).SetEase(Ease.OutCubic).OnComplete(() => {
            this.gameObject.SetActive(false);

        }).SetUpdate(true);
    }

    public void Click_Home () {
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
        PlayManager.Instance.LoadScene(Data.scene_home);
    }

    public void Click_Again () {
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
        PlayManager.Instance.LoadScene(Data.scene_play);
        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_in);
    }


    public Image soundButton;
    public Sprite[] spriteSoundButton;

    public void Click_Sound () {
        //if (World.SoundMute) {
        //    World.SoundMute = false;
        //    SetSundButton(World.SoundMute);
        //    SoundManager.Instance.ResumeBGM();
        //} else {
        //    World.SoundMute = true;
        //    SetSundButton(World.SoundMute);
        //}
    }

    public TextMeshProUGUI textMusic;
    public Image imageArrowR;
    public Image imageArrowL;
    public CanvasGroup musicGroup;

    public void Click_Music_Left_Arrow () {
        GameData.BgmCount -= 1;

        if (GameData.BgmCount <= 0) {
            GameData.BgmCount = 9;
        }

        textMusic.text = $"Music {GameData.BgmCount + 1}";
        // SoundManager.Instance.PlayBGM(SoundList.sound_play_bgm);
    }

    public void Click_Music_Right_Arrow () {
        GameData.BgmCount += 1;

        if (GameData.BgmCount >= 9) {
            GameData.BgmCount = 0;
        }

        textMusic.text = $"Music {GameData.BgmCount + 1}";
        // SoundManager.Instance.PlayBGM(SoundList.sound_play_bgm);
    }

    public void SetMusicOnOff (bool value) {
        if (value) {
            Color color = Utility.HexToColor("dbd5e4");
            textMusic.color = color;
            imageArrowR.color = color;
            imageArrowL.color = color;
            musicGroup.blocksRaycasts = true;

        } else {
            Color color = Utility.HexToColor("3b3253");
            textMusic.color = color;
            imageArrowR.color = color;
            imageArrowL.color = color;
            musicGroup.blocksRaycasts = false;
        }
    }


    public void SetSoundButton (bool value) {
        if (value) {
            soundButton.sprite = spriteSoundButton[0];
        } else {
            soundButton.sprite = spriteSoundButton[1];
        }
    }
}

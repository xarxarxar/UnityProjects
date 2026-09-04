using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MergeBeast
{
    public class SettingScr : BaseScreen
    {
        [SerializeField] private SoundManager _soundMgr;
        [SerializeField] private UIButton _btnRate5Star;
        [SerializeField] private UIButton _btnSound;
        [SerializeField] private UIButton _btnMusic;
        [SerializeField] private UIButton _btnContactUs;
        [SerializeField] private UIButton _btnMoregame;
        [SerializeField] private Slider _slideMusic;
        [SerializeField] private Text _txtSoundStatus;
        [SerializeField] private Text _txtMusicStatus;

        [SerializeField] private Sprite _sprSoundOn;
        [SerializeField] private Sprite _sprSoundOff;
        [SerializeField] private Sprite _sprMusicOn;
        [SerializeField] private Sprite _sprMusicOff;
        [SerializeField] private UIButton _btnClose;
        [SerializeField] private Animator _animPanel;

        private bool _soundIsOn;
        private bool _musicIsOn;

        // Start is called before the first frame update
        void Start()
        {
            _btnRate5Star?.onClick.AddListener(this.OnClickRate5Star);
            _btnSound?.onClick.AddListener(this.OnClickSwichSound);
            _btnMusic?.onClick.AddListener(this.OnClickSwictMusic);
            _btnContactUs?.onClick.AddListener(this.OnClickContactUs);
            _btnMoregame?.onClick.AddListener(this.OnClickMoregame);
            _slideMusic?.onValueChanged.AddListener(this.OnChangeValueSlideMusic);
            _btnClose?.onClick.AddListener(this.OnClickClose);

            _btnSound.image.sprite = _soundMgr.SoundIsOn ? _sprSoundOn : _sprSoundOff;
            _btnMusic.image.sprite = _soundMgr.MusicIsOn ? _sprMusicOn : _sprMusicOff;
            _btnSound.image.SetNativeSize();
            _btnMusic.image.SetNativeSize();

            float volum = PlayerPrefs.GetFloat(StringDefine.MUSIC_KEY, 1);
            _slideMusic.value = volum;
            _txtMusicStatus.text = $"{Mathf.RoundToInt(volum * 10)}/10";
            _txtSoundStatus.text = _soundMgr.SoundIsOn ? "音效开" : "音效关";
        }

        private void OnEnable()
        {
            _animPanel.Play("infoAscend-show");
        }


        private void OnClickSwichSound()
        {
            if (_soundMgr.SoundIsOn)
            {
                _btnSound.image.sprite = _sprSoundOff;
                _btnSound.image.SetNativeSize();
                _txtSoundStatus.text = "音效关";
                _soundMgr.UpdateSound(false);
            }
            else
            {
                _btnSound.image.sprite = _sprSoundOn;
                _btnSound.image.SetNativeSize();
                _txtSoundStatus.text = "音效开";
                _soundMgr.UpdateSound(true);
            }
        }

        private void OnClickSwictMusic()
        {
            if (_soundMgr.MusicIsOn)
            {
                _btnMusic.image.sprite = _sprMusicOff;
                _btnMusic.image.SetNativeSize();
                _slideMusic.value = 0f;
                _txtMusicStatus.text = "0/10";
                _soundMgr.UpdateSliderMusic(0f);
            }
            else
            {
                _btnMusic.image.sprite = _sprMusicOn;
                _btnMusic.image.SetNativeSize();
                _slideMusic.value = 1f;
                _txtMusicStatus.text = "10/10";
                _soundMgr.UpdateSliderMusic(1f);
            }
        }

        private void OnChangeValueSlideMusic(float vl)
        {
            int round = Mathf.RoundToInt(_slideMusic.value * 10f);
            _txtMusicStatus.text = $"{round}/10";
            _soundMgr.UpdateSliderMusic(_slideMusic.value);
            _btnMusic.image.sprite = vl > 0 ? _sprMusicOn : _sprMusicOff;
            _btnMusic.image.SetNativeSize();
        }

        private void OnClickRate5Star()
        {
#if UNITY_ANDROID
            Application.OpenURL($"https://play.google.com/store/apps/details?id=com.Metaverse.Cat.Crypto");
#else
            Application.OpenURL($"https://apps.apple.com/us/app/metacat-cat-metaverse/id1601019075");
#endif
        }

        private void OnClickMoregame()
        {
            Application.OpenURL("https://play.google.com/store/apps/developer?id=Iway+Game+Studio");
        }

        private void OnClickContactUs()
        {
            string email = "iwaygamestudio@gmail.com";
            string subject = "Merge Beast";
            string body = "游戏很好玩！";

            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        private void OnClickClose()
        {
            _animPanel.Play("infoAscend-hide");
            StartCoroutine(IEDeactiveMe());
        }

        private IEnumerator IEDeactiveMe()
        {
            yield return new WaitForSeconds(0.5f);
            ScreenManager.Instance?.DeActiveScreen();
        }
    }
}

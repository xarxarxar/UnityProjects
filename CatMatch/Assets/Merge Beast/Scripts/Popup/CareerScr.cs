using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MergeBeast
{
    public class CareerScr : MonoBehaviour
    {
        [Header("Profile")]
        [SerializeField] private Text _txtVersion;
        [SerializeField] private Text _txtMail;
        [SerializeField] private Button _btnWebsite;

        [Header("Stats")]
        [SerializeField] private Text _txtPlayerLevel;
        [SerializeField] private Text _txtPlayerName;
        [SerializeField] private Text _txtEarning;
        [SerializeField] private Text _txtTotalEarning;
        [SerializeField] private Text _txtRootEarning;
        [SerializeField] private Text _txtRootTotalEarning;

        [Header("Login")]
        [SerializeField] private GameObject _txtErrorLogin;
        [SerializeField] private Button _btnLogin;
        [SerializeField] private GameObject _objLoadingLogin;

        [Header("Register")]
        [SerializeField] private Button _btnRegister;
        [SerializeField] private GameObject _objLoadingRegister;
        [SerializeField] private GameObject _panelRegister;
        [SerializeField] private GameObject _panelChangeName;
        [SerializeField] private InputField _inputGmail;
        [SerializeField] private InputField _inpuConfirmMail;

        [Header("SetName")]
        [SerializeField] private InputField _inputName;

        // Start is called before the first frame update
        void Start()
        {
            _btnLogin?.onClick.AddListener(this.OnClickLogin);
            _btnRegister?.onClick.AddListener(this.OnClickRegister);
        }

        private void OnEnable()
        {
            _txtPlayerLevel.text = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER).ToString();
            _txtPlayerName.text = PlayerPrefs.GetString(StringDefine.KEY_PLAYER_NAME);
            _txtVersion.text = "版本：" + Application.version;
            _txtMail.text = PlayerPrefs.GetString("my-gmail");
            _btnWebsite.gameObject.SetActive(PlayerPrefs.GetInt("has-login",0) == 0);
        }

        // Update is called once per frame
        void Update()
        {
            _txtEarning.text = _txtRootEarning.text;
            _txtTotalEarning.text = _txtRootTotalEarning.text;
        }

        public void OnClickDirectLink(string link)
        {
            Application.OpenURL(link);
        }

        private void OnClickLogin()
        {
            _txtErrorLogin.SetActive(false);
            StartCoroutine(IEFakeLogin());
        }

        private void OnClickRegister()
        {
            if (_inputGmail.text != _inpuConfirmMail.text) return;
            StartCoroutine(IEFakeRegister());
        }

        private IEnumerator IEFakeLogin()
        {
            _btnLogin.interactable = false;
            _objLoadingLogin.SetActive(true);

            yield return new WaitForSeconds(Random.Range(2f, 3.5f));
            _objLoadingLogin.SetActive(false);
            _btnLogin.interactable = true;
            _txtErrorLogin.SetActive(true);
        }

        private IEnumerator IEFakeRegister()
        {
            _btnRegister.interactable = false;
            _objLoadingRegister.SetActive(true);

            yield return new WaitForSeconds(Random.Range(2f, 3.5f));
            _btnRegister.interactable = true;
            _objLoadingRegister.SetActive(false);

            _panelRegister.SetActive(false);
            _panelChangeName.SetActive(true);

            _btnWebsite.gameObject.SetActive(false);
            _txtMail.text = _inputGmail.text;
            PlayerPrefs.SetString("my-gmail", _inputGmail.text);
            PlayerPrefs.SetInt("has-login", 1);
        }

        public void OnClickChangeName()
        {
            if (string.IsNullOrEmpty(_inputName.text)) return;

            PlayerPrefs.SetString(StringDefine.KEY_PLAYER_NAME, _inputName.text);
            _panelChangeName.SetActive(false);
            _txtPlayerName.text = _inputName.text;
            UIManager.Instance?.OnUpdatePlayerName();
            
        }
    }
}
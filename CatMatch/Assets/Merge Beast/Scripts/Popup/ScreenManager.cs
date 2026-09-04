using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MergeBeast
{
    public class ScreenManager : MonoBehaviour
    {
        private static ScreenManager _instance;
        public static ScreenManager Instance => _instance;

        [Header("Confirm")]
        [SerializeField] private GameObject _pnConfirm;
        [SerializeField] private GameObject _pnCongrat;
        [SerializeField] private GameObject _objPopup;
        [SerializeField] private Text _txtConfirm;
        [SerializeField] private Text _txtCongrat;
        [SerializeField] private UIButton _btnOK;
        [SerializeField] private UIButton _btnOKCongrat;
        [SerializeField] private UIButton _btnCancel;
        [SerializeField] private Toggle _tgAskAgain;


        private Dictionary<EnumDefine.SCREEN, GameObject> _dictionScreen;
        private UnityAction _actionOK;
        private UnityAction _actionCancel;
        private Stack<GameObject> _stackScr;

        private Dictionary<int, int> _dictionCheck;
        private int _idConfirm;

        // Use this for initialization
        void Start()
        {
            if (_instance == null) _instance = this;
            _dictionScreen = new Dictionary<EnumDefine.SCREEN, GameObject>();
            _dictionCheck = new Dictionary<int, int>();
            _stackScr = new Stack<GameObject>();

            foreach(Transform child in this.transform)
            {
                BaseScreen scr = child.GetComponent<BaseScreen>();
                if (scr != null)
                    _dictionScreen.Add(scr.Screen, child.gameObject);
            }

            _btnOK?.onClick.AddListener(this.OnClickOK);
            _btnOKCongrat?.onClick.AddListener(this.OnClickOK);
            _btnCancel?.onClick.AddListener(this.OnClickCancle);
            _tgAskAgain?.onValueChanged.AddListener((bool s) => OnChangeCheck(s));
        }

        private void OnChangeCheck(bool isOn)
        {
            if (isOn)
            {
                if (!_dictionCheck.ContainsKey(_idConfirm)) _dictionCheck.Add(_idConfirm, 1);
            }
            else
            {
                if (_dictionCheck.ContainsKey(_idConfirm)) _dictionCheck.Remove(_idConfirm);
            }
        }

        public void ActiveScreen(EnumDefine.SCREEN scr)
        {
            if (_dictionScreen.ContainsKey(scr))
            {
                _dictionScreen[scr].SetActive(true);
                _stackScr.Push(_dictionScreen[scr]);
            }
        }

        public void DeActiveScreen()
        {
            if (_stackScr.Count <= 0) return;

            GameObject obj = _stackScr.Pop();
            obj.SetActive(false);
            if(_stackScr.Count > 0)
            {
                GameObject pek = _stackScr.Peek();
                pek.SetActive(true);
            }

            AdsManager.Instance?.OnShowInter();
        }

        public void ShowConfirm(int id, string mgs,UnityAction action = null,UnityAction cancle = null)
        {
            _idConfirm = id;

            if (_dictionCheck.ContainsKey(id))
            {
                if(_dictionCheck[id] == 1)
                {
                    if (id != 1)
                        action?.Invoke();
                }
            }
            else
            {
                _objPopup.SetActive(true);
                _txtConfirm.text = mgs;
                _actionOK = action;
                _actionCancel = cancle;
                _pnConfirm.SetActive(true);
                _pnCongrat.SetActive(false);
                _tgAskAgain.isOn = false;
            }
        }

        public void ShowCfGem(string mgs, UnityAction action = null)
        {
            _objPopup.SetActive(true);
            _txtConfirm.text = mgs;
            _actionOK = action;
            _pnConfirm.SetActive(true);
            _pnCongrat.SetActive(false);
        }

        public void ShowNoti(string mgs)
        {
            _pnConfirm.SetActive(false);
            _pnCongrat.SetActive(true);
            _txtCongrat.text = mgs;
            _objPopup.SetActive(true);
            _actionOK = null;
        }

        private void OnClickOK()
        {
            _objPopup.SetActive(false);
            _actionOK?.Invoke();
        }

        private void OnClickCancle()
        {
            _actionCancel?.Invoke();
            _actionCancel = null;
        }
    }
}

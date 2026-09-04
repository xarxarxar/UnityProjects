using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Numerics;
using UnityEngine.Events;


namespace MergeBeast
{
    public class OfflineRewardScr : MonoBehaviour
    {
        [SerializeField] private Text _txtTimeOff;
        [SerializeField] private Text _txtSoulIncome;
        [SerializeField] private Text _txtx1;
        [SerializeField] private Text _txtx3;
        [SerializeField] private Button _btnGetx1;
        [SerializeField] private Button _btnGetx3;
        [SerializeField] private Animator _myAnim;

        private UnityAction _callBack;
        private BigInteger _cacheSoul;


        // Start is called before the first frame update
        void Start()
        {
            _btnGetx1?.onClick.AddListener(this.OnClickGetX1);
            _btnGetx3?.onClick.AddListener(this.OnClickGetX3);
            
        }


        public void ShowPopup(TimeSpan time,BigInteger soul,UnityAction callbackClose = null)
        {
            PlayerPrefs.DeleteKey(StringDefine.CHECK_POINT_OFFLINE);
            _txtTimeOff.text = string.Empty;

            if (time.TotalSeconds > 43200)
            {
                _txtTimeOff.text += $"12时00分00秒";
            }
            else
            {
                if (time.Days > 0) _txtTimeOff.text = $"{time.Days}天 ";
                if (time.Hours > 0) _txtTimeOff.text += $"{time.Hours}小时 ";
                _txtTimeOff.text += $"{time.Minutes}分 {time.Seconds}秒";
            }

            _txtSoulIncome.text = _txtx1.text = Utils.FormatNumber(soul);
            _txtx3.text = Utils.FormatNumber(soul * 3);
            _callBack = callbackClose;
            _cacheSoul = soul;
            _myAnim.SetTrigger("appear");
        }

        private void OnClickGetX1()
        {
            UIManager.Instance.UpdateMoneyCoin(_cacheSoul, false);
            _myAnim.SetTrigger("hide");
            StartCoroutine(IEHideMe());
        }

        private void OnClickGetX3()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                UIManager.Instance.UpdateMoneyCoin(_cacheSoul * 3, false);
                _myAnim.SetTrigger("hide");
                StartCoroutine(IEHideMe());
            },StringDefine.ADS_OFFLINE_REWARD_X3);
        }


        private IEnumerator IEHideMe()
        {
            yield return new WaitForSeconds(0.45f);
            this.gameObject.SetActive(false);
            _callBack?.Invoke();
            GameManager.Instance?.ReadySpawnGift();
        }

    }
}

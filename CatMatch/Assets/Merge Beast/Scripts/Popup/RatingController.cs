using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MergeBeast {
    public class RatingController : BaseScreen {
        [Header("LIKE")]
        [SerializeField] private GameObject likePopup;
        [SerializeField] private UIButton btnDislike;
        [SerializeField] private UIButton btnLike;
        [SerializeField] private UIButton btnCloseLikePopup;


        [Header("RATE STORE")]
        [SerializeField] private GameObject ratePopup;
        [SerializeField] private UIButton btnCloseRatePopup;
        [SerializeField] private UIButton btnGotoStore;

        [Header("FeedBack")]
        [SerializeField] private GameObject feedbackPopup;
        [SerializeField] private UIButton btnCloseFeedbackPopup;
        [SerializeField] private InputField feedbackInput;
        [SerializeField] private UIButton btnSendFeedback;

        [Header("Claim")]
        [SerializeField] private GameObject claimPopup;
        [SerializeField] private UIButton btnClaim;

        private void OnEnable() {
            Show();
        }

        private void Awake() {
            btnDislike.onClick.AddListener(() => ShowFeedBack());
            btnLike.onClick.AddListener(() => ShowRateStore());
            btnCloseLikePopup.onClick.AddListener(() => ClosePopupLike());
            btnCloseFeedbackPopup.onClick.AddListener(() => CloseFeedBack());
            btnCloseRatePopup.onClick.AddListener(() => CloseRateStore());
            btnSendFeedback.onClick.AddListener(() => SendFeedback());
            btnGotoStore.onClick.AddListener(() => GoToStore());
            btnClaim.onClick.AddListener(() => Claim());
            //feedbackInput.onEndEdit.AddListener((txt) => {
            //    if (txt.Trim().Length == 0) return;
            //    SendFeedback();
            //});
        }

        private void ClosePopupLike() {
            likePopup.SetActive(false);
            ScreenManager.Instance.DeActiveScreen();
        }

        public void Show() {
            UIManager.Instance.ShowPopup(likePopup.transform);            
            ratePopup.SetActive(false);
            feedbackPopup.SetActive(false);
           
        }

        private void ShowRateStore() {
            UIManager.Instance.HidePopup(likePopup.transform, () => {
                UIManager.Instance.ShowPopup(ratePopup.transform);
            });            
        }

        private void CloseRateStore() {
            UIManager.Instance.HidePopup(ratePopup.transform, () => {
                ScreenManager.Instance.DeActiveScreen();
            });                        
        }

        private void ShowFeedBack() {
            UIManager.Instance.HidePopup(likePopup.transform, () => {
                UIManager.Instance.ShowPopup(feedbackPopup.transform);
            });
            
        }

        private void CloseFeedBack() {
            UIManager.Instance.HidePopup(feedbackPopup.transform, () => {
                ScreenManager.Instance.DeActiveScreen();
            });            
            
        }

        void SendFeedback() {
            if (feedbackInput.text.Trim().Length == 0) return;
            GameManager.Instance.StartCoroutine(PostRate());
            ShowPopupClaim();

        }

        public IEnumerator PostRate() {
            string url = "http://iwaystudio.vn:82/rate_app";
            string comment = feedbackInput.text.Trim();
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            string deviceName = SystemInfo.deviceName;
            string version = Application.version;
            string platform = "";
#if UNITY_ANDROID 
            platform = "android";
#elif UNITY_IOS
            platform = "ios";
#endif
            WWWForm form = new WWWForm();
            form.AddField("comment", comment);
            form.AddField("device_id", deviceId);
            form.AddField("device_name", deviceName);
            form.AddField("version", version);
            form.AddField("platform", platform);
            form.AddField("star", 1);
            form.AddField("app_name", "MergeBeast");

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError) {                
                PlayerPrefs.SetInt(StringDefine.RATE_SUCCESS, 0);
            } else {
                //post data success
                PlayerPrefs.SetInt(StringDefine.RATE_SUCCESS, 1);                          
            }
            feedbackInput.text = "";            
        }

        void GoToStore() {
            ratePopup.SetActive(false);
            ShowPopupClaim();
            Application.OpenURL($"https://play.google.com/store/apps/details?id=com.Merge.Beast.IDLE.Tycoon");
            PlayerPrefs.SetInt(StringDefine.RATE_SUCCESS, 1);
            //ScreenManager.Instance.DeActiveScreen();
            
        }

        void ShowPopupClaim() {
            UIManager.Instance.ShowPopup(claimPopup.transform);                    
        }

        void Claim() {            
            UIManager.Instance.HidePopup(claimPopup.transform, () => {
                Utils.AddRubyCoin(30);                
                UIManager.Instance.UpdateMoneyRuby();
                ScreenManager.Instance.DeActiveScreen();
            });

        }


    } //end class
}

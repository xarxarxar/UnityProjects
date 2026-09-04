using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MergeBeast
{
    public class BatMan : MonoBehaviour
    {
        private RectTransform rtf;
        private Button button;
        private float time = 0f;
        private bool isFlying = false;

        private Vector2 startPos = new Vector2(-300, -140);
        private Vector2 endPos = new Vector2(300, -140);
        private Vector2 leftPos = new Vector2(-800, -140);
        private Vector2 rightPos = new Vector2(800, -140);

        [SerializeField] private GameObject popup;
        [SerializeField] private Button btnClosePopup;
        [SerializeField] private Button btnWatchVideo;
        [SerializeField] private Transform gemIcon;

        private void Awake()
        {
            rtf = GetComponent<RectTransform>();
            button = GetComponent<Button>();

        }

        // Start is called before the first frame update
        void Start()
        {
            rtf.anchoredPosition = leftPos;
            button.onClick.AddListener(() => this.OnClick());
            btnClosePopup.onClick.AddListener(() => ClosePopup());
            btnWatchVideo.onClick.AddListener(() => WatchVideo());
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if (time >= Config.TIME_SHOW_INTERSTITIAL && !isFlying && CanShowVideo() && PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 0) > 8) {
                MoveToScreen();
                time = 0f;
            }

        }

        void MoveToScreen()
        {
            isFlying = true;
            rtf.DOAnchorPos(startPos, 1f).OnComplete(() =>
            {
                StartCoroutine(Fly());
            });

        }

        IEnumerator Fly()
        {
            yield return new WaitForSeconds(0.3f);
            int count = 0;
            while (count < 5)
            {
                if (rtf.anchoredPosition.x < 0)
                {
                    rtf.DOAnchorPos(endPos, 3f).SetEase(Ease.Linear);
                    transform.localScale = new Vector2(1, 1);
                }
                else
                {
                    rtf.DOAnchorPos(startPos, 3f).SetEase(Ease.Linear);
                    transform.localScale = new Vector2(-1, 1);
                }
                yield return new WaitForSeconds(3f);
                count++;

            }
            rtf.DOAnchorPos(rightPos, 0.5f).OnComplete(() =>
            {
                isFlying = false;
                rtf.anchoredPosition = leftPos;
                time = 0f;
                transform.GetChild(0).gameObject.SetActive(true);
            });

        }

        void OnClick()
        {
            UIManager.Instance.ShowPopup(popup.transform);
            // StopAllCoroutines();
            // rtf.anchoredPosition = leftPos;            
            transform.GetChild(0).gameObject.SetActive(false);            
            
        }

        void ClosePopup()
        {
            UIManager.Instance.HidePopup(popup.transform);
        }
        void WatchVideo()
        {
            ClosePopup();
            AdsManager.Instance.ShowAds(() =>
            {
                Utils.AddRubyCoin(10);
                UIManager.Instance.UpdateMoneyRuby();
                PlayerPrefs.SetString(StringDefine.DAY_FREE_VIDEO, DateTime.Today.ToString());

                int totalTime = PlayerPrefs.GetInt(StringDefine.TOTAL_FREE_VIDEO, 0);
                totalTime++;
                PlayerPrefs.SetInt(StringDefine.TOTAL_FREE_VIDEO, totalTime);

                EffectManager.Instance.CollectGem(gemIcon.transform.position);

            }, "", EnumDefine.ADSTYPE.Rewarded);
        }

        bool CanShowVideo()
        {
            int totalTime = PlayerPrefs.GetInt(StringDefine.TOTAL_FREE_VIDEO, 0);

            DateTime dayPref = DateTime.Parse(PlayerPrefs.GetString(StringDefine.DAY_FREE_VIDEO, DateTime.Today.ToString()));
            DateTime today = DateTime.Today;
            int dayDiff = (today - dayPref).Days;
            if(dayDiff > 0) {
                PlayerPrefs.SetInt(StringDefine.TOTAL_FREE_VIDEO, 0);
            }
            if(dayDiff > 0 || totalTime < Config.TOTAL_INTERSTITIAL) {
                
                return true;
            }
            return false;
        }
    }
}

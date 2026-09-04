using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using UnityEngine.UI;
using System;


namespace MergeBeast
{
    public class BoostAds : MonoBehaviour
    {
        [SerializeField] private EnumDefine.BOOST _boostType;
        [SerializeField] private Text _txtTimeBoost;
        [SerializeField] private UIButton _btnBuyByAds;
        [SerializeField] private GameObject _objNoticeAds;

        private bool _countingTime;
        private Coroutine _ieCountFree;

        private int remainTime;


        // Start is called before the first frame update
        void Start()
        {
            //string date = PlayerPrefs.GetString($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)_boostType}");
            string timeUp = PlayerPrefs.GetString($"{StringDefine.BOOST_TIME_UP}{(int)_boostType}", TimeUtil.TimeStampSecond.ToString());
            long timeUpLong = long.Parse(timeUp);
            long now = TimeUtil.TimeStampSecond;
            if (now < timeUpLong)
            {
                //van con boost                
                _countingTime = true;
                remainTime = (int)(timeUpLong - now);

                if (_ieCountFree != null)
                {
                    StopCoroutine(_ieCountFree);
                    _ieCountFree = StartCoroutine(IETimeFreeBoost());
                }
                else
                {
                    _ieCountFree = StartCoroutine(IETimeFreeBoost());
                }
                if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1)
                {
                    BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, remainTime, 0);
                }
                else
                {
                    BoostManager.Instance.AddBoost(_boostType, remainTime);
                }
                //Debug.LogError("remain Time: " + remainTime);
            }
            else
            {
                //het thoi gian
                _btnBuyByAds.interactable = true;
                _countingTime = false;
                //PlayerPrefs.DeleteKey($"{StringDefine.BOOST_TIME_UP}{(int)_boostType}");
            }

            _btnBuyByAds?.onClick.AddListener(this.OnClickBuyByAds);
        }

        private void OnClickBuyByAds()
        {
            if (_countingTime) return;
            
            if (AdsManager.Instance.IsLoaded())
            {
                AdsManager.Instance.ShowAds(() =>
                {
                    remainTime = Utils.GetBuffMinute();
                    if (_ieCountFree != null)
                    {
                        StopCoroutine(_ieCountFree);
                        _ieCountFree = StartCoroutine(IETimeFreeBoost());
                    }
                    else
                    {
                        _ieCountFree = StartCoroutine(IETimeFreeBoost());
                    }
                    if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1)
                    {
                        BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, remainTime, 0);
                    }
                    else
                    {
                        BoostManager.Instance.AddBoost(_boostType, remainTime);
                    }
                    var boostType = BoostManager.Instance.Boost(_boostType);

                    UIManager.Instance?.ShowNotify($"获得{boostType.Name}");
                }, StringDefine.ADS_RECEIVED_BOOST);
            }
            else
            {
                _btnBuyByAds.interactable = false;
                StartCoroutine(_Count());
            }
        }

        IEnumerator _Count()
        {
            yield return new WaitForSeconds(30f);
            _btnBuyByAds.interactable = true;

            
        }

        private IEnumerator IETimeFreeBoost()
        {

            _countingTime = true;
            _objNoticeAds.SetActive(false);
            _btnBuyByAds.interactable = false;

            string days = "";
            string hours = "";
            string minutes = "";
            string seconds = "";
            
            while (remainTime > 0)
            {
                var b = new TimeSpan(0,0,remainTime);
                days = b.Days > 0 ? b.Days.ToString("D2") : "";
                hours = b.Hours > 0 ? b.Hours.ToString("D2") + ":" : "";
                minutes = b.Minutes > 0 ? b.Minutes.ToString("D2") + ":" : "";
                seconds = b.Seconds > 0 ? b.Seconds.ToString("D2") : "00";

                if(b.Days > 0) {
                    _txtTimeBoost.text = $"{days}天";
                } else {
                    if(b.Minutes < 1) minutes = "00:";
                    _txtTimeBoost.text = $"{hours}{minutes}{seconds}";
                }
            
                
                
                yield return new WaitForSeconds(1f);
                remainTime--;

            }
            _txtTimeBoost.text = "";
            _btnBuyByAds.interactable = true;
            _objNoticeAds.SetActive(true);
            _countingTime = false;
        }

        public void UpdateStatus()
        {
            string timeUp = PlayerPrefs.GetString($"{StringDefine.BOOST_TIME_UP}{(int)_boostType}", TimeUtil.TimeStampSecond.ToString());
            long timeUpLong = long.Parse(timeUp);
            long now = TimeUtil.TimeStampSecond;
            if (now < timeUpLong)
            {
                remainTime = (int)(timeUpLong - now);
                if (_ieCountFree == null)
                {
                    _ieCountFree = StartCoroutine(IETimeFreeBoost());
                }
                else
                {
                    StopCoroutine(_ieCountFree);
                    _ieCountFree = null;
                    _ieCountFree = StartCoroutine(IETimeFreeBoost());
                }
            }

        }

    }
}


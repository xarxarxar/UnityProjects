using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Observer;

namespace MergeBeast
{
    public class BoostItemAds : MonoBehaviour
    {
        [SerializeField] private EnumDefine.BOOST _boostType;
        [SerializeField] private Text _txtTimeBoost;
        [SerializeField] private UIButton _btnBuyByAds;
        [SerializeField] private UIButton _btnBuyByGem;
        [SerializeField] private Text _txtReceiveSoul;
        [SerializeField] private Text _txtDesc;
        [SerializeField] private GameObject activeObj;

        private int _currentPrice;
        private Coroutine _ieCountBoost;
        private Coroutine _ieCountFree;
        
        private bool _countingTime;

        int remainTime;

        // Start is called before the first frame update
        void Start()
        {
            BoostData data = BoostManager.Instance.Boost(_boostType);
            _currentPrice = data.Price;
            _btnBuyByGem.GetComponentInChildren<Text>().text = $"{_currentPrice}";

            _btnBuyByAds?.onClick.AddListener(this.OnClickBuyByAds);
            _btnBuyByGem?.onClick.AddListener(this.OnClickBuyByGem);
        }

        private void OnClickBuyByAds()
        {

            if (_countingTime) return;

            AdsManager.Instance.ShowAds(() =>
            {
                if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1) {
                    BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, Utils.GetBuffMinute(), 0);
                } else if (_boostType == EnumDefine.BOOST.PILE_SOUL) {
                    BoostManager.Instance.AddBoost(_boostType);
                }
                else {
                    BoostManager.Instance.AddBoost(_boostType, Utils.GetBuffMinute());
                }

                var boostType = BoostManager.Instance.Boost(_boostType);
                // set thoi gian de hien thi thoi gian o nut free video                
                if(boostType != null) {
                    remainTime = (int) boostType.Time;                    
                }
                _btnBuyByAds.interactable = false;
                //_ieCountFree = StartCoroutine(IETimeFreeBoost());

                if (_ieCountBoost != null) StopCoroutine(_ieCountBoost);
                var boost = BoostManager.Instance.GetBoostActive(_boostType);
                if (boost != null) _ieCountBoost = StartCoroutine(IECountTimeBoost());
                                
                UIManager.Instance?.ShowNotify($"购买成功{boostType.Name}");
                
            },StringDefine.ADS_RECEIVED_BOOST);
        }

        private void OnClickBuyByGem()
        {
            if (Utils.GetCurrentRubyMoney() >= _currentPrice)
            {
                Utils.AddRubyCoin(-_currentPrice);
                if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1) {
                    BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, Utils.GetBuffMinute(), 0);
                    
                } else if (_boostType == EnumDefine.BOOST.PILE_SOUL) {
                    UIManager.Instance.UpdateMoneyCoin(GameManager.Instance.TotalDamage() * Config.PILE_SOUL_REWARD, false);
                } else {
                    BoostManager.Instance.AddBoost(_boostType, Utils.GetBuffMinute());
                }
                this.PostEvent(EventID.OnUpDateMoney);


                //CD auto merge
                if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1) {
                    var freeBoost = BoostManager.Instance.GetFreeBoostActive(EnumDefine.FREEBOOST.AUTO_MERGE);
                    if (freeBoost != null) {
                        Debug.LogError(freeBoost.TimeRemanin);
                        if (_ieCountBoost != null) {
                            StopCoroutine(_ieCountBoost);
                            _ieCountBoost = null;
                            remainTime = (int)freeBoost.TimeRemanin + 1;
                            _ieCountBoost = StartCoroutine(IECountTimeBoost());
                        } else {
                            remainTime = Utils.GetBuffMinute();
                            _ieCountBoost = StartCoroutine(IECountTimeBoost());
                        }

                    }
                } else if (_boostType == EnumDefine.BOOST.PILE_SOUL) { 
                    
                } else {
                    var boost = BoostManager.Instance.GetBoostActive(_boostType);
                    if (_ieCountBoost != null) {
                        StopCoroutine(_ieCountBoost);
                        _ieCountBoost = null;
                        remainTime = (int)boost.TimeRemanin;
                        _ieCountBoost = StartCoroutine(IECountTimeBoost());
                    } else {
                        remainTime = Utils.GetBuffMinute();
                        _ieCountBoost = StartCoroutine(IECountTimeBoost());
                    }

                    //if (boost != null) _ieCountBoost = StartCoroutine(IECountTimeBoost(boost.TimeRemanin));
                }

                
                var boostType = BoostManager.Instance.Boost(_boostType);
                UIManager.Instance?.ShowNotify($"购买成功{boostType.Name}");
            }
            else
            {
                ScreenManager.Instance.ShowConfirm(5, "你的宝石不足，是否要购买宝石？", () =>
                {
                    ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);
                });
            }
        }

        public void DailyActiveBoost()
        {
            if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1) {
                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, Utils.GetBuffMinute(), 0);
            } else if (_boostType == EnumDefine.BOOST.PILE_SOUL) {
                UIManager.Instance.UpdateMoneyCoin(GameManager.Instance.TotalDamage() * Config.PILE_SOUL_REWARD, false);
            } else {
                BoostManager.Instance.AddBoost(_boostType, Utils.GetBuffMinute());
            }
            if (_ieCountBoost != null) StopCoroutine(_ieCountBoost);
            var boost = BoostManager.Instance.GetBoostActive(_boostType);
            if (boost != null) _ieCountBoost = StartCoroutine(IECountTimeBoost());
            var boostType = BoostManager.Instance.Boost(_boostType);
            UIManager.Instance.ShowNotify($"领取成功{boostType.Name}");
        }

        public EnumDefine.BOOST GetBoostType() {
            return _boostType;
        }

        private void OnEnable()
        {
            var boost = BoostManager.Instance.GetBoostActive(_boostType);
            
            
            if (_boostType == EnumDefine.BOOST.PILE_SOUL)
            {
                BigInteger dps = GameManager.Instance.TotalDamage() * Config.PILE_SOUL_REWARD;                
                if (_txtReceiveSoul != null) _txtReceiveSoul.text = $"<color=#D37100FF>{Utils.FormatNumber(dps)}</color>";
            }


            string timeUp = PlayerPrefs.GetString($"{StringDefine.BOOST_TIME_UP}{(int)_boostType}", TimeUtil.TimeStampSecond.ToString());
            long timeUpLong = long.Parse(timeUp);
            long now = TimeUtil.TimeStampSecond;

            _btnBuyByAds.interactable = AdsManager.Instance.IsLoaded();

            if (now < timeUpLong) {
                //con thoi gian
                remainTime = (int)(timeUpLong - now);


                if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1 || _boostType == EnumDefine.BOOST.PILE_SOUL) {
                    if (_ieCountFree == null)
                        _ieCountFree = StartCoroutine(IETimeFreeBoost());
                }
                if (activeObj)
                    activeObj.SetActive(true);

                if (boost != null) {
                    if (_ieCountBoost == null) {
                        _ieCountBoost = StartCoroutine(IECountTimeBoost());
                    }
                } else {
                    _txtTimeBoost.text = string.Empty;
                }

                _btnBuyByAds.interactable = false;
            } else {
                if(activeObj)
                    activeObj.SetActive(false);
                //het thoi gian
                _txtTimeBoost.text = string.Empty;
                _countingTime = false;
                _btnBuyByAds.interactable = true;
                _btnBuyByAds.GetComponentInChildren<Text>().text = "看视频";
            }

            

            if(_txtDesc != null) {
                if(_boostType == EnumDefine.BOOST.DAMAGE_BOOST_1) {
                    _txtDesc.text = $"猫的伤害提升100%，持续<color=#D37100FF>{CPlayer.GetVipConfig().buffMinute}分钟</color>";
                } else if (_boostType == EnumDefine.BOOST.SPAWN_FASTER_1) {
                    _txtDesc.text = $"新猫生成速度提升<color=#D37100FF>2倍</color>，持续<color=#D37100FF>{CPlayer.GetVipConfig().buffMinute}分钟</color>";
                } else if (_boostType == EnumDefine.BOOST.AUTO_MERGE_1) {
                    _txtDesc.text = $"自动合成你的猫，最长<color=#D37100FF>{CPlayer.GetVipConfig().buffMinute}分钟</color>";
                }
            }
        }

        private void OnDisable()
        {
            if (_ieCountBoost != null)
            {
                StopCoroutine(_ieCountBoost);
                _ieCountBoost = null;
            }

            if (_ieCountFree != null)
            {
                StopCoroutine(_ieCountFree);
                _ieCountFree = null;
            }
        }

        private IEnumerator IECountTimeBoost()
        {
            if(_boostType == EnumDefine.BOOST.PILE_SOUL) {
                _txtTimeBoost.text = "";
                yield break;
            }
string days = "";
            string hours = "";
            string minutes = "";
            string seconds = "";
            activeObj.SetActive(true);
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
                if(remainTime <= 0) {
                    activeObj.SetActive(false);
                }
            }
            
            _txtTimeBoost.text = "";
        }

        private IEnumerator IETimeFreeBoost()
        {
            //string date = PlayerPrefs.GetString($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)_boostType}");
            //DateTime markDate = DateTime.Parse(date);
            //TimeSpan span = DateTime.Now - markDate;

            //int remainTime = _timeCD - (int)span.TotalSeconds;
            Text txt = _btnBuyByAds.GetComponentInChildren<Text>();
            float maxtime = Utils.GetBuffMinute();
            _countingTime = true;
            Debug.LogError("free " + gameObject.name);
            string days = "";
            string hours = "";
            string minutes = "";
            string seconds = "";
            if(activeObj)
                activeObj.SetActive(true);
            while (remainTime > 0)
            {                
                var b = new TimeSpan(0,0,remainTime);
                days = b.Days > 0 ? b.Days.ToString("D2") : "";
                hours = b.Hours > 0 ? b.Hours.ToString("D2") + ":" : "";
                minutes =  b.Minutes.ToString("D2") + ":";
                seconds = b.Seconds > 0 ? b.Seconds.ToString("D2") : "00";

                if(b.Days > 0) {
                    _txtTimeBoost.text = $"{days}天";
                } else {
                    _txtTimeBoost.text = $"{hours}{minutes}{seconds}";
                }
            
                
                
                yield return new WaitForSeconds(1f);
                remainTime--;
                if (remainTime <= 0) {
                    if(activeObj)
                        activeObj.SetActive(false);
                }
            }
            _txtTimeBoost.text = "";
            _countingTime = false;
            txt.text = "看视频";
            _btnBuyByAds.interactable = true;
            _btnBuyByAds.enabled = true;
        }

        public void UpdateStatus() {
            string timeUp = PlayerPrefs.GetString($"{StringDefine.BOOST_TIME_UP}{(int)_boostType}", TimeUtil.TimeStampSecond.ToString());
            long timeUpLong = long.Parse(timeUp);
            long now = TimeUtil.TimeStampSecond;
            if (now < timeUpLong) {
                remainTime = (int)(timeUpLong - now);

                if(_boostType == EnumDefine.BOOST.AUTO_MERGE_1 || _boostType == EnumDefine.BOOST.PILE_SOUL) {
                    if (_ieCountFree == null) {
                        _ieCountFree = StartCoroutine(IETimeFreeBoost());
                    } else {
                        StopCoroutine(_ieCountFree);
                        _ieCountFree = null;
                        _ieCountFree = StartCoroutine(IETimeFreeBoost());
                    }
                } else {
                    if(_ieCountBoost == null) {
                        _ieCountBoost = StartCoroutine(IECountTimeBoost());
                    } else {
                        StopCoroutine(_ieCountBoost);
                        _ieCountBoost = null;
                        _ieCountBoost = StartCoroutine(IECountTimeBoost());
                    }
                }

                
            }

        }


    }
}

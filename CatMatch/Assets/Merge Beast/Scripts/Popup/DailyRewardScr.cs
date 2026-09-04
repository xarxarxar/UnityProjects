using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using Observer;
using System;

namespace MergeBeast {
    public class DailyRewardScr : MonoBehaviour {
        [SerializeField] private DailyConfig _config;
        [SerializeField] private GameObject _prfDailyItem;
        [SerializeField] private Sprite _borrowBG;
        [SerializeField] private Sprite _redBG;
        [SerializeField] private Animator _myAnim;
        [SerializeField] private GameObject noticeDaily;

        [Header("Tooltip")]
        [SerializeField] private RectTransform _rectTooltip;
        [SerializeField] private Image _imgIconTooltip;
        [SerializeField] private Text _txtNameTooltip;
        [SerializeField] private Text _txtDescriptionTooltip;

        [Header("Popup Chuc Mung")]
        [SerializeField] private GameObject _pnCongrat;
        [SerializeField] private Image _imgIconCongrat;
        [SerializeField] private Text _txtQuantityCongrat;
        [SerializeField] private GameObject _objNotice;
        [SerializeField] private Text _txtRewardName;
        [SerializeField] private Text _txtRewardDes;
        [SerializeField] private Animator _animDone;



        private int totalReward;

        // Start is called before the first frame update
        void SetUp() {            
            totalReward = PlayerPrefs.GetInt(StringDefine.TOTAL_DAY_REWARD);
            
            if (totalReward >= 30) totalReward = 0;
            //Debug.LogFormat("total reward: {0}, lastDay: {1}, span: {2}", totalReward, lastDay, span.Days);

            

            if(_prfDailyItem.transform.parent.childCount == 1) {
                //chua co, tao moi
                for (int i = 0; i < _config.TotalDaily.Count; i++) {
                    GameObject prf = Instantiate(_prfDailyItem, _prfDailyItem.transform.parent);
                    prf.SetActive(true);                 
                }

                UpdateValue();
            } else {
                UpdateValue();
            }

            
        }

        void UpdateValue() {
            VipConfig currentVip = CPlayer.GetVipConfig();
            string lastDay = PlayerPrefs.GetString(StringDefine.LAST_DAY_REWARD);
            TimeSpan span = DateTime.Today - Convert.ToDateTime(lastDay);
            
            for (int i = 1; i < _prfDailyItem.transform.parent.childCount; i++) {
                var prf = _prfDailyItem.transform.parent.GetChild(i);
                
                ItemDaily ctrl = prf.GetComponent<ItemDaily>();
                DailyItem daily = _config.TotalDaily[i-1];

                ctrl.BgItem.sprite = daily.DailyType == EnumDefine.DailyRewardType.GEM ? _redBG : _borrowBG;
                ctrl.Icon.sprite = daily.Icon;
                ctrl.TxtDay.text = daily.DayStt;

                int value = Mathf.RoundToInt(daily.HeSo * Mathf.Pow(10, daily.SoMu));

                if (daily.DailyType == EnumDefine.DailyRewardType.MERGE) {
                    if (currentVip.moreMedalMerge > 0) {
                        value += currentVip.moreMedalMerge / 6;
                    }
                } else if (daily.DailyType == EnumDefine.DailyRewardType.STAR) {
                    if (currentVip.moreStar > 0) {
                        value += currentVip.moreStar / 6;
                    }
                } else if (daily.DailyType == EnumDefine.DailyRewardType.AUTO_MERGE) {
                    if (currentVip.moreAutoMerge > 0) {
                        value += currentVip.moreAutoMerge / 6;
                    }
                } else if (daily.DailyType == EnumDefine.DailyRewardType.GEM) {
                    if (currentVip.rateMoreGem > 0) {
                        value += value * currentVip.rateMoreGem / 100;
                    }
                } else if (daily.DailyType == EnumDefine.DailyRewardType.BOOST_CHEST) {
                    if (currentVip.moreBoostChest > 0) {
                        value += currentVip.moreBoostChest / 6;
                    }
                }
                ctrl.TxtQuantity.text = "×" + value;// daily.SymbolReceived;

                bool isReward = daily.Id - totalReward == 1 && (span.Days > 0 || totalReward == 0);
                ctrl.SetEvent(isReward, daily, value);
                ctrl.DelegateReward = OnClickReward;
                ctrl.DelegateTooltip = OnClickTooltip;
                ctrl.DelegateClose = OnClickCloseToolTip;
                ctrl.ObjSelect.SetActive(isReward);
                ctrl.Icon.SetNativeSize();

                if (daily.Id <= totalReward) {
                    ctrl.ObjDone.SetActive(true);
                }
            }
        }

        private void OnEnable() {
            _myAnim.SetTrigger("appear");
            SetUp();
        }

        private void OnClickReward(DailyItem item, int value) {
            _pnCongrat.SetActive(true);
            _imgIconCongrat.sprite = item.Icon;
            _txtQuantityCongrat.text = string.Format(item.SymbolReceived, value);
            _imgIconCongrat.SetNativeSize();
            _txtRewardName.text = item.Name;
            _txtRewardDes.text = string.Format(item.Description, value);
            _animDone.Play("StarShop-Appear");

            CPlayer.AddVipPoint(Config.VIP_PER_DAY);
            this.PostEvent(EventID.OnVipPointChange);


            switch (item.DailyType) {
                case EnumDefine.DailyRewardType.AUTO_MERGE:
                GameManager.Instance.AddBoostToInventory((int)EnumDefine.BOOST.AUTO_MERGE_1, value);
                break;
                //case EnumDefine.DailyRewardType.SOUL:
                //BigInteger soul = item.HeSo * BigInteger.Pow(10, item.SoMu);
                //UIManager.Instance.UpdateMoneyCoin(soul, false);
                //break;
                case EnumDefine.DailyRewardType.MERGE:
                Utils.AddMedalMerge(value, true);
                this.PostEvent(EventID.OnUpdateAutoMergeMedal);
                break;
                case EnumDefine.DailyRewardType.STAR:
                Utils.AddStar(value);
                break;
                case EnumDefine.DailyRewardType.GEM:
                Utils.AddRubyCoin(value);
                this.PostEvent(EventID.OnUpDateMoney);
                break;
                //case EnumDefine.DailyRewardType.TIME_JUMP:
                //_objNotice.SetActive(true);
                //if (item.HeSo == 2) {
                //    int n = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_2H);
                //    PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_2H, item.SoMu + n);
                //} else if (item.HeSo == 4) {
                //    int m = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_4H);
                //    PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_4H, item.SoMu + m);
                //} else if (item.HeSo == 8) {
                //    int k = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_8H);
                //    PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_8H, item.SoMu + k);
                //}
                //break;
                case EnumDefine.DailyRewardType.BOOST_CHEST:
                int current = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST);
                PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST, current + value);
                _objNotice.SetActive(true);
                break;
            }

            PlayerPrefs.SetInt(StringDefine.TOTAL_DAY_REWARD, ++totalReward);
            PlayerPrefs.SetString(StringDefine.LAST_DAY_REWARD, DateTime.Today.ToString());
            noticeDaily.SetActive(false);

            UIManager.Instance.optionCtl.CheckShowNotice();

        }

        private void OnClickTooltip(DailyItem item, RectTransform pos) {
            _rectTooltip.gameObject.SetActive(true);
            _rectTooltip.position = new UnityEngine.Vector2(_rectTooltip.position.x, pos.position.y + 100f);
            _imgIconTooltip.sprite = item.Icon;
            _txtNameTooltip.text = item.Name;
            _txtDescriptionTooltip.text = item.Description;
            _imgIconTooltip.SetNativeSize();
        }

        private void OnClickCloseToolTip() {
            _rectTooltip.gameObject.SetActive(false);
        }


        public void OnClickClose() {
            _myAnim.SetTrigger("hide");
            StartCoroutine(IEHideMe());
        }

        private IEnumerator IEHideMe() {
            yield return new WaitForSeconds(0.45f);
            this.gameObject.SetActive(false);
        }

    }
}

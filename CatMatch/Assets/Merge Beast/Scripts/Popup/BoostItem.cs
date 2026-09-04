using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;
using System.Numerics;
using System;

namespace MergeBeast
{
    public class BoostItem : MonoBehaviour
    {

        public EnumDefine.BOOSTTYPE ItemType;
        public EnumDefine.BOOST BoostType;

        [SerializeField] private Text _txtIconTime;
        [SerializeField] private Text _txtRemainTime;
        [SerializeField] private Text _txtPrice;
        [SerializeField] private Text _txtWatchAds;
        [SerializeField] private UIButton _btnPurchase;
        [SerializeField] private Sprite _spriteValid;
        [SerializeField] private Sprite _spriteInvalid;

        private int _currentPrice;
        private Coroutine _ieCountBoost;
        private Coroutine _ieCountFree;
        private bool _watched;

        private float _totalTimeBoost;

        // Use this for initialization
        void Start()
        {
            _btnPurchase?.onClick.RemoveAllListeners();
            _txtPrice.transform.parent.gameObject.SetActive(false);
            _txtWatchAds.gameObject.SetActive(false);
            _txtRemainTime.gameObject.SetActive(false);
            _btnPurchase.image.sprite = _spriteValid;

            switch (ItemType)
            {
                case EnumDefine.BOOSTTYPE.ADS:
                    _txtWatchAds.gameObject.SetActive(true);
                    if (AdsManager.Instance.IsLoaded())
                    {
                        _btnPurchase?.onClick.AddListener(this.OnClickWatchAds);
                    }
                    else
                    {
                        _btnPurchase.image.sprite = _spriteInvalid;
                    }
                    break;
                case EnumDefine.BOOSTTYPE.CASH:
                    BoostData data = BoostManager.Instance.Boost(BoostType);
                    _txtPrice.transform.parent.gameObject.SetActive(true);
                    _txtPrice.text = data.Price.ToString();
                    _btnPurchase?.onClick.AddListener(this.OnClickPurchase);
                    _currentPrice = data.Price;
                    break;
                case EnumDefine.BOOSTTYPE.FREE:
                    int use = PlayerPrefs.GetInt($"{StringDefine.FREE_BOOST}{(int)BoostType}", 0);
                    _txtWatchAds.gameObject.SetActive(true);
                    if (use == 0)
                    {
                        BoostData bst = BoostManager.Instance.Boost(BoostType);
                        PlayerPrefs.SetInt($"{StringDefine.FREE_BOOST}{(int)BoostType}", bst.Free);
                        use = bst.Free;
                    }
                    _txtWatchAds.text = use.ToString();
                    break;
            }
        }

        private void OnEnable()
        {
            _txtIconTime.text = string.Empty;
            var boost = BoostManager.Instance.GetBoostActive(BoostType);
            if (boost != null)
            {
                _ieCountBoost = StartCoroutine(IECountTimeBoost(boost.TimeRemanin));
            }

            switch (BoostType)
            {
                case EnumDefine.BOOST.SERVARAL_SOUL:
                case EnumDefine.BOOST.PACK_SOUL:
                case EnumDefine.BOOST.CHEST_SOUL:
                    BigInteger dps = GameManager.Instance.TotalDameBattle() * (long)BoostManager.Instance.Boost(BoostType).Time;
                    Text txt = this.transform.Find("txtPrice").GetComponent<Text>();
                    if (txt != null) txt.text = $"<color=#D37100FF>{Utils.FormatNumber(dps)}</color>";
                    break;
                default: break;
            }

            string date = PlayerPrefs.GetString($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)BoostType}");
            var cd = _btnPurchase.transform.Find("clockCD");
            if (cd != null) cd.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(date))
            {
                DateTime markDate = DateTime.Parse(date);
                TimeSpan span = DateTime.Now - markDate;

                int remainTime = GetMaxTimeFree() - (int)span.TotalSeconds;
                //    Debug.Log($"OldDate : {markDate} - Now : {DateTime.Now} - Span time : {span.TotalSeconds}");
                if (remainTime > 0)
                {
                    _ieCountFree = StartCoroutine(IETimeFreeBoost());
                }
                else
                {
                    PlayerPrefs.DeleteKey($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)BoostType}");
                    _btnPurchase.enabled = true;
                    _txtWatchAds.text = "<size=25><color=#FF8600FF>获得</color></size>\n看视频";
                    _watched = false;
                }
            }
            else
            {
                if (ItemType == EnumDefine.BOOSTTYPE.ADS)
                {
                    if (AdsManager.Instance.IsLoaded())
                    {
                        _btnPurchase?.onClick.AddListener(this.OnClickWatchAds);
                        _txtWatchAds.text = "<size=25><color=#FF8600FF>获得</color></size>\n看视频";
                        _btnPurchase.image.sprite = _spriteValid;
                    }
                    else
                    {
                        _btnPurchase.image.sprite = _spriteInvalid;
                        _txtWatchAds.text = "无视频";
                    }
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

        private IEnumerator IECountTimeBoost(float time)
        {
            int remainTime = (int)time;
            while (remainTime > 0)
            {
                int minute = remainTime / 60;
                int second = (remainTime - minute * 60) % 60;

                _txtIconTime.text = $"{minute.ToString("D2")}:{second.ToString("D2")}";

                yield return new WaitForSeconds(1f);
                remainTime--;
            }
            _txtIconTime.text = "";
        }

        private IEnumerator IETimeFreeBoost()
        {
            string date = PlayerPrefs.GetString($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)BoostType}");
            DateTime markDate = DateTime.Parse(date);
            TimeSpan span = DateTime.Now - markDate;

            int remainTime = GetMaxTimeFree() - (int)span.TotalSeconds;
            Image img = _btnPurchase.transform.Find("clockCD").GetComponent<Image>();
            img.gameObject.SetActive(true);
            float maxtime = GetMaxTimeFree();

            while (remainTime > 0)
            {
                int minute = remainTime / 60;
                int second = (remainTime - minute * 60) % 60;

                _txtWatchAds.text = $"{minute.ToString("D2")}:{second.ToString("D2")}";
                img.fillAmount = remainTime / maxtime;

                yield return new WaitForSeconds(1f);
                remainTime--;

            }
            _txtWatchAds.text = "<size=25><color=#FF8600FF>获得</color></size>\n看视频";
            _btnPurchase.enabled = true;
            img.gameObject.SetActive(false);
            _watched = false;
        }

        private void OnClickPurchase()
        {
            int gem = PlayerPrefs.GetInt(StringDefine.MONEY_GEM);
            if (gem > _currentPrice) // Du tien de mua boost
            {
                string mgs = $"你确定要花费<color=red>{_currentPrice}宝石</color>购买此道具吗";
                ScreenManager.Instance.ShowConfirm(3, mgs, () =>
                 {
                     BoostManager.Instance.AddBoost(BoostType);
                     int remainGem = gem - _currentPrice;                     
                     PlayerPrefs.SetInt(StringDefine.MONEY_GEM, remainGem);
                     this.PostEvent(EventID.OnUpDateMoney);

                     BoostData data = BoostManager.Instance.Boost(BoostType);
                     _ieCountBoost = StartCoroutine(IECountTimeBoost(data.Time));
                 });
            }
            else
            {
                string mgs = "你的宝石不足，是否要充值？";
                ScreenManager.Instance.ShowConfirm(3, mgs, () =>
                {
                    Debug.Log($"Open Panel Nap Gem");
                    ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);
                });
            }

        }

        private void OnClickWatchAds()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                string date = DateTime.Now.ToString();
                PlayerPrefs.SetString($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)BoostType}", date);
                BoostManager.Instance.AddBoost(BoostType);
                _btnPurchase.enabled = false;
                _ieCountFree = StartCoroutine(IETimeFreeBoost());

                var boost = BoostManager.Instance.GetBoostActive(BoostType);
                if (boost != null)_ieCountBoost = StartCoroutine(IECountTimeBoost(boost.TimeRemanin));

            });
        }

        private int GetMaxTimeFree()
        {
            int time = 0;
            switch (BoostType)
            {
                case EnumDefine.BOOST.AUTO_MERGE_1:
                    time = 300;
                    break;
                case EnumDefine.BOOST.SPAWN_FASTER_1:
                    time = 600;
                    break;
                case EnumDefine.BOOST.DAMAGE_BOOST_1:
                    time = 900;
                    break;
                case EnumDefine.BOOST.SERVARAL_SOUL:
                    time = 1800;
                    break;
            }
            return time;
        }

    }
}

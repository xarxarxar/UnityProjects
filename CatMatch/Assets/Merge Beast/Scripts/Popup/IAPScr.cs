using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;
using System;

namespace MergeBeast {
    public class IAPScr : BaseScreen {
        [SerializeField] private IAPConfig _config;
        [SerializeField] private IAPItem _prfIap;
        [SerializeField] private Transform _parentIAP;

        [SerializeField] private Text _txtCoin;
        [SerializeField] private Text _txtHomeCoin;
        [SerializeField] private Text _txtGem;
        [SerializeField] private Button _btnClose;

        [SerializeField] Text minText;
        [SerializeField] Text nextText;
        [SerializeField] Text currentVipProgressText;
        [SerializeField] Slider vipSlider;
        [SerializeField] Button btnShowVip;

        [Header("=======The thang")]
        [SerializeField] private GameObject monthObj;
        [SerializeField] private Button btnBuyMonth;

        [Header("=======The tuan 1")]
        [SerializeField] private GameObject week1Obj;
        [SerializeField] private Button btnBuyWeek1;

        [Header("=======The tuan 2")]
        [SerializeField] private GameObject week2Obj;
        [SerializeField] private Button btnBuyWeek;

        private string gemStatus;
        private Dictionary<string, int> dicBuyStatus = new Dictionary<string, int>();

        private List<IAPItem> listItem = new List<IAPItem>();

        // Use this for initialization
        void Start() {

            SetUp();
            this.RegisterListener(EventID.OnUpDateMoney, (sender, param) => OnUpdateRuby());
            this.RegisterListener(EventID.OnPurchaseSuccess, (sender, param) => OnPurchaseSuccess(param.ToString()));
            _btnClose?.onClick.AddListener(this.OnClickClose);
            btnShowVip.onClick.AddListener(() => ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.VIP));
            
        }

        private void OnPurchaseSuccess(string productId) {
            if (IsBuyGem(productId)) {
                BuyGemSuccess(productId);
            }

            UpdateVipPoint();
        }


        void SetUp() {
            for (int i = 0; i < _config.Packs.Count; i++) {
                IAPPack pack = _config.Packs[i];
                IAPItem item = Instantiate(_prfIap, _parentIAP);
                listItem.Add(item);
                item.BtnBuy?.onClick.AddListener(() => this.OnClickBuyIAP(pack));
            }

            UpdateGemPackStatus();
        }

        #region VIp

        public void UpdateVipPoint() {
            int prevVipIndex = 0;
            int nextVipIndex = 0;

            VipConfig currentData = null;
            for (int i = 0; i < GameAssets.Instance.listVipConfig.Count; i++) {
                VipConfig data = GameAssets.Instance.listVipConfig[i];
                if (CPlayer.vipPoint >= data.min && CPlayer.vipPoint <= data.max) {
                    currentData = data;
                    prevVipIndex = i - 1;
                    nextVipIndex = i + 1;
                    break;
                }
            }
            if (prevVipIndex < 0) {
                //dang o 0
                vipSlider.minValue = currentData.min;
                vipSlider.maxValue = currentData.max;
                vipSlider.value = CPlayer.vipPoint;                

            } else {
                VipConfig prevData = GameAssets.Instance.listVipConfig[prevVipIndex];
                vipSlider.minValue = currentData.min - prevData.max;
                vipSlider.maxValue = currentData.max - prevData.max;
                vipSlider.value = CPlayer.vipPoint - prevData.max;                

            }


            if (prevVipIndex >= GameAssets.Instance.listVipConfig.Count - 2) {
                //max vip 4
                vipSlider.minValue = 0;
                vipSlider.maxValue = 1;
                vipSlider.value = 1;
                currentVipProgressText.text = "已满";
            } else {
                currentVipProgressText.text = $"{CPlayer.vipPoint}/{currentData.max}";
            }

            minText.text = currentData.status;
            if (nextVipIndex >= GameAssets.Instance.listVipConfig.Count) {
                //max vip
                nextText.text = "";
            } else {
                nextText.text = GameAssets.Instance.listVipConfig[nextVipIndex].status;
            }
        }
        #endregion


        #region GEM
        private bool IsBuyGem(string productId) {
            for (int i = 0; i < _config.Packs.Count; i++) {
                if (_config.Packs[i].PurchaseID.Equals(productId)) {
                    return true;
                }
            }
            return false;
        }

        private void BuyGemSuccess(string productId) {
            //check co dc bonus ko
            int numOfBuy = NumOfBuy(productId);
            int gemReceive = (int)_config.GetPackIAPByProductId(productId).GemReceived;

            Utils.AddRubyCoin(gemReceive * (numOfBuy == 0 ? 2 : 1));
            this.PostEvent(EventID.OnUpDateMoney);
            OnUpdateRuby();
            CPlayer.AddVipPoint(_config.GetPackIAPByProductId(productId).vipPoint);

            //update value to dic
            dicBuyStatus[productId]++;

            //luu vao pref
            gemStatus = "";
            foreach (var item in dicBuyStatus) {
                gemStatus += $"{item.Key}-{item.Value};";
            }

            PlayerPrefs.SetString(StringDefine.BUY_GEM_STATUS, gemStatus);

            UpdateGemPackStatus();

            string msg = "购买成功";
            if (numOfBuy == 0) {
                msg += $"\n赠送{gemReceive}";
            }
            UIManager.Instance.ShowNotify(msg);
        }

        void UpdateGemPackStatus() {
            for (int i = 0; i < _config.Packs.Count; i++) {
                IAPPack pack = _config.Packs[i];
                IAPItem item = listItem[i];

                item.TxtGem.text = pack.GemReceived.ToString();
                item.TxtName.text = pack.Name.ToString();
                string price = IAPManager.Instance.LocalizePrice(pack.PurchaseID).Equals("$0.01") ? $"{ pack.Price}$" : IAPManager.Instance.LocalizePrice(pack.PurchaseID);
                item.TxtPrice.text = price;//  //$"{pack.Price}$";
                item.ImgIcon.sprite = pack.Icon;
                item.ImgIcon.SetNativeSize();
                item.TxtVip.text = "+" + pack.vipPoint;

                //check is first Time buy                
                item.FirstBonus.SetActive(NumOfBuy(pack.PurchaseID) == 0);
                item.TxtBonus.text = $"首充奖励<color=#27E351>+{pack.GemReceived}</color>";
            }

        }

        private int NumOfBuy(string productId) {
            int numOfBuy = 0;
            if (dicBuyStatus.ContainsKey(productId)) {
                numOfBuy = dicBuyStatus[productId];
            }
            return numOfBuy;
        }
        #endregion


        private void Update() {
            _txtCoin.text = _txtHomeCoin.text;
        }

        private void OnEnable() {
            OnUpdateRuby();
            SetUpGemStatus();
            UpdateVipPoint();

        }

        void SetUpGemStatus() {
            if (PlayerPrefs.HasKey(StringDefine.BUY_GEM_STATUS)) {
                //da co key
                gemStatus = PlayerPrefs.GetString(StringDefine.BUY_GEM_STATUS, "");
            } else {
                //chua co key
                gemStatus = "";
                for (int i = 0; i < _config.Packs.Count; i++) {
                    gemStatus += $"{_config.Packs[i].PurchaseID}-0;";
                }
            }

            dicBuyStatus.Clear();
            string[] list = gemStatus.Split(';');
            for (int i = 0; i < list.Length; i++) {
                if (!string.IsNullOrEmpty(list[i])) {
                    string item = list[i];
                    string[] detail = item.Split('-');
                    string dicKey = detail[0];
                    int dicValue = int.Parse(detail[1]);
                    dicBuyStatus.Add(dicKey, dicValue);
                }
            }

        }

        private void OnClickBuyIAP(IAPPack pack) {
            IAPManager.Instance.BuyPackage(pack.PurchaseID);
            // OnPurchaseSuccess(pack.PurchaseID);
        }

        private void OnUpdateRuby() {
            _txtGem.text = "" + Utils.GetCurrentRubyMoney();
        }

        private void OnClickClose() {
            ScreenManager.Instance.DeActiveScreen();
        }
    }
}

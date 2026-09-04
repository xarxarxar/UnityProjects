using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using UnityEngine.Events;
using Observer;
using System;

namespace MergeBeast {
    public enum StarShopType {
        Soul, MergeMedal, Beast, AutoMerge, Gem
    }
    public class StarShopSrc : BaseScreen {
        //public List<GroupStarShop> ShopItem;
        public List<StarShopItem> ListItem;
        public List<StarShop> StarShopData;

        [SerializeField] private Image _imgItemIcon;
        [SerializeField] private Text _txtItemName;
        [SerializeField] private Text _txtPrice;
        [SerializeField] private Text _txtInstock;
        [SerializeField] private Text _txtDescription;
        [SerializeField] private Text _txtQuantity;
        [SerializeField] private UIButton _btnMinus;
        [SerializeField] private UIButton _btnAdd;
        [SerializeField] private UIButton _btnSetMax;
        [SerializeField] private GameObject _objBuy;
        [SerializeField] private GameObject _objNotify;
        [SerializeField] private GameObject _objNotyRuby;
        [SerializeField] private Text _txtCountDownTime;
        [SerializeField] private UIButton _btnRefresh;
        [SerializeField] private Text _txtCost;
        [SerializeField] private UIButton _btnBuy;
        [SerializeField] private Button _btnClosePopup;
        [SerializeField] private Text currentStarText;

        [Header("Buy Done")]
        [SerializeField] private GameObject _objBuyDone;
        [SerializeField] private Animator _animDone;
        [SerializeField] private Image _imgDoneIcon;
        [SerializeField] private Text _txtDoneName;
        [SerializeField] private Text _txtDoneDes;

        private UnityAction _actionBuy;
        private UnityAction _actionDone;
        private int _totalInstock;
        private int _currentInstock;
        private int _price;
        private int _currentMoneyMerge;
        private Coroutine _ieRefreshTime;
        private bool _isRefresh;

        //List<int> _listSelected = new List<int>();
        string listSelectedString = "";        
        private List<StarShopLoader> listData = new List<StarShopLoader>();
        // Start is called before the first frame update
        void Start() {
            _btnMinus?.onClick.AddListener(this.OnMinusStock);
            _btnAdd?.onClick.AddListener(this.OnClickAddStock);
            _btnSetMax?.onClick.AddListener(this.OnSetMaxStock);
            _btnBuy?.onClick.AddListener(this.OnClickBuyItem);
            _btnRefresh?.onClick.AddListener(this.OnClickRefresh);
            _btnClosePopup?.onClick.AddListener(this.OnClickClosePopup);
        }

        private int GetTimeRefresh() {
            return CPlayer.GetVipConfig().timeRefreshStarShop * 60;
        }

        private void OnEnable() {
            currentStarText.text = PlayerPrefs.GetInt(StringDefine.MONEY_MERGE).ToString();
            CheckListItem();

            string date = PlayerPrefs.GetString(StringDefine.TIME_REFRESH_STAR_SHOP, "");
            if (string.IsNullOrEmpty(date)) {
                string now = DateTime.Now.ToString();
                PlayerPrefs.SetString(StringDefine.TIME_REFRESH_STAR_SHOP, now);
                _ieRefreshTime = StartCoroutine(IECountTimeFresh(GetTimeRefresh()));
            } else {
                DateTime markDate = DateTime.Parse(date);
                TimeSpan span = DateTime.Now - markDate;
                int remainTime = GetTimeRefresh() - (int)span.TotalSeconds;
                _ieRefreshTime = StartCoroutine(IECountTimeFresh(remainTime));
            }

            this.OnShowItem();
            //if (!_isRefresh) {
            //    this.OnShowItem();
            //    _isRefresh = true;
            //}
        }

        void CheckListItem() {
            listData.Clear();
            List<int> listIndex = new List<int>();
            for (int i = 0; i < StarShopData.Count; i++) {
                listIndex.Add(i);
            }
            string list = PlayerPrefs.GetString(StringDefine.LIST_STAR_SHOP, "");
            if (string.IsNullOrEmpty(list)) {
                //khong co data, thi refresh data
                for (int i = 0; i < ListItem.Count; i++) {
                    int n = 0;
                    chonlai: n = UnityEngine.Random.Range(0, listIndex.Count);
                    n = listIndex[n];
                    //kiem tra 
                    StarShop starShop = StarShopData[n];
                    if (starShop.type == StarShopType.Beast) {
                        int upgradeLevel = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                        int beast = upgradeLevel + starShop.RewardValue;
                        int shopLevel = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 0);
                        if (beast > shopLevel) goto chonlai;
                    }
                    listIndex.Remove(n);

                    StarShopLoader loader = new StarShopLoader(n, Quantity());
                    listData.Add(loader);
                    listSelectedString += loader.GetId().ToString() + "-" + loader.GetAmount().ToString() + (i < ListItem.Count - 1 ? ";" : "");
                }
                PlayerPrefs.SetString(StringDefine.LIST_STAR_SHOP, listSelectedString);
            } else {
                //da co data, ko refresh data nua
                string[] spl = list.Split(';');
                for (int i = 0; i < spl.Length; i++) {
                    string[] loaderData = spl[i].Split('-');
                    StarShopLoader loader = new StarShopLoader();
                    int id = int.Parse(loaderData[0]);
                    int amount = int.Parse(loaderData[1]);
                    loader.SetId(id);
                    loader.SetAmount(amount);
                    listData.Add(loader);
                }
            }
        }

        private void OnDisable() {
            if (_ieRefreshTime != null) StopCoroutine(_ieRefreshTime);
            GameManager.Instance.ReadySpawnGift();
        }

        private void OnShowItem() {
            Debug.LogError("Show Item");
            for (int i = 0; i < ListItem.Count; i++) {
                StarShopItem shopItem = ListItem[i];
                StarShop starShop = null;

                StarShopLoader loader = listData[i];
                int n = loader.GetId();// _listSelected[i];

                shopItem._btnSlect?.onClick.RemoveAllListeners();
                starShop = StarShopData[n];
                //if (starShop.type == StarShopType.Beast) {
                //    int upgradeLevel = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                //    int beast = upgradeLevel + starShop.RewardValue;

                //    int price = starShop.Price;
                //    starShop = GetShopBeastItem(beast);
                //    starShop.Price = price;
                //}

                shopItem._btnSlect?.onClick.AddListener(() => {                    
                    if (starShop.type == StarShopType.Soul) {
                        ItemSoul(starShop.RewardValue, starShop.Description);
                    } else if (starShop.type == StarShopType.MergeMedal) {
                        MergeMedal(starShop.RewardValue, starShop.Description);
                    } else if (starShop.type == StarShopType.Gem) {
                        Gems(starShop.RewardValue, starShop.Description);
                    } else if (starShop.type == StarShopType.Beast) {
                        int upgradeLevel = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                        int beast = upgradeLevel + starShop.RewardValue;

                        //int price = starShop.Price;
                        //starShop = GetShopBeastItem(beast);
                        //starShop.Price = price;
                        GetBeast(starShop); //them icon vao day
                    } else if (starShop.type == StarShopType.AutoMerge) {
                        AutoMerge(starShop.RewardValue, starShop.Description);
                    }
                });

                if (starShop.type == StarShopType.Beast) {
                    int upgradeLevel = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
                    int beast = upgradeLevel + starShop.RewardValue;
                    BeastData data = GameManager.Instance.GetBeast(beast);
                    if (data != null) {
                        shopItem._imgIcon.sprite = data.Character;
                        shopItem._txtName.text = data.Name + $" 等级{beast + 1}";
                        shopItem._txtPrice.text = starShop.Price.ToString();
                    }
                } else {
                    shopItem._imgIcon.sprite = starShop.Icon;
                    shopItem._txtName.text = starShop.ItemName;
                    shopItem._txtPrice.text = starShop.Price.ToString();
                }
                shopItem._imgIcon.SetNativeSize();
                shopItem._txtReward.text = starShop.ItemReward;
                
                int stock = loader.GetAmount();
                shopItem._txtQuantity.text = $"库存：{stock}";
                shopItem._objSoldOut.SetActive(false);

                if (stock <= 0) {
                    shopItem._btnSlect?.onClick.RemoveAllListeners();
                    shopItem._objSoldOut.SetActive(true);
                } else {

                    shopItem._btnSlect?.onClick.AddListener(() => {
                        _imgItemIcon.sprite = _imgDoneIcon.sprite = shopItem._imgIcon.sprite;
                        _imgItemIcon.SetNativeSize();
                        _imgDoneIcon.SetNativeSize();
                        _txtItemName.text = _txtDoneName.text = shopItem._txtName.text;
                        _txtPrice.text = _txtCost.text = shopItem._txtPrice.text;
                        _txtQuantity.text = "1";
                        string str = shopItem._txtQuantity.text.Substring(9);
                        _totalInstock = loader.GetAmount();
                        _txtInstock.text = $"库存：{_totalInstock}";
                        _objBuy.SetActive(true);
                        _btnMinus.interactable = false;
                        _btnAdd.interactable = _totalInstock > 1;
                        _currentInstock = 1;
                        _btnSetMax.interactable = _totalInstock > 1;



                        _actionDone = () => {
                            loader.SetAmount(loader.GetAmount() - _currentInstock);
                            if (loader.GetAmount() <= 0) {
                                shopItem._btnSlect?.onClick.RemoveAllListeners();
                                shopItem._objSoldOut.SetActive(true);
                                shopItem._txtQuantity.text = $"库存 0";
                            } else {
                                shopItem._txtQuantity.text = $"库存：{loader.GetAmount()}";
                            }

                            //luu vao data
                            SaveData();
                        };
                    });
                }
            }
        }

        void SaveData() {
            listSelectedString = "";
            for (int k = 0; k < listData.Count; k++) {
                StarShopLoader loaderSet = listData[k];
                listSelectedString += loaderSet.GetId() + "-" + loaderSet.GetAmount();
                if (k < listData.Count - 1) {
                    listSelectedString += ";";
                }
            }
            PlayerPrefs.SetString(StringDefine.LIST_STAR_SHOP, listSelectedString);
        }

        private void ItemSoul(int second, string desc) {
            BigInteger soul = GameManager.Instance.TotalDamage() * second;
            _txtDescription.text = desc + "\n灵魂：" + Utils.FormatNumber(soul);

            _actionBuy = () => {
                //        Debug.Log($"Mua thanh cong {soul} Soul");
                UIManager.Instance?.UpdateMoneyCoin(soul * _currentInstock, true);
                this.OnBuyDone();
            };
        }

        private void MergeMedal(int merge, string desc) {
            _txtDescription.text = desc;
            _actionBuy = () => {
                //        Debug.Log($"Mua thanh cong {merge} lan merge");
                Utils.AddMedalMerge(merge * _currentInstock, true);
                this.PostEvent(EventID.OnUpdateAutoMergeMedal);
                this.OnBuyDone();
            };
        }

        private void GetBeast(StarShop starShop) {
            int upgradeLevel = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            int beast = upgradeLevel + starShop.RewardValue;

            BigInteger dps = Utils.GetDameByLevel(beast + 1) + Utils.GetDameByLevel(beast + 1) * Utils.GetTotalAscend() / 100;

            _txtDescription.text = $"获得：{starShop.ItemName}\nDPS：{Utils.FormatNumber(dps)}";            
            
            _actionBuy = () => {
                //    Debug.Log($"Mua them con beast thanh cong");
                
                for (int i = 0; i < _currentInstock; i++) {                    
                    GameManager.Instance.AddGift(beast);
                }
                this.OnBuyDone();
            };
        }

        private void AutoMerge(int second, string desc) {
            _txtDescription.text = desc;
            _actionBuy = () => {
                //    Debug.Log($"Mua auto merge thanh cong");
                BoostManager.Instance?.AddFreeBoost(EnumDefine.FREEBOOST.AUTO_MERGE, second * _currentInstock, 0);                
                this.OnBuyDone();
            };
        }

        /// <summary>
        /// Add gem
        /// </summary>
        /// <param name="gem">so gem can add</param>
        /// <param name="desc">mo ta cua item</param>
        private void Gems(int gem, string desc) {
            _txtDescription.text = desc;
            _actionBuy = () => {
                //    Debug.Log($"Mua gem {gem} thanh cong");
                Utils.AddRubyCoin(gem * _currentInstock);
                this.PostEvent(EventID.OnUpDateMoney);
                this.OnBuyDone();
            };
        }

        private void OnClickAddStock() {
            _currentInstock++;
            _currentInstock = Mathf.Clamp(_currentInstock, 1, _totalInstock);
            _btnAdd.interactable = _currentInstock < _totalInstock;
            _btnMinus.interactable = _currentInstock > 1;
            this.UpdateCost();
        }

        private void OnMinusStock() {
            _currentInstock--;
            _currentInstock = Mathf.Clamp(_currentInstock, 1, _totalInstock);
            _btnAdd.interactable = _currentInstock < _totalInstock;
            _btnMinus.interactable = _currentInstock > 1;
            this.UpdateCost();
        }

        private void OnSetMaxStock() {
            _currentInstock = _totalInstock;
            _btnAdd.interactable = false;
            _btnMinus.interactable = _currentInstock > 1;
            this.UpdateCost();
        }

        private void UpdateCost() {
            int price = int.Parse(_txtPrice.text);
            int cost = price * _currentInstock;
            _txtCost.text = cost.ToString();
            _txtQuantity.text = _currentInstock.ToString();
        }

        private bool CheckCanBuy() {
            int price = int.Parse(_txtPrice.text);
            _currentMoneyMerge = PlayerPrefs.GetInt(StringDefine.MONEY_MERGE, 0);
            return _currentMoneyMerge >= price * _currentInstock;
        }

        private void OnClickBuyItem() {
            if (CheckCanBuy()) {                
                _actionBuy?.Invoke();
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.BuyStarShop);
            } else {
                _objNotify.SetActive(true);
                StartCoroutine(IEOffNotify(_objNotify));
            }
        }

        private void OnClickRefresh() {
            if (Utils.GetCurrentRubyMoney() >= 5) {
                listSelectedString = "";
                PlayerPrefs.SetString(StringDefine.LIST_STAR_SHOP, listSelectedString);
                CheckListItem();
                this.OnShowItem();
                Utils.AddRubyCoin(-5);
                this.PostEvent(EventID.OnUpDateMoney);

                string now = DateTime.Now.ToString();
                PlayerPrefs.SetString(StringDefine.TIME_REFRESH_STAR_SHOP, now);

                if (_ieRefreshTime != null) StopCoroutine(_ieRefreshTime);
                _ieRefreshTime = StartCoroutine(IECountTimeFresh(GetTimeRefresh()));
            } else {
                _objNotyRuby.SetActive(true);
                StartCoroutine(IEOffNotify(_objNotyRuby));
            }
        }

        private void OnClickClosePopup() {
            ScreenManager.Instance.DeActiveScreen();
        }

        private IEnumerator IEOffNotify(GameObject obj) {
            yield return new WaitForSeconds(1.5f);
            obj.SetActive(false);
        }

        private IEnumerator IECountTimeFresh(int remainTime) {
            while (remainTime >= 0) {
                int hour = remainTime / 3600;
                int minute = (remainTime - hour * 3600) / 60;
                int second = (remainTime - hour * 3600 - minute * 60) % 60;
                
                _txtCountDownTime.text = $"刷新于 {hour.ToString("D2")}:{minute.ToString("D2")}:{second.ToString("D2")}";

                yield return new WaitForSeconds(1f);
                remainTime--;

            }

            string now = DateTime.Now.ToString();
            PlayerPrefs.SetString(StringDefine.TIME_REFRESH_STAR_SHOP, now);
            listSelectedString = "";
            PlayerPrefs.SetString(StringDefine.LIST_STAR_SHOP, listSelectedString);
            CheckListItem();
            this.OnShowItem();

            if (_ieRefreshTime != null) StopCoroutine(_ieRefreshTime);
            _ieRefreshTime = StartCoroutine(IECountTimeFresh(GetTimeRefresh()));

        }

        private void OnBuyDone() {
            _objBuy.SetActive(false);
            _objBuyDone.SetActive(true);
            _animDone.Play("StarShop-Appear");
            _txtDoneName.text += $"<color=#fff> ×{_currentInstock}</color>";
            _txtDoneDes.text = _txtDescription.text;

            int cost = int.Parse(_txtCost.text); // Tru Tien
            UIManager.Instance?.AddMoneyMerge(-cost);
            currentStarText.text = PlayerPrefs.GetInt(StringDefine.MONEY_MERGE, 0).ToString();
            _actionDone?.Invoke();
        }

        private StarShop GetShopBeastItem(int beast) {
            StarShop shop = new StarShop();
            BeastData data = GameManager.Instance.GetBeast(beast);
            if (data != null) {
                shop.Icon = data.Character;
                shop.ItemName = data.Name + $" Lv.{beast + 1}";
                shop.Level = beast;
            }
            return shop;
        }


        private int Quantity() {
            int num = 1;
            int ran = UnityEngine.Random.Range(0, 100);
            if (ran < 5) num = 3;
            else if (ran < 15) num = 2;
            return num;
        }
    }

    [System.Serializable]
    public class StarShop {
        public StarShopType type;
        public string ItemName;
        public string ItemReward; //text nho hien o goc ben trai
        public int RewardValue;
        public string Description;
        public int Rarity;
        public int Price;
        public Sprite Icon;
        public int Level; //chi dung cho beast

    }

    [System.Serializable]
    public class GroupStarShop {
        public string GroupName;
        public List<StarShop> GroupItem;
    }

    class StarShopLoader {
        private int id;
        private int amount;

        public StarShopLoader(int id, int amount) {
            this.id = id;
            this.amount = amount;
        }

        public StarShopLoader() { }

        public void SetId(int id) {
            this.id = id;
        }
        public int GetId() {
            return id;
        }
        public void SetAmount(int amount) {
            this.amount = amount;
        }
        public int GetAmount() {
            return amount;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;

namespace MergeBeast {
    public class DailyBoost : MonoBehaviour {
        [SerializeField] private Image _imgIconBoost;
        [SerializeField] private Text _txtQuantity;
        [SerializeField] private Text _txtNameBoost;
        [SerializeField] private Text _txtDesBoost;
        [SerializeField] private Button _btnUse;

        [Header("Boost Chest")]
        [SerializeField] private List<BoostItemAds> _listBoost;
        [SerializeField] private List<Sprite> _iconChest;

        [Header("popup chuc mung")]
        [SerializeField] private GameObject popup;
        [SerializeField] private Image icon;
        [SerializeField] private Text txtQuantity;        
        [SerializeField] private Text txtName;


        private EnumDefine.DailyBoost _myBoostType;
        private int _quantity;

        private BoostScreen boostScreen;

        private void Awake() {
            boostScreen = FindObjectOfType<BoostScreen>();
        }


        public void SetBoost(int bost, int quantity) {
            //Debug.LogError("boost: " + bost + ", quantity: " + quantity);
            _myBoostType = (EnumDefine.DailyBoost)bost;
            _quantity = quantity;
            _txtQuantity.text = $"x{_quantity}";
            _btnUse.onClick.RemoveAllListeners();
            switch (_myBoostType) {
                case EnumDefine.DailyBoost.BOOST_CHEST:
                _txtNameBoost.text = "增益宝箱";
                _txtDesBoost.text = "获得随机增益";
                _btnUse.onClick?.AddListener(this.OnClickBoostChest);
                this.SetIconBoost(0);
                break;
                case EnumDefine.DailyBoost.TIME_JUMP_2H:
                _txtNameBoost.text = "时间跳跃2小时";
                _txtDesBoost.text = "前进至2小时后";
                _btnUse.onClick?.AddListener(() => this.OnClickTimeJump(2));
                this.SetIconBoost(1);
                break;
                case EnumDefine.DailyBoost.TIME_JUMP_4H:
                _txtNameBoost.text = "时间跳跃4小时";
                _txtDesBoost.text = "前进至4小时后";
                _btnUse.onClick?.AddListener(() => this.OnClickTimeJump(4));
                this.SetIconBoost(2);
                break;
                case EnumDefine.DailyBoost.TIME_JUMP_8H:
                _txtNameBoost.text = "时间跳跃8小时";
                _txtDesBoost.text = "前进至8小时后";
                _btnUse.onClick?.AddListener(() => this.OnClickTimeJump(8));
                this.SetIconBoost(3);
                break;
            }
        }

        private void SetIconBoost(int index) {
            _imgIconBoost.sprite = _iconChest[index];
            _imgIconBoost.SetNativeSize();
        }

        private void OnClickBoostChest() {
            int bost = Random.Range(0, _listBoost.Count);
            BoostItemAds randomBoost = _listBoost[bost];

            //show popup
            popup.SetActive(true);
            BoostData data = BoostManager.Instance.Boost(randomBoost.GetBoostType());
            icon.sprite = data.Icon;
            icon.SetNativeSize();
            txtName.text = data.Name;


            var boostType = BoostManager.Instance.Boost(randomBoost.GetBoostType());

            if (randomBoost.GetBoostType() == EnumDefine.BOOST.PILE_SOUL) {               
                randomBoost.DailyActiveBoost();
                UIManager.Instance?.ShowNotify($"获得{boostType.Name}");
                txtQuantity.text = "×" + GameManager.Instance.TotalDamage() * Config.PILE_SOUL_REWARD;                
            } else {
                boostScreen.UpdateBoostChest((int)randomBoost.GetBoostType(), 1);
                UIManager.Instance?.ShowNotify($"获得{boostType.Name}");
                txtQuantity.text = "×1";
            }
            this.UpdateQuantityBoost();

            

        }

        private void OnClickTimeJump(int time) {
            int second = 3600 * time;
            System.Numerics.BigInteger soul = GameManager.Instance.TotalDameBattle() * second;
            UIManager.Instance.UpdateMoneyCoin(soul, false);
            this.PostEvent(EventID.OnTimeJump, second);
            UIManager.Instance.ShowNotify($"使用{_txtNameBoost.text}");
            this.UpdateQuantityBoost();

            //spawn beast
            int lvSpam = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
            float _timeSpawnBeast = 6.1f - lvSpam * 0.1f;
            int beastSpawned = Mathf.RoundToInt((float)second / _timeSpawnBeast);
            var arrBeast = Utils.ArrayPower2(beastSpawned);
            var shop = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 1);

            int _currentIDBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            for (int i = 0; i < arrBeast.Count; i++) {
                //int beastGift = Mathf.Clamp(_currentIDBeast + arrBeast[i] + 1, _currentIDBeast, shop);                
                int beastGift = _currentIDBeast + arrBeast[i] + 1;
                GameManager.Instance.AddGift(beastGift);
            }
            GameManager.Instance.ReadySpawnGift();
        }

        private void UpdateQuantityBoost() {
            _quantity--;
            if (_quantity <= 0) {
                Destroy(this.gameObject);
            } else {
                _txtQuantity.text = $"x{_quantity}";
            }

            PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)_myBoostType, _quantity);
        }
    }//end class

    public class BoostChestData {
        private int id;
        private int amount;

        public BoostChestData() { }

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

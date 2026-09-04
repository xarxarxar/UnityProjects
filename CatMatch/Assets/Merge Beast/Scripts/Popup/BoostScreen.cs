using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;
using System.Numerics;
using System;

namespace MergeBeast
{
    public class BoostScreen : BaseScreen
    {
        [Header("TAB")]
     //   [SerializeField] private List<Button> _btnTabs;
    //    [SerializeField] private List<GameObject> _panelTabs;

        [SerializeField] private UIButton _btnClose;
        [SerializeField] private GameObject _prfDailyBoost;

        //    [SerializeField] private Sprite _spriteActive;
        //    [SerializeField] private Sprite _spriteDisable;
        [SerializeField] private BoostChestItem[] arrChestObject;        
        private Dictionary<int, GameObject> _dictDilyBoost;
        private List<BoostChestData> listItemChest;
        private void Awake()
        {
            _dictDilyBoost = new Dictionary<int, GameObject>();
        }

        private void Start()
        {
            //for(int i = 0; i < _btnTabs.Count; i++)
            //{
            //    int n = i;
            //    _btnTabs[i]?.onClick.AddListener(() => this.OnClickTab(n));
            //}

            _btnClose?.onClick.AddListener(this.OnClickClose);

            
        }

        #region boost chest
        /// <summary>
        /// Setup boost chest, dua cac item o playerpref vao list
        /// </summary>
        private void SetUpBoostChest() {
            listItemChest = new List<BoostChestData>();
            //kiem tra storage xem co item chest nao ko
            string boostChest = PlayerPrefs.GetString(StringDefine.BOOST_CHEST, "");
            if (!string.IsNullOrEmpty(boostChest)) {
                //co data thi load ra
                string[] list = boostChest.Split(';');
                for (int i = 0; i < list.Length; i++) {
                    if (!string.IsNullOrEmpty(list[i])) {
                        string[] data = list[i].Split('-');
                        //khi split ra phai dung format thi moi parse, format x-y
                        BoostChestData bcd = new BoostChestData();
                        bcd.SetId(int.Parse(data[0]));
                        bcd.SetAmount(int.Parse(data[1]));
                        listItemChest.Add(bcd);
                    }
                }

            }

            //kiem tra neu tat ca cac item trong list co amount = 0 thi remove list
            int count = 0;
            for(int i = 0; i < listItemChest.Count; i++) {
                count += listItemChest[i].GetAmount();
            }
            if(count == 0) {
                listItemChest.Clear();
                PlayerPrefs.SetString(StringDefine.BOOST_CHEST, "");
            }
            

            ReloadBoostChestItem();

        }

        /// <summary>
        /// Update so luong item trong boostchest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        public void UpdateBoostChest(int id, int amount) {

            //kiem tra xem co id nay trong list chua
            if (CheckIdInListChest(id)) {
                for (int i = 0; i < listItemChest.Count; i++) {
                    BoostChestData bcd = listItemChest[i];
                    if (bcd.GetId() == id) {
                        //co id roi thi update amount
                        bcd.SetAmount(bcd.GetAmount() + amount);
                    }
                }
            } else {
                BoostChestData bcd = new BoostChestData();
                bcd.SetId(id);
                bcd.SetAmount(amount <= 0 ? 0 : amount);
                listItemChest.Add(bcd);
            }

            //save data
            string data = "";
            for(int i = 0; i < listItemChest.Count; i++) {
                BoostChestData bcd = listItemChest[i];
                data += $"{bcd.GetId()}-{bcd.GetAmount()};";
            }
            PlayerPrefs.SetString(StringDefine.BOOST_CHEST, data);
            
            ReloadBoostChestItem();

        }

        private void ReloadBoostChestItem() {
            for(int i = 0; i < listItemChest.Count; i++) {
                BoostChestData bcd = listItemChest[i];
                EnumDefine.BOOST boostType = (EnumDefine.BOOST)bcd.GetId();
                int boostIndex = ConvertBoostToIndex(boostType);                
                if(boostIndex >= 0) {
                    arrChestObject[boostIndex].gameObject.SetActive(true);
                    arrChestObject[boostIndex].SetInfo(boostType, bcd.GetAmount());
                }                
            }
        }

        private int ConvertBoostToIndex(EnumDefine.BOOST boost) {
            if (boost == EnumDefine.BOOST.DAMAGE_BOOST_1) return 0;
            if (boost == EnumDefine.BOOST.SPAWN_FASTER_1) return 1;
            if (boost == EnumDefine.BOOST.AUTO_MERGE_1) return 2;
            if (boost == EnumDefine.BOOST.SEVERAL_MERGE_MEDAL) return 3;
            return -1;
        }

        private bool CheckIdInListChest(int id) {            
            for (int i = 0; i < listItemChest.Count; i++) {
                if (id == listItemChest[i].GetId()) return true;
            }
            return false;
        }

        #endregion

        private void OnEnable()
        {
            SetUpBoostChest();
            
            for (int i = 0; i < 4; i++)
            {
                int sl = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + i);                
                if(sl <= 0)
                {
                    if (_dictDilyBoost.ContainsKey(i))
                    {
                        if (_dictDilyBoost[i] != null) Destroy(_dictDilyBoost[i]);
                        _dictDilyBoost.Remove(i);
                    }
                }
                else
                {
                    if (!_dictDilyBoost.ContainsKey(i))
                    {
                        //ko co thi tao moi
                        GameObject obj = Instantiate(_prfDailyBoost, _prfDailyBoost.transform.parent);
                        obj.transform.SetSiblingIndex(5);
                        obj.SetActive(true);
                        DailyBoost ctrl = obj.GetComponent<DailyBoost>();
                        ctrl.SetBoost(i, sl);
                        _dictDilyBoost.Add(i, obj);
                    } else {
                        //co roi thi update so luong
                        GameObject go = _dictDilyBoost[i];
                        if(go == null) {
                            go = Instantiate(_prfDailyBoost, _prfDailyBoost.transform.parent);
                            go.transform.SetSiblingIndex(5);
                            go.SetActive(true);
                            _dictDilyBoost[i] = go;
                        }
                        go.GetComponent<DailyBoost>().SetBoost(i, PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + i));
                    }
                }
            }

            
        }


        private void OnClickTab(int n)
        {
            //for(int i = 0; i < _panelTabs.Count; i++)
            //{
            //    _panelTabs[i].SetActive(i == n);
            //    _btnTabs[i].image.sprite = i == n ? _spriteActive : _spriteDisable;
            //}
        }

        private void OnClickClose()
        {
            ScreenManager.Instance.DeActiveScreen();
        }

        private void OnClickBuy3BoostFirst(EnumDefine.BOOST boost)
        {
            var bst = BoostManager.Instance.Boost(boost);
            if(Utils.GetCurrentRubyMoney() >= bst.Price)
            {
                Utils.AddRubyCoin(-bst.Price);
                BoostManager.Instance.AddBoost(boost);
                this.PostEvent(EventID.OnUpDateMoney);

            //    string date = DateTime.Now.ToString();
            //    PlayerPrefs.SetString($"{StringDefine.COUNT_DOWN_FREE_ADS}{(int)boost}", date);
            }
            else
            {
                ScreenManager.Instance.ShowConfirm(5, "你的宝石不足，是否要购买宝石？", () =>
                {
                    ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);
                });
            }
        }

        private void OnClickBuy2BoostLast(EnumDefine.BOOST boost)
        {
            if(boost == EnumDefine.BOOST.FEW_MERGE_MEDAL)
            {
                if (Utils.GetCurrentRubyMoney() >= 20)
                {
                    Utils.AddRubyCoin(-20);
                    Utils.AddMedalMerge(200, true);
                    this.PostEvent(EventID.OnUpdateAutoMergeMedal);
                    this.PostEvent(EventID.OnUpDateMoney);
                }
                else
                {
                    ScreenManager.Instance.ShowConfirm(5, "你的宝石不足，是否要购买宝石？", () =>
                    {
                        ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);
                    });
                }
            }
            else
            {
                if (Utils.GetCurrentRubyMoney() >= 25)
                {
                    Utils.AddRubyCoin(-25);
                    this.PostEvent(EventID.OnUpDateMoney);
                }
                else
                {
                    ScreenManager.Instance.ShowConfirm(5, "你的宝石不足，是否要购买宝石？", () =>
                    {
                        ScreenManager.Instance.ActiveScreen(EnumDefine.SCREEN.IAP);
                    });
                }
            }
        }


    }
}

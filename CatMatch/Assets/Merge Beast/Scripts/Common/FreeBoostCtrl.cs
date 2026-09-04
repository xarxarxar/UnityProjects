using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Numerics;
using System;
using Spine.Unity;

namespace MergeBeast {
    public class FreeBoostCtrl : MonoBehaviour {
        [SerializeField] private List<GroupBoostFree> groupBoost;
        [SerializeField] private FreeBoostConfig _config;
        [SerializeField] private List<Transform> _listPosition;
        [SerializeField] private GameObject[] _listBootsPos;
        [SerializeField] private UIButton _btnBoost;
        [SerializeField] private UIButton _btnBoost2;
        [SerializeField] private Image _boot1ICNormal;
        [SerializeField] private Image _boot1ICFlash;
        [SerializeField] private Image _boot2ICNormal;
        [SerializeField] private Image _boot2ICFlash;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtDescription;
        [SerializeField] private Image _imgIconBoost;
        [SerializeField] private Image _imgIcMaxboost;
        [SerializeField] private Image _imgLifeTime;
        [SerializeField] private Animator _animPopup;

        [SerializeField] private UIButton _btnCancelFree;
        [SerializeField] private UIButton _btnOkFree;
        [SerializeField] private UIButton _btnCancelAds;
        [SerializeField] private UIButton _btnOkAds;

        [SerializeField] private GameObject _fxBoost;

        [Header("Object Boost")]
        [SerializeField] private List<GameObject> _listBoostType;
        [SerializeField] private Text _txtValueDPS;
        [SerializeField] private Text _txtValueSoul;
        [SerializeField] private Text _txtValueGem;
        [SerializeField] private GameObject _pnTutorial;
        [SerializeField] private GameObject _tutFirstGame;

        [Header("Boost Spawn Time")]
        [SerializeField] private SkeletonGraphic _imgSpawnTimeBeast1;
        [SerializeField] private SkeletonGraphic _imgSpawnTimeBeast2;
        [SerializeField] private Text _txtSpawnTimeTime1;
        [SerializeField] private Text _txtSpawnTimeTime2;

        [Header("Boost Spawn Level")]
        [SerializeField] private SkeletonGraphic _imgSpawnLevelBeast1;
        [SerializeField] private SkeletonGraphic _imgSpawnLevelBeast2;
        [SerializeField] private Text _txtSpawnLevelLevel1;
        [SerializeField] private Text _txtSpawnLevelLevel2;

        [Header("Boost Merge Level")]
        [SerializeField] private SkeletonGraphic _imgMergeLevelBeast1;
        [SerializeField] private SkeletonGraphic _imgMergeLevelBeast2;
        [SerializeField] private SkeletonGraphic _imgMergeLevelBeast3;
        [SerializeField] private Text _txtMergeLevelLevl1;
        [SerializeField] private Text _txtMergeLevelLevl2;
        [SerializeField] private Text _txtMergeLevelLevl3;


        private Animator _myAnim;

        private float _timeCount;
        private float _timeFlash;
        private float _timeCountFlash;
        private float _timeCountBoost, _timeBoost;
        private bool _isActive, _hasTutorial;
        private BoostFree _currentBoost;
        private BoostFree _currentBoost1;
        private BoostFree _currentBoost2;
        private BoostFree _cacheBoost;

        private UnityAction _receiveBoost;
        private UnityAction _receivedMaxBoost;
        private UnityAction _normalBoost;

        private bool _stopCountBoost;

        private FreeBoostPopupAnim popupAnimScript;

        private ButtonBoost boostScript1, boostScript2;
        private int boostIndex; //1 : trai, 2: phai

        private bool needRefresh = true;

        // Use this for initialization
        void Start() {
            _btnCancelFree?.onClick.AddListener(OnClickCancelFree);
            _btnOkFree?.onClick.AddListener(OnClickOkFree);
            _btnCancelAds?.onClick.AddListener(OnClickCancelAds);
            _btnOkAds?.onClick.AddListener(OnClickOkAds);
            _btnBoost?.onClick.AddListener(() => ShowPopup(1));
            _btnBoost2?.onClick.AddListener(() => ShowPopup(2));

            _hasTutorial = PlayerPrefs.GetInt(StringDefine.HAS_TUTORIAL_FREEBOOST, 0) != 0;
            if (_hasTutorial) {
                Destroy(_pnTutorial);
            }
            popupAnimScript = _animPopup.GetComponent<FreeBoostPopupAnim>();
            boostScript1 = _btnBoost.GetComponent<ButtonBoost>();
            boostScript2 = _btnBoost2.GetComponent<ButtonBoost>();

            LoadPrevBoost();
        }

        void Update() {            
            if (_stopCountBoost) return;
            //if (_tutFirstGame != null) {
            //    if (_tutFirstGame.activeInHierarchy) return;
            //}
            _timeCount += Time.deltaTime;            
            if (_timeCount >= Config.TIME_BONUS_BOOST) {

                _timeCount = _timeCountBoost = 0f;
                _isActive = true;

                //kiem tra xem con slot ko                        
                bool hasSlotEmpty = false;
                for (int i = 0; i < _listBootsPos.Length; i++) {
                    if (!_listBootsPos[i].activeInHierarchy) {
                        _listBootsPos[i].SetActive(true);
                        hasSlotEmpty = true;
                        if (i == 0) {
                            _currentBoost1 = _config.GetRandomBoost();
                            while (_currentBoost1 == _currentBoost2 || _currentBoost1.FreeBoost == EnumDefine.FREEBOOST.MERGE_LEVEL) {
                                _currentBoost1 = _config.GetRandomBoost();
                            }
                            _boot1ICNormal.sprite = _currentBoost1.ImgNormal;
                            _boot1ICFlash.sprite = _currentBoost1.ImgFlash;
                            _currentBoost = _currentBoost1;
                            _btnBoost.GetComponent<ButtonBoost>().SetLifeTime(_currentBoost1.TimeFreeBoost);

                            BoostFreeData data = new BoostFreeData();
                            data.SetId((int)_currentBoost1.FreeBoost);
                            data.SetTimeUp(TimeUtil.TimeStampSecond + _currentBoost1.TimeFreeBoost);

                            string dataSave = $"{data.GetId()}-{data.GetValue()}-{data.GetRemainTime()}-{data.GetTimeUp()}-{data.GetTimeUpUse()}";                            
                            PlayerPrefs.SetString(StringDefine.BOOST_FREE_SLOT1, dataSave);


                        } else if (i == 1) {
                            _currentBoost2 = _config.GetRandomBoost();
                            while (_currentBoost2 == _currentBoost1 || _currentBoost2.FreeBoost == EnumDefine.FREEBOOST.MERGE_LEVEL) {
                                _currentBoost2 = _config.GetRandomBoost();
                            }
                            _boot2ICNormal.sprite = _currentBoost2.ImgNormal;
                            _boot2ICFlash.sprite = _currentBoost2.ImgFlash;
                            _currentBoost = _currentBoost2;
                            _btnBoost2.GetComponent<ButtonBoost>().SetLifeTime(_currentBoost2.TimeFreeBoost);

                            BoostFreeData data = new BoostFreeData();
                            data.SetId((int)_currentBoost2.FreeBoost);
                            data.SetTimeUp(TimeUtil.TimeStampSecond + _currentBoost2.TimeFreeBoost);

                            string dataSave = $"{data.GetId()}-{data.GetValue()}-{data.GetRemainTime()}-{data.GetTimeUp()}-{data.GetTimeUpUse()}";
                            PlayerPrefs.SetString(StringDefine.BOOST_FREE_SLOT2, dataSave);

                        }
                        break;
                    }
                }

                //ca 2 slot deu co boost
                if (!hasSlotEmpty) {
                    return;
                }


                if (!_hasTutorial) {
                    if (_tutFirstGame != null && _tutFirstGame.activeInHierarchy) return;
                    //_pnTutorial.SetActive(true);
                    _hasTutorial = true;
                    PlayerPrefs.SetInt(StringDefine.HAS_TUTORIAL_FREEBOOST, 1);
                }
            }
        }

        /// <summary>
        /// Active slot free boost neu con thoi gian
        /// </summary>
        /// <param name="slotId">1 hoac 2</param>
        /// <param name="data"></param>
        private void ActiveSlot(int slotId, string data) {
            

            string[] list = data.Split('-');
            if (list.Length < 5) return;

            var id = int.Parse(list[0]);
            var value = int.Parse(list[1]);
            var remainTime = long.Parse(list[2]);
            var timeUp = long.Parse(list[3]);            
            if (timeUp <= TimeUtil.TimeStampSecond) return;
            _listBootsPos[slotId - 1].SetActive(true);
            //long timeConlai = timeUp - TimeUtil.TimeStampSecond;
            //long timeRun = remainTime - timeConlai;

            if (slotId == 1) {
                _currentBoost1 = _config.GetBoost(id);
                _boot1ICNormal.sprite = _currentBoost1.ImgNormal;
                _boot1ICFlash.sprite = _currentBoost1.ImgFlash;
                _currentBoost = _currentBoost1;

                long timeConlai = timeUp - TimeUtil.TimeStampSecond;
                long timeRun = _currentBoost1.TimeFreeBoost - timeConlai;

                _btnBoost.GetComponent<ButtonBoost>().SetLifeTime(_currentBoost1.TimeFreeBoost);
                _btnBoost.GetComponent<ButtonBoost>().SetTimeCount((int)timeRun);
            } else {
                _currentBoost2 = _config.GetBoost(id);
                _boot2ICNormal.sprite = _currentBoost2.ImgNormal;
                _boot2ICFlash.sprite = _currentBoost2.ImgFlash;
                _currentBoost = _currentBoost2;

                long timeConlai = timeUp - TimeUtil.TimeStampSecond;
                long timeRun = _currentBoost2.TimeFreeBoost - timeConlai;

                _btnBoost2.GetComponent<ButtonBoost>().SetLifeTime(_currentBoost2.TimeFreeBoost);
                _btnBoost2.GetComponent<ButtonBoost>().SetTimeCount((int)timeRun);
            }
        }

        private void LoadPrevBoost() {
            string slot1 = PlayerPrefs.GetString(StringDefine.BOOST_FREE_SLOT1, "");
            string slot2 = PlayerPrefs.GetString(StringDefine.BOOST_FREE_SLOT2, "");

            if (!string.IsNullOrEmpty(slot1)) {
                //co boost => tao ra boost
                ActiveSlot(1, slot1);
            }
            if (!string.IsNullOrEmpty(slot2)) {
                ActiveSlot(2, slot2);
            }
        }

        private void ShowPopup(int btnIndex) {
            boostIndex = btnIndex;
            if (boostIndex == 1) {
                //button boost 1
                _currentBoost = _currentBoost1;
                boostScript1.AllowCountDown(false);
            } else {
                //button boost 2
                _currentBoost = _currentBoost2;
                boostScript2.AllowCountDown(false);
            }
            CheckCurrentBoost();
            _animPopup.gameObject.SetActive(true);
            _animPopup.SetTrigger("freeboost");
            _stopCountBoost = true;

            //if (_currentBoost.FreeBoost == EnumDefine.FREEBOOST.SOUL)
            //    this.SetSoulBoost();
        }

        private void CheckCurrentBoost() {
            _timeFlash = UnityEngine.Random.Range(3.5f, 5f);

            _imgIconBoost.sprite = _currentBoost.ImgIcon;
            _imgIcMaxboost.sprite = _currentBoost.ImgIconMaxBoost;
            //_boot1ICNormal.SetNativeSize();
            //_boot1ICFlash.SetNativeSize();
            _imgIconBoost.SetNativeSize();
            //_timeBoost = _currentBoost.TimeFreeBoost;
            //_cacheBoost = _currentBoost;
            _imgIcMaxboost.SetNativeSize();
            int lvBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            _txtTitle.text = _currentBoost.Name;

            switch (_currentBoost.FreeBoost) {
                case EnumDefine.FREEBOOST.WOOD_BOX:
                SetWoodBoxBoost();
                break;
                case EnumDefine.FREEBOOST.SOUL:
                this.SetSoulBoost();
                break;
                case EnumDefine.FREEBOOST.MERGE_LEVEL:
                break;
                case EnumDefine.FREEBOOST.SPAWN_LEVEL:
                SetSpawnLevelBoost(lvBeast);
                break;
                case EnumDefine.FREEBOOST.SPAWN_TIME:
                SetSpawnTimeBoost(lvBeast);
                break;
                case EnumDefine.FREEBOOST.DPS:
                SetDPSBoost();
                break;
            }
        }

        private void OnClickCancelFree() {
            _stopCountBoost = false;
            _animPopup.gameObject.SetActive(false);
            needRefresh = false;
            //khi dong popup thi lai dem tiep
            AllowCountDownWhenClose();
        }

        private void OnClickOkFree() {
            _stopCountBoost = false;
            DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.FreeBoost, EnumDefine.Mission.ActiveFreeBoost);
            _receiveBoost?.Invoke();
            _normalBoost?.Invoke(); //khi click vao ok free thi cho nhan normal, khi xem dc quang cao thi x2
            HideBoostWhenClickOk();
            needRefresh = true;
        }

        private void OnClickCancelAds() {
            //_normalBoost?.Invoke();
            _animPopup.gameObject.SetActive(false);
            _stopCountBoost = false;
            AllowCountDownWhenClose();
            needRefresh = true;
        }

        private void OnClickOkAds() {
            _animPopup.gameObject.SetActive(false);
            _stopCountBoost = false;
            _receivedMaxBoost?.Invoke();
            HideBoostWhenClickOk();
            needRefresh = true;
        }

        private void AllowCountDownWhenClose() {
            if (boostIndex == 1) {
                boostScript1.AllowCountDown(true);
            } else {
                boostScript2.AllowCountDown(true);
            }
        }

        private void HideBoostWhenClickOk() {
            _listBootsPos[boostIndex - 1].SetActive(false);
        }

        public void DelayAction(System.Action cb) {
            //if (cb != null) cb();
        }


        // Update is called once per frame


        private int Rarity() {
            return UnityEngine.Random.Range(0, 100);
        }

        private void ActiveBoost(int n) {
            for (int i = 0; i < _listBoostType.Count; i++)
                _listBoostType[i].SetActive(i == n);
        }

        #region SET BOOST

        private void SetSoulBoost() {
            this.ActiveBoost(1);
            BigInteger soul = GameManager.Instance.TotalDamage() * 60 * 5; //5 phut
            _txtDescription.text = $"获得<color=#22E731>{Utils.FormatNumber(soul)}</color>灵魂";
            _txtValueSoul.text = $"+{Utils.FormatNumber(soul)}";

            string data = PlayerPrefs.GetString(boostIndex == 1 ? StringDefine.BOOST_FREE_SLOT1 : StringDefine.BOOST_FREE_SLOT2, "");
            BoostFreeData bfd = new BoostFreeData();
            bfd.ParseData(data);

            if (bfd.GetValue() != 0) {
                //data ok
                soul = bfd.GetValue();

                bfd.SaveData(boostIndex);
            } else {
                bfd.SetValue(soul);
                bfd.SaveData(boostIndex);
            }

            _normalBoost = () => {
                UIManager.Instance.UpdateMoneyCoin(soul, false);

                bfd.SetTimeUp(TimeUtil.TimeStampSecond - 100);
                bfd.SaveData(boostIndex);
            };

            _receiveBoost = () => {
                if (this.Rarity() < 50 && AdsManager.Instance.IsLoaded()) {
                    BigInteger maxsoul = GameManager.Instance.TotalDamage() * 60 * 25; //25 p, tong la 30p
                    BigInteger soulView = GameManager.Instance.TotalDamage() * 60 * 30;
                    _txtDescription.text = $"看视频获得<color=#22E731>{Utils.FormatNumber(soulView)}</color>灵魂";
                    _txtValueSoul.text = $"+{Utils.FormatNumber(soulView)}";
                    _animPopup.SetTrigger("maxboost");
                    _stopCountBoost = true;

                    _receivedMaxBoost = () => {
                        AdsManager.Instance.ShowAds(() => {
                            UIManager.Instance.UpdateMoneyCoin(maxsoul, false);
                        }, StringDefine.ADS_FREEBOOST);
                    };
                } else {
                    //_normalBoost?.Invoke();
                    _animPopup.gameObject.SetActive(false);
                }
            };
        }

        private void SetDPSBoost() {
            int hs = 0;
            int tm = 0;
            int xacsuat = this.Rarity();
            if (xacsuat < 80) {
                hs = 2;
                tm = 60;
            } else  {
                hs = 3;
                tm = 30;
            }


            string data = PlayerPrefs.GetString(boostIndex == 1 ? StringDefine.BOOST_FREE_SLOT1 : StringDefine.BOOST_FREE_SLOT2, "");
            BoostFreeData bfd = new BoostFreeData();
            bfd.ParseData(data);

            if (bfd.GetValue() != 0) {
                //data ok
                hs = (int) bfd.GetValue();                                
            } else {
                bfd.SetValue(hs);                
            }

            if(bfd.GetRemainTime() != 0) {
                tm = bfd.GetRemainTime();
            } else {
                bfd.SetRemainTime(tm);
            }
            bfd.SaveData(boostIndex);

            this.ActiveBoost(0);
            _txtDescription.text = $"所有魔兽造成<color=#22E731>{hs}</color>倍伤害，持续<color=#22E731>{tm}</color>秒";
            _txtValueDPS.text = $"每秒×{hs}";

            _normalBoost = () => {
                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.DPS, tm, hs);

                bfd.SetTimeUp(TimeUtil.TimeStampSecond - 100);
                bfd.SetTimeUpUse(TimeUtil.TimeStampSecond + bfd.GetRemainTime());
                bfd.SaveData(boostIndex);                
            };

            _receiveBoost = () => {
                if (this.Rarity() < 30 && AdsManager.Instance.IsLoaded()) {

                    _txtDescription.text = $"看视频使所有魔兽伤害提升<color=#22E731>3倍</color>，持续<color=#22E731>60</color>秒";
                    _txtValueDPS.text = "每秒×3";
                    _animPopup.SetTrigger("maxboost");
                    _stopCountBoost = true;

                    _receivedMaxBoost = () => {
                        AdsManager.Instance.ShowAds(() => {
                            BoostManager.Instance.ForceStopDPSFree(() => {
                                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.DPS, 60, 3);
                                bfd.SetValue(3);
                                bfd.SetRemainTime(60);
                                bfd.SetTimeUpUse(TimeUtil.TimeStampSecond + 60);
                                bfd.SaveData(boostIndex);
                            });                           
                        }, StringDefine.ADS_FREEBOOST);
                    };
                } else {
                    //_normalBoost?.Invoke();
                    _animPopup.gameObject.SetActive(false);
                }
            };
        }

        private void SetSpawnTimeBoost(int lvBeast) {
            int heso = 0;
            int time = 0;
            int rari = this.Rarity();
            if (rari > 60) {
                heso = 50;
                time = 45;
            } else if (rari > 30) {
                heso = 65;
                time = 25;
            } else {
                heso = 75;
                time = 10;
            }

            string data = PlayerPrefs.GetString(boostIndex == 1 ? StringDefine.BOOST_FREE_SLOT1 : StringDefine.BOOST_FREE_SLOT2, "");
            BoostFreeData bfd = new BoostFreeData();
            bfd.ParseData(data);

            if (bfd.GetValue() != 0) {
                //data ok
                heso = (int) bfd.GetValue();
            } else {
                bfd.SetValue(heso);
            }

            if (bfd.GetRemainTime() != 0) {
                time = bfd.GetRemainTime();
            } else {
                bfd.SetRemainTime(time);
            }
            bfd.SaveData(boostIndex);

            this.ActiveBoost(5);
            _txtDescription.text = $"新猫生成速度提升<color=#22E731>{heso}%</color>，持续<color=#22E731>{time}</color>秒";

            BeastData dt = GameManager.Instance.GetBeast(lvBeast);
            this.OnSetCatSkin(_imgSpawnTimeBeast2, dt.Level.ToString("D3"));
            this.OnSetCatSkin(_imgSpawnTimeBeast1, dt.Level.ToString("D3"));

            int lvSpam = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
            float tspam = 6f - lvSpam * 0.1f;
            _txtSpawnTimeTime1.text = $"{tspam}s";
            _txtSpawnTimeTime2.text = $"{System.Math.Round(tspam * heso / 100f, 2)}s";

            _normalBoost = () => {
                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_TIME, time, heso);

                bfd.SetTimeUp(TimeUtil.TimeStampSecond - 100);
                bfd.SetTimeUpUse(TimeUtil.TimeStampSecond + bfd.GetRemainTime());
                bfd.SaveData(boostIndex);
            };

            _receiveBoost = () => {
                if (this.Rarity() < 30 && AdsManager.Instance.IsLoaded()) {
                    _txtDescription.text = $"看视频使猫生成速度提升<color=#22E731>75%</color>，持续<color=#22E731>60</color>秒";
                    _txtSpawnTimeTime2.text = $"{System.Math.Round(tspam * 0.5, 2)}s";
                    _animPopup.SetTrigger("maxboost");
                    _stopCountBoost = true;

                    _receivedMaxBoost = () => {
                        AdsManager.Instance.ShowAds(() => {
                            BoostManager.Instance.ForceStopBoostFree(EnumDefine.FREEBOOST.SPAWN_TIME, () => {
                                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_TIME, 60, 75);
                                bfd.SetValue(75);
                                bfd.SetRemainTime(50);
                                bfd.SetTimeUpUse(TimeUtil.TimeStampSecond + 50);
                                bfd.SaveData(boostIndex);
                            });
                            
                        }, StringDefine.ADS_FREEBOOST);
                    };
                } else {
                    //_normalBoost?.Invoke();
                    _animPopup.gameObject.SetActive(false);
                }
            };
        }

        private void SetSpawnLevelBoost(int lvBeast) {
            this.ActiveBoost(4);
            BeastData be = GameManager.Instance.GetBeast(lvBeast);
            this.OnSetCatSkin(_imgSpawnLevelBeast1, be.Level.ToString("D3"));
            _txtSpawnLevelLevel1.text = $"等级{be.Level}";

            int random = UnityEngine.Random.Range(0, 2);
            int lv;
            int time;
            if(random == 0) {
                lv = 1;
                time = 15;
            } else {
                lv = 2;
                time = 10;
            }

            string data = PlayerPrefs.GetString(boostIndex == 1 ? StringDefine.BOOST_FREE_SLOT1 : StringDefine.BOOST_FREE_SLOT2, "");
            BoostFreeData bfd = new BoostFreeData();
            bfd.ParseData(data);

            if (bfd.GetValue() != 0) {
                //data ok
                lv = (int) bfd.GetValue();
            } else {
                bfd.SetValue(lv);
            }

            if (bfd.GetRemainTime() != 0) {
                time = bfd.GetRemainTime();
            } else {
                bfd.SetRemainTime(time);
            }
            bfd.SaveData(boostIndex);


            _txtDescription.text = $"新猫生成等级提升<color=#22E731>{lv}</color>级，持续<color=#22E731>{time}</color>秒";

            BeastData be2 = GameManager.Instance.GetBeast(lvBeast + lv);
            this.OnSetCatSkin(_imgSpawnLevelBeast2, be2.Level.ToString("D3"));
            _txtSpawnLevelLevel2.text = $"等级{be2.Level}";

            _normalBoost = () => {
                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_LEVEL, time, lv);

                bfd.SetTimeUp(TimeUtil.TimeStampSecond - 100);
                bfd.SetTimeUpUse(TimeUtil.TimeStampSecond + bfd.GetRemainTime());
                bfd.SaveData(boostIndex);
            };

            _receiveBoost = () => {
                if (this.Rarity() < 20 && AdsManager.Instance.IsLoaded()) {
                    _txtDescription.text = $"看视频使生成等级提升<color=#22E731>2</color>级，持续<color=#22E731>60</color>秒";
                    _animPopup.SetTrigger("maxboost");
                    _stopCountBoost = true;

                    BeastData bemax = GameManager.Instance.GetBeast(lvBeast + 2);
                    this.OnSetCatSkin(_imgSpawnLevelBeast2, bemax.Level.ToString("D3"));
                    _txtSpawnLevelLevel2.text = $"等级{bemax.Level}";

                    _receivedMaxBoost = () => {
                        AdsManager.Instance.ShowAds(() => {
                            BoostManager.Instance.ForceStopBoostFree(EnumDefine.FREEBOOST.SPAWN_LEVEL, () => {
                                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_LEVEL, 60, 2);
                                bfd.SetValue(2);
                                bfd.SetRemainTime(60);
                                bfd.SetTimeUpUse(TimeUtil.TimeStampSecond + 60);
                                bfd.SaveData(boostIndex);
                            });                            
                        }, StringDefine.ADS_FREEBOOST);
                    };
                } else {
                    //_normalBoost?.Invoke();
                    _animPopup.gameObject.SetActive(false);
                }
            };
            
        }

        private void OnSetCatSkin(SkeletonGraphic skel, string skin)
        {
            StartCoroutine(IESetCat(skel, skin));
        }

        private IEnumerator IESetCat(SkeletonGraphic skel,string skin)
        {
            yield return new WaitForEndOfFrame();
            skel.Skeleton.SetSkin(skin);
        }

        private void SetMergeLevelBoost(int lvBeast) {
            this.ActiveBoost(3);
            BeastData me = GameManager.Instance.GetBeast(lvBeast);
            this.OnSetCatSkin(_imgMergeLevelBeast1, me.Level.ToString("D3"));
            this.OnSetCatSkin(_imgMergeLevelBeast2, me.Level.ToString("D3"));
            _txtMergeLevelLevl1.text = _txtMergeLevelLevl2.text = $"等级{me.Level}";

            if (this.Rarity() > 50) {
                _txtDescription.text = $"猫合成等级提升<color=#22E731>2</color>级，持续<color=#22E731>15</color>秒";
                BeastData me2 = GameManager.Instance.GetBeast(lvBeast + 2);
                this.OnSetCatSkin(_imgMergeLevelBeast3, me2.Level.ToString("D3"));
                _txtMergeLevelLevl3.text = $"等级{me2.Level}";

                _normalBoost = () => {
                    BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.MERGE_LEVEL, 15, 2);
                };

                _receiveBoost = () => {
                    if (this.Rarity() < 20 && AdsManager.Instance.IsLoaded()) {
                        _txtDescription.text = $"看视频使合成等级提升<color=#22E731>2</color>级，持续<color=#22E731>60</color>秒";
                        _animPopup.SetTrigger("maxboost");
                        _stopCountBoost = true;

                        _receivedMaxBoost = () => {
                            AdsManager.Instance.ShowAds(() => {
                                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.MERGE_LEVEL, 60, 2);
                            }, StringDefine.ADS_FREEBOOST);
                        };
                    } else {
                        _normalBoost?.Invoke();
                        _animPopup.gameObject.SetActive(false);
                    }
                };
            } else {
                _txtDescription.text = $"猫合成等级提升<color=#22E731>3</color>级，持续<color=#22E731>10</color>秒";
                BeastData me3 = GameManager.Instance.GetBeast(lvBeast + 3);
                this.OnSetCatSkin(_imgMergeLevelBeast3, me3.Level.ToString("D3"));
                _txtMergeLevelLevl3.text = $"等级{me3.Level}";

                _normalBoost = () => {
                    BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.MERGE_LEVEL, 10, 3);
                };

                _receiveBoost = () => {
                    if (this.Rarity() < 10 && AdsManager.Instance.IsLoaded()) {
                        _txtDescription.text = $"看视频使合成等级提升<color=#22E731>3</color>级，持续<color=#22E731>30</color>秒";
                        _animPopup.SetTrigger("maxboost");
                        _stopCountBoost = true;

                        _receivedMaxBoost = () => {
                            AdsManager.Instance.ShowAds(() => {
                                BoostManager.Instance.AddFreeBoost(EnumDefine.FREEBOOST.MERGE_LEVEL, 30, 3);
                            }, StringDefine.ADS_FREEBOOST);
                        };
                    } else {
                        _normalBoost?.Invoke();
                        _animPopup.gameObject.SetActive(false);
                    }
                };
            }
        }

        private void SetWoodBoxBoost() {
            int rarity = this.Rarity();
            this.ActiveBoost(2);

            //Sau nay muon co nhieu item, thi data cua woodbox nen lưu riêng ra 1 config khác
            //data chung thì vẫn chỉ lưu đến woodbox, khi mở woodbox thì lưu thêm 1 data mới
            BigInteger gem = 1;
            if (rarity > 98) gem = 5;
            else if (rarity > 94) gem = 4;
            else if (rarity > 80) gem = 3;
            else if (rarity > 70) gem = 2;

            //neu chua co value thi set value, co roi thi lay ra

            string data = PlayerPrefs.GetString(boostIndex == 1 ? StringDefine.BOOST_FREE_SLOT1 : StringDefine.BOOST_FREE_SLOT2, "");
            BoostFreeData bfd = new BoostFreeData();
            bfd.ParseData(data);

            if (bfd.GetValue() != 0) {
                //data ok
                gem = bfd.GetValue();

                bfd.SaveData(boostIndex);
            } else {
                bfd.SetValue(gem);
                bfd.SaveData(boostIndex);
            }

            _txtDescription.text = $"获得<color=#22E731>{gem}</color>宝石";
            _txtValueGem.text = $"+{gem}";
            _txtValueGem.transform.GetChild(1).gameObject.SetActive(false);

            _normalBoost = () => {
                BoostManager.Instance.OnBoostAddGem((int)gem);

                bfd.SetTimeUp(TimeUtil.TimeStampSecond - 100);
                bfd.SaveData(boostIndex);

            };

            _receiveBoost = () => {
                if (this.Rarity() < 50 && AdsManager.Instance.IsLoaded()) {
                    _txtDescription.text = "看视频获得<color=#22E731>2倍</color>宝石";
                    _txtValueGem.text = $"+{gem * 2}";
                    _animPopup.SetTrigger("maxboost");
                    _stopCountBoost = true;

                    _receivedMaxBoost = () => {
                        AdsManager.Instance.ShowAds(() => {
                            BoostManager.Instance.OnBoostAddGem((int)gem);
                        }, StringDefine.ADS_FREEBOOST);
                    };
                } else {
                    //_normalBoost?.Invoke();
                    _animPopup.gameObject.SetActive(false);
                }
            };

        }

        private void MedalMerge() {
            int rarity = 0;
            if (rarity > 50) {
                _txtDescription.text = "获得<color=#22E731>20</color>合成勋章";
                _txtValueGem.text = "+20";
                _txtValueGem.transform.GetChild(1).gameObject.SetActive(true);

                _normalBoost = () => {
                    BoostManager.Instance.OnBoostMedalMerge(20);
                };

                _receiveBoost = () => {
                    if (this.Rarity() < 50 && AdsManager.Instance.IsLoaded()) {
                        _txtDescription.text = "看视频获得<color=#22E731>2倍</color>合成勋章";
                        _txtValueGem.text = "+40";
                        _animPopup.SetTrigger("maxboost");

                        _receivedMaxBoost = () => {
                            AdsManager.Instance.ShowAds(() => {
                                //BoostManager.Instance.OnBoostMedalMerge(20);   
                                _normalBoost?.Invoke();
                            }, StringDefine.ADS_FREEBOOST);
                        };
                    } else {
                        //_normalBoost?.Invoke();
                        _animPopup.gameObject.SetActive(false);
                    }
                };
            }
        }
        #endregion

    } //end class


    [System.Serializable]
    public class GroupBoostFree {
        public EnumDefine.FREEBOOST GroupName;
        public List<BoostFree> listBoost;
    }

    [System.Serializable]
    public class BoostFreeData {
        private int id;
        private BigInteger value; //x dps, Giamr % sinh beast, so gem
        private int remainTime; //hieu luc cua boost
        private long timeup; //thoi gian show o lootbox
        private long timeUpUse; //thoi gian dc set neu su dung item

        public BoostFreeData() { }

        public void SetId(int id) {
            this.id = id;
        }
        public int GetId() {
            return id;
        }

        public void SetValue(BigInteger value) {
            this.value = value;
        }
        public BigInteger GetValue() {
            return value;
        }

        public void SetRemainTime(int time) {
            remainTime = time;
        }
        public int GetRemainTime() {
            return remainTime;
        }

        public void SetTimeUp(long time) {
            timeup = time;
        }
        public long GetTimeUp() {
            return timeup;
        }

        public void SetTimeUpUse(long time) {
            timeUpUse = time;
        }
        public long GetTimeUpUse() {
            return timeUpUse;
        }

        public void ParseData(string data) {
            string[] list = data.Split('-');
            if (list.Length < 5) return;
            SetId(int.Parse(list[0]));
            SetValue(int.Parse(list[1]));
            SetRemainTime(int.Parse(list[2]));
            SetTimeUp(long.Parse(list[3]));
            SetTimeUpUse(long.Parse(list[4]));
            
        }

        public void SaveData(int boostIndex) {
            string dataSave = $"{GetId()}-{GetValue()}-{GetRemainTime()}-{GetTimeUp()}-{GetTimeUpUse()}";
            PlayerPrefs.SetString(boostIndex == 1 ? StringDefine.BOOST_FREE_SLOT1 : StringDefine.BOOST_FREE_SLOT2, dataSave);
        }

    }
}

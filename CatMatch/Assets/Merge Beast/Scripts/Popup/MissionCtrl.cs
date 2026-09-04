using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using UnityEngine.UI;
using System.Linq;

namespace MergeBeast {
    public class MissionCtrl : MonoBehaviour {

        [SerializeField] private DailyQuestConfig _config;
        [SerializeField] private DailyQuestItem _prfMission;

        [SerializeField] private Animator _animChucMung;
        [SerializeField] private GameObject _objNotice;
        [SerializeField] private Animator _animNoticeClaim;
        [SerializeField] private Text _txtTitleClaim;
        [SerializeField] private Text _txtQuantity;
        [SerializeField] private GameObject _pnDaily;
        [SerializeField] private GameObject _pnAchivement;
        [SerializeField] private Sprite _btnDeactive;
        [SerializeField] private Sprite _btnActive;
        [SerializeField] private GameObject _noticeAchivement;
        [SerializeField] private GameObject _noticeDaily;

        private List<ProgressMission> _allMission;
        private List<DailyQuestItem> _listMission;


        // Start is called before the first frame update
        void Start() {
            this.InitMission();

        }

        private void InitMission() {
            _allMission = new List<ProgressMission>();
            _listMission = new List<DailyQuestItem>();
            string data = PlayerPrefs.GetString("mission-status");
            //Debug.LogError("mission: " + data);
            if (string.IsNullOrEmpty(data)) {
                string str = string.Empty;               
                for (int i = 0; i < _config.ListMission.Count; i++) {

                    List<int> listTarget = _config.ListMission[i].Targets;
                    ProgressMission ctrl = new ProgressMission() {
                        Total = _config.ListMission[i].Targets[0],
                        Description = string.Format(_config.ListMission[i].Description, $"<color=#24F3FF>{_config.ListMission[i].Targets[0]}</color>"),
                        MissionType = _config.ListMission[i].Type,
                        IconQuest = _config.ListMission[i].IconMission,
                        MaxValue = listTarget[listTarget.Count - 1],                        
                        
                    };
                    
                    if (ctrl.MissionType == EnumDefine.Mission.UpgradeLevelSpawn)
                        ctrl.Percent = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 1);
                    _allMission.Add(ctrl);
                    str += $"{(int)ctrl.MissionType}.{ctrl.Total}.0-";
                }
                PlayerPrefs.SetString("mission-status", str);

            } else {
                string[] arr = data.Split('-');
                for (int i = 0; i < arr.Length; i++) {
                    if (!string.IsNullOrEmpty(arr[i])) {
                        string[] detail = arr[i].Split('.');

                        int type = int.Parse(detail[0]);
                        int total = int.Parse(detail[1]);
                        int per = int.Parse(detail[2]);
                        int hasReceive = 0;
                        if (detail.Length > 3) {
                            hasReceive = int.Parse(detail[3]);
                        }
                        


                        MissionType mission = this.GetMission((EnumDefine.Mission)type);
                        if (mission != null) {
                            List<int> listTarget = _config.ListMission[(int)mission.Type].Targets;
                            ProgressMission ctrl = new ProgressMission() {
                                Total = total,
                                Percent = per,
                                MissionType = mission.Type,
                                Description = string.Format(mission.Description, $"<color=#24F3FF>{total}</color>"),
                                IconQuest = mission.IconMission,
                                MaxValue = listTarget[listTarget.Count - 1],
                                HasReceived = hasReceive,
                            };                            
                            _allMission.Add(ctrl);

                            if (per >= total && hasReceive == 0) {
                                _objNotice.SetActive(true);
                                _noticeAchivement.SetActive(true);
                                UIManager.Instance.optionCtl.CheckShowNotice();
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 15; i++) {
                DailyQuestItem ctrl = Instantiate(_prfMission, _prfMission.transform.parent) as DailyQuestItem;
                _listMission.Add(ctrl);
            }
        }

        private MissionType GetMission(EnumDefine.Mission mis) {
            return _config.ListMission.Find(x => x.Type == mis);
        }

        public void UpdateMission(EnumDefine.Mission mis) {

            if (_allMission == null) return;            
            for (int i = 0; i < _allMission.Count; i++) {                
                if (_allMission[i].MissionType == mis) {
                    //check truoc 1 level, neu level + 1 > max thi hien tai dang la max roi
                    if (_allMission[i].Percent + 1 > _allMission[i].MaxValue) {
                        //het nhiem vu
                        return;
                    }
                    switch (mis) {
                        case EnumDefine.Mission.RaidBoss:
                        _allMission[i].Percent = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER, 0) + 1;
                        break;
                        case EnumDefine.Mission.UnlockBeast:
                        _allMission[i].Percent = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST, 0) + 1;
                        break;
                        case EnumDefine.Mission.UpgradeLevelSpawn:
                        int level = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 1) + 1;                        
                        _allMission[i].Percent = level;
                        break;
                        case EnumDefine.Mission.UpgradeTimeSpawn:
                        _allMission[i].Percent = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
                        break;
                        default:                        
                        _allMission[i].Percent++;
                        break;
                    }

                    //if (_pnAchivement.activeInHierarchy) {
                    //    this.ShowMission();
                    //}               
                    
                    if (_allMission[i].Percent >= _allMission[i].Total && _allMission[i].HasNotice == 0) {                        
                        _txtTitleClaim.text = _allMission[i].Description + "完成";
                        _animNoticeClaim.Play("noticeQuest-Show");
                        _allMission[i].HasNotice = 1;


                        _objNotice.SetActive(true);
                        _noticeAchivement.SetActive(true);
                        UIManager.Instance.optionCtl.CheckShowNotice();

                    }
                    break;
                }
            }
            //CheckNotice();
        }

        private bool IsShowNoticeMission() {
            foreach (var item in _allMission) {
                if (item.Percent >= item.Total && item.HasReceived == 0) {                    
                    return true;
                }
            }
            return false;
        }

        public void ShowMission() {
            Debug.Log("ShowMission");
            foreach (var mis in _allMission) {
                if (mis.Percent >= mis.Total) mis.order = 1;
                if (mis.Percent >= mis.MaxValue && mis.HasReceived == 1) mis.order = -1;
            }
            _allMission = _allMission.OrderByDescending(c => c.order).ToList();

            for (int i = 0; i < _listMission.Count; i++) {
                _listMission[i].gameObject.SetActive(i < _allMission.Count);
                
                if (i < _allMission.Count) {
   

                    float per = (float)_allMission[i].Percent / _allMission[i].Total;
                    _listMission[i].SlidePercent.fillAmount = Mathf.Clamp01(per);
                    _listMission[i].Descriptio.text = _allMission[i].Description;
                    _listMission[i].IconQuest.sprite = _allMission[i].IconQuest;
                    _listMission[i].IconQuest.SetNativeSize();
                    _listMission[i].BtnGet.onClick.RemoveAllListeners();

                    ProgressMission mis = _allMission[i];
                    DailyQuestItem item = _listMission[i];

                    int gemReward = GetMilestone(mis) * 10;
                    item.BtnGet.GetComponentInChildren<Text>().text = $"领取\n{gemReward}";

                    Debug.LogError(_allMission[i].MissionType + ", percen: " + _allMission[i].Percent + " total: " + _allMission[i].Total + ", max" + _allMission[i].MaxValue);

                    if (_allMission[i].Percent >= _allMission[i].Total) {
                        _listMission[i].TxtPercent.text = $"{_allMission[i].Total}/{_allMission[i].Total}";
                        //Debug.LogError("percent: " + mis.Percent + ", max: " + mis.MaxValue + ", total" + mis.Total);
                        //check neu dat den lv max cua mission roi thi thoi ko cho nhan phan thuong nua
                        if(_allMission[i].Percent >= _allMission[i].MaxValue && mis.Total == mis.MaxValue) {
                            //het nhiem vu
                            if(_allMission[i].HasReceived == 0) {
                                //chua nhan thuong
                                mis.HasNotice = 1;
                                item.Done.SetActive(false);
                                item.BtnGet.gameObject.SetActive(true);
                                item.BtnGet.image.sprite = _btnActive;                                
                                item.BtnGet?.onClick.AddListener(() => this.ShowReward(mis, item));
                            } else {
                                mis.HasNotice = 0;
                                item.BtnGet.image.sprite = _btnDeactive;
                                item.SlidePercent.fillAmount = 0f;
                                item.Done.SetActive(true);
                                item.BtnGet.gameObject.SetActive(false);
                            }
                        } else {
                            //van con nhiem vu
                            mis.HasNotice = 1;
                            mis.HasReceived = 0;
                            item.BtnGet.image.sprite = _btnActive;
                            item.BtnGet?.onClick.AddListener(() => this.ShowReward(mis, item));
                            
                        }
                    } else {
                        item.Done.SetActive(false);
                        item.BtnGet.gameObject.SetActive(true);
                        item.BtnGet.image.sprite = _btnDeactive;
                        item.TxtPercent.text = $"{_allMission[i].Percent}/{_allMission[i].Total}";
                    }
                }
            }

            CheckNotice();

        }

        private int GetMilestone(ProgressMission mis) {
            MissionType currentMis = this.GetMission(mis.MissionType);
            for(int i = 0; i < currentMis.Targets.Count; i++) {
                if(currentMis.Targets[i] >= mis.Total) {
                    return i + 1;
                }
            }
            return 1;
        }

        private void ShowReward(ProgressMission mis, DailyQuestItem item) {

            MissionType currentMis = this.GetMission(mis.MissionType);
            
            _animChucMung.transform.parent.gameObject.SetActive(true);
            _animChucMung.Play("StarShop-Appear");
            int gemReward = GetMilestone(mis) * 10;
            Utils.AddRubyCoin(gemReward);
            this.PostEvent(EventID.OnUpDateMoney);
            _txtQuantity.text = $"x{gemReward}";

            
            for (int i = 0; i < currentMis.Targets.Count; i++) {
                if (currentMis.Targets[i] > mis.Total) {
                    mis.Total = currentMis.Targets[i];
                    mis.Description = string.Format(currentMis.Description, $"<color=#24F3FF>{ mis.Total}</color>");
                    mis.HasNotice = 0;
                    mis.HasReceived = 1;
                    break;
                }
            }

            //switch (mis.MissionType) {
            //    case EnumDefine.Mission.RaidBoss:
            //    mis.Percent = PlayerPrefs.GetInt(StringDefine.LEVEL_MONSTER, 0) + 1;
            //    break;
            //    case EnumDefine.Mission.UnlockBeast:
            //    mis.Percent = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST, 0) + 1;
            //    break;
            //    case EnumDefine.Mission.UpgradeLevelSpawn:
            //    mis.Percent = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0) + 1;
            //    break;
            //    case EnumDefine.Mission.UpgradeTimeSpawn:
            //    mis.Percent = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
            //    break;
            //    case EnumDefine.Mission.SpendGem:
            //    mis.Percent++;
            //    break;
            //    default:
            //    mis.Percent++;
            //    break;
            //}

            float slid = (float)mis.Percent / mis.Total;
            item.SlidePercent.fillAmount = Mathf.Clamp01(slid);
            item.BtnGet?.onClick.RemoveAllListeners();
            item.BtnGet.GetComponentInChildren<Text>().text = $"领取\n{gemReward}";


            item.Descriptio.text = mis.Description;

            if (mis.Percent >= mis.Total) {
                if(mis.Percent >= mis.MaxValue && mis.Total == mis.MaxValue) {
                    //het nhiem vu
                    item.TxtPercent.text = $"{mis.MaxValue}/{mis.MaxValue}";
                    item.BtnGet.image.sprite = _btnDeactive;
                    item.SlidePercent.fillAmount = 0f;
                    item.Done.SetActive(true);
                    item.BtnGet.gameObject.SetActive(false);
                    mis.HasReceived = 1;
                } else {
                    item.TxtPercent.text = $"{mis.Total}/{mis.Total}";
                    item.BtnGet.image.sprite = _btnActive;
                    item.BtnGet?.onClick.AddListener(() => this.ShowReward(mis, item));
                    mis.HasReceived = 0;
                }
                
            } else {
                item.TxtPercent.text = $"{mis.Percent}/{mis.Total}";
                item.BtnGet.image.sprite = _btnDeactive;
                mis.HasReceived = 0;
            }
            ShowMission();
            SaveMission();
            CheckNotice();
        }

        public void SpendGem(int gem) {
            for (int i = 0; i < _allMission.Count; i++) {
                if (_allMission[i].MissionType == EnumDefine.Mission.SpendGem) {
                    _allMission[i].Percent += gem;
                    break;
                }
            }
        }

        public void QuickBuyBeast(int amount) {
            for (int i = 0; i < _allMission.Count; i++) {
                if (_allMission[i].MissionType == EnumDefine.Mission.GetBeast) {
                    _allMission[i].Percent += amount;
                    break;
                }
            }
        }


        private void OnApplicationPause(bool pause) {
            this.SaveMission();
        }

        private void OnApplicationQuit() {
            this.SaveMission();
        }

        private void SaveMission() {
            string data = string.Empty;
            if (_allMission == null) return;
            for (int i = 0; i < _allMission.Count; i++) {
                string str = $"{(int)_allMission[i].MissionType}.{_allMission[i].Total}.{_allMission[i].Percent}.{_allMission[i].HasReceived}";
                data += $"{str}-";
            }
            
            PlayerPrefs.SetString("mission-status", data);
        }

        public void ActiveDaily() {
            _pnDaily.SetActive(true);
            _pnAchivement.SetActive(false);
        }

        public void ActiveAchievement() {
            _pnDaily.SetActive(false);
            _pnAchivement.SetActive(true);
            this.ShowMission();
            //_noticeAchivement.SetActive(false);
        }

        private void CheckNotice() {            
            _noticeAchivement.SetActive(IsShowNoticeMission());
            _objNotice.SetActive(_noticeDaily.activeInHierarchy || _noticeAchivement.activeInHierarchy);
            UIManager.Instance.optionCtl.CheckShowNotice();
        }
    }

    [System.Serializable]
    public class ProgressMission : ProgressQuest {
        public EnumDefine.Mission MissionType;
    }
}

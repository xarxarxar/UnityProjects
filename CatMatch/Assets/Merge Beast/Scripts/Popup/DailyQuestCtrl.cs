using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Observer;
using UnityEngine.UI;

namespace MergeBeast {
    public class DailyQuestCtrl : MonoBehaviour {
        private static DailyQuestCtrl _instance;
        public static DailyQuestCtrl Instane => _instance;

        [SerializeField] private DailyQuestConfig _config;
        [SerializeField] private DailyQuestItem _prfItem;
        [SerializeField] private MissionCtrl _mission;
        [SerializeField] private GameObject _pnQuest;
        [SerializeField] private UIButton _btnDailyQuest;
        [SerializeField] private UIButton _btnGetBigQuest;
        [SerializeField] private UIButton _btnClose;
        [SerializeField] private GameObject _doneBigQuest;
        [SerializeField] private Animator _animChucMung;
        [SerializeField] private Animator _animQuest;
        [SerializeField] private GameObject _objNotice;
        [SerializeField] private Text _txtQuantity;
        [SerializeField] private Sprite _btnDeactive;
        [SerializeField] private Sprite _btnActive;
        [SerializeField] private Text _txtPercentBigQuest;
        [SerializeField] private Image _imgPercentBigQuest;
        [SerializeField] private Animator _animNoticeClaim;
        [SerializeField] private Text _txtTitleClaim;
        [SerializeField] private Sprite _imgPercentComplete;
        [SerializeField] private Text _txtCounDown;
        [SerializeField] private UIButton _btnDaily;
        [SerializeField] private UIButton _btnAchievement;
        [SerializeField] private GameObject _noticeDaily;
        [SerializeField] private GameObject _noticeAchivement;
        [SerializeField] private Sprite activeSpr;
        [SerializeField] private Sprite inActiveSpr;

        private Dictionary<EnumDefine.DailyQuest, ProgressQuest> _currentQuest;
        private List<DailyQuestItem> _listQuest;
        private Coroutine _ieTimeLeft;

        private void Awake() {
            _instance = this;
            _currentQuest = new Dictionary<EnumDefine.DailyQuest, ProgressQuest>();
        }
        // Start is called before the first frame update
        void Start() {
            _btnDailyQuest?.onClick.AddListener(this.OnClickOpenQuest);
            _btnClose?.onClick.AddListener(this.OnClickClose);
            _btnDaily?.onClick.AddListener(this.OnClickActiveDaily);
            _btnAchievement?.onClick.AddListener(this.OnClickAchievement);
            _listQuest = new List<DailyQuestItem>();
            for (int i = 0; i < 5; i++) {
                DailyQuestItem ctrl = Instantiate(_prfItem, _prfItem.transform.parent) as DailyQuestItem;
                _listQuest.Add(ctrl);
            }
            InitQuest();

            //_btnGetBigQuest?.onClick.AddListener(this.OnClickGetBigQuest);
            //_imgPercentBigQuest.fillAmount = 1f;
            //_txtPercentBigQuest.text = "5/5";
            //_btnGetBigQuest.image.sprite = _btnActive;
        }

        void CreateQuest() {
            string keyquest = string.Empty;

            while (_currentQuest.Count < 5) {
            loping: int pos = UnityEngine.Random.Range(0, _config.DailyQuest.Count);
                DailyQuestType quest = _config.DailyQuest[(EnumDefine.DailyQuest)pos]; ;
                if (_currentQuest.ContainsKey(quest.EnumType)) {
                    goto loping;
                }

                int sl = UnityEngine.Random.Range(0, quest.Types.Count);
                int total = quest.Types[sl];
                ProgressQuest ctrl = new ProgressQuest() {
                    Percent = 0,
                    Total = total,
                    Description = string.Format(quest.Description, $"<color=#24F3FF>{total}</color>"),
                    IconQuest = quest.IconQuest
                };

                _currentQuest.Add(quest.EnumType, ctrl);
                string qu = $"{(int)quest.EnumType}.{total}.{0}.{0}";
                keyquest += $"{qu}-";
            }
            PlayerPrefs.SetString("Quest-Day", DateTime.Today.ToString());
            PlayerPrefs.SetString("Key-Quest", keyquest);
        }

        private void InitQuest() {
            string questDay = PlayerPrefs.GetString("Quest-Day");
            if (questDay != DateTime.Today.ToString()) {
                CreateQuest();
            } else {
                string keyquest = PlayerPrefs.GetString("Key-Quest");
                //Debug.LogError("key quest 1: " + keyquest);
                if (string.IsNullOrEmpty(keyquest)) {
                    CreateQuest();
                    keyquest = PlayerPrefs.GetString("Key-Quest");
                }

                string[] arr = keyquest.Split('-');

                for (int i = 0; i < arr.Length; i++) {
                    if (!string.IsNullOrEmpty(arr[i])) {
                        string[] daily = arr[i].Split('.');
                        int key = int.Parse(daily[0]);
                        int total = int.Parse(daily[1]);
                        int per = int.Parse(daily[2]);
                        int recei = int.Parse(daily[3]);
                        DailyQuestType quest = _config.DailyQuest[(EnumDefine.DailyQuest)key];

                        ProgressQuest ctrl = new ProgressQuest() {
                            Percent = per,
                            Total = total,
                            Description = string.Format(quest.Description, $"<color=#24F3FF>{total}</color>"),
                            HasReceived = recei,
                            IconQuest = quest.IconQuest
                        };
                        if (!_currentQuest.ContainsKey((EnumDefine.DailyQuest)key)) {
                            _currentQuest.Add((EnumDefine.DailyQuest)key, ctrl);
                        }
                        //neu % hoan thanh > total va chua nhan thuong thi show notice
                        if (per >= total && recei == 0) {
                            _objNotice.SetActive(true);
                            _noticeDaily.SetActive(true);
                            UIManager.Instance.optionCtl.CheckShowNotice();
                        }
                    }
                }

            }
        }

        private void OnApplicationPause(bool pause) {
            this.SaveQuest();
        }

        private void OnApplicationQuit() {
            this.SaveQuest();
        }

        private void SaveQuest() {
            string keyquest = string.Empty;
            foreach (KeyValuePair<EnumDefine.DailyQuest, ProgressQuest> item in _currentQuest) {
                string qu = $"{(int)item.Key}.{item.Value.Total}.{item.Value.Percent}.{item.Value.HasReceived}";
                keyquest += $"{qu}-";
            }

            PlayerPrefs.SetString("Key-Quest", keyquest);
        }

        public void UpdateQuest(EnumDefine.DailyQuest quest = EnumDefine.DailyQuest.None, EnumDefine.Mission mission = EnumDefine.Mission.None) {
            if (quest != EnumDefine.DailyQuest.None) {
                if (_currentQuest.ContainsKey(quest)) {
                    _currentQuest[quest].Percent++;
                    if (_currentQuest[quest].Percent >= _currentQuest[quest].Total && _currentQuest[quest].HasReceived == 0 && _currentQuest[quest].HasNotice == 0) {

                        _txtTitleClaim.text = _currentQuest[quest].Description + "完成";
                        _animNoticeClaim.Play("noticeQuest-Show");
                        _currentQuest[quest].HasNotice = 1;

                        _objNotice.SetActive(true);
                        _noticeDaily.SetActive(true);
                        UIManager.Instance.optionCtl.CheckShowNotice();
                    }

                }
            }

            if (mission != EnumDefine.Mission.None) {
                _mission.UpdateMission(mission);
            }

            SaveQuest();

        }

        private void CheckNotice() {
            _noticeDaily.SetActive(IsShowNoticeDaily());
            _objNotice.SetActive(_noticeDaily.activeInHierarchy || _noticeAchivement.activeInHierarchy);
            UIManager.Instance.optionCtl.CheckShowNotice();
        }

        public void SpendGem(int gem) {
            _mission.SpendGem(Mathf.Abs(gem));
        }

        public void QuickBuyBeast(int amount) {
            _mission.QuickBuyBeast(amount);

            EnumDefine.DailyQuest quest = EnumDefine.DailyQuest.QuickBuyBeast;
            if (_currentQuest.ContainsKey(quest)) {
                _currentQuest[quest].Percent += amount;
                if (_currentQuest[quest].Percent >= _currentQuest[quest].Total && _currentQuest[quest].HasReceived == 0 && _currentQuest[quest].HasNotice == 0) {

                    _txtTitleClaim.text = _currentQuest[quest].Description + "完成";
                    _animNoticeClaim.Play("noticeQuest-Show");
                    _currentQuest[quest].HasNotice = 1;

                    _objNotice.SetActive(true);
                    _noticeDaily.SetActive(true);
                    UIManager.Instance.optionCtl.CheckShowNotice();
                }

            }

        }

        private void OnClickOpenQuest() {
            _pnQuest.SetActive(true);
            _objNotice.SetActive(false);
            _animQuest.Play("infoAscend-show");
            this.RefrestItem();
            _mission.ShowMission();
            _ieTimeLeft = StartCoroutine(IECounDownTimeLeft());
            //init quest khi open popup
            InitQuest();
            UIManager.Instance.OnClickShowOption();
        }

        private void OnClickReceived(ProgressQuest progres, DailyQuestItem ctrl) {
            progres.HasReceived = 1;
            ctrl.TxtPercent.text = $"{progres.Total}/{progres.Total}";
            ctrl.BtnGet.gameObject.SetActive(false);
            ctrl.Done.SetActive(true);
            _txtQuantity.text = "x5";
            _animChucMung.transform.parent.gameObject.SetActive(true);
            _animChucMung.Play("StarShop-Appear");
            ctrl.SlidePercent.sprite = _imgPercentComplete;
            Utils.AddRubyCoin(5);
            this.PostEvent(EventID.OnUpDateMoney);

            CheckNotice();
            SaveQuest();
        }

        private void OnClickGetBigQuest() {
            Debug.LogError("zzzz");
            int totalReward = 50;
            int index = 0;
            foreach (KeyValuePair<EnumDefine.DailyQuest, ProgressQuest> item in _currentQuest) {
                if (item.Value.HasReceived == 0) {
                    item.Value.HasReceived = 1;
                    _listQuest[index].TxtPercent.text = "";
                    _listQuest[index].BtnGet.gameObject.SetActive(false);
                    _listQuest[index].Done.SetActive(true);
                    _listQuest[index].SlidePercent.sprite = _imgPercentComplete;
                    totalReward += 5;
                }
                index++;
            }

            _txtQuantity.text = $"x{totalReward}";
            _animChucMung.transform.parent.gameObject.SetActive(true);
            _animChucMung.Play("StarShop-Appear");
            Utils.AddRubyCoin(totalReward);
            this.PostEvent(EventID.OnUpDateMoney);
            _doneBigQuest.SetActive(true);
            _btnGetBigQuest.gameObject.SetActive(false);
            PlayerPrefs.SetInt($"Big-Quest-{DateTime.Today.Day}", 1);
        }

        private void RefrestItem() {
            int index = 0;
            foreach (KeyValuePair<EnumDefine.DailyQuest, ProgressQuest> item in _currentQuest) {
                _listQuest[index].gameObject.SetActive(true);
                _listQuest[index].Descriptio.text = item.Value.Description;
                float per = (float)item.Value.Percent / item.Value.Total;
                _listQuest[index].SlidePercent.fillAmount = Mathf.Clamp01(per);
                _listQuest[index].IconQuest.sprite = item.Value.IconQuest;
                _listQuest[index].IconQuest.SetNativeSize();
                _listQuest[index].BtnGet.onClick.RemoveAllListeners();

                if (item.Value.Percent >= item.Value.Total) {
                    if (item.Value.HasReceived == 0) {
                        DailyQuestItem ctrl = _listQuest[index];
                        _listQuest[index].TxtPercent.text = $"{item.Value.Total}/{item.Value.Total}";
                        _listQuest[index].BtnGet.image.sprite = _btnActive;
                        _listQuest[index].BtnGet.onClick.AddListener(() => this.OnClickReceived(item.Value, ctrl));
                    } else {
                        _listQuest[index].TxtPercent.text = $"{item.Value.Total}/{item.Value.Total}";
                        _listQuest[index].BtnGet.gameObject.SetActive(false);
                        _listQuest[index].Done.SetActive(true);
                        _listQuest[index].SlidePercent.sprite = _imgPercentComplete;
                    }
                } else {
                    _listQuest[index].TxtPercent.text = $"{item.Value.Percent}/{item.Value.Total}";
                    _listQuest[index].BtnGet.image.sprite = _btnDeactive;
                }
                index++;
            }

            for (int i = index; i < _listQuest.Count; i++) {
                _listQuest[i].gameObject.SetActive(false);
            }

            int hasBig = PlayerPrefs.GetInt($"Big-Quest-{DateTime.Today.Day}", 0);
            if (hasBig != 0) {
                //finish big quest
                _imgPercentBigQuest.fillAmount = 1f;
                _txtPercentBigQuest.text = "5/5";
                _doneBigQuest.SetActive(true);
                _btnGetBigQuest.gameObject.SetActive(false);
            } else {
                if (this.IsComplete()) {
                    _btnGetBigQuest?.onClick.AddListener(this.OnClickGetBigQuest);
                    _imgPercentBigQuest.fillAmount = 1f;
                    _txtPercentBigQuest.text = "5/5";
                    _btnGetBigQuest.image.sprite = _btnActive;
                } else {
                    _btnGetBigQuest.image.sprite = _btnDeactive;
                    float complete = 0;
                    foreach (KeyValuePair<EnumDefine.DailyQuest, ProgressQuest> item in _currentQuest) {
                        if (item.Value.Percent >= item.Value.Total)
                            complete++;
                    }

                    _imgPercentBigQuest.fillAmount = complete / 5f;
                    _txtPercentBigQuest.text = $"{complete}/5";
                }


            }

            //check notice

            CheckNotice();

        }

        private void OnClickClose() {
            _animQuest.Play("infoAscend-hide");
            StartCoroutine(IEDeactive());
            StopCoroutine(_ieTimeLeft);
        }

        private IEnumerator IEDeactive() {
            yield return new WaitForSeconds(0.5f);
            _pnQuest.SetActive(false);
        }

        private bool IsComplete() {
            foreach (KeyValuePair<EnumDefine.DailyQuest, ProgressQuest> item in _currentQuest) {
                if (item.Value.Percent < item.Value.Total)
                    return false;
            }

            return true;
        }

        private bool IsShowNoticeDaily() {
            foreach (var item in _currentQuest) {
                //Debug.LogError(item.Value.Description + ", " + item.Value.Percent + "/" + item.Value.Total + ", receive: " + item.Value.HasReceived);
                if (item.Value.Percent >= item.Value.Total && item.Value.HasReceived == 0)
                    return true;
            }
            return false;
        }

        private IEnumerator IECounDownTimeLeft() {
            DateTime mai = DateTime.Today.AddDays(1);
            DateTime tomorow = new DateTime(mai.Year, mai.Month, mai.Day, 0, 0, 1);
            while (true) {
                TimeSpan differen = tomorow - DateTime.Now;
                string time = new DateTime(differen.Ticks).ToString("HH:mm:ss");
                //Debug.LogError("second: " + differen.Hours + ", " + differen.Minutes + ", " + differen.Seconds);
                if(differen.Hours == 0 && differen.Minutes == 0 && differen.Seconds == 1) {
                    //khi con 1s thi xoa key di de tao key moi
                    RefreshQuestForNewDay();
                }
                _txtCounDown.text = time + "剩余时间";
                yield return new WaitForSeconds(1f);
            }

        }

        public void RefreshQuestForNewDay() {
            PlayerPrefs.SetString("Quest-Day", "");
            _currentQuest = new Dictionary<EnumDefine.DailyQuest, ProgressQuest>();
            foreach (var item in _listQuest) {
                Destroy(item.gameObject);
            }
            _listQuest.Clear();
            for (int i = 0; i < 5; i++) {
                DailyQuestItem ctrl = Instantiate(_prfItem, _prfItem.transform.parent) as DailyQuestItem;
                //ctrl.gameObject.SetActive(true);
                _listQuest.Add(ctrl);
            }
            InitQuest();
            RefrestItem();
            if(_ieTimeLeft != null) {
                StopCoroutine(_ieTimeLeft);
                _ieTimeLeft = null;
            }
            StartCoroutine(_WaitCountNewDay());
            
            
        }

        IEnumerator _WaitCountNewDay() {
            yield return new WaitForSeconds(3f);
            _ieTimeLeft = StartCoroutine(IECounDownTimeLeft());
        }

        private void OnClickActiveDaily() {
            _mission.ActiveDaily();
            _btnDaily.image.sprite = activeSpr;
            _btnAchievement.image.sprite = inActiveSpr;
            this.RefrestItem();
        }

        private void OnClickAchievement() {
            _mission.ActiveAchievement();
            _btnDaily.image.sprite = inActiveSpr;
            _btnAchievement.image.sprite = activeSpr;
        }

    }

    [System.Serializable]
    public class ProgressQuest {
        public int order;
        public int Percent;
        public int Total;
        public int HasReceived;
        public string Description;
        public Sprite IconQuest;
        public int HasNotice;
        public int MaxValue; //gioi han cua mission
    }
}

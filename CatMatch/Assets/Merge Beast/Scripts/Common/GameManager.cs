using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Observer;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System.IO;
using UnityEngine.Networking;
using Spine.Unity;

namespace MergeBeast
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [SerializeField] private MonsterConfig _monsterConfig;
        [SerializeField] private BeastConfig _beastConfig;
        [SerializeField] private SkeletonGraphic _imgBeastDrag;
        [SerializeField] private SkeletonGraphic _imgBeastDrag2;
        [SerializeField] private Image _imgProgressSpawnBeast;
        [SerializeField] private Text _txtTotalDame;
        [SerializeField] private GameObject _objUnlock;
        [SerializeField] private GameObject _objUnlockBattle;
        [SerializeField] private GameObject _objTutorial;
        [SerializeField] private GameObject _objDailyReward;
        [SerializeField] private List<BeastItem> _listBeatTraning;
        [SerializeField] private List<BeastItem> _listBeastBattle;
        [SerializeField] private EnemyManager _enemyMgr;
        [SerializeField] private UIManager _uiMgr;
        [SerializeField] private OfflineRewardScr _offReward;
        [SerializeField] private GameObject noticeDaily;

        [SerializeField] private GameObject _newTut;
        [SerializeField] private GameObject _objTutorialMerge;

        private float _timeCountAttack;
        private float _timeCountSpawnBeast;
        private float _timeSpawnBeast;
        private float _timeBoostMerge;
        private int _totalAutoMerge;
        private int _currentIDBeast;
        private int _levelMerge = 1;
        private bool _stopSpawnBeast;
        private bool _isBoostAutoMerge;
        private List<int> _lockSlot = new List<int>() { 12, 13, 14, 15, 16, 17 };
        private List<GiftBeast> _listGift;
        private bool _checkGiftBeast;
        private bool _hasFullCat;
        private float _timeCountTutorial;

        private float timeCountVideo;
        private float _timeTimeboost = 1f;

        [HideInInspector] public bool isFirstTime = false;

        [HideInInspector] public int boostSpawnLevel = 0; //bien de luu khi use nhan dc tang lv beast, khi het thoi gian se + - di bien nay
        [HideInInspector] public int levelBeastDiff = 1;//độ chênh lệch beast, ví dụ beast cao nhất là 2 mà mở đc con 5 thì chênh lệch 3

        private List<BoostChestData> listItemChest; //danh sach cac boost o trong chest


        // Use this for initialization
        private void Awake()
        {
            if (_instance == null) _instance = this;
            this.InitSlotBeast();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            SaveSystem.Init();

            //request banner
        }

        void Start()
        {
            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                _listBeastBattle[i].AddEvtAttack(this.BeastAttack);
            }
            this.RegisterListener(EventID.OnTimeJump, (sender, param) => this.OnTimeJump());
            this.RegisterListener(EventID.OnAscend, (sender, pram) => this.OnAscend());

            InitListReward();

            StartCoroutine(CopyDataOfTileToPersitantPath());
            this.SetActiveAutoMerge();
        }



        private void InitListReward()
        {
            if (!File.Exists(SaveSystem.DATA_PATH))
            {
                string sould = "[";
                string gem = "[";
                string medal = "[";
                string chest = "[";
                for (int i = 0, n = _monsterConfig.Monster.Count; i < n; i++)
                {
                    sould += "\"" + Utils.GetSould(i) + "\"" + (i < n - 1 ? "," : "");
                    gem += Utils.GetGem(i) + (i < n - 1 ? "," : "");
                    medal += Utils.GetMedalMerge(i) + (i < n - 1 ? "," : "");
                    chest += Utils.GetBoostChest(i) + (i < n - 1 ? "," : "");
                }
                sould += "]";
                gem += "]";
                medal += "]";
                chest += "]";

                JSONNode json = new JSONObject();
                json["listSoulReward"] = JSON.Parse(sould).AsArray;
                json["listGemReward"] = JSON.Parse(gem).AsArray;
                json["listMedalReward"] = JSON.Parse(medal).AsArray;
                json["listChestReward"] = JSON.Parse(chest).AsArray;

                DataConfig.ParseJson(json);
                DataConfig.SaveData();
                return;
            }

            DataConfig.ParseJson(JSON.Parse(SaveSystem.Load()));
        }

        public void ResetRewardWhenAscend()
        {
            if (File.Exists(SaveSystem.DATA_PATH))
            {
                //xoa file va tao lai
                try
                {
                    File.Delete(SaveSystem.DATA_PATH);
                }
                catch (Exception ex)
                {
                    Debug.LogError("error" + ex.ToString());
                }
            }

            InitListReward();

        }

        void GetTimeBoostData()
        {
            for (int i = 0; i < 22; i++)
            {
                if (PlayerPrefs.GetFloat($"{StringDefine.TIME_BOOST}{i}", 0) > 0f)
                {
                    switch ((EnumDefine.BOOST)i)
                    {
                        case EnumDefine.BOOST.SPAWN_FASTER_1:
                        case EnumDefine.BOOST.SPAWN_FASTER_2:
                        case EnumDefine.BOOST.SPAWN_FASTER_3:
                            BoostTimeSpawn(0.25f);
                            Debug.LogError("co vao day");
                            break;
                    }
                }
            }
        }

        private void InitSlotBeast()
        {
            if (PlayerPrefs.GetInt(StringDefine.FIRST_OPEN_GAME, 0) == 0)
            {
                isFirstTime = true;
                for (int i = 0; i < _lockSlot.Count; i++)
                    PlayerPrefs.SetInt($"{StringDefine.TRANING_SLOT}{_lockSlot[i]}", StringDefine.NULL);
                PlayerPrefs.SetInt(StringDefine.BATTLE_SLOT_0, StringDefine.NULL);
                PlayerPrefs.SetInt(StringDefine.BATTLE_SLOT_5, StringDefine.NULL);
                PlayerPrefs.SetInt(StringDefine.FIRST_OPEN_GAME, StringDefine.NULL);
                PlayerPrefs.SetString(StringDefine.LAST_DAY_REWARD, DateTime.Today.AddDays(-1).ToString());

                for (int i = 0; i < _listBeatTraning.Count; i++)
                    PlayerPrefs.SetInt($"{StringDefine.BEAST_TRANING_INDEX}{i}", StringDefine.NULL);

                for (int j = 0; j < _listBeastBattle.Count; j++)
                    PlayerPrefs.SetInt($"{StringDefine.BEAST_BATTLE_INDEX}{j}", StringDefine.NULL);

                // Set Defaul Money
                PlayerPrefs.SetInt(StringDefine.MONEY_GEM, 5);
                PlayerPrefs.SetString(StringDefine.MONEY_COIN, "5000");
                //SceneManager.LoadScene(StringDefine.SCENE_MAP, LoadSceneMode.Additive);
                _objTutorial.SetActive(true);
                //Firebase.Analytics.FirebaseAnalytics.LogEvent(StringDefine.USER_FIRST_LUANCH);

            }
            else
            {
                isFirstTime = false;
                Destroy(_objTutorial);
                Invoke("CheckOfflineReward", 0.5f);
            }

            _listGift = new List<GiftBeast>();
            _currentIDBeast = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            _uiMgr.SetPriceBeast(this.GetBeast(_currentIDBeast));
            int lvSpam = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM, 1);
            _timeSpawnBeast = 6.1f - lvSpam * 0.1f;


            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                bool open = true;
                if (_lockSlot.Contains(i))
                {
                    open = PlayerPrefs.GetInt($"{StringDefine.TRANING_SLOT}{i}") == 0;
                }
                _listBeatTraning[i].SetStateSlot(open, i);

                int id = PlayerPrefs.GetInt($"{StringDefine.BEAST_TRANING_INDEX}{i}");
                if (id != StringDefine.NULL)
                {
                    id = Mathf.Max(id, _currentIDBeast);
                    var data = this.GetBeast(id);
                    _listBeatTraning[i].SetBeast(data);
                }
            }

            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                bool open = true;
                if (i == 0 || i == 5)
                {
                    open = PlayerPrefs.GetInt($"{StringDefine.BATTLE_SLOT}{i}") == 0;
                }
                _listBeastBattle[i].SetStateSlot(open, i);

                int id = PlayerPrefs.GetInt($"{StringDefine.BEAST_BATTLE_INDEX}{i}");
                if (id != StringDefine.NULL)
                {
                    id = Mathf.Max(id, _currentIDBeast);
                    var data = this.GetBeast(id);
                    _listBeastBattle[i].SetBeast(data);
                }
            }


            CheckUnlockTraning();
            CheckUnlockBattle();
            this.OnUpdateAutoMergeMedal();
            this.RegisterListener(EventID.OnUpdateAutoMergeMedal, (sender, pram) => OnUpdateAutoMergeMedal());
            this.RegisterListener(EventID.OnUpdateLevelMerge, (sender, param) => this.OnSetLevlMerge((int)param));
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(StringDefine.USER_ACTIVE);        
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                if (AdsManager.Instance.IsShowing()) return;

                PlayerPrefs.SetString(StringDefine.CHECK_POINT_OFFLINE, DateTime.Now.ToString());
                int highLV = PlayerPrefs.GetInt(StringDefine.HIGHEST_LEVEL_BEAST);
                //Firebase.Analytics.FirebaseAnalytics.LogEvent(StringDefine.HIGHER_LV_QUIT, new Firebase.Analytics.Parameter("Level",highLV));
                //Debug.Log($"Application Pause : {pause}");
            }
            else
            {
                this.CheckOfflineReward();
            }
        }

        // Update is called once per frame
        void Update()
        {

            _timeCountAttack += Time.deltaTime;


            if (_timeCountAttack >= 1f)
            {
                this.BeastSpawnMoney();
                _timeCountAttack = 0f;
            }

            //if (!_stopSpawnBeast)
            if (!isFirstTime)
            { //khong phai lan dau tien cai game, khi tutorial xong thi cung set = false
                _timeCountSpawnBeast += Time.deltaTime;

                timeCountVideo += Time.deltaTime;
            }

            if (_timeCountSpawnBeast >= _timeSpawnBeast)
            {
                this.SpawnBeastFullTime();
            }
            else
            {
                _imgProgressSpawnBeast.fillAmount = _timeCountSpawnBeast / _timeSpawnBeast;
            }

            if (_isBoostAutoMerge)
            {
                if (this.CanAutoMerge() && !_stopSpawnBeast)
                {
                    _timeBoostMerge += Time.deltaTime;
                    if (_timeBoostMerge > 1.5f)
                    {
                        StartCoroutine(IEAutoMerge());
                        _timeBoostMerge = 0f;
                    }
                }
            }

            if (_checkGiftBeast)
            {
                int n = 0;
                for (int i = 0; i < _listGift.Count; i++)
                {
                    if (!_listGift[i].IsSpawn)
                    {
                        for (int j = 0; j < _listBeatTraning.Count; j++)
                        {
                            if (_listBeatTraning[j].GetData() == null && _listBeatTraning[j].enabled && !this.CheckMarkSlot(j))
                            {
                                _listGift[i].Slot = j;
                                _listGift[i].IsSpawn = true;
                                _uiMgr.ShowGiftBeast(_listBeatTraning[j].GetPositionSlot(), j, _listGift[i].ID);
                                return;
                            }
                        }
                    }
                    else
                    {
                        n++;
                    }
                }

                if (_listGift.Count == n) _checkGiftBeast = false;
            }

            if (_hasFullCat)
            {
                _timeCountTutorial += Time.deltaTime;
                _objTutorialMerge.SetActive(_timeCountTutorial >= 2f);
            }
            else
            {
                _objTutorialMerge.SetActive(false);
            }

        }

        public SkeletonGraphic GetBeastDrag()
        {
            return _imgBeastDrag;
        }

        public SkeletonGraphic GetBeastDrag2()
        {
            return _imgBeastDrag2;
        }

        public float GetTimeCountVideo()
        {
            return timeCountVideo;
        }

        public void OnClickSpawnBeast(UnityAction buyDone)
        {
            //if (_stopSpawnBeast) return;
            this.SpawnBeastFullTime(true, buyDone);
        }

        private void SpawnBeastFullTime(bool isBuy = false, UnityAction buyDone = null)
        {

            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                if (_listBeatTraning[i].GetData() == null && _listBeatTraning[i].enabled && !this.CheckMarkSlot(i))
                {
                    UIManager.Instance.btnSpawnScriptAnimation.BreakEgg(_listBeatTraning[i].transform.position);

                    _listBeatTraning[i].SpawnBeast(_beastConfig.GetBeast((EnumDefine.BEAST)_currentIDBeast));
                //    _listBeatTraning[i].HideBeastIconWhenSpawn();
                    this.SetActiveAutoMerge();
                    if (!isBuy) _timeCountSpawnBeast = 0f;
                    else buyDone?.Invoke();

                    SoundManager.Instance?.PlaySound(EnumDefine.SOUND.BEAST_SPAM);

                    //tang them beast neu dc double spawn
                    int percentDouble = PlayerPrefs.GetInt(StringDefine.LEVEL_DOUBLE_SPAWN, 0);
                    int random = new System.Random().Next(1, 100);
                    if (random <= percentDouble)
                    {

                        AddGift(_currentIDBeast);
                        ReadySpawnGift();

                        UIManager.Instance.ShowNotify("额外1只猫");
                    }
                    _hasFullCat = false;
                    _timeCountTutorial = 0f;
                    return;

                }
            }

            _hasFullCat = true;

        }

        public BeastData GetNextBeast(int id)
        {
            int nextID = id + _levelMerge;
            return _beastConfig.GetBeast((EnumDefine.BEAST)nextID);
        }

        public BeastData GetBeast(int id)
        {
            return _beastConfig.GetBeast((EnumDefine.BEAST)id);
        }

        public void OnSuggestMerge(BeastItem item)
        {
            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                BeastData dt = _listBeatTraning[i].GetData();
                if (dt != null)
                {
                    if (_listBeatTraning[i].CanMerge())
                    {
                        if (item != _listBeatTraning[i])
                        {
                            if (dt.ID == item.GetData().ID)
                            {
                                _listBeatTraning[i].OnActiveSuggestMerge(true);
                            }
                        }
                    }
                }
            }
        }

        public void OnDeActiveSuggestMerge()
        {
            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                _listBeatTraning[i].OnActiveSuggestMerge(false);
            }
        }

        public void UpgradeBeast(int id, bool isBuy)
        {
            //_stopSpawnBeast = true;
            _currentIDBeast = id;
            if (!isBuy) return;
            var beast = this.GetBeast(id);
            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                if (_listBeastBattle[i].GetData() != null)
                {
                    if (_listBeastBattle[i].GetData().ID < id)
                        _listBeastBattle[i].SetBeast(beast);
                }
            }

            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                if (_listBeatTraning[i].GetData() != null)
                {
                    if (_listBeatTraning[i].GetData().ID < id)
                        _listBeatTraning[i].SetBeast(beast);
                }
            }
            //_stopSpawnBeast = false;
        }

        public void UpgradeTimeSpam(int lv)
        {
            //if (!BoostManager.Instance.HasSpawnTimeBoost()) {
            _timeSpawnBeast = 6.1f - lv * 0.1f;
            _timeSpawnBeast *= _timeTimeboost;
            //}
        }

        public void BoostTimeSpawn(float hs)
        {
            _timeTimeboost = hs;
            _timeSpawnBeast *= hs;
        }

        public void EndBoostFreeTimeSpawn()
        {
            int lv = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM);
            _timeSpawnBeast = (6.1f - lv * 0.1f); // /2
            _timeTimeboost = 1f;
        }

        public void BoostAutoMerge(bool isAuto)
        {
            this._isBoostAutoMerge = isAuto;
            if (this.CanAutoMerge())
                StartCoroutine(IEAutoMerge());
        }

        public void EndBoostTimeSpawn()
        {
            int lv = PlayerPrefs.GetInt(StringDefine.LEVEL_SPAM);
            _timeSpawnBeast = 6.1f - lv * 0.1f;
            _timeTimeboost = 1f;
            PlayerPrefs.SetFloat($"{StringDefine.TIME_BOOST}{(int)EnumDefine.BOOST.SPAWN_FASTER_1}", 1);
            PlayerPrefs.SetFloat($"{StringDefine.TIME_BOOST}{(int)EnumDefine.BOOST.SPAWN_FASTER_2}", 1);
            PlayerPrefs.SetFloat($"{StringDefine.TIME_BOOST}{(int)EnumDefine.BOOST.SPAWN_FASTER_3}", 1);
        }

        public bool SomeBeastDraging()
        {
            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                if (_listBeatTraning[i].IsDraging())
                    return true;
            }

            return false;
        }

        private void BeastSpawnMoney()
        {
            BigInteger totalDame = TotalDameBattle();
            _txtTotalDame.text = $"+{Utils.FormatNumber(totalDame)}/s";
            if (totalDame > 0)
            {
                _uiMgr.UpdateMoneyCoin(totalDame, _stopSpawnBeast);
            }
        }

        public BigInteger TotalDameBattle()
        {
            BigInteger totalDame = 0;
            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                if (_listBeastBattle[i].GetData() != null)
                {
                    totalDame += _listBeastBattle[i].MyDPS(); // Utils.GetDameByLevel(_listBeastBattle[i].GetData().Level);
                }
            }

            return totalDame;
        }

        public BigInteger TotalDamageTraining()
        {
            BigInteger total = 0;
            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                if (_listBeatTraning[i].GetData() != null)
                {
                    total += Utils.GetDameByLevelAndAscend(_listBeatTraning[i].GetData().Level);
                }
            }
            return total;
        }

        public BigInteger TotalDamage()
        {
            return TotalDameBattle() + TotalDamageTraining();
        }

        private bool CanAutoMerge()
        {
            for (int i = 0; i < _listBeatTraning.Count - 1; i++)
            {
                if (_listBeatTraning[i].GetData() != null)
                {
                    for (int j = i + 1; j < _listBeatTraning.Count; j++)
                    {
                        if (_listBeatTraning[j].GetData() != null)
                        {
                            if (_listBeatTraning[i].GetData().ID == _listBeatTraning[j].GetData().ID)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public void UpdateDPS()
        {
            BigInteger totalDame = TotalDameBattle();
            _txtTotalDame.text = $"每秒：{Utils.FormatNumber(totalDame)}/s";
        }

        public void OnClickAutoMerge()
        {
            if (_stopSpawnBeast) return;
            StartCoroutine(IEAutoMerge());
        }

        private IEnumerator IEBoostAutoMerge()
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(IEAutoMerge());
        }

        private IEnumerator IEAutoMerge()
        {
            _stopSpawnBeast = true;
        //   _uiMgr.DisableBtnSpawn();
        checkpoint:;

            for (int i = 0; i < _listBeatTraning.Count - 1; i++)
            {
                if (_listBeatTraning[i].GetData() != null)
                {
                    for (int j = i + 1; j < _listBeatTraning.Count; j++)
                    {
                        if (_listBeatTraning[j].GetData() != null)
                        {
                            if (_listBeatTraning[i].GetData().ID == _listBeatTraning[j].GetData().ID)
                            {
                                _listBeatTraning[i].OnUpgradeBeast();
                                _listBeatTraning[j].SetEmptySlot();

                                if (_isBoostAutoMerge)
                                {
                                    _imgBeastDrag.transform.localScale = Vector3.zero;
                                    yield return new WaitForSeconds(0.7f);
                                    goto checkpoint;
                                }

                            }
                        }
                    }
                }
            }

            if (!_isBoostAutoMerge)
            {
                _totalAutoMerge--;
                _imgBeastDrag.transform.localScale = Vector3.zero;
                PlayerPrefs.SetInt(StringDefine.AUTO_MERGE, _totalAutoMerge);
                yield return new WaitForSeconds(0.7f);

            }

            _stopSpawnBeast = false;
            this.SetActiveAutoMerge();
            _uiMgr.CheckInteracBtnSpawn();
        }

        private void BeastAttack(BigInteger dmg, int lv)
        {
            _enemyMgr.Attack(dmg, lv);
        }

        private void SetActiveAutoMerge()
        {
            _uiMgr.SetActiveAutoMerge(this.CanAutoMerge(), _totalAutoMerge, _totalAutoMerge <= 0);
        }

        private int GetBeastHasUnlock()
        {
            int n = 1;
            for (int i = 0; i < _lockSlot.Count; i++)
            {
                if (PlayerPrefs.GetInt($"{StringDefine.TRANING_SLOT}{_lockSlot[i]}") == 0)
                    n++;
            }
            return n;
        }

        private void CheckUnlockTraning()
        {
            _objUnlock.transform.parent = null;
            _objUnlock.transform.position = Vector3.right * 100f;

            for (int i = 0; i < _lockSlot.Count; i++)
            {
                if (PlayerPrefs.GetInt($"{StringDefine.TRANING_SLOT}{_lockSlot[i]}") == StringDefine.NULL)
                {
                    int n = _lockSlot[i];
                    _objUnlock.transform.SetParent(_listBeatTraning[n].transform);
                    _objUnlock.transform.localPosition = Vector3.zero;
                    _objUnlock.transform.localScale = Vector3.one;

                    _listBeatTraning[n]._btnUnlock?.onClick.RemoveAllListeners();
                    _listBeatTraning[n]._btnUnlock?.onClick.AddListener(() => OnClickUnlockTraning(n));

                    int price = GetBeastHasUnlock() * 50;
                    _objUnlock.transform.Find("txtGem").GetComponent<Text>().text = price.ToString();
                    break;
                }
            }
        }

        private void CheckUnlockBattle()
        {
            _objUnlockBattle.transform.parent = null;
            _objUnlockBattle.transform.position = Vector3.right;

            if (PlayerPrefs.GetInt($"{StringDefine.BATTLE_SLOT}{0}") == StringDefine.NULL)
            {
                _objUnlockBattle.transform.SetParent(_listBeastBattle[0].transform);
                _objUnlockBattle.transform.localPosition = Vector3.zero;
                _objUnlockBattle.transform.localScale = Vector3.one;

                _listBeastBattle[0]._btnUnlock?.onClick.RemoveAllListeners();
                _listBeastBattle[0]._btnUnlock?.onClick.AddListener(() => OnClickUnlockBattle(0));

                int price = 1000;
                _objUnlockBattle.transform.Find("txtGem").GetComponent<Text>().text = price.ToString();
                return;
            }

            if (PlayerPrefs.GetInt($"{StringDefine.BATTLE_SLOT}{5}") == StringDefine.NULL)
            {
                _objUnlockBattle.transform.SetParent(_listBeastBattle[5].transform);
                _objUnlockBattle.transform.localPosition = Vector3.zero;
                _objUnlockBattle.transform.localScale = Vector3.one;

                _listBeastBattle[5]._btnUnlock?.onClick.RemoveAllListeners();
                _listBeastBattle[5]._btnUnlock?.onClick.AddListener(() => OnClickUnlockBattle(5));

                int price = 2000;
                _objUnlockBattle.transform.Find("txtGem").GetComponent<Text>().text = price.ToString();
                return;
            }
        }


        private void OnClickUnlockTraning(int slot)
        {
            Debug.Log($"Click Unlock Traning : {slot}");
            int price = GetBeastHasUnlock() * 50;
            string mgs = $"确认花费<color=green>{price}</color>宝石开启槽位";
            string err = $"你需要<color=red>{price}</color>宝石才能解锁该槽位";

            if (PlayerPrefs.GetInt(StringDefine.MONEY_GEM) >= price)
            {
                ScreenManager.Instance.ShowConfirm(0, mgs, () =>
                {
                    int gem = PlayerPrefs.GetInt(StringDefine.MONEY_GEM) - price;
                    PlayerPrefs.SetInt(StringDefine.MONEY_GEM, gem);
                    PlayerPrefs.SetInt($"{StringDefine.TRANING_SLOT}{slot}", 0);
                    _listBeatTraning[slot]._btnUnlock?.onClick.RemoveAllListeners();
                    _listBeatTraning[slot].enabled = true;
                    _listBeatTraning[slot].SetStateSlot(true, slot);
                    _uiMgr.UpdateMoneyRuby();
                    CheckUnlockTraning();
                    ScreenManager.Instance.ShowNoti("你已开启训练槽位！！");
                });
            }
            else
            {
                ScreenManager.Instance.ShowNoti(err);
            }
        }

        private void OnClickUnlockBattle(int slot)
        {
            int price = slot == 0 ? 1000 : 2000;
            string mgs = $"确认花费<color=green>{price}</color>宝石开启槽位";
            string err = $"你需要<color=red>{price}</color>宝石才能解锁该槽位";

            if (Utils.GetCurrentRubyMoney() >= price)
            {
                ScreenManager.Instance.ShowConfirm(0, mgs, () =>
                {
                    Utils.AddRubyCoin(-price);
                    string key = slot == 0 ? StringDefine.BATTLE_SLOT_0 : StringDefine.BATTLE_SLOT_5;
                    PlayerPrefs.SetInt($"{key}", 0);
                    _listBeastBattle[slot]._btnUnlock?.onClick.RemoveAllListeners();
                    _listBeastBattle[slot].enabled = true;
                    _listBeastBattle[slot].SetStateSlot(true, slot);
                    _uiMgr.UpdateMoneyRuby();
                    CheckUnlockBattle();
                    ScreenManager.Instance.ShowNoti("你已开启战斗槽位！！");
                });
            }
            else
            {
                ScreenManager.Instance.ShowNoti(err);
            }
        }

        private void OnUpdateAutoMergeMedal()
        {
            _totalAutoMerge = PlayerPrefs.GetInt(StringDefine.AUTO_MERGE, 50);
            this.SetActiveAutoMerge();
        }

        private void OnSetLevlMerge(int n)
        {
            _levelMerge = n;
        }

        public void AddGift(int id)
        {
            //Debug.LogError("add gift: " + id);
            GiftBeast gift = new GiftBeast() { ID = id, IsSpawn = false, Slot = -99 };
            _listGift.Add(gift);
        }

        public void ReadySpawnGift()
        {
            _checkGiftBeast = _listGift.Count > 0;
        }

        public void SpawnBeastAtSlot(int id, int slot)
        {
            BeastData data = this.GetBeast(id);
            _listBeatTraning[slot].SpawnBeast(data);

            for (int i = 0; i < _listGift.Count; i++)
            {
                if (_listGift[i].ID == id && _listGift[i].Slot == slot)
                {
                    _listGift.RemoveAt(i);
                    break;
                }
            }

            if (_listGift.Count == 0) _checkGiftBeast = false;
        }

        private bool CheckMarkSlot(int slot)
        {
            if (_listGift.Count == 0) return false;

            for (int i = 0; i < _listGift.Count; i++)
                if (_listGift[i].Slot == slot) return true;

            return false;
        }

        public bool CheckBeastSlot()
        {
            return _listBeatTraning[0].GetData() != null && _listBeatTraning[1].GetData() != null;
        }



        public bool CheckCompleteTutorial()
        {
            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                if (_listBeastBattle[i].GetData() != null) return true;
            }

            return false;
        }

        public BigInteger GetRootDPS()
        {
            BigInteger totalDame = 0;
            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                if (_listBeastBattle[i].GetData() != null)
                {
                    totalDame += _listBeastBattle[i].RootDPS(); // Utils.GetDameByLevel(_listBeastBattle[i].GetData().Level);
                }
            }
            return totalDame;
        }

        private void OnTimeJump()
        {
            //for (int i = 0; i < _listBeatTraning.Count; i++) {
            //    if (_listBeatTraning[i].GetData() == null && _listBeatTraning[i].enabled && !this.CheckMarkSlot(i)) {
            //        _listBeatTraning[i].SpawnBeast(_beastConfig.GetBeast((EnumDefine.BEAST)_currentIDBeast));
            //        this.SetActiveAutoMerge();
            //    }
            //}
        }

        private void CheckOfflineReward()
        {
            if (_offReward == null) return;

            string endTime = PlayerPrefs.GetString(StringDefine.CHECK_POINT_OFFLINE);
            //Debug.LogError("end time: " + endTime);
            if (!string.IsNullOrEmpty(endTime))
            {
                TimeSpan timeDelta = DateTime.Now - Convert.ToDateTime(endTime);
                //Debug.LogError("Time delta: " + timeDelta);
                double totalTime = Math.Min(timeDelta.TotalSeconds, 43200);
                //Debug.LogError("total time: " + totalTime);
                if (totalTime > 60)
                {
                    BigInteger soul = this.GetRootDPS() * Convert.ToInt32(totalTime) / 10;

                    int beastSpawned = Mathf.RoundToInt((float)totalTime / (_timeSpawnBeast * 10f));
                    var arrBeast = Utils.ArrayPower2(beastSpawned);

                    var shop = PlayerPrefs.GetInt(StringDefine.CURRENT_SHOP_BEAST, 1);

                    for (int i = 0; i < arrBeast.Count; i++)
                    {
                        //chỉ tặng những con beast max = con đã ghép và max số lượng là 4 con.
                        int beastGift = Mathf.Clamp(_currentIDBeast + arrBeast[i] + 1, _currentIDBeast, shop);
                        if (_listGift.Count < 4)
                        {
                            //Debug.LogError("Check Offline: " + beastGift);

                            this.AddGift(beastGift);
                        }
                    }

                    ReadySpawnGift();

                    if (soul > 0)
                    {

                        _offReward.gameObject.SetActive(true);
                        _offReward.ShowPopup(timeDelta, soul, this.CheckDailyReward);
                    }
                    else
                    {
                        this.CheckDailyReward();
                    }
                }
                else
                {
                    this.CheckDailyReward();
                }
            }
            else
            {
                this.CheckDailyReward();
            }
        }

        private void CheckDailyReward()
        {
            string lastDayReward = PlayerPrefs.GetString(StringDefine.LAST_DAY_REWARD);
            if (string.IsNullOrEmpty(lastDayReward)) return;
            TimeSpan time = DateTime.Today - Convert.ToDateTime(lastDayReward);
            if (time.Days > 0)
            {
                noticeDaily.SetActive(true);
                UIManager.Instance.optionCtl.CheckShowNotice();
                _objDailyReward.SetActive(true);
            }
            else
            {
                //        Destroy(_objDailyReward);
            }
            //    _objDailyReward.SetActive(true);
        }

        private void OnAscend()
        {
            //_stopSpawnBeast = true;
            VipConfig vipConfig = CPlayer.GetVipConfig();
            _currentIDBeast = vipConfig.levelUpgradeAscend - 1;
            PlayerPrefs.SetInt(StringDefine.LEVEL_BEAST, vipConfig.levelUpgradeAscend - 1);
            PlayerPrefs.SetInt(StringDefine.LEVEL_SPAM, vipConfig.levelUpgradeAscend);
            PlayerPrefs.SetInt(StringDefine.LEVEL_DOUBLE_SPAWN, 0);
            PlayerPrefs.SetInt(StringDefine.LEVEL_LEVEL_MERGE, 0);
            //PlayerPrefs.SetInt(StringDefine.HIGHEST_LEVEL_BEAST, 0);
            PlayerPrefs.SetInt(StringDefine.CURRENT_SHOP_BEAST, vipConfig.levelUpgradeAscend - 1);
            _uiMgr.SetPriceBeast(this.GetBeast(_currentIDBeast));
            _uiMgr.DeactiveShopStar();

            for (int i = 0; i < _listBeatTraning.Count; i++)
            {
                bool open = true;
                if (_lockSlot.Contains(i))
                {
                    open = PlayerPrefs.GetInt($"{StringDefine.TRANING_SLOT}{i}") == 0;
                }

                if (open)
                    _listBeatTraning[i].OnAscendBeast();
            }

            for (int i = 0; i < _listBeastBattle.Count; i++)
            {
                bool open = true;
                if (i == 0 || i == 5)
                {
                    open = PlayerPrefs.GetInt($"{StringDefine.BATTLE_SLOT}{i}") == 0;
                }

                if (open)
                    _listBeastBattle[i].OnAscendBeast();
            }
            //_stopSpawnBeast = false;
        }

        public void StopAutoSpawnBeast(bool stop)
        {
            _stopSpawnBeast = stop;
        }


        #region BOOST CHEST

        private void SetUpBoostChest()
        {
            listItemChest = new List<BoostChestData>();
            //kiem tra storage xem co item chest nao ko
            string boostChest = PlayerPrefs.GetString(StringDefine.BOOST_CHEST, "");
            if (!string.IsNullOrEmpty(boostChest))
            {
                //co data thi load ra
                string[] list = boostChest.Split(';');
                for (int i = 0; i < list.Length; i++)
                {
                    if (!string.IsNullOrEmpty(list[i]))
                    {
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
            for (int i = 0; i < listItemChest.Count; i++)
            {
                count += listItemChest[i].GetAmount();
            }
            if (count == 0)
            {
                listItemChest.Clear();
                PlayerPrefs.SetString(StringDefine.BOOST_CHEST, "");
            }


        }
        public void AddBoostToInventory(int id, int amount)
        {
            SetUpBoostChest();
            //kiem tra xem co id nay trong list chua
            if (CheckIdInListChest(id))
            {
                for (int i = 0; i < listItemChest.Count; i++)
                {
                    BoostChestData bcd = listItemChest[i];
                    if (bcd.GetId() == id)
                    {
                        //co id roi thi update amount
                        bcd.SetAmount(bcd.GetAmount() + amount);
                    }
                }
            }
            else
            {
                BoostChestData bcd = new BoostChestData();
                bcd.SetId(id);
                bcd.SetAmount(amount <= 0 ? 0 : amount);
                listItemChest.Add(bcd);
            }

            //save data
            string data = "";
            for (int i = 0; i < listItemChest.Count; i++)
            {
                BoostChestData bcd = listItemChest[i];
                data += $"{bcd.GetId()}-{bcd.GetAmount()};";
                Debug.Log(data);
            }
            PlayerPrefs.SetString(StringDefine.BOOST_CHEST, data);
        }

        private bool CheckIdInListChest(int id)
        {
            for (int i = 0; i < listItemChest.Count; i++)
            {
                if (id == listItemChest[i].GetId()) return true;
            }
            return false;
        }
        #endregion

        public void ResetTimeCountVideo()
        {
            timeCountVideo = 0f;
        }

        IEnumerator CopyDataOfTileToPersitantPath()
        {
            for (int i = 1; i <= Config.LEVEL_TILE; i++)
            {
                int currentLevel = CPlayer.currentLevel;
                string path = Application.streamingAssetsPath + "/" + i + ".json";

                var loadingRequest = UnityWebRequest.Get(path);
                // Debug.Log("== =======start load map: " + TimeUtil.TimeStampSecond);
                yield return loadingRequest.SendWebRequest();
                // Debug.Log("== ===========end load map: " + TimeUtil.TimeStampSecond);
                if (loadingRequest.isHttpError || loadingRequest.isNetworkError)
                {
                    Debug.Log("Het Map");
                    yield break;
                }
                string filePath = SaveSystem.TILE_ORIGIN + i + ".json";
                //Debug.LogError("file path origin : " + filePath);
                if (!File.Exists(filePath))
                {
                    //Debug.LogError("Save to origin");
                    //chi copy 1 lan duy nhat
                    SaveSystem.Save(filePath, loadingRequest.downloadHandler.text);
                }

            }

        }

        public void AddDailyBoost(EnumDefine.DailyBoost boost, int value)
        {
            int current = PlayerPrefs.GetInt(StringDefine.DAILY_BOOST + (int)boost, 0);
            current += value;
            PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)boost, current);
        }
    }


}

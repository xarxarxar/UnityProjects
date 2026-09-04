using Observer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

namespace MergeBeast {
    public class BoostManager : MonoBehaviour {
        private static BoostManager _instance;
        public static BoostManager Instance => _instance;

        [SerializeField] private BoostConfig _config;
        [SerializeField] private BoostNotify _prfBoost;
        [SerializeField] private Transform _parentBoost;

        private Dictionary<EnumDefine.BOOST, BoostAction> _dictionBoost;
        private Dictionary<EnumDefine.FREEBOOST, BoostAction> _freeBoost;


        private void Awake() {
            if (_instance == null) _instance = this;
            _dictionBoost = new Dictionary<EnumDefine.BOOST, BoostAction>();
            _freeBoost = new Dictionary<EnumDefine.FREEBOOST, BoostAction>();
        }

        // Use this for initialization
        private void Start() {

            string freeSlot1 = PlayerPrefs.GetString(StringDefine.BOOST_FREE_SLOT1, "");
            string freeSlot2 = PlayerPrefs.GetString(StringDefine.BOOST_FREE_SLOT2, "");
            if(!string.IsNullOrEmpty(freeSlot1)) {
                CheckFreeBoostAlive(freeSlot1);
            }

            if(!string.IsNullOrEmpty(freeSlot2)) {
                CheckFreeBoostAlive(freeSlot2);
            }

            return;
            //yield return new WaitForSeconds(0.3f);
            // Load boost
            for (int i = 0; i < 21; i++) {
                if (PlayerPrefs.GetFloat($"{StringDefine.TIME_BOOST}{i}", 0) > 0f) {
                    UnityAction actionEndBoost = null;
                    var data = _config.GetBoost((EnumDefine.BOOST)i);
                    switch ((EnumDefine.BOOST)i) {
                        case EnumDefine.BOOST.SPAWN_FASTER_1:
                        case EnumDefine.BOOST.SPAWN_FASTER_2:
                        case EnumDefine.BOOST.SPAWN_FASTER_3:
                        GameManager.Instance.BoostTimeSpawn(0.25f);
                        actionEndBoost = StopFastSpawn;
                        break;
                        case EnumDefine.BOOST.DAMAGE_BOOST_1:
                        this.OnDamageBoost(2);
                        actionEndBoost = StopDPS;
                        break;
                        case EnumDefine.BOOST.DAMAGE_BOOST_2:
                        this.OnDamageBoost(3);
                        actionEndBoost = StopDPS;
                        break;
                        case EnumDefine.BOOST.DAMAGE_BOOST_3:
                        this.OnDamageBoost(5);
                        actionEndBoost = StopDPS;
                        break;
                        case EnumDefine.BOOST.DAMAGE_BOOST_4:
                        this.OnDamageBoost(10);
                        actionEndBoost = StopDPS;
                        break;
                        case EnumDefine.BOOST.LEVEL_HIGHER_1:
                        case EnumDefine.BOOST.LEVEL_HIGHER_2:
                        case EnumDefine.BOOST.LEVEL_HIGHER_3:
                        actionEndBoost = Lower2Level;
                        break;
                        case EnumDefine.BOOST.AUTO_MERGE_1:
                        case EnumDefine.BOOST.AUTO_MERGE_2:
                        GameManager.Instance.BoostAutoMerge(true);
                        actionEndBoost = StopAutoMerge;
                        break;
                        case EnumDefine.BOOST.SPAWN_LEVEL_1:
                        this.OnBoostLevelMerge(2);
                        actionEndBoost = StopLevelMerge;
                        break;
                        case EnumDefine.BOOST.SPAWN_LEVEL_2:
                        case EnumDefine.BOOST.SPAWN_LEVEL_3:
                        this.OnBoostLevelMerge(3);
                        actionEndBoost = StopLevelMerge;
                        break;
                    }

                    BoostNotify noty = Instantiate(_prfBoost, _parentBoost);
                    noty.SetBoost(data.Icon, data.Description);

                    if (!_dictionBoost.ContainsKey((EnumDefine.BOOST)i)) {
                        BoostAction action = new BoostAction() {
                            TimeRemanin = PlayerPrefs.GetFloat($"{StringDefine.TIME_BOOST}{i}"),
                            StopBoost = actionEndBoost,
                            Data = data,
                            Noty = noty
                        };
                        _dictionBoost.Add((EnumDefine.BOOST)i, action);
                    }
                }
            }
        }


        private void CheckFreeBoostAlive(string data) {            
            BoostFreeData bfd = new BoostFreeData();
            bfd.ParseData(data);
            if (bfd.GetTimeUpUse() == 0 || bfd.GetTimeUpUse() <= TimeUtil.TimeStampSecond) return;

            var boostType = (EnumDefine.FREEBOOST)bfd.GetId();
            switch(boostType) {
                case EnumDefine.FREEBOOST.DPS:
                AddFreeBoost(EnumDefine.FREEBOOST.DPS, (int) (bfd.GetTimeUpUse() - TimeUtil.TimeStampSecond), (int) bfd.GetValue());
                break;
                case EnumDefine.FREEBOOST.SPAWN_LEVEL:
                    AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_LEVEL, (int)(bfd.GetTimeUpUse() - TimeUtil.TimeStampSecond), (int) bfd.GetValue());
                break;
                case EnumDefine.FREEBOOST.SPAWN_TIME:
                AddFreeBoost(EnumDefine.FREEBOOST.SPAWN_TIME, (int)(bfd.GetTimeUpUse() - TimeUtil.TimeStampSecond),(int) bfd.GetValue());
                break;
            }
        }

        private void Update() {
            try {
                lock (_dictionBoost) {
                    if (_dictionBoost.Count > 0) {
                        foreach (var boost in _dictionBoost) {
                            boost.Value.TimeRemanin -= Time.deltaTime;
                            boost.Value.Noty.UpdateTime(boost.Value.TimeRemanin);

                            PlayerPrefs.SetFloat($"{StringDefine.TIME_BOOST}{(int)boost.Key}", boost.Value.TimeRemanin);

                            if (boost.Value.TimeRemanin <= 0) {
                                Debug.Log($"End Boost : {boost.Key}");
                                boost.Value.Noty.DestroyMe();
                                PlayerPrefs.SetFloat($"{StringDefine.TIME_BOOST}{(int)boost.Key}", StringDefine.NULL);
                                _dictionBoost.Remove(boost.Key);
                                boost.Value.StopBoost?.Invoke();
                                break;
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Debug.Log($"Dictionary Exception : {e.ToString()}");
            }



            try {
                if (_freeBoost.Count > 0) {
                    foreach (var boost in _freeBoost) {
                        boost.Value.TimeRemanin -= Time.deltaTime;
                        boost.Value.Noty.UpdateTime(boost.Value.TimeRemanin);

                        if (boost.Value.TimeRemanin <= 0) {
                            boost.Value.Noty.DestroyMe();
                            _freeBoost.Remove(boost.Key);
                            boost.Value.StopBoost?.Invoke();
                            break;
                        }
                    }
                }
            } catch (Exception e) {

            }
        }

        public void AddBoost(EnumDefine.BOOST boost, float time = -1) {
            Debug.Log($"Your boost : {boost}");
            float finalTime = 0;
            var data = _config.GetBoost(boost);

            if (time == -1) {
                finalTime = data.Time;
            } else {
                finalTime = time;
            }
            

            if (_dictionBoost == null) {                
                return; 
            }
            if (_dictionBoost.ContainsKey(boost)) {

                string timeUp = PlayerPrefs.GetString($"{StringDefine.BOOST_TIME_UP}{(int)boost}", TimeUtil.TimeStampSecond.ToString());
                long timeUpLong = long.Parse(timeUp);

                _dictionBoost[boost].TimeRemanin += finalTime;
                timeUpLong += (long)finalTime;

                PlayerPrefs.SetString($"{ StringDefine.BOOST_TIME_UP}{ (int)boost}", timeUpLong.ToString());

                BoostAds[] list = FindObjectsOfType<BoostAds>();
                foreach (var item in list) {
                    item.UpdateStatus();
                }

                BoostItemAds[] list2 = FindObjectsOfType<BoostItemAds>();
                foreach (var item in list2) {
                    item.UpdateStatus();
                }

            } else {
                UnityAction actionEndBoost = null;
                bool ignoreDisplay = false;

                switch (boost) {
                    case EnumDefine.BOOST.SPAWN_FASTER_1:
                    GameManager.Instance.BoostTimeSpawn(0.5f);
                    actionEndBoost = StopFastSpawn;
                    break;
                    case EnumDefine.BOOST.SPAWN_FASTER_2:
                    GameManager.Instance.BoostTimeSpawn(0.33f);
                    actionEndBoost = StopFastSpawn;
                    break;
                    case EnumDefine.BOOST.SPAWN_FASTER_3:
                    GameManager.Instance.BoostTimeSpawn(0.25f);
                    actionEndBoost = StopFastSpawn;
                    break;
                    case EnumDefine.BOOST.DAMAGE_BOOST_1:
                    this.OnDamageBoost(2);
                    actionEndBoost = StopDPS;
                    break;
                    case EnumDefine.BOOST.DAMAGE_BOOST_2:
                    this.OnDamageBoost(3);
                    actionEndBoost = StopDPS;
                    break;
                    case EnumDefine.BOOST.DAMAGE_BOOST_3:
                    this.OnDamageBoost(5);
                    actionEndBoost = StopDPS;
                    break;
                    case EnumDefine.BOOST.DAMAGE_BOOST_4:
                    this.OnDamageBoost(10);
                    actionEndBoost = StopDPS;
                    break;
                    case EnumDefine.BOOST.LEVEL_HIGHER_1:
                    case EnumDefine.BOOST.LEVEL_HIGHER_2:
                    case EnumDefine.BOOST.LEVEL_HIGHER_3:
                    this.OnBoostHigherLevel(3);
                    actionEndBoost = Lower2Level;
                    break;
                    case EnumDefine.BOOST.AUTO_MERGE_1:
                    case EnumDefine.BOOST.AUTO_MERGE_2:
                    GameManager.Instance.BoostAutoMerge(true);
                    actionEndBoost = StopAutoMerge;
                    break;
                    case EnumDefine.BOOST.SERVARAL_SOUL:
                    case EnumDefine.BOOST.PACK_SOUL:
                    case EnumDefine.BOOST.CHEST_SOUL:                    
                    BigInteger dps = GameManager.Instance.TotalDamage() * (long)Boost(boost).Time;
                    this.OnBoostSoul(dps);
                    ignoreDisplay = true;
                    break;
                    case EnumDefine.BOOST.PILE_SOUL:
                    BigInteger dps_pile = GameManager.Instance.TotalDamage() * Config.PILE_SOUL_REWARD;
                    this.OnBoostSoul(dps_pile);
                    ignoreDisplay = true;
                    break;
                    case EnumDefine.BOOST.FEW_MERGE_MEDAL:
                    this.OnBoostMedalMerge(50);
                    ignoreDisplay = true;
                    break;
                    case EnumDefine.BOOST.SEVERAL_MERGE_MEDAL:
                    this.OnBoostMedalMerge(200);
                    ignoreDisplay = true;
                    break;
                    case EnumDefine.BOOST.PILE_MERGE_MEDAL:
                    this.OnBoostMedalMerge(500);
                    break;
                    case EnumDefine.BOOST.SPAWN_LEVEL_1:
                    this.OnBoostLevelMerge(2);
                    actionEndBoost = StopLevelMerge;
                    break;
                    case EnumDefine.BOOST.SPAWN_LEVEL_2:
                    case EnumDefine.BOOST.SPAWN_LEVEL_3:
                    this.OnBoostLevelMerge(3);
                    actionEndBoost = StopLevelMerge;
                    break;
                }

                //set time
                long now = TimeUtil.TimeStampSecond;
                long timeUp = now + (long) finalTime;                
                PlayerPrefs.SetString($"{ StringDefine.BOOST_TIME_UP}{ (int)boost}", timeUp.ToString());

                if (!ignoreDisplay) {
                    BoostNotify noty = Instantiate(_prfBoost, _parentBoost);
                    noty.SetBoost(data.Icon, data.Description);

                    BoostAction action = new BoostAction() {
                        TimeRemanin = finalTime,
                        StopBoost = actionEndBoost,
                        Data = data,
                        Noty = noty
                    };
                    _dictionBoost.Add(boost, action);
                }

                BoostAds[] list = FindObjectsOfType<BoostAds>();
                foreach (var item in list) {
                    item.UpdateStatus();
                }

                BoostItemAds[] list2 = FindObjectsOfType<BoostItemAds>();
                foreach (var item in list2) {
                    item.UpdateStatus();
                }
            }

            // count mission
            switch (boost) {
                case EnumDefine.BOOST.AUTO_MERGE_1:
                case EnumDefine.BOOST.AUTO_MERGE_2:
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.UseAutoMerge, EnumDefine.Mission.ActiveAutoMerge);
                break;
                case EnumDefine.BOOST.SPAWN_FASTER_1:
                case EnumDefine.BOOST.SPAWN_FASTER_2:
                case EnumDefine.BOOST.SPAWN_FASTER_3:
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.None, EnumDefine.Mission.ActiveSpawnTime);
                break;
                case EnumDefine.BOOST.DAMAGE_BOOST_1:
                case EnumDefine.BOOST.DAMAGE_BOOST_2:
                case EnumDefine.BOOST.DAMAGE_BOOST_3:
                case EnumDefine.BOOST.DAMAGE_BOOST_4:
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.None, EnumDefine.Mission.ActiveDpsBoost);
                break;
                default: break;
            }
        }

        public void AddFreeBoost(EnumDefine.FREEBOOST boostType, int time, int lv) {      
            
            if (_freeBoost.ContainsKey(boostType)) {                
                _freeBoost[boostType].TimeRemanin += time;                

                if (boostType == EnumDefine.FREEBOOST.AUTO_MERGE) {
                    string timeUp = PlayerPrefs.GetString($"{StringDefine.BOOST_TIME_UP}{(int)EnumDefine.BOOST.AUTO_MERGE_1}", TimeUtil.TimeStampSecond.ToString());
                    long timeUpLong = long.Parse(timeUp);
                    timeUpLong += time;                    
                    PlayerPrefs.SetString($"{ StringDefine.BOOST_TIME_UP}{ (int)EnumDefine.BOOST.AUTO_MERGE_1}", timeUpLong.ToString());
                }
            } else {                
                BoostAction action = new BoostAction();
                BoostData data = null;

                switch (boostType) {
                    case EnumDefine.FREEBOOST.MERGE_LEVEL:
                    OnBoostLevelMerge(lv);
                    action.StopBoost = StopLevelMerge;
                    data = _config.GetBoost(EnumDefine.BOOST.LEVEL_HIGHER_1);
                    UIManager.Instance.LockSpawnBeast(true);
                    break;
                    case EnumDefine.FREEBOOST.SPAWN_LEVEL:
                    OnBoostHigherLevel(lv);
                    if (lv == 1) action.StopBoost = Lower1LevelFree;
                    else action.StopBoost = Lower2LevelFree;
                    data = _config.GetBoost(EnumDefine.BOOST.SPAWN_LEVEL_1);
                    UIManager.Instance.LockSpawnBeast(true);
                    break;
                    case EnumDefine.FREEBOOST.SPAWN_TIME:
                    float hs = 1f - lv / 100f;
                    GameManager.Instance.BoostTimeSpawn(hs);
                    action.StopBoost = StopFastSpawnFree;
                    data = _config.GetBoost(EnumDefine.BOOST.SPAWN_FASTER_1);
                    break;
                    case EnumDefine.FREEBOOST.DPS:
                    this.OnDpsBoostFree(lv);
                    action.StopBoost = StopDPSFree;
                    data = _config.GetBoost(EnumDefine.BOOST.DAMAGE_BOOST_1);
                    break;
                    case EnumDefine.FREEBOOST.AUTO_MERGE:
                    GameManager.Instance.BoostAutoMerge(true);
                    action.StopBoost = StopAutoMerge;
                    data = _config.GetBoost(EnumDefine.BOOST.AUTO_MERGE_1);

                    //set time
                    long now = TimeUtil.TimeStampSecond;
                    long timeUp = now + time;                    
                    PlayerPrefs.SetString($"{ StringDefine.BOOST_TIME_UP}{ (int)EnumDefine.BOOST.AUTO_MERGE_1}", timeUp.ToString());
                    break;
                }

                BoostNotify noty = Instantiate(_prfBoost, _parentBoost);
                noty.SetBoost(data.IconFree != null ? data.IconFree : data.Icon, data.Description);

                action.TimeRemanin = time;
                action.Noty = noty;

                _freeBoost.Add(boostType, action);
                
            }
            // count mission
            switch (boostType) {
                case EnumDefine.FREEBOOST.SPAWN_TIME:
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.None, EnumDefine.Mission.ActiveSpawnTime);
                break;
                case EnumDefine.FREEBOOST.AUTO_MERGE:
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.UseAutoMerge, EnumDefine.Mission.ActiveAutoMerge);
                break;
                case EnumDefine.FREEBOOST.DPS:
                DailyQuestCtrl.Instane?.UpdateQuest(EnumDefine.DailyQuest.None, EnumDefine.Mission.ActiveDpsBoost);
                break;
                default: break;
            }

            //update trang thai cua cac nut
            BoostAds[] list = FindObjectsOfType<BoostAds>();
            foreach (var item in list) {
                item.UpdateStatus();
            }

            BoostItemAds[] list2 = FindObjectsOfType<BoostItemAds>();
            foreach(var item in list2) {
                item.UpdateStatus();
            }

        }

        public BoostAction GetBoostActive(EnumDefine.BOOST bost) {
            if (_dictionBoost.ContainsKey(bost)) return _dictionBoost[bost];
            return null;
        }

        public BoostData Boost(EnumDefine.BOOST bost) {
            return _config.GetBoost(bost);
        }  
        
        public BoostAction GetFreeBoostActive(EnumDefine.FREEBOOST boost) {
            if(_freeBoost.ContainsKey(boost)) {
                return _freeBoost[boost];
            }
            return null;
        }

        public class BoostAction {
            public float TimeRemanin;
            public UnityAction StopBoost;
            public BoostData Data;
            public BoostNotify Noty;
        }

        #region BOOST
        public void OnBoostMedalMerge(int mege) {
            Utils.AddMedalMerge(mege, true);
            this.PostEvent(EventID.OnUpdateAutoMergeMedal);
        }

        public void OnBoostAddGem(int gem) {
            Utils.AddRubyCoin(gem);
            this.PostEvent(EventID.OnUpDateMoney);
        }

        private void OnBoostSoul(BigInteger coin) {
            UIManager.Instance.UpdateMoneyCoin(coin, false);
        }

        private void OnDamageBoost(int n) {
            //if (_freeBoost.ContainsKey(EnumDefine.FREEBOOST.DPS))
            //    this.PostEvent(EventID.OnBoostDpsFree, n);
            //else
                this.PostEvent(EventID.OnBoostDPS, n);
        }

        private void OnDpsBoostFree(int n) {
            this.PostEvent(EventID.OnBoostDpsFree, n);
        }

        private void OnBoostLevelMerge(int n) {
            this.PostEvent(EventID.OnUpdateLevelMerge, n);
        }

        private void OnBoostHigherLevel(int n) {
            GameManager.Instance.boostSpawnLevel = n;
            int lv = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            lv += n;
            PlayerPrefs.SetInt(StringDefine.LEVEL_BEAST, lv);
            var beast = GameManager.Instance.GetBeast(lv);
            UIManager.Instance.SetPriceBeast(beast);
            GameManager.Instance.UpgradeBeast(lv, false);
        }

        public bool HasSpawnTimeBoost() {
            if (_dictionBoost.ContainsKey(EnumDefine.BOOST.SPAWN_FASTER_1) || _freeBoost.ContainsKey(EnumDefine.FREEBOOST.SPAWN_TIME))
                return true;
            return false;
        }
        #endregion

        private void StopFastSpawn() {
            if (!_freeBoost.ContainsKey(EnumDefine.FREEBOOST.SPAWN_TIME))
                GameManager.Instance.EndBoostTimeSpawn();
        }

        private void StopFastSpawnFree() {
            if (_dictionBoost.ContainsKey(EnumDefine.BOOST.SPAWN_FASTER_1) || _dictionBoost.ContainsKey(EnumDefine.BOOST.SPAWN_FASTER_2)
                || _dictionBoost.ContainsKey(EnumDefine.BOOST.SPAWN_FASTER_3)) {
                GameManager.Instance.EndBoostFreeTimeSpawn();
            } else {
                GameManager.Instance.EndBoostTimeSpawn();
            }
        }

        private void StopDPS() {
            Debug.LogError("stop dps");
            //if (!_freeBoost.ContainsKey(EnumDefine.FREEBOOST.DPS))
            //    this.PostEvent(EventID.OnBoostDPS, 1);

            if (_dictionBoost.ContainsKey(EnumDefine.BOOST.DAMAGE_BOOST_1)) {
                this.PostEvent(EventID.OnBoostDPS, 1);
            } else if (_dictionBoost.ContainsKey(EnumDefine.BOOST.DAMAGE_BOOST_2)) {
                this.PostEvent(EventID.OnBoostDPS, 1);
            } else if (_dictionBoost.ContainsKey(EnumDefine.BOOST.DAMAGE_BOOST_3)) {
                this.PostEvent(EventID.OnBoostDPS, 1);
            } else {
                this.PostEvent(EventID.OnBoostDPS, 1);
            }
        }

        private void StopDPSFree() {                        
            this.PostEvent(EventID.OnBoostDpsFree, 1);
                     
        }

        #region force stop
        public void ForceStopDPSFree(Action cb) {
            Debug.LogError("force stop dps free");
            //_freeBoost[EnumDefine.FREEBOOST.DPS].StopBoost?.Invoke();
            if (_freeBoost.ContainsKey(EnumDefine.FREEBOOST.DPS)) {
                _freeBoost[EnumDefine.FREEBOOST.DPS].Noty.DestroyMe();
                _freeBoost.Remove(EnumDefine.FREEBOOST.DPS);
                if(cb != null) {
                    cb();
                }
            }
           
        }

        public void ForceStopBoostFree(EnumDefine.FREEBOOST freeBoost, Action cb = null) {
            Debug.LogError("force stop free boost: " + freeBoost);
            if(_freeBoost.ContainsKey(freeBoost)) {
                _freeBoost[freeBoost].Noty.DestroyMe();
                _freeBoost.Remove(freeBoost);
                if(freeBoost == EnumDefine.FREEBOOST.SPAWN_TIME) {
                    StopFastSpawnFree();
                } else if (freeBoost == EnumDefine.FREEBOOST.SPAWN_LEVEL) {
                    LowerLevel(GameManager.Instance.boostSpawnLevel);
                }
                if (cb != null) cb();
            }
        }
       
        #endregion

        private void StopAutoMerge() {
            if (!_freeBoost.ContainsKey(EnumDefine.FREEBOOST.AUTO_MERGE))
                GameManager.Instance.BoostAutoMerge(false);
        }

        private void StopBoostAutoMergeFree() {
            if (!_dictionBoost.ContainsKey(EnumDefine.BOOST.AUTO_MERGE_1) && !_dictionBoost.ContainsKey(EnumDefine.BOOST.AUTO_MERGE_2))
                GameManager.Instance.BoostAutoMerge(false);
        }

        private void StopLevelMerge() {
            UIManager.Instance.LockSpawnBeast(false);
            this.PostEvent(EventID.OnUpdateLevelMerge, 1);
        }


        private void Lower2Level() {
            this.LowerLevel(3);
        }

        private void LowerLevel(int lvLow) {            
            GameManager.Instance.boostSpawnLevel = 0;
            int lv = PlayerPrefs.GetInt(StringDefine.LEVEL_BEAST, 0);
            lv -= lvLow;
            PlayerPrefs.SetInt(StringDefine.LEVEL_BEAST, lv);
            var beast = GameManager.Instance.GetBeast(lv);
            UIManager.Instance.SetPriceBeast(beast);
            GameManager.Instance.UpgradeBeast(lv, false);
            UIManager.Instance.LockSpawnBeast(false);
        }

        private void Lower1LevelFree() {
            UIManager.Instance.LockSpawnBeast(false);
            this.LowerLevel(1);
        }

        private void Lower2LevelFree() {
            UIManager.Instance.LockSpawnBeast(false);
            this.LowerLevel(2);
        }
    }
}

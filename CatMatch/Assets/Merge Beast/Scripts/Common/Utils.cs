using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using System.Numerics;

namespace MergeBeast {
    public class Utils {
        public static int GetDPSAscend(int level) {
            //Debug.LogError("level de tinh dps: " + level);
            return Mathf.RoundToInt(level * 3 * Mathf.Pow(1.01f, level));
        }
        public static BigInteger GetDameByLevel(int lv) {
            return BigInteger.Pow(2, lv + 1) - 2; // Convert.ToInt64(Mathf.Pow(2, lv + 1) - 2);
        }

        public static BigInteger GetDameByLevelAndAscend(int lv) {
            return (GetDameByLevel(lv) * (100 + GetTotalAscend())) / 100;
        }

        public static BigInteger GetHPEnemy(int lv) {
            int phanduchomuoi = lv % 10;

            BigInteger hp = phanduchomuoi % 3 == 0 ? GetDameByLevel(lv) * 960 : GetDameByLevel(lv) * 600;
            return hp;
        }

        public static BigInteger PriceOfBeast(int lv) {
            return (256 * lv * (BigInteger.Pow(2, lv + 1) - 2));
        }


        public static BigInteger PriceUpgradeBeast(int lv) // price upgrade beast to lv + 1
        {

            //BigInteger thamso = 4; //cong thuc cu
            //BigInteger thamso = (BigInteger)(2.8f + 1 / Mathf.Pow(1.2f, (lv - 1))); //cong thuc moi
            double total = 16384; // Price Level 0            
            for (int i = 1; i <= lv; i++) {
                double thamso = (2.8f + 1 / Mathf.Pow(1.2f, (i - 1))); //cong thuc moi                
                total *= thamso;
            }
            return (BigInteger)total;
        }

        public static BigInteger PriceUpgradeSpam(int lv) // price upgrade time spam beast
        {
            var total = 4096f; // Price Level 0
            for (int i = 2; i <= lv; i++) {
                total *= (2.8f + 1.2f / (lv - 1));
            }
            return (BigInteger)total;
        }

        public static BigInteger PriceUpgradeDoubleSpawn(int level) {
            var price = Mathf.Pow(2, 13) * Mathf.Pow(2.5f, level);
            return (BigInteger)price;
        }

        public static BigInteger PriceUpgradeLevelMerge(int level) {
            var price = Mathf.Pow(10, 13) * Mathf.Pow(10, level);
            return (BigInteger)price;
        }

        public static string FormatNumber(int number) {
            string str = number.ToString();
            if (str.Length > 9) {
                str = $"{SubNumber(str, 9)}B";
            } else
            if (str.Length > 6) {
                str = $"{SubNumber(str, 6)}M";
            } else
            if (str.Length > 3) {
                str = $"{SubNumber(str, 3)}K";
            }
            return str;
        }

        public static string FormatNumber(BigInteger number) {
            string str = number.ToString();

            if (str.Length > 51) {
                str = $"{SubNumber(str, 51)}mm";
            } else
            if (str.Length > 48) {
                str = $"{SubNumber(str, 48)}ll";
            } else
            if (str.Length > 45) {
                str = $"{SubNumber(str, 45)}kk";
            } else
            if (str.Length > 42) {
                str = $"{SubNumber(str, 42)}jj";
            } else
            if (str.Length > 39) {
                str = $"{SubNumber(str, 39)}ii";
            } else
            if (str.Length > 36) {
                str = $"{SubNumber(str, 36)}hh";
            } else
            if (str.Length > 33) {
                str = $"{SubNumber(str, 33)}gg";
            } else
            if (str.Length > 30) {
                str = $"{SubNumber(str, 30)}ff";
            } else
            if (str.Length > 27) {
                str = $"{SubNumber(str, 27)}ee";
            } else
            if (str.Length > 24) {
                str = $"{SubNumber(str, 24)}dd";
            } else
            if (str.Length > 21) {
                str = $"{SubNumber(str, 21)}cc";
            } else
            if (str.Length > 18) {
                str = $"{SubNumber(str, 18)}bb";
            } else
            if (str.Length > 15) {
                str = $"{SubNumber(str, 15)}aa";
            } else
            if (str.Length > 12) {
                str = $"{SubNumber(str, 12)}T";
            } else
            if (str.Length > 9) {
                str = $"{SubNumber(str, 9)}B";
            } else
            if (str.Length > 6) {
                str = $"{SubNumber(str, 6)}M";
            } else
            if (str.Length > 3) {
                str = $"{SubNumber(str, 3)}K";
            }

            return str;
        }

        private static string SubNumber(string number, int pow) {
            string sub = number.Substring(0, number.Length - pow + 2);
            string strRound = sub.Insert(sub.Length - 2, ".");
            return strRound;
        }

        public static int GetCurrentRubyMoney() {
            return PlayerPrefs.GetInt(StringDefine.MONEY_GEM);
        }

        public static void SetRubyMoney(int ruby) {
            PlayerPrefs.SetInt(StringDefine.MONEY_GEM, ruby);
            EventDispatcher.Instance.PostEvent(EventID.OnUpDateMoney, ruby);
        }

        public static void AddRubyCoin(int ruby) {
            SetRubyMoney(GetCurrentRubyMoney() + ruby);
            if (ruby < 0) {
                DailyQuestCtrl.Instane?.SpendGem(ruby);
            }
        }

        public static void SetStar(int star) {
            PlayerPrefs.SetInt(StringDefine.MONEY_MERGE, star);
            UIManager.Instance.UpdateMoneyMerge();
        }

        public static void AddStar(int star) {
            int current = PlayerPrefs.GetInt(StringDefine.MONEY_MERGE, 0);
            current += star;
            SetStar(current);
        }

        public static void SetTotalAscend(int percen) {
            int total = GetTotalAscend() + percen;
            PlayerPrefs.SetInt(StringDefine.TOTAL_DPS_ASCEND, total);
        }

        public static int GetTotalAscend() {
            return PlayerPrefs.GetInt(StringDefine.TOTAL_DPS_ASCEND);
        }

        public static void AddMedalMerge(int sl, bool isBuy = false) {
            
            int _totalAutoMerge = PlayerPrefs.GetInt(StringDefine.AUTO_MERGE, 50);
            //Debug.LogFormat("amount: {0}, current: {1}", sl, _totalAutoMerge);
            if (!isBuy && _totalAutoMerge > 100) {
               return;
            }
            _totalAutoMerge += sl;
            PlayerPrefs.SetInt(StringDefine.AUTO_MERGE, _totalAutoMerge);
        }

        public static List<int> ArrayPower2(int number) {
            List<int> _listNumber = new List<int>();
            int n = number;
            while (n > 0) {
                for (int i = 0; i < n; i++) {
                    var m = Mathf.Pow(2, i);
                    if (m >= n) {
                        int z = Mathf.Max(0, i - 1);
                        _listNumber.Add(z);
                        n -= (int)Mathf.Pow(2, z);
                        break;
                    }
                }
            }

            return _listNumber;
        }


        public static int GetHesoKillBoss(int level) {
            int phantrammuoi = level % 10;
            if (phantrammuoi == 0) return 3;
            if (phantrammuoi % 3 == 0) {
                return 2;
            } else {
                return 1;
            }
        }

        #region REward kill monster
        public static BigInteger GetSould(int _currentEnemyID) {
            int heso = GetHesoKillBoss(_currentEnemyID + 1);
            BigInteger soul = BigInteger.Pow(2, 14) * BigInteger.Pow(2, Mathf.Max(0, (_currentEnemyID + 1 - 7))) * heso;
            return soul;
        }

        public static int GetGem(int _currentEnemyID) {
            int heso = GetHesoKillBoss(_currentEnemyID + 1);
            int rate = 0;
            if (heso == 3) {
                rate = 100;
            } else if (heso == 2) {
                rate = 80;
            } else {
                rate = 50;
            }

            if (rate == 100 || UnityEngine.Random.Range(0, 100) > rate) {
                //nhan thuong
                int randomGem = UnityEngine.Random.Range(0, 100);
                if (randomGem > 98) return 5 * heso;
                else if (randomGem > 93) return 4 * heso;
                else if (randomGem > 83) return 3 * heso;
                else if (randomGem > 70) return 2 * heso;
                else if (randomGem > 40) return heso;
                else return 0;
            }

            return 0;
        }

        public static int GetMedalMerge(int _currentEnemyID) {
            int random = UnityEngine.Random.Range(0, 100);
            int heso = GetHesoKillBoss(_currentEnemyID + 1);
            int value = 1;
            if (random > 90) {
                value = 0;
            } else if (random > 70) {
                value = 3;
            } else if (random > 40) {
                value = 2;
            } else {
                value = 1;
            }
            int mm = GetRateMedalMerge(_currentEnemyID) * value * heso;
            //Debug.LogFormat("MM: {0}, rate: {1}, value: {2}, hero boss: {3}", mm, GetRateMedalMerge(_currentEnemyID), value, heso);
            return mm;
        }

        public static int GetBoostChest(int _currentEnemyID) {
            int heso = GetHesoKillBoss(_currentEnemyID + 1);

            int rate = 0;
            if (heso == 3) {
                rate = 100;
            } else if (heso == 2) {
                rate = 60;
            } else {
                rate = 40;
            }

            if (rate == 100 || UnityEngine.Random.Range(0, 100) > rate) {
                int random = UnityEngine.Random.Range(0, 100);
                if (random > 70) {
                    return Mathf.RoundToInt(0.7f * heso);
                } else if (random > 40) {
                    return Mathf.RoundToInt(0.5f * heso);
                } else {
                    return 0;
                }
            }
            return 0;
        }

        public static int GetRateMedalMerge(int _currentEnemyId) {
            return Mathf.FloorToInt((_currentEnemyId + 1 + 10) / 10);
        }
        #endregion

        public static int GetBuffMinute() {
            return 5 * 60;
        }

    }

    public class TimeUtil {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        public static long TimeStamp {
            get {
                return (long)(DateTime.UtcNow.Subtract(UnixEpoch)).TotalMilliseconds;
            }
        }

        public static long TimeStampSecond {
            get {
                return TimeStamp / 1000;
            }
        }
    }
}

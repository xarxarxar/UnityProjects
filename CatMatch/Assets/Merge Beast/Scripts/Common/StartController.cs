using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using SimpleJSON;

namespace MergeBeast
{
    public class StartController : MonoBehaviour
    {
        [SerializeField] private PoolManager _pool;
        [SerializeField] private Text _txtPercent;
        [SerializeField] private Text _txtLoading;
        [SerializeField] private Image _imgProgress;

        private void Start() {
            CPlayer.Init();
        }

        // Use this for initialization
        void Awake()
        {

            //StartCoroutine(GetConfig());
            //PlayerPrefs.DeleteKey(StringDefine.LIST_STAR_SHOP);
            

            _pool.ActionLoading += ProgressLoading;
            StartCoroutine(IELoadingFX());

            //Debug.LogError(Utils.GetTotalAscend());

            string lastDayLogin = PlayerPrefs.GetString(StringDefine.LASY_DAY_LOGIN);
            if (string.IsNullOrEmpty(lastDayLogin))
            {
                //Firebase.Analytics.FirebaseAnalytics.LogEvent(StringDefine.COUNT_ACTIVE_IN_DAY, new Firebase.Analytics.Parameter("Count_PLay",1));
            }
            else
            {
                TimeSpan time = DateTime.Today - Convert.ToDateTime(lastDayLogin);
                if(time.Days > 0)
                {
                    //Firebase.Analytics.FirebaseAnalytics.LogEvent(StringDefine.COUNT_ACTIVE_IN_DAY, new Firebase.Analytics.Parameter("Count_PLay", 1));
                    PlayerPrefs.SetString(StringDefine.LASY_DAY_LOGIN, DateTime.Today.ToString());
                    PlayerPrefs.SetInt("count_play", 1);
                }
                else
                {
                    int count = PlayerPrefs.GetInt("count_play");
                    count++;
                    //Firebase.Analytics.FirebaseAnalytics.LogEvent(StringDefine.COUNT_ACTIVE_IN_DAY, new Firebase.Analytics.Parameter("Count_PLay", count));
                    PlayerPrefs.SetInt("count_play", count);
                }
            }
            //PlayerPrefs.DeleteAll();
            //PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.BOOST_CHEST, 20);
            //PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_4H, 2);
            //PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + (int)EnumDefine.DailyBoost.TIME_JUMP_8H, 2);
            //Utils.SetRubyMoney(200000);
            //CPlayer.AddCommission(1000);
            //Utils.AddStar(10000);
            //PlayerPrefs.SetString(StringDefine.MONEY_COIN, "100000000000000000000000000000000");
            //PlayerPrefs.SetInt(StringDefine.LEVEL_SPAM, 48);
            //PlayerPrefs.SetInt(StringDefine.LEVEL_MONSTER, 49);
            //PlayerPrefs.SetInt(StringDefine.LEVEL_BEAST, 30);
            //PlayerPrefs.SetInt(StringDefine.CURRENT_SHOP_BEAST, 18);
            //PlayerPrefs.SetInt(StringDefine.HIGHEST_LEVEL_BEAST, 40);
            //        PlayerPrefs.SetInt(StringDefine.DAILY_BOOST + 1,1);
            //       PlayerPrefs.SetInt(StringDefine.MONEY_MERGE, 50000);
            //       PlayerPrefs.SetInt(StringDefine.TOTAL_DAY_REWARD, 2);
            //PlayerPrefs.SetInt(StringDefine.AUTO_MERGE, 200);
        }


        private IEnumerator IELoadingFX()
        {
            int count = 0;
            while (true)
            {
                int n = count % 4;
                switch (n)
                {
                    case 0:
                        _txtLoading.text = "加载中";
                        break;
                    case 1:_txtLoading.text = "加载中";
                        break;
                    case 2:_txtLoading.text = "加载中";break;
                    case 3:_txtLoading.text = "加载中";break;
                }

                count++;
                yield return new WaitForSeconds(0.3f);
            }
        }

        void ProgressLoading(float percent,int total)
        {
            float amount = percent / total;
            _txtPercent.text = $"{Mathf.RoundToInt(amount * 100)}%";
            _imgProgress.fillAmount = amount;
        }    

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tiledom;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn打开 http://web3incubators.com/
//电报https://t.me/gamecode999
namespace MergeBeast
{
    public static class CPlayer
    {
        public static int vipPoint;
        public static bool goToVip3;

        public static int currentLevel = 1;
        public static int levelUnlock = 1;
        public static int carot;
        public static int gem;
        public static int undoAmount;
        public static int commissionAmount;
        public static int shuffleAmount;
        public static int revivalAmount;
        public static int mcoin;
        public static bool OutMainGame = false;

        public static void Init()
        {
            vipPoint = PlayerPrefs.GetInt(StringDefine.VIP_POINT, 0);
            currentLevel = PlayerPrefs.GetInt(StringDefine.CURRENT_LEVEL, 1);
            levelUnlock = PlayerPrefs.GetInt(StringDefine.LEVEL_UNLOCK, 1);
            undoAmount = GetUndo();
            commissionAmount = GetCommission();
            shuffleAmount = GetShuffle();
            mcoin = GetMCoin();
        }

        #region VIP POINT 

        public static void AddVipPoint(int point)
        {
            vipPoint += point;
            SaveVipPoint();
        }

        public static void SaveVipPoint()
        {
            PlayerPrefs.SetInt(StringDefine.VIP_POINT, vipPoint);
            VipConfig vipConfig = GetVipConfig();
            if (vipConfig.disableBanner)
            {
                PlayerPrefs.SetInt(StringDefine.REMOVE_BANNER_ADS, 1);
            }
            if (vipConfig.disableVideo)
            {
                bool removeVideo = PlayerPrefs.GetInt(StringDefine.REMOVE_VIDEO_ADS, 0) == 1;
                if (!removeVideo)
                {
                    PlayerPrefs.SetInt(StringDefine.REMOVE_VIDEO_ADS, 1);
                }
            }
            UIManager.Instance.ShowQuickBuy(vipConfig.quickBuy);
        }

        public static VipConfig GetVipConfig()
        {
            for (int i = 0; i < GameAssets.Instance.listVipConfig.Count; i++)
            {
                VipConfig data = GameAssets.Instance.listVipConfig[i];
                if (vipPoint >= data.min && vipPoint <= data.max)
                {
                    return data;
                }
            }

            return null;
        }
        #endregion

        public static void SaveLevel()
        {
            PlayerPrefs.SetInt(StringDefine.CURRENT_LEVEL, currentLevel);
        }

        public static void SetLevel(int level)
        {
            currentLevel = level;
            SaveLevel();
        }

        public static void SaveUnlock()
        {
            PlayerPrefs.SetInt(StringDefine.LEVEL_UNLOCK, levelUnlock);
        }

        #region MCOIN
        public static int GetMCoin() {
            return PlayerPrefs.GetInt(StringDefine.MCOIN, 0);
        }
        public static void SaveMCoin() {
            PlayerPrefs.SetInt(StringDefine.MCOIN, mcoin);
            BroadCastReceiver.Broadcast(StringDefine.CHANGE_MCOIN);
        }

        public static void AddMCoin(int amount) {
            mcoin += amount;
            mcoin = Mathf.Max(0, mcoin);
            SaveMCoin();
        }
        #endregion

        #region UNDO

        public static void SaveUndo()
        {
            PlayerPrefs.SetInt(StringDefine.UNDO_AMOUNT, undoAmount);
            BroadCastReceiver.Broadcast(StringDefine.CHANGE_ITEM);
        }
        public static int GetUndo()
        {
            return PlayerPrefs.GetInt(StringDefine.UNDO_AMOUNT, 10);
        }
        public static void AddUndo(int amount)
        {
            undoAmount += amount;
            undoAmount = Mathf.Max(0, undoAmount);
            SaveUndo();
        }
        public static void SetUndo(int value)
        {
            undoAmount = value;
            undoAmount = Mathf.Max(0, undoAmount);
            SaveUndo();
        }
        #endregion

        #region COMMISSION

        public static void SaveCommission()
        {
            PlayerPrefs.SetInt(StringDefine.COMMISSION_AMOUNT, commissionAmount);
            BroadCastReceiver.Broadcast(StringDefine.CHANGE_ITEM);
        }

        public static int GetCommission()
        {
            return PlayerPrefs.GetInt(StringDefine.COMMISSION_AMOUNT, 10);
        }
        public static void AddCommission(int amount)
        {
            commissionAmount += amount;
            commissionAmount = Mathf.Max(0, commissionAmount);
            SaveCommission();
        }
        public static void SetCommission(int value)
        {
            commissionAmount = value;
            commissionAmount = Mathf.Max(0, commissionAmount);
            SaveCommission();
        }

        #endregion

        #region SHUFFLE
        public static void SaveShuffle()
        {
            PlayerPrefs.SetInt(StringDefine.SHUFFLE_AMOUNT, shuffleAmount);
            BroadCastReceiver.Broadcast(StringDefine.CHANGE_ITEM);
        }
        public static int GetShuffle()
        {
            return PlayerPrefs.GetInt(StringDefine.SHUFFLE_AMOUNT, 10);
        }
        public static void AddShuffle(int amount)
        {
            shuffleAmount += amount;
            shuffleAmount = Mathf.Max(0, shuffleAmount);
            SaveShuffle();
        }
        public static void SetShuffle(int value)
        {
            shuffleAmount = value;
            shuffleAmount = Mathf.Max(0, shuffleAmount);
            SaveShuffle();
        }
        #endregion

        #region REVIVAL
        public static void SaveRevival() {
            PlayerPrefs.SetInt(StringDefine.REVIVE_AMOUNT, revivalAmount);
            BroadCastReceiver.Broadcast(StringDefine.CHANGE_ITEM);
        }
        public static int GetRevival() {
            return PlayerPrefs.GetInt(StringDefine.REVIVE_AMOUNT, 0);
        }
        public static void AddRevival(int amount) {
            revivalAmount += amount;
            revivalAmount = Mathf.Max(0, revivalAmount);
            SaveRevival();
        }
        public static void SetRevival(int value) {
            revivalAmount = value;
            revivalAmount = Mathf.Max(0, revivalAmount);
            SaveRevival();
        }
        #endregion


    }
}
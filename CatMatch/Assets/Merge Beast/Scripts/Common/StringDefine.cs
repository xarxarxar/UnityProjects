using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    public class StringDefine
    {
        public const string BEAST_TRANING_INDEX = "BeastTraningIndex";
        public const string BEAST_BATTLE_INDEX = "BeastBattleIndex";
        public const string FIRST_OPEN_GAME = "FirstOpenGame";
        public const string BATTLE_SLOT_0 = "BattleSlot0";
        public const string BATTLE_SLOT_5 = "BattleSlot5";
        public const string TRANING_SLOT = "TrainingSlot";
        public const string BATTLE_SLOT = "BattleSlot";
        public const string AUTO_MERGE = "AutoMerge";
        public const string LEVEL_BEAST = "LevelBeast";
        public const string LEVEL_SPAM = "LevelSpam";
        public const string LEVEL_DOUBLE_SPAWN = "LEVEL_DOUBLE_SPAWN";
        public const string LEVEL_LEVEL_MERGE = "LEVEL_LEVEL_MERGE";
        public const string LEVEL_MONSTER = "LevelMonster";
        public const string CHARACTER_MONSTER = "CharacterMonster";
        public const string MONSTER_HP = "MonsterHP";
        public const string MONEY_COIN = "MoneyCoins";
        public const string MONEY_GEM = "MoneyGems";
        public const string MONEY_MERGE = "MoneyMerge";
        public const string TIME_ATTACK = "TimeAttack";
        public const string TIME_BOOST = "TimeBoost";
        public const string FREE_BOOST = "FreeBoost_";
        public const string HIGHEST_LEVEL_BEAST = "BeastHigest";
        public const string COUNT_DOWN_FREE_ADS = "CoundownFreeAds";
        public const string TIME_REFRESH_STAR_SHOP = "RefreshStarShop";
        public const string HAS_TUTORIAL_UPGRADE = "has_tutorial_upgrade";
        public const string HAS_TUTORIAL_FREEBOOST = "has_tutorial_freeboost";
        public const string TOTAL_DAY_REWARD = "total-day-reward";
        public const string DAILY_BOOST = "daily-boost-";
        public const string LAST_DAY_REWARD = "last-day-reward";
        public const string CHECK_POINT_OFFLINE = "check-point-offline";
        public const string LASY_DAY_LOGIN = "Last_Day_Login";
        public const string TOTAL_DPS_ASCEND = "total_dps_ascend";
        public const string OLD_LEVEL_ASCEND = "old_level_monster_ascend";
        public const string SOUND_KEY = "sound-key";
        public const string MUSIC_KEY = "music-key";
        public const string HIGHER_LV_BEAST_ASCEND = "higher-level-before-ascend";
        public const string KEY_PLAYER_NAME = "key_player_name";

        public const string RATE_SUCCESS = "RateSuccess";
        public const string LIST_STAR_SHOP = "ListStarShop";
        public const string CURRENT_SHOP_BEAST = "CurrentShopBeast";//con beast to nhat ma trong shop co the mua dc        
        public const string ASCEND_REWARD = "AscendReward";
        public const string BOOST_TIME_UP = "BoostTimeUp"; //Thời gian mà các boost sẽ hết hiệu lực, tính theo timestamp
        public const string BOOST_FREE_SLOT1 = "BoostFreeSlot1"; //data cuar cac freebost format type-%-timeup
        public const string BOOST_FREE_SLOT2 = "BoostFreeSlot2";
        public const string BOOST_CHEST = "BoostChest";
        public const string VIP_POINT = "VIP_POINT";
        public const string REMOVE_VIDEO_ADS = "REMOVE_VIDEO_ADS";
        public const string REMOVE_BANNER_ADS = "REMOVE_BANNER_ADS";
        public const string VIP_REWARDED = "VIP_REWARDED";

        public const string TOTAL_FREE_VIDEO = "TOTAL_FREE_VIDEO"; //tong so video trong 1 ngay
        public const string DAY_FREE_VIDEO = "DAY_INTERSTITIAL"; //ngay xem video gan nhat

        public const string MONTH_CARD = "month_card";
        public const string WEEKLY_1 = "weekly_card_1";
        public const string WEEKLY_2 = "weekly_card_2";

        public const string TOTAL_PIECE = "TOTAL_PIECE"; //tong so manh ghep nhan dc        

        //SHOP IAP
        public const string BUY_MONTH_CARD = "BUY_MONTH_CARD";
        public const string DAY_CLAIM_MONTH_CARD = "DAY_CLAIM_MONTH_CARD";
        public const string DAY_COUNT_MONTH = "DAY_COUNT_MONTH";

        public const string BUY_WEEK_CARD_1 = "BUY_WEEK_CARD_1";
        public const string DAY_CLAIM_WEEK_CARD_1 = "DAY_CLAIM_WEEK_CARD_1";
        public const string DAY_COUNT_WEEK_1 = "DAY_COUNT_WEEK_1";

        public const string BUY_WEEK_CARD_2 = "BUY_WEEK_CARD_2";
        public const string DAY_CLAIM_WEEK_CARD_2 = "DAY_CLAIM_WEEK_CARD_2";
        public const string DAY_COUNT_WEEK_2 = "DAY_COUNT_WEEK_2";

        public const string BUY_GEM_STATUS = "BUY_GEM_STATUS"; //luu trạng thái của các gói gem để biết mua lần mấy

        // Key Log Event Firebase
        public const string ADS_FREEBOOST = "Ads_FreeBoost";
        public const string ADS_RECEIVED_BIG_BEAST = "Ads_Received_Big_Beast";
        public const string ADS_RECEIVED_BEAST_BOOK = "Ads_Received_Beast_In_Book";
        public const string ADS_RECEIVED_BOOST = "Ads_Received_Boost";
        public const string ADS_RECEIVED_NEW_BEAST = "Ads_Received_New_Beast";
        public const string ADS_OFFLINE_REWARD_X3 = "Ads_Offline_Reward_x3";
        public const string ADS_SHOP_SOUL = "Ads_Shop_Soul";
        public const string USER_FIRST_LUANCH = "User_First_Luanch";
        public const string USER_ACTIVE = "User_Active";
        public const string HIGHER_LV_QUIT = "Highest_Lv_When_User_Quit";
        public const string COUNT_ACTIVE_IN_DAY = "Count_Active_In_Day";

        public const string SCENE_MAIN = "Main";
        public const string SCENE_MAP = "Map";
        public const string SCENE_TILE = "TileMonster";
        public const string SCENE_CRUSH = "InGame";

        public const int NULL = -99;

        public static string MGS_1 = "欢迎来到猫的世界";
        public static string MGS_2 = "组建强大的猫咪联盟，以猫制猫";
        public static string MGS_3 = "合成同等级的猫，获得更强的猫";
        public static string MGS_4 = "将猫放置在战斗格子上与猫战斗";
        public static string MGS_5 = "攻击猫时它们会掉落灵魂碎片，攻击直到所有灵魂碎片掉落";
        public static string MGS_6 = "灵魂碎片可用于购买新的或升级的猫";
        public static string MGS_7 = "干得好！！";
        public static string MGS_8 = "现在我来指导你加速猫的击杀进程";
        public static string MGS_9 = "DPS提升2倍，所有猫对猫的伤害翻倍，持续5分钟，试试吧";


        #region TILE MONSTER
        public static readonly string MOVE_DONE = "MOVE_DONE";
        public static readonly string CHANGE_LANGUAGE = "CHANGE_LANGUAGE";
        public static readonly string CHANGE_ITEM = "CHANGE_ITEM";
        public static readonly string CHANGE_MONEY = "CHANGE_MONEY";
        public static readonly string CHANGE_CURRENT_POINT = "CHANGE_CURRENT_POINT"; //so point con lai cua man choi
        public static readonly string COLLECT_POINT = "COLLECT_POINT";// khi nguoi choi coolect point
        public static readonly string MATCH_TILE = "MATCH_TILE"; //khi move 3 item xuong duoi va match
        public static readonly string LEVEL_COMPLETE = "LEVEL_COMPLETE";
        public static readonly string CHANGE_VIP_POINT = "CHANGE_VIP_POINT";
        public static readonly string DAILY_GET_REWARD = "DAILY_GET_REWARD";
        public static readonly string CHANGE_THEME = "CHANGE_THEME";
        public static readonly string CHANGE_MCOIN = "CHANGE_MCOIN";

        //
        public static readonly string CURRENT_LEVEL = "CURRENT_LEVEL";
        public static readonly string LEVEL_UNLOCK = "LEVEL_UNLOCK";
        public static readonly string UNDO_AMOUNT = "UNDO_AMOUNT";
        public static readonly string COMMISSION_AMOUNT = "COMMISSION_AMOUNT";
        public static readonly string SHUFFLE_AMOUNT = "SHUFFLE_AMOUNT";
        public static readonly string REVIVE_AMOUNT = "REVIVE_AMOUNT";
        public static readonly string LAST_DAY_TILE = "LAST_DAY_TILE";
        public static readonly string COUNT_TILE_TURN = "COUNT_TILE_TURN"; //so luot choi con lai trong ngay
        public static readonly string COUNT_BUY_TURN = "COUNT_BUY_TURN";// so lan da mua trong ngay
        public static readonly string LAST_DAY_BUY_TURN = "LAST_DAY_BUY_TURN";// ngay gan nhat mua luot
        public static readonly string PUZZLE_INDEX = "PUZZLE_INDEX"; //index cua puzzle to
        public static readonly string MCOIN = "MCOIN";


        #endregion
    }
}

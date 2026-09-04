using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    public class EnumDefine
    {
        public enum ADSTYPE
        {
            Rewarded, Interstitial
        }
        public enum GameType
        {
            Beast, Tile, Crush
        }
        public enum BEAST : int
        {
            PIG_WINGED = 0,
            THUNDER_HEDGEHOG,
            MOTHER_OF_PIG,
            FUNNY_FOX,
            PIG_FHISHY,
            BROWN_DOG,
            AKRAKORA,
            ABOLETH,
            ANGEL,
            PLANETAR,
            SOLAR,
            ANIMATED_ARMOR,
            ANKHEG,
            AZER,
            BANSHEE,
            BASILISK,
            BEHIR,
            BEHOLDER,
            DEATH_TYRANT,
            SPECTATOR,
            TWIG_BLIGHT,
            VINE_BLIGHT,
            NEEDLE_BLIGHT,
            BUGBEER,
            BEAST_25,
            BEAST_26,
            BEAST_27,
            BEAST_28,
            BEAST_29,
            BEAST_30,
            BEAST_31,
            BEAST_32,
            BEAST_33,
            BEAST_34,
            BEAST_35,
            BEAST_36,
            BEAST_37,
            BEAST_38,
            BEAST_39,
            BEAST_40,
            BEAST_41,
            BEAST_42,
            BEAST_43,
            BEAST_44,
            BEAST_45,
            BEAST_46,
            BEAST_47,
            BEAST_48,
            BEAST_49,
            BEAST_50,
            BEAST_51,
            BEAST_52,
            BEAST_53,
            BEAST_54,
            BEAST_55,
            BEAST_56,
            BEAST_57,
            BEAST_58,
            BEAST_59,
            BEAST_60,
            BEAST_61,
            BEAST_62,
            BEAST_63,
            BEAST_64,
            BEAST_65,
            BEAST_66,
            BEAST_67,
            BEAST_68,
            BEAST_69,
            BEAST_70,
            BEAST_71,
            BEAST_72,
            BEAST_73,
            BEAST_74,
            BEAST_75,
            BEAST_76,
            BEAST_77,
            BEAST_78,
            BEAST_79,
            BEAST_80,
            BEAST_81,
            BEAST_82,
            BEAST_83,
            BEAST_84,
            BEAST_85,
            BEAST_86,
            BEAST_87,
            BEAST_88,
            BEAST_89,
            BEAST_90,
            BEAST_91,
            BEAST_92,
            BEAST_93,
            BEAST_94,
            BEAST_95,
            BEAST_96,
            BEAST_97,
            BEAST_98,
            BEAST_99,
            BEAST_100,
        }

        public enum MONSTER : int
        {
            Baiko,
            Bean,
            Dalmatian,
            Fishie,
            Flaime,
            Flash,
            Gostie,
            Hero_Cat,
            Holi,
            Jiao,
            Kagura,
            Kina,
            Hero_Cat_2,
            Kyuzo,
            Mijus,
            Miu,
            Modie,
            Mora,
            Naruto,
            Nemu,
            Pank,
            Pomper,
            Seiji,
            Shiba_Fire,
            Sun,
            The_Elder_One,
            Tsuki,
            Bomerang,
            Ceshi,
            Little_Miju,
            Pomper_Mace,
            Shiba_Dark,
            Shiba_Light,
            Shiba_Thuner,
            Shiba_Water,
            The_Elder_Two,
            Antler_Sheep,
            Bee,
            Devil_Dragon,
            Monkey_Mob,
            The_Elder_Three,
            Drake,
            Agnes,
            Beloved,
            Cuddle_bug,
            Darling,
            Dearie,
            Terry,
            Rowan,
            Victor,
            Finn,
            Eric,
            Duane,
            Matilda,
            Harvey,
            Arnold,
            Phelan,
            Ebola,
            Grainne,
            Snuggler,
            Boo,
            Azura,
            Godiva,
            Kaylin,
            Kitten,
            Ladonna,
            Maynard,
            Phedra
        }

        public enum BOOST : byte
        {
            DAMAGE_BOOST_1,
            DAMAGE_BOOST_2,
            DAMAGE_BOOST_3,
            DAMAGE_BOOST_4,
            SERVARAL_SOUL,
            PACK_SOUL,
            CHEST_SOUL,
            SPAWN_LEVEL_1,
            SPAWN_LEVEL_2,
            SPAWN_LEVEL_3,
            AUTO_MERGE_1,
            AUTO_MERGE_2,
            FEW_MERGE_MEDAL,
            SEVERAL_MERGE_MEDAL,
            PILE_MERGE_MEDAL,
            LEVEL_HIGHER_1,
            LEVEL_HIGHER_2,
            LEVEL_HIGHER_3,
            SPAWN_FASTER_1,
            SPAWN_FASTER_2,
            SPAWN_FASTER_3,
            PILE_SOUL,//30p
        }

        public enum FREEBOOST : byte
        {
            WOOD_BOX,
            SOUL,
            MERGE_LEVEL,
            SPAWN_LEVEL,
            SPAWN_TIME,
            DPS,
            AUTO_MERGE
        }

        public enum HERO : byte
        {
            ARIAS,
            DAVEY,
            LILIN,
            YUNA,
            LOTUS,
            LUCID,
            QUAN,
            KOA,
            TONG
        }

        public enum FXTYPE : byte
        {
            HEALING_HP,
            GET_HIT,
            ENEMY_DEAD,
            BEAST_MERGE,
            NEW_BEAST
        }

        public enum SEX : byte
        {
            MALE,
            FEAMALE
        }

        public enum RARITY : byte
        {
            COMMON,
            EPIC,
            RARE
        }

        public enum SCREEN : byte
        {
            UPGRADE,
            CONFIRM,
            VICTORY,
            BOOST,
            BOOK,
            FADE_SCR,
            TOOLTIP_ENEMY,
            NEW_BEAST,
            IAP,
            SHOP_SOUL,
            BIG_BEAST,
            STAR_SHOP,
            UPGRADE_TUTORIAL,
            ASCEND,
            SETTING,
            RATING,
            VIP,
            BATTLE,
            TEAM_BUILD,
            TEMP
        }

        public enum BOOSTTYPE : byte
        {
            CASH,
            FREE,
            ADS
        }

        public enum SOUND : byte
        {
            BEAST_ATTACK,
            BEAST_SPAM,
            BOSS_DIE,
            BUTTON_CLICK,
            BOSS_BUFF_HP,
            BEAST_MERGE,
            BEAST_NEW,
            TileClick
        }

        public enum DailyRewardType : int
        {
            SOUL,
            MERGE,
            TIME_JUMP,
            BOOST_CHEST,
            GEM,
            AUTO_MERGE,
            STAR,
        }

        public enum DailyBoost : int
        {
            BOOST_CHEST = 0,
            TIME_JUMP_2H,
            TIME_JUMP_4H,
            TIME_JUMP_8H
        }

        public enum DailyQuest : int
        {
            Merge,
            FreeBoost,
            Upgrade,
            QuickBuyBeast,
            UseMedalMerge,
            Ascend,
            DefeatBoss,
            WatchAds,
            UseAutoMerge,
            BuyStarShop,
            None
        }

        public enum Mission : int
        {
            RaidBoss,
            UnlockBeast,
            ReachStar,
            GetBeast,
            UpgradeLevelSpawn,
            UpgradeTimeSpawn,
            ActiveFreeBoost,
            SpendGem,
            //CollectSoul,
            UseMergeMedal,
            ActiveDpsBoost,
            ActiveSpawnTime,
            ActiveAutoMerge,
            KillEnemies,//giet tong so quai
            Ascend,
            None
        }

        public enum ShopPack
        {
            Month, Week1, Week2
        }

        public enum UpgradeItemType
        {
            LevelSpawn, TimeSpawn, DoubleSpawn, LevelMerge
        }

        public enum TileLoot
        {
            Soul = 20,
            AutoMerge,
            Dps,
            SpawnTime,
            BoostChest,
            Gem,
            Star,
            MedalMerge,
            Robin,
            MCoin,
            Undo,
            Commit,
            Shuffle,
            Revive,
            Chaos
        }
    }
}

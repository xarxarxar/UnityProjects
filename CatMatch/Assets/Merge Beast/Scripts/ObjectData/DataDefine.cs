using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
     

namespace MergeBeast
{
    [System.Serializable]
    public class BeastData 
    {
        public int ID;
        public int Level;
        public string Name;
        public Sprite Character;
    }

    [System.Serializable]
    public class MonsterData
    {
        public int ID;
        public int Level;
        public string Name;
    //    public Sprite Display;
        public SkeletonDataAsset Skeleton;
    }

    [System.Serializable]
    public class BoostData
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public Sprite IconFree;
        public float Time;
        public int Price;
        public int Free;
    }

    [System.Serializable]
    public class HeroData
    {
        public string Name;
        public EnumDefine.SEX Sex;
        public EnumDefine.RARITY Rarity;
        public float MinDps;
        public float MaxDps;
        public float MinLuck;
        public float MaxLuck;
        public List<int> Slot;
        public Sprite Character;
        public Sprite Avatar;
        public Sprite SkillIcon;
    }

    public class GiftBeast
    {
        public int Slot;
        public int ID;
        public bool IsSpawn;
    }


    [System.Serializable]
    public class IAPPack
    {
        public string Name;
        public string PurchaseID;
        public int ID;
        public float GemReceived;
        public float Price;
        public Sprite Icon;
        public int vipPoint;
    }

    [System.Serializable]
    public class BoostFree
    {
        public string Name;
        public EnumDefine.FREEBOOST FreeBoost;
        public Sprite ImgNormal;
        public Sprite ImgFlash;
        public Sprite ImgIcon;
        public Sprite ImgIconMaxBoost;
        public int TimeFreeBoost;
    }

    [System.Serializable]
    public class DailyQuestType
    {
        public EnumDefine.DailyQuest EnumType;
        public List<int> Types;
        public string Description;
        public Sprite IconQuest;
    }

    [System.Serializable]
    public class MissionType
    {
        public string MissionName;
        public EnumDefine.Mission Type;
        public List<int> Targets;
        public string Description;
        public Sprite IconMission;
    }


    [System.Serializable]
    public class DailyQuestDictionary : SerializableDictionary<EnumDefine.DailyQuest, DailyQuestType> { }

    [System.Serializable]
    public class BeastDictionary : SerializableDictionary<EnumDefine.BEAST, BeastData> { }

    [System.Serializable]
    public class MonsterDictionary : SerializableDictionary<EnumDefine.MONSTER, MonsterData> { }

    [System.Serializable]
    public class BoostDictionary : SerializableDictionary<EnumDefine.BOOST, BoostData> { }

    [System.Serializable]
    public class HeroDictionary : SerializableDictionary<EnumDefine.HERO, HeroData> { }

    [System.Serializable]
    public class PoolDictionary : SerializableDictionary<EnumDefine.FXTYPE, List<ParticleSystem>> { }

    [System.Serializable]
    public class SoundDictionary : SerializableDictionary<EnumDefine.SOUND, AudioClip> { }
}

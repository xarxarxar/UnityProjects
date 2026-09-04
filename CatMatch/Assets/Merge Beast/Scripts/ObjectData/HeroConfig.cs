using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "HeroConfig", menuName = "ConfigData/HeroConfig")]
    public class HeroConfig : ScriptableObject
    {

        public HeroDictionary Heroes;

        public HeroData GetHero(EnumDefine.HERO hero)
        {
            if (Heroes.ContainsKey(hero))
                return Heroes[hero];
            return null;
        }

    }
}

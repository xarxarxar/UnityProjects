using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "BoostConfig", menuName = "ConfigData/BoostConfig")]
    public class BoostConfig : ScriptableObject
    {
        public BoostDictionary Boost;

        public BoostData GetBoost(EnumDefine.BOOST boost)
        {
            if (Boost.ContainsKey(boost))
                return Boost[boost];
            return null;
        }
    }
}

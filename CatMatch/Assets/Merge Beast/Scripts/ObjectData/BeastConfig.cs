using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "BeastConfig", menuName = "ConfigData/BeastConfig")]
    public class BeastConfig : ScriptableObject
    {
        public BeastDictionary BeastData;

        public BeastData GetBeast(EnumDefine.BEAST beast)
        {
            if (BeastData.ContainsKey(beast))
                return BeastData[beast];
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "DailyItemConfig", menuName = "ConfigData/DailyItemConfig")]
    public class DailyConfig : ScriptableObject
    {
        public List<DailyItem> TotalDaily;
    }

    [System.Serializable]
    public class DailyItem
    {
        public string DayStt;
        public string Name;
        public int Id;
        public Sprite Icon;
        public EnumDefine.DailyRewardType DailyType;
        public int HeSo;
        public int SoMu;
        public string Description;
        public string SymbolReceived;
    }

}

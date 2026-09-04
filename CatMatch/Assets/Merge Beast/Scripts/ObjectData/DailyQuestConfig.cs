using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "DailyQuestConfig", menuName = "ConfigData/DailyQuestConfig")]
    public class DailyQuestConfig : ScriptableObject
    {
        public DailyQuestDictionary DailyQuest;
        public List<MissionType> ListMission;
    }

    
}


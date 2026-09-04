using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    [CreateAssetMenu(fileName = "BoostFreeConfig", menuName = "ConfigData/BoostFreeConfig")]
    public class FreeBoostConfig : ScriptableObject
    {

        public List<BoostFree> ListBoost;

        public BoostFree GetRandomBoost()
        {
            return ListBoost[Random.Range(0, ListBoost.Count)];
        }

        public BoostFree GetBoost(int index) {
            return ListBoost[index];
        }
    }
}

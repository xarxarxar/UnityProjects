namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "JokerDataSO", menuName = "JokerDataSO")]
    public class JokerDataSO : ScriptableObject
    {
        [SerializeField] private int jokerCount;
        public int JokerCount
        {
            get { return jokerCount; }
            set
            {
                if (value >= 0)
                {
                    jokerCount = value;
                }
            }
        }
    
        [SerializeField] private int priceLevel;
        public int PriceLevel { get { return priceLevel; } set { if (value < PriceList.Count) priceLevel = value; } }
    
        public List<int> PriceList;
    
    
        [Button]
        public void ResetData()
        {
            JokerCount = 0;
            PriceLevel = 0;
        }
    }
    
}

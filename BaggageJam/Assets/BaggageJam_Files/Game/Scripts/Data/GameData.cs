namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using NaughtyAttributes;
    using System;
    [CreateAssetMenu(fileName = "Data", menuName = "GameData", order = 0)]
    public class GameData : ScriptableObject
    {
        public float totalMoney;
        public int levelCount;
        public float conveyorSpeed = 1;
    
    
        public AllJokerButtonData AllJokerButtonSO;
        public AllSo AllSo;
    
    
        [Button]
        void ResetData()
        {
            totalMoney = 0;
            levelCount = 1;
            conveyorSpeed = 1;
            AllJokerButtonSO.BackJokerDataSO.ResetData();
            AllJokerButtonSO.HintJokerDataSO.ResetData();
            AllJokerButtonSO.ShuffleJokerDataSO.ResetData();
            AllJokerButtonSO.TimeJokerDataSO.ResetData();
        }
    
        [Button]
        void FullSource()
        {
            totalMoney = 10000;
        }
    }
    
    
    [Serializable]
    public class AllJokerButtonData
    {
        public JokerDataSO BackJokerDataSO;
        public JokerDataSO HintJokerDataSO;
        public JokerDataSO ShuffleJokerDataSO;
        public JokerDataSO TimeJokerDataSO;
    }
    
    [Serializable]
    public class AllSo
    {
        public AISO AISO;
        public BaggageSO BaggageSO;
        public LevelDataSO LevelDataSO;
        public ParticleSO ParticleSO;
    }
    
    
}

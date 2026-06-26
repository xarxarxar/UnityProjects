namespace EKStudio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public List<Level> Levels;
    }
    
    [Serializable]
    public class Level
    {
        public int winMoney;
        public List<LevelStickmanProperty> LevelStickmanProperty;
    }
    
    [Serializable]
    public class LevelStickmanProperty
    {
        public BaggageType StickManType;
        public int StickManCount;
    }
    
}

namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public static class AllSaveManager
    {
        public static void Save(GameData data)
        {


            SaveManager.SaveData(data);
    
            SaveManager.SaveData(data.AllJokerButtonSO.HintJokerDataSO, "HintJokerDataSO");
            SaveManager.SaveData(data.AllJokerButtonSO.BackJokerDataSO, "BackJokerDataSO");
            SaveManager.SaveData(data.AllJokerButtonSO.TimeJokerDataSO, "TimeJokerDataSO");
            SaveManager.SaveData(data.AllJokerButtonSO.ShuffleJokerDataSO, "ShuffleJokerDataSO");
        }
    
        public static void Load(GameData data)
        {
            SaveManager.LoadData(data);
    
            SaveManager.LoadData(data.AllJokerButtonSO.HintJokerDataSO, "HintJokerDataSO");
            SaveManager.LoadData(data.AllJokerButtonSO.BackJokerDataSO, "BackJokerDataSO");
            SaveManager.LoadData(data.AllJokerButtonSO.TimeJokerDataSO, "TimeJokerDataSO");
            SaveManager.LoadData(data.AllJokerButtonSO.ShuffleJokerDataSO, "ShuffleJokerDataSO");
        }
    }
    
}

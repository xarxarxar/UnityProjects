namespace EKStudio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine;
    public class LevelManager : InstanceManager<LevelManager>
    {
        [SerializeField] private GameData gameData;
        [Range(1, 100)]
        public int mainBaggageRatio;
        public List<BaggageType> stickmanList;
        public List<BaggageType> baggageList;
        public List<BaggageType> otherBaggageList;
    
        void Start()
        {
            CreateStickmanList();
            InvokeRepeating(nameof(SpawnStickman), .1f, 2f);
            InvokeRepeating(nameof(SpawnBaggage), .1f, .1f);
    
        }
    
        private void SpawnBaggage()
        {
            ConveyorBelt.Instance.SpawnBaggage();
        }
    
        private void SpawnStickman()
        {
            AIManager.Instance.SpawnAI();
        }
    
        void ShuffleList<T>(List<T> list)
        {
            list.Sort((x, y) => Random.Random.Range(-1, 2));
        }
    
        void CreateStickmanList()
        {
            int levelStickmanProperty = gameData.AllSo.LevelDataSO.Levels[gameData.levelCount - 1].LevelStickmanProperty.Count;
    
            for (int i = 0; i < levelStickmanProperty; i++)
            {
                for (int j = 0; j < gameData.AllSo.LevelDataSO.Levels[gameData.levelCount - 1].LevelStickmanProperty[i].StickManCount; j++)
                {
                    stickmanList.Add(gameData.AllSo.LevelDataSO.Levels[gameData.levelCount - 1].LevelStickmanProperty[i].StickManType);
                }
            }
    
            for (int i = 0; i < levelStickmanProperty; i++)
            {
                for (int j = 0; j < gameData.AllSo.LevelDataSO.Levels[gameData.levelCount - 1].LevelStickmanProperty[i].StickManCount * 3; j++)
                {
                    baggageList.Add(gameData.AllSo.LevelDataSO.Levels[gameData.levelCount - 1].LevelStickmanProperty[i].StickManType);
                }
            }
    
            ShuffleList<BaggageType>(baggageList);
    
            for (int i = 0; i < 100; i++)
            {
                // BaggageType[] AIArray = (BaggageType[])Enum.GetValues(typeof(BaggageType)); ;
                // List<BaggageType> AITypes = new List<BaggageType>(AIArray);
    
                otherBaggageList.Add((BaggageType)Random.Random.Range(1, Enum.GetValues(typeof(BaggageType)).Length));
            }
    
            if (gameData.levelCount != 1)
            {
                for (int i = 0; i < stickmanList.Count; i++)
                {
                    int r = Random.Random.Range(0, stickmanList.Count);
    
                    BaggageType temp = stickmanList[r];
                    stickmanList[r] = stickmanList[i];
                    stickmanList[i] = temp;
                }
    
                System.Random rnd = new();
                double ratio = 0.1;
    
                int mixCount = (int)(ratio * baggageList.Count);
    
                for (int i = 0; i < mixCount; i++)
                {
                    int j = rnd.Next(i, baggageList.Count);
    
                    BaggageType temp = baggageList[i];
                    baggageList[i] = baggageList[j];
                    baggageList[j] = temp;
                }
            }
        }
    }
    
}

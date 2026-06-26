namespace EKStudio
{
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

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

        public void SpawnBaggage()
        {
            ConveyorBelt.Instance.SpawnBaggage();
        }

        private void SpawnStickman()
        {
            AIManager.Instance.SpawnAI();
        }

        void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        void CreateStickmanList()
        {
            stickmanList.Clear();
            baggageList.Clear();
            otherBaggageList.Clear();

            Level runtimeLevel = gameData.AllSo.LevelDataSO.GetRuntimeLevel(gameData.levelCount);

            for (int i = 0; i < runtimeLevel.LevelStickmanProperty.Count; i++)
            {
                LevelStickmanProperty property = runtimeLevel.LevelStickmanProperty[i];

                for (int j = 0; j < property.StickManCount; j++)
                {
                    stickmanList.Add(property.StickManType);
                }

                for (int j = 0; j < property.StickManCount * 3; j++)
                {
                    baggageList.Add(property.StickManType);
                }
            }

            ShuffleList<BaggageType>(baggageList);
        }
    }
}
namespace EKStudio
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LevelDataSO", menuName = "LevelDataSO")]
    public class LevelDataSO : ScriptableObject
    {
        public List<Level> Levels;

        public int GetWinMoney(int levelCount)
        {
            return 200;
        }

        public int GetActiveRowCount(int levelCount)
        {
            return levelCount < 3 ? 1 : levelCount < 7 ? 2 : 3;
        }

        public int GetStickmanCountPerRow(int levelCount)
        {
            return Mathf.Clamp(3 + ((levelCount - 1) / 2), 3, 15);
        }

        public Level GetRuntimeLevel(int levelCount)
        {
            Level level = new Level();
            level.winMoney = GetWinMoney(levelCount);
            level.LevelStickmanProperty = new List<LevelStickmanProperty>();

            int activeRowCount = GetActiveRowCount(levelCount);
            int stickmanCountPerRow = GetStickmanCountPerRow(levelCount);

            for (int rowIndex = 0; rowIndex < stickmanCountPerRow; rowIndex++)
            {
                List<BaggageType> rowTypes = GetShuffledBaggageTypes();

                for (int queueIndex = 0; queueIndex < activeRowCount; queueIndex++)
                {
                    LevelStickmanProperty property = new LevelStickmanProperty();
                    property.StickManType = rowTypes[queueIndex];
                    property.StickManCount = 1;

                    level.LevelStickmanProperty.Add(property);
                }
            }

            return level;
        }

        private List<BaggageType> GetShuffledBaggageTypes()
        {
            List<BaggageType> types = new List<BaggageType>
            {
                BaggageType.Blue,
                BaggageType.Green,
                BaggageType.Orange,
                BaggageType.Pink,
                BaggageType.Purple,
                BaggageType.Red
            };

            for (int i = types.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                BaggageType temp = types[i];
                types[i] = types[j];
                types[j] = temp;
            }

            return types;
        }
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
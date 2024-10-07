#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(menuName = "Content/Level Database/Level Database", fileName = "Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        [SerializeField] LevelData[] levels;
        public LevelData[] Levels => levels;

        [SerializeField] TileData[] tiles;
        public TileData[] Tiles => tiles;

        [SerializeField] TileEffect[] tileEffects;
        public TileEffect[] TileEffects => tileEffects;

        [SerializeField] BackgroundData[] backgroundData;
        public BackgroundData[] BackgroundData => backgroundData;

        // Editor stuff
        [SerializeField] Color selectedColor = new Color(0.204f, 1f, 0f, 1f);
        [SerializeField] Color unselectedColor = new Color(1f, 1f, 1f, 0f);
        [SerializeField] Color tintColor = new Color(0f, 0f, 0f, 0.11f);
        [SerializeField] Color backgroundColor = new Color(0.576f, 0.576f, 0.576f, 0.823f);
        [SerializeField] Color gridColor = new Color(0, 0, 0, 0.537f);

        public int AmountOfLevels => levels.Length;

        public void Initialise()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].Initialise(i);
            }

            for (int i = 0; i < tileEffects.Length; i++)
            {
                tileEffects[i].Initialise();
            }
        }

        public int GetRandomLevelIndex(int displayLevelNumber, int lastPlayedLevelNumber, bool replayingLevel)
        {
            if (levels.IsInRange(displayLevelNumber))
            {
                return displayLevelNumber;
            }

            if (replayingLevel)
            {
                return lastPlayedLevelNumber;
            }

            int randomLevelIndex;

            do
            {
                randomLevelIndex = Random.Range(0, levels.Length);
            }
            while (!levels[randomLevelIndex].UseInRandomizer && randomLevelIndex != lastPlayedLevelNumber);

            return randomLevelIndex;
        }

        public LevelData GetLevel(int i)
        {
            if (i < AmountOfLevels && i >= 0)
                return levels[i];

            return null;
        }

        public TileData GetTile(int i)
        {
            if (i < tiles.Length && i >= 0)
                return tiles[i];

            return null;
        }

        public TileData[] AvailableForLevel(LevelData level)
        {
            int levelIndex = System.Array.FindIndex(levels, x => x == level);
            if (levelIndex != -1)
            {
                return AvailableForLevel(levelIndex);
            }

            return AvailableForLevel(0);
        }

        public TileData[] AvailableForLevel(int levelId)
        {
            LevelData levelData = levels[levelId];

            List<TileData> result = new List<TileData>();

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].AvailableFromLevel <= levelId)
                {
                    result.Add(tiles[i]);
                }
            }

            int elementsPerLevel = Mathf.Clamp(levelData.ElementsPerLevel, 1, result.Count);
            if (result.Count > elementsPerLevel)
            {
                result.Shuffle();

                result.RemoveRange(elementsPerLevel, result.Count - elementsPerLevel);
            }

            return result.ToArray();
        }

        public BackgroundData GetLastAvailableBackgroundData()
        {
            if (BackgroundData.Length == 0)
                return null;

            var last = BackgroundData[^1];

            if (last.AvailableFromLevel <= (LevelController.MaxReachedLevelIndex + 1))
                return last;

            for (int i = 0; i < BackgroundData.Length; i++)
            {
                var data = BackgroundData[i];

                if (data.AvailableFromLevel > (LevelController.MaxReachedLevelIndex + 1) && i > 0)
                {
                    return BackgroundData[i - 1];
                }
            }

            return null;
        }

        [Button]
        public void AdjustLevelsDifficulty()
        {
            for (int i = 0; i < levels.Length; i++)
            {
                float desiredDifficulty = GetDifficulty(levels[i].SetsAmount, levels[i].AmountOfLayers);

                // set / elements count = dif / 1
                // el count = set / dif

                int elementsRequired = Mathf.RoundToInt(levels[i].SetsAmount / desiredDifficulty);

                ReflectionUtils.InjectInstanceComponent<LevelData>(levels[i], "elementsPerLevel", elementsRequired);

                RuntimeEditorUtils.SetDirty(levels[i]);
            }

            RuntimeEditorUtils.SetDirty(this);
        }

        private float GetDifficulty(int setsAmount, int layersAmount)
        {
            if (setsAmount < 20 || layersAmount <= 2)
            {
                return Mathf.Round(Random.Range(1f, 1.7f) * 10.0f) * 0.1f;
            }
            else if (setsAmount < 30 || layersAmount <= 3)
            {
                return Mathf.Round(Random.Range(2f, 2.4f) * 10.0f) * 0.1f;
            }
            else if (setsAmount < 40 || layersAmount <= 4)
            {
                return Mathf.Round(Random.Range(2.4f, 3.2f) * 10.0f) * 0.1f;
            }
            else
            {
                return Mathf.Round(Random.Range(3f, 4f) * 10.0f) * 0.1f;
            }
        }
    }
}

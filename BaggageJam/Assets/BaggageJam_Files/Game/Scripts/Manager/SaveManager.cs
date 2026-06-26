namespace EKStudio
{
    using System;
    using System.IO;
    using UnityEngine;
    using WeChatWASM;

    public class SaveManager
    {
        private const string GameDataKey = "GameData";
        private const string GameDataFileName = "GameData.txt";

        [Serializable]
        private class GameDataSave
        {
            public float totalMoney;
            public int levelCount;
            public float conveyorSpeed;
        }

        [Serializable]
        private class JokerDataSave
        {
            public int jokerCount;
            public int priceLevel;
        }

        public static void SaveData(GameData gameData)
        {
            string json = JsonUtility.ToJson(ToSaveData(gameData));

#if UNITY_WECHAT_GAME
            WX.StorageSetStringSync(GameDataKey, json);
#else
            string path = Path.Combine(Application.persistentDataPath, GameDataFileName);
            File.WriteAllText(path, json);
#endif
        }

        public static void LoadData(GameData gameData)
        {
#if UNITY_WECHAT_GAME
            string json = WX.StorageGetStringSync(GameDataKey, "");
            if (string.IsNullOrEmpty(json))
            {
                SaveData(gameData);
                return;
            }

            if (!TryApplyGameData(json, gameData))
            {
                SaveData(gameData);
            }
#else
            string filePath = Path.Combine(Application.persistentDataPath, GameDataFileName);
            if (!File.Exists(filePath))
            {
                SaveData(gameData);
                return;
            }

            string json = File.ReadAllText(filePath);
            if (!TryApplyGameData(json, gameData))
            {
                SaveData(gameData);
            }
#endif
        }

        public static void SaveData(ScriptableObject data, string textName)
        {
            string json = JsonUtility.ToJson(ToSaveData(data));

#if UNITY_WECHAT_GAME
            WX.StorageSetStringSync(textName, json);
#else
            string path = Path.Combine(Application.persistentDataPath, textName + ".txt");
            File.WriteAllText(path, json);
#endif
        }

        public static void LoadData(ScriptableObject data, string textName)
        {
#if UNITY_WECHAT_GAME
            string json = WX.StorageGetStringSync(textName, "");
            if (string.IsNullOrEmpty(json))
            {
                SaveData(data, textName);
                return;
            }

            if (!TryApplyScriptableObjectData(json, data))
            {
                SaveData(data, textName);
            }
#else
            string filePath = Path.Combine(Application.persistentDataPath, textName + ".txt");
            if (!File.Exists(filePath))
            {
                SaveData(data, textName);
                return;
            }

            string json = File.ReadAllText(filePath);
            if (!TryApplyScriptableObjectData(json, data))
            {
                SaveData(data, textName);
            }
#endif
        }

        private static GameDataSave ToSaveData(GameData gameData)
        {
            return new GameDataSave
            {
                totalMoney = gameData.totalMoney,
                levelCount = gameData.levelCount,
                conveyorSpeed = gameData.conveyorSpeed
            };
        }

        private static object ToSaveData(ScriptableObject data)
        {
            if (data is JokerDataSO jokerData)
            {
                return new JokerDataSave
                {
                    jokerCount = jokerData.JokerCount,
                    priceLevel = jokerData.PriceLevel
                };
            }

            return data;
        }

        private static bool TryApplyGameData(string json, GameData gameData)
        {
            try
            {
                GameDataSave save = JsonUtility.FromJson<GameDataSave>(json);
                if (save == null)
                    return false;

                gameData.totalMoney = Mathf.Max(0, save.totalMoney);

                if (save.levelCount > 0)
                {
                    gameData.levelCount = save.levelCount;
                }

                if (save.conveyorSpeed > 0)
                {
                    gameData.conveyorSpeed = save.conveyorSpeed;
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("GameData load failed, current defaults will be kept. " + exception.Message);
                return false;
            }
        }

        private static bool TryApplyScriptableObjectData(string json, ScriptableObject data)
        {
            if (data is JokerDataSO jokerData)
            {
                return TryApplyJokerData(json, jokerData);
            }

            return true;
        }

        private static bool TryApplyJokerData(string json, JokerDataSO jokerData)
        {
            try
            {
                JokerDataSave save = JsonUtility.FromJson<JokerDataSave>(json);
                if (save == null)
                    return false;

                jokerData.JokerCount = Mathf.Max(0, save.jokerCount);

                int maxPriceLevel = Mathf.Max(0, jokerData.PriceList.Count - 1);
                jokerData.PriceLevel = Mathf.Clamp(save.priceLevel, 0, maxPriceLevel);

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(jokerData.name + " load failed, current defaults will be kept. " + exception.Message);
                return false;
            }
        }
    }
}
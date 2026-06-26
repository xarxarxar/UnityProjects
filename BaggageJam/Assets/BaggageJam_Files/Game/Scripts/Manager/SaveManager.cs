namespace EKStudio
{
    using System.IO;
    using UnityEngine;
    
    public class SaveManager
    {
        public static void SaveData(GameData gameData)
        {
            var json = JsonUtility.ToJson(gameData);
            File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "GameData.txt", json);
        }
    
        public static void LoadData(GameData gameData)
        {
            if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "GameData.txt"))
            {
                var json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "GameData.txt");
                JsonUtility.FromJsonOverwrite(json, gameData);
            }
            else
            {
                var json = JsonUtility.ToJson(gameData);
                File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "GameData.txt", json);
            }
        }
    
        public static void SaveData(ScriptableObject data, string textName)
        {
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + textName + ".txt", json);
        }
    
        public static void LoadData(ScriptableObject data, string textName)
        {
            if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + textName + ".txt"))
            {
                var json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + textName + ".txt");
                JsonUtility.FromJsonOverwrite(json, data);
            }
            else
            {
                var json = JsonUtility.ToJson(data);
                File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + textName + ".txt", json);
            }
        }
    }
    
}

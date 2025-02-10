using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LevelConfigManager : MonoBehaviour
{
    // 本地的 LevelDatabase 配置（ScriptableObject）
    public LevelDatabase localDatabase;

    // 网络端的 LevelDatabase 配置（JSON）
    private const string serverUrl = "http://your-server.com/levels.json";
    private const string localVersionKey = "LevelDatabaseVersion";  // 本地保存的版本号

    // 从JSON加载（网络或本地）
    public void LoadFromJson(string json)
    {
        LevelDataWrapper wrapper = JsonUtility.FromJson<LevelDataWrapper>(json);
        localDatabase.levels = wrapper.levels;
        localDatabase.version = wrapper.version;  // 更新版本
    }

    // 从本地加载 LevelDatabase 或从服务器获取并更新
    public void LoadDatabase()
    {
        if (localDatabase != null && !string.IsNullOrEmpty(localDatabase.version))
        {
            string savedVersion = PlayerPrefs.GetString(localVersionKey, "");
            // 如果本地版本与服务器版本一致，直接使用本地配置
            if (savedVersion == localDatabase.version)
            {
                Debug.Log("Using local database version: " + localDatabase.version);
                return; // 本地配置无须更新
            }
        }

        // 如果没有数据库或版本不一致，从网络加载数据
        StartCoroutine(LoadFromServer());
    }

    // 从服务器加载 LevelDatabase 配置
    IEnumerator LoadFromServer()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;

                // 加载服务器数据
                LoadFromJson(json);

                // 保存从服务器获取的数据版本
                string serverVersion = JsonUtility.FromJson<LevelDataWrapper>(json).version;
                PlayerPrefs.SetString(localVersionKey, serverVersion);
                PlayerPrefs.Save();

                Debug.Log("Loaded and updated database from server with version: " + serverVersion);
            }
            else
            {
                Debug.LogError("Failed to load database from server.");
            }
        }
    }

    // 数据包封装类，用于 JSON 的反序列化
    [System.Serializable]
    public class LevelDataWrapper
    {
        public string version;  // 版本号
        public List<LevelConfig> levels;
    }
}

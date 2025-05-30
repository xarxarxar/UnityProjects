using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FileManager : MonoBehaviour
{
    public static FileManager instance;

    private string fileCachePath => Path.Combine(Application.persistentDataPath, "file_cache.json");
    private Dictionary<string, FileInfoData> fileCache = new();

    private void Awake()
    {
        instance = this;
        LoadCache();
    }

    private void Start()
    {
        LoadOrDownloadFile("123456", "https://7465-test01cloud-8g9b0glp7aab2737-1322886618.tcb.qcloud.la/testFolder/testFile/00010-730882846390286.png?sign=42348dabb0f699b4287df43ecf237b3b&t=1748417890",
            "1.0.0", (bytes) => { });
    }

    /// <summary>
    /// 下载或加载本地文件（例如图片）
    /// </summary>
    public void LoadOrDownloadFile(string fileId, string url, string version, System.Action<byte[]> onLoaded)
    {
        if (fileCache.TryGetValue(fileId, out var file) && file.version == version && File.Exists(file.localPath))
        {
            Debug.Log("加载本地缓存文件：" + file.localPath);
            byte[] data = File.ReadAllBytes(file.localPath);
            onLoaded?.Invoke(data);
        }
        else
        {
            Debug.Log("开始下载新文件...");
            StartCoroutine(DownloadAndSaveFile(fileId, url, version, onLoaded));
        }
    }

    /// <summary>
    /// 下载远程文件并保存到本地
    /// </summary>
    private IEnumerator DownloadAndSaveFile(string fileId, string url, string version, System.Action<byte[]> onLoaded)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("文件下载失败：" + www.error);
            yield break;
        }

        byte[] bytes = www.downloadHandler.data;
        string ext = Path.GetExtension(url);
        string fileName = $"file_{fileId}_{version}{ext}";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        var fileInfo = new FileInfoData
        {
            id = fileId,
            version = version,
            url = url,
            localPath = filePath
        };
        fileCache[fileId] = fileInfo;
        SaveCache();

        Debug.Log("文件保存成功：" + filePath);
        onLoaded?.Invoke(bytes);
    }

    /// <summary>
    /// 创建并保存file_cache.json
    /// </summary>
    private void SaveCache()
    {
        File.WriteAllText(fileCachePath, JsonUtility.ToJson(new FileList { files = new List<FileInfoData>(fileCache.Values) }));
    }

    /// <summary>
    /// 加载file_cache.json
    /// </summary>
    private void LoadCache()
    {
        if (File.Exists(fileCachePath))
        {
            Debug.Log("有缓存字典");
            var json = File.ReadAllText(fileCachePath);
            var list = JsonUtility.FromJson<FileList>(json);
            foreach (var file in list.files)
                fileCache[file.id] = file;
        }
    }
}
[System.Serializable]
public class FileList
{
    public List<FileInfoData> files;
}

/// <summary>
/// 通用文件信息结构体
/// </summary>
[System.Serializable]
public class FileInfoData
{
    public string id;
    public string version;
    public string url;
    public string localPath;
}

[System.Serializable]
public class FileLoadRequest
{
    public string id;
    public string url;
    public string version;
}

[System.Serializable]
public class FileLoadResponse
{
    public string id;
    public string localPath;
}

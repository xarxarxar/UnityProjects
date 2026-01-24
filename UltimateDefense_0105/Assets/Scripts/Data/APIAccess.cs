using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Globalization;

/// <summary>
/// 云端接口访问封装（Flask 服务器）
/// </summary>
public class APIAccess : MonoBehaviour
{
    // 单例
    public static APIAccess Instance;

    // Flask 服务地址
    private const string BASE_URL = "https://api.xargame.cn";

    // 临时密钥和 token，通过 get_sts 获取
    private string tmp_secret_id=string.Empty;
    private string tmp_secret_key=string.Empty;
    private string tmp_token=string.Empty;
    private string expiration_time=string.Empty ;//临时密钥到期时间

    private bool IsExpired => IsStsExpired(expiration_time);//临时密钥是否到期

    // JSON 设置（忽略 null，避免多余字段）
    private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None
    };

    // 文件路径和 uid 可以在项目里写好
    public string filePath=>DataManager.Instance.FilePath;
    public string uid =>DataManager.Instance.UserID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //StartCoroutine(GetSTS(uid, (res) =>
        //{
        //    StartCoroutine(UploadFile());
        //}));
        //StartCoroutine(GetSTS(uid, (res) =>
        //{
        //    StartCoroutine(DownloadPlayerData((PlayerInfo) =>
        //    {

        //    }));
        //}));
    }

    /// <summary>
    /// 从云端加载玩家信息
    /// </summary>
    public void DownloadPlayerInfo(string userID,UnityAction<FinalSavePlayerInfo> onSuccess=null, UnityAction<string> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        StartCoroutine(LoadPlayerInfoFromCloud(userID, onSuccess: (PlayerInfo) =>
        {
            onSuccess?.Invoke(PlayerInfo);
        }, onFail: (res) =>
        {
            Debug.Log($"错误信息为{res}");
            onFail?.Invoke(res);
        }));
    }

    public void UploadPlayerInfo(string userID, UnityAction<string> onSuccess = null, UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        StartCoroutine(UploadPlayerInfoToCloud(userID, onSuccess: (res) =>
        {
            onSuccess?.Invoke(res);
        }));
    }

    // 通过 Code2Session 接口获取微信用户的 OpenID
    public void Code2Session(UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/jixianfangyu_code2Session?js_code={WeChatManager.UserCode}";
        StartCoroutine(SendRequest(url, "GET", null, onSuccess, onFail, onError, onComplete));
    }

    // ========================
    // 通用请求方法
    // ========================
    private IEnumerator SendRequest(string url, string method, string jsonData,
        UnityAction<ApiResponse> onSuccess = null, UnityAction<ApiResponse> onFail = null,
        UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        UnityWebRequest request;

        if (method == "GET")
        {
            request = UnityWebRequest.Get(url);
        }
        else
        {
            request = new UnityWebRequest(url, method);
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = TryParseResponse(request.downloadHandler.text);
            if (response.success == true)
            {
                onSuccess?.Invoke(response);
            }
            else
            {
                onFail?.Invoke(response);
            }
        }
        else
        {
            Debug.LogError($"请求失败: {request.error}");
            onError?.Invoke(request.error);
        }
        onComplete?.Invoke();//无论结果如何都执行
    }
    // =====================
    // 通用解析函数
    // =====================

    public ApiResponse TryParseResponse(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("响应为空或请求失败。");
            return null;
        }

        try
        {
            //直接反序列化为 ApiResponse（其中 data 是 JObject）
            var response = JsonConvert.DeserializeObject<ApiResponse>(json);

            if (response == null)
            {
                Debug.LogWarning("反序列化结果为空。");
                return null;
            }

            //打印基本信息
            //Debug.Log($"解析成功：code={response.code}, success={response.success}, message={response.message}");

            return response;
        }
        catch (JsonSerializationException e)
        {
            // 当 data 不是对象（比如数组或字符串）时会到这里
            Debug.LogError($"JSON结构不符合预期（data 不是对象）: {e.Message}\n原始内容: {json}");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON解析失败: {e.Message}\n原始内容: {json}");
            return null;
        }
    }


    /// <summary>
    /// 判断 STS 临时密钥是否过期，留有300秒的缓冲时间
    /// expirationUtcString 例如：2025-12-20T13:18:54Z
    /// </summary>
    private bool IsStsExpired(string expirationUtcString, int safeSeconds = 300)
    {
        if (!TryParseUtc(expirationUtcString, out DateTime expirationUtc))
        {
            //解析失败，直接当作已过期（安全第一）
            Debug.LogWarning($"STS Expiration 解析失败: {expirationUtcString}");
            return true;
        }

        DateTime nowUtc = DateTime.UtcNow;
        return nowUtc >= expirationUtc.AddSeconds(-safeSeconds);
    }

    //尝试将string的到期时间转为datetime，都是UTC格式的，不是北京时间
    private bool TryParseUtc(string expirationUtcString, out DateTime expirationUtc)
    {
        expirationUtc = default;

        if (string.IsNullOrEmpty(expirationUtcString))
            return false;

        return DateTime.TryParse(
            expirationUtcString,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out expirationUtc
        );
    }


    /// <summary>
    /// 从云端加载玩家数据
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    /// <param name="onError"></param>
    /// <param name="onComplete"></param>
    private  IEnumerator LoadPlayerInfoFromCloud(string userId, UnityAction<FinalSavePlayerInfo> onSuccess = null,
        UnityAction<string> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        //临时密钥已过期
        if (IsExpired)
        {
            yield return StartCoroutine(GetSTS(userId, null));
        }
        StartCoroutine(DownloadPlayerData((playerInfo) =>
        {
            onSuccess?.Invoke(playerInfo);
        }, onFail: (res) =>
        {
            onFail?.Invoke(res);
        }));

    }

    private IEnumerator UploadPlayerInfoToCloud(string userId, UnityAction<string> onSuccess = null,
        UnityAction<string> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        //临时密钥已过期
        if (IsExpired)
        {
            yield return StartCoroutine(GetSTS(userId, null));
        }
        StartCoroutine(UploadFile((res) =>
        {
            onSuccess?.Invoke(res);
        }));
    }

    //获取临时密钥
    private IEnumerator GetSTS(string uid, UnityAction<string> onSuccess = null,
        UnityAction<string> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        // 替换成你 Flask 服务的地址
        string url = $"{BASE_URL}/cos/get_sts?uid={UnityWebRequest.EscapeURL(uid)}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 发送请求并等待响应
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("请求失败: " + request.error);
            }
            else
            {
                // 请求成功，解析 JSON
                string json = request.downloadHandler.text;
                Debug.Log("STS 返回数据:\n" + json);

                try
                {
                    STSResponseWrapper data =JsonConvert.DeserializeObject<STSResponseWrapper>(json);

                    if (data != null && data.data != null)
                    {
                        var creds = data.data;
                        tmp_secret_id = creds.SecretId;
                        tmp_secret_key = creds.SecretKey;
                        tmp_token = creds.Token;
                        expiration_time = creds.Expiration;
                        onSuccess?.Invoke("Success");
                        Debug.Log("\n提取临时密钥:");
                        Debug.Log("TmpSecretId: " + creds.SecretId);
                        Debug.Log("TmpSecretKey: " + creds.SecretKey);
                        Debug.Log("Token: " + creds.Token);
                        Debug.Log("ExpiredTime: " + creds.Expiration);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("解析返回 JSON 失败: " + e.Message);
                }
            }
        }
    }

    //上传文件
    private IEnumerator UploadFile(UnityAction<string> onSuccess = null,
        UnityAction<string> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        // Flask 上传接口地址
        string uploadUrl = $"{BASE_URL}/cos/upload_cos";
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在: {filePath}");
            yield break;
        }

        byte[] fileData = File.ReadAllBytes(filePath);

        // 构建 MultipartFormData
        WWWForm form = new WWWForm();
        Debug.Log($"SecretId is {tmp_secret_id},SecretKey is {tmp_secret_key},Token is {tmp_token},uid is {uid}");
        form.AddField("SecretId", tmp_secret_id);
        form.AddField("SecretKey", tmp_secret_key);
        form.AddField("Token", tmp_token);
        form.AddField("uid", uid);
        form.AddBinaryData("file", fileData, Path.GetFileName(filePath), "application/octet-stream");

        using (UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form))
        {
            // 发送请求
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("上传失败: " + request.error);
                Debug.LogError(request.downloadHandler.text);
            }
            else
            {
                // 上传成功，解析返回 JSON
                string json = request.downloadHandler.text;
                Debug.Log("上传成功，返回信息: " + json);

                // 可以用 JsonUtility 或其他 JSON 库进一步解析
                // 例如：
                // var result = JsonUtility.FromJson<UploadResponse>(json);
            }
        }
    }

    //下载文件
    private IEnumerator DownloadPlayerData(UnityAction<FinalSavePlayerInfo> onSuccess = null,
        UnityAction<string> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("SecretId", tmp_secret_id);
        form.AddField("SecretKey", tmp_secret_key);
        form.AddField("Token", tmp_token);
        form.AddField("uid", uid);

        string downloadUrl= $"{BASE_URL}/cos/download_cos";

        using (UnityWebRequest request = UnityWebRequest.Post(downloadUrl, form))
        {
            request.timeout = 15;

            yield return request.SendWebRequest();

            // 1网络 / HTTP 错误
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("网络错误: " + request.error);
                onError?.Invoke(request.error);
                onComplete?.Invoke();
                yield break;
            }

            // 2判断 Content-Type
            string contentType = request.GetResponseHeader("Content-Type");

            //服务端业务错误（JSON）
            if (!string.IsNullOrEmpty(contentType) &&
                contentType.Contains("application/json"))
            {
                string json = request.downloadHandler.text;
                Debug.Log("服务器返回错误: " + json);

                Response res = JsonConvert.DeserializeObject<Response>(json);
                Debug.Log($"错误信息为{res.code}");
                onFail?.Invoke(res.code);
                onComplete?.Invoke();
                yield break;
            }

            //成功：二进制文件
            byte[] compressed = request.downloadHandler.data;

            try
            {
                string json = DataManager.DecompressToString(compressed);
                FinalSavePlayerInfo finalSave =
                    JsonConvert.DeserializeObject<FinalSavePlayerInfo>(json);

                Debug.Log($"云端传回来：{json}");

                onSuccess?.Invoke(finalSave);
            }
            catch (Exception e)
            {
                Debug.LogError("解压 / 解析失败: " + e);
                onFail?.Invoke("-1");//解压失败
            }
            finally
            {
                onComplete?.Invoke();
            }
        }
    }

}
[System.Serializable]
public class Response
{
    public string code;
    public string msg;
}


// 临时密钥数据类，用于解析 JSON
[System.Serializable]
public class STSResponseWrapper
{
    public int code;
    public CredentialsData data;
}

[System.Serializable]
public class CredentialsData
{
    public string SecretId;
    public string SecretKey;
    public string Token;
    public string Expiration;
}

/// <summary>
/// 通用响应结构（对应 Flask 的统一返回格式）
/// success:布尔值,True 表示成功。
///code: 状态码(推荐)
///0 → 成功
///1001 → 参数缺失
///1002 → 用户不存在
///2000 → 数据库错误
///9999 → 未知错误
///message:人类可读信息
///data:实际数据内容(可为空)
/// </summary>
public class ApiResponse
{
    public int code;       // 例如 200 / 1001 / 500
    public bool success;   // 是否成功
    public string message; // 消息文本
    public JObject data;         // 实际数据（可为任意类型）
}
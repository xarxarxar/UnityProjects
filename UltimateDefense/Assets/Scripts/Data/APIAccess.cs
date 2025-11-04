using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;


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

/// <summary>
/// 云端接口访问封装（Flask 服务器）
/// 支持：get_data / save_data / update_todayinfo / update_field / update_lastonline / update_todayonlineminutes
/// </summary>
public class APIAccess : MonoBehaviour
{
    // 单例
    public static APIAccess Instance;

    // Flask 服务地址
    private const string BASE_URL = "https://api.xargame.cn";

    // JSON 设置（忽略 null，避免多余字段）
    private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None
    };

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

    // ========================
    // 通用请求方法
    // ========================
    private IEnumerator SendRequest(string url, string method, string jsonData,
        UnityAction<ApiResponse> onSuccess=null, UnityAction<ApiResponse> onFail = null,
        UnityAction<string> onError=null,UnityAction onComplete=null)
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
            var response =TryParseResponse(request.downloadHandler.text);
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
            Debug.Log($"解析成功：code={response.code}, success={response.success}, message={response.message}");

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

    // ========================
    // 各接口封装
    // ========================

    /// <summary>
    /// 获取玩家数据
    /// </summary>
    public void GetData(string userId, UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null,UnityAction < string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/get_data?UserID={userId}";
        StartCoroutine(SendRequest(url, "GET", null, onSuccess, onFail,onError, onComplete));
    }

    /// <summary>
    /// 保存玩家数据（传入 JSON 字符串）
    /// </summary>
    public void SaveData(UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/save_data";
        var jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            // NullValueHandling = NullValueHandling.Ignore, // 可选
        };
        PlayerInfo tmpPlayerInfo =DataManager.Instance. PlayerInfo.ConvertToPlayerInfo();//保存的是PlayerInfo格式
        string json = JsonConvert.SerializeObject(tmpPlayerInfo, jsonSettings);

        // 把原始 JSON 改造成带有额外字段的新 JSON
        // 在最后一个 "}" 前插入
        if (json.EndsWith("}"))
        {
            json = json.Substring(0, json.Length - 1) +
                   $", \"UserID\": \"{DataManager.Instance.UserID}\", \"UserFrom\": \"{DataManager.Instance.UserFrom}\"" +
                   "}";
        }

        StartCoroutine(SendRequest(url, "POST", json, onSuccess, onFail, onError, onComplete));
    }

    /// <summary>
    /// 累加当天在线时长（3分钟/次）
    /// </summary>
    public void UpdateTodayOnlineMinutes(string userId, UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/update_todayonlineminutes";
        var body = new Dictionary<string, object> { { "UserID", userId } };
        string json = JsonConvert.SerializeObject(body, jsonSettings);
        StartCoroutine(SendRequest(url, "POST", json, onSuccess, onFail, onError, onComplete));
    }

    /// <summary>
    /// 更新用户最后在线时间
    /// </summary>
    public void UpdateLastOnline(string userId, UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/update_lastonline";
        var body = new Dictionary<string, object> { { "UserID", userId } };
        string json = JsonConvert.SerializeObject(body, jsonSettings);
        StartCoroutine(SendRequest(url, "POST", json, onSuccess, onFail, onError, onComplete));
    }

    /// <summary>
    /// 更新今日战斗相关信息
    /// </summary>
    public void UpdateTodayInfo(string userId, int enemyDie, int wave, int fresh, int pass, int share,
        UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/update_todayinfo";
        var body = new Dictionary<string, object>
        {
            { "UserID", userId },
            { "TodayEnemyDieCount", enemyDie },
            { "TodayWaveCount", wave },
            { "TodayFreshCount", fresh },
            { "TodayPassCount", pass },
            { "TodayShareCount", share }
        };
        string json = JsonConvert.SerializeObject(body, jsonSettings);
        StartCoroutine(SendRequest(url, "POST", json, onSuccess, onFail, onError, onComplete));
    }

    /// <summary>
    /// 更新玩家字段（如钻石、皇冠等）
    /// </summary>
    public void UpdateField(string userId, Dictionary<string, int> updates,
        UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/update_field";
        var body = new Dictionary<string, object> { { "UserID", userId }, { "updates", updates } };
        string json = JsonConvert.SerializeObject(body, jsonSettings);
        StartCoroutine(SendRequest(url, "POST", json, onSuccess, onFail, onError, onComplete));
    }

    // 通过 Code2Session 接口获取微信用户的 OpenID
    public void Code2Session(UnityAction<ApiResponse> onSuccess = null,
        UnityAction<ApiResponse> onFail = null, UnityAction<string> onError = null, UnityAction onComplete = null)
    {
        string url = $"{BASE_URL}/jixianfangyu_code2Session?js_code={WeChatManager.UserCode}";
        StartCoroutine(SendRequest(url, "GET", null, onSuccess, onFail, onError, onComplete));
    }
}

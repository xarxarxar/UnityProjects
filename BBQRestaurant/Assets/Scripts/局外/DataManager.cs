using System.IO;
using UnityEngine;
using Newtonsoft.Json;
/// <summary>
/// 全局数据管理器
/// - 管理玩家进度（金币、解锁图鉴）
/// - 管理顾客/食材图鉴解锁
/// - 管理局外升级（可扩展）
/// - 单例 + 持久化
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private string fileName = "playerData.json";

    //==========================
    // 1. 玩家数据结构
    //==========================
    [System.Serializable]
    public class PlayerData
    {
        public int g = 0;   // 局外金币,gold的缩写
        public int l = 1;   //当前关卡数，level的缩写
        public int s = 0;   //当前星星数，stars的缩写
        public ulong c = 0; //当前解锁的顾客，customer的缩写
        public ulong i = 0; //当前解锁的食材，ingredien的缩写

        public PlayerData()
        {
            g = 0;
            l = 1;
            s = 0;
            c = 0;
            i = 0;
        }
    }

    public PlayerData playerData = new PlayerData();

    //==========================
    // 生命周期
    //==========================
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //Save();
    }

    //==========================
    // 5. 玩家金币
    //==========================
    public int Gold => playerData.g;


    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // 保存数据到本地
    public void Save()
    {
        string json = JsonConvert.SerializeObject(playerData);
        string path = GetFilePath();
        File.WriteAllText(path, json);
        Debug.Log($"PlayerData 已保存到: {path}");
        Debug.Log($"内容: {json}");
        Debug.Log($"文件大小: {System.Text.Encoding.UTF8.GetByteCount(json)} 字节");
    }

}

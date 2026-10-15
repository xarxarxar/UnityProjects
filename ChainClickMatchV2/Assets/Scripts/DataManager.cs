using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DataManager：本机持久化唯一入口（PlayerPrefs + JSON）。
/// 存档内容：各章节通关进度与每关星级、三个道具的剩余数量。
///
/// 说明：
/// - 所有存档读写必须走本类（其他脚本禁止直接碰 PlayerPrefs）；
/// - 微信小游戏平台 PlayerPrefs 由适配层映射到本地存储；若后续需要原生
///   WX.SetStorageSync 后端，只需替换本类内部实现，外部调用不变。
///
/// 用法：场景中挂到常驻物体（与 GameManager 同一物体即可），自动单例 + 加载存档。
/// </summary>
public class DataManager : MonoBehaviour
{
    /// <summary>存档数据（JsonUtility 序列化用，禁止在运行时手动改字典）。</summary>
    [System.Serializable]
    public class SaveData
    {
        public List<ChapterSave> chapters = new List<ChapterSave>();

        public int addStepsProps = 3;   // 步数+3 道具剩余
        public int hintProps = 3;       // 提示道具剩余
        public int shuffleProps = 3;    // 洗牌道具剩余
    }

    /// <summary>单章节存档：通关关卡数 + 每难度每关星级。</summary>
    [System.Serializable]
    public class ChapterSave
    {
        public int chapter;

        /// <summary>已通关关卡数（旧版顺序解锁用，通关即推进）。</summary>
        public int cleared;

        /// <summary>
        /// 每难度每关星级，**平铺一维数组**（长度 30 = 3 难度 × 10 关）：
        /// 索引 = 难度(0=Normal,1=Hard,2=ExtremelyHard) × 10 + (关卡-1)。
        /// 元素值为 -1 表示该关从未通关，0~3 为通关星级（0 星也算通关）。
        /// 注意：字段默认 null，由 RepairStars 统一初始化为全 -1——
        /// 不能依赖 `new int[30]` 的默认 0，否则"未通关"与"0 星通关"无法区分，
        /// 会导致未通关关卡被误判为 0 星通关。
        /// 另注意：不用 int[3][]（交错数组）——Unity JsonUtility 不支持交错数组序列化。
        /// </summary>
        public int[] stars;
    }

    public const string SaveKey = "ChainClick_GameSave";

    public static DataManager Instance { get; private set; }

    /// <summary>运行时存档数据（首次无存档时为默认值）。</summary>
    public SaveData Data { get; private set; }

    public int AddStepsPropCount => Data.addStepsProps;
    public int HintPropCount => Data.hintProps;
    public int ShufflePropCount => Data.shuffleProps;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    /// <summary>从 PlayerPrefs 读取存档（无存档则用默认值），并修复星级结构（兼容旧版一维存档）。</summary>
    public void Load()
    {
        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        Data = string.IsNullOrEmpty(json) ? new SaveData() : JsonUtility.FromJson<SaveData>(json);
        if (Data == null)
        {
            Data = new SaveData();
        }
        for (int i = 0; i < Data.chapters.Count; i++)
        {
            RepairStars(Data.chapters[i]);
        }
    }

    /// <summary>新建一份长度为 30 的星级数组，全部初始化为 -1（未通关）。</summary>
    private static int[] CreateEmptyStars()
    {
        int[] s = new int[30];
        for (int i = 0; i < 30; i++)
        {
            s[i] = -1;
        }
        return s;
    }

    /// <summary>
    /// 确保章节星级数组为长度 30 且每格为有效值（-1 未通关 / 0~3 星级）。
    /// 兼容旧存档：
    /// - null / 长度不符 → 重建为全 -1；
    /// - 全 0 数组（旧版误把未通关存成 0）→ 视为从未初始化，全部置 -1（未通关）。
    /// </summary>
    private void RepairStars(ChapterSave cs)
    {
        if (cs.stars == null || cs.stars.Length != 30)
        {
            cs.stars = CreateEmptyStars();
            return;
        }
        bool allZero = true;
        for (int i = 0; i < cs.stars.Length; i++)
        {
            if (cs.stars[i] != 0)
            {
                allZero = false;
                break;
            }
        }
        if (allZero)
        {
            // 旧版/未初始化存档：0 无法区分"未通关"与"0 星通关"，视为从未初始化
            cs.stars = CreateEmptyStars();
            return;
        }
        for (int i = 0; i < cs.stars.Length; i++)
        {
            if (cs.stars[i] < -1 || cs.stars[i] > 3)
            {
                cs.stars[i] = -1;
            }
        }
    }

    /// <summary>立即写入 PlayerPrefs 并落盘。</summary>
    public void SaveNow()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(Data));
        PlayerPrefs.Save();
    }

    // ---------- 章节进度 ----------

    /// <summary>查找章节存档，不存在则新建。</summary>
    private ChapterSave GetOrCreateChapter(int chapter)
    {
        for (int i = 0; i < Data.chapters.Count; i++)
        {
            if (Data.chapters[i].chapter == chapter)
            {
                return Data.chapters[i];
            }
        }
        ChapterSave cs = new ChapterSave { chapter = chapter };
        Data.chapters.Add(cs);
        return cs;
    }

    /// <summary>某章节已通关关卡数（无记录返回 0）。</summary>
    public int GetClearedLevels(int chapter)
    {
        for (int i = 0; i < Data.chapters.Count; i++)
        {
            if (Data.chapters[i].chapter == chapter)
            {
                return Data.chapters[i].cleared;
            }
        }
        return 0;
    }

    /// <summary>直接覆盖某章节已通关关卡数（调试/重置用）。</summary>
    public void SetClearedLevels(int chapter, int value)
    {
        GetOrCreateChapter(chapter).cleared = Mathf.Max(0, value);
        SaveNow();
    }

    /// <summary>记录某章节某关卡已通关（取旧值与关卡号的较大值，向前推进解锁）。</summary>
    public void RecordCleared(int chapter, int level)
    {
        ChapterSave cs = GetOrCreateChapter(chapter);
        cs.cleared = Mathf.Max(cs.cleared, Mathf.Max(1, level));
        SaveNow();
    }

    // ---------- 星级 ----------

    /// <summary>某章节×难度×关卡的星级（0~3；-1 表示该关从未通关；无存档章节/越界返回 -1）。</summary>
    public int GetStars(int chapter, LevelDifficulty difficulty, int level)
    {
        int idx = (int)difficulty * 10 + (level - 1);
        if (level < 1 || level > 10)
        {
            return -1;
        }
        for (int i = 0; i < Data.chapters.Count; i++)
        {
            if (Data.chapters[i].chapter == chapter)
            {
                int[] stars = Data.chapters[i].stars;
                if (stars == null || idx < 0 || idx >= stars.Length)
                {
                    return -1;
                }
                return stars[idx];
            }
        }
        return -1;
    }

    /// <summary>记录某章节×难度×关卡的星级（取历史最高，不降级；0 星也算通关）。</summary>
    public void RecordStars(int chapter, LevelDifficulty difficulty, int level, int stars)
    {
        ChapterSave cs = GetOrCreateChapter(chapter);
        RepairStars(cs);
        int idx = (int)difficulty * 10 + (level - 1);
        if (level < 1 || level > 10)
        {
            return;
        }
        cs.stars[idx] = Mathf.Max(cs.stars[idx], Mathf.Clamp(stars, 0, 3));
        SaveNow();
    }

    // ---------- 道具 ----------

    /// <summary>保存三个道具的剩余数量并落盘。</summary>
    public void SavePropCounts(int addSteps, int hint, int shuffle)
    {
        Data.addStepsProps = Mathf.Max(0, addSteps);
        Data.hintProps = Mathf.Max(0, hint);
        Data.shuffleProps = Mathf.Max(0, shuffle);
        SaveNow();
    }
}

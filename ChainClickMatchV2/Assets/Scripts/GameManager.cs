using UnityEngine;

/// <summary>
/// 关卡难度，每个章节都有不同的难度
/// 关卡解锁条件为，前一个关卡通关，并且当前关卡的上一个难度的关卡为3星.章节的解锁条件为前一章节满星通关
/// 举例：第2章节的困难模式的第5关解锁条件为：第2章节的困难模式的第4关已通关（无论几星）且第2章节的普通模式的第5关已经三星通关
/// 举例：第5章节的普通模式的第1关的解锁条件为：第4章节的普通，困难，极难模式的所有关卡全部都三星通关
/// </summary>
public enum LevelDifficulty 
{ 
    Normal,
    Hard,
    ExtremelyHard
}


/// <summary>
/// GameManager：全局游戏流程状态管理（单例，常驻）。
/// - 当前章节/关卡、最近一次通关星级；
/// - 通关记录（进度 + 星级）统一封装并转发给 DataManager 持久化；
/// - 道具数量变化统一转发给 DataManager 保存。
///
/// 用法：场景中挂到常驻物体（与 DataManager 同一物体即可）。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// <summary>当前章节（1 起）。</summary>
    public int CurrentChapter { get; private set; } = 1;

    /// <summary>当前关卡（1 起）。</summary>
    public int CurrentLevel { get; private set; } = 1;

    /// <summary>当前难度（Normal / Hard / ExtremelyHard）。</summary>
    public LevelDifficulty CurrentDifficulty { get; private set; } = LevelDifficulty.Normal;

    /// <summary>最近一次通关获得的星级（0~3；未通关为 0）。</summary>
    public int LastStars { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>设置当前进行的章节/难度/关卡（由 GamePanel 开局时调用）。</summary>
    public void SetCurrentLevel(int chapter, LevelDifficulty difficulty, int level)
    {
        CurrentChapter = Mathf.Max(1, chapter);
        CurrentDifficulty = difficulty;
        CurrentLevel = Mathf.Max(1, level);
    }

    /// <summary>记录一次通关：更新最近星级，并持久化进度（解锁下一关）与星级（取最高）。</summary>
    public void RecordWin(int chapter, LevelDifficulty difficulty, int level, int stars)
    {
        LastStars = stars;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.RecordCleared(chapter, level);
            DataManager.Instance.RecordStars(chapter, difficulty, level, stars);
        }
    }

    /// <summary>保存三个道具的剩余数量（由 CardGridLayout 使用道具后调用）。</summary>
    public void SavePropCounts(int addSteps, int hint, int shuffle)
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SavePropCounts(addSteps, hint, shuffle);
        }
    }

    // ---------- 三维解锁判定（规则见文件顶部 LevelDifficulty 注释） ----------

    /// <summary>
    /// 计算某章节×难度×关卡的解锁状态：
    /// -2 未解锁；-1 已解锁未通关；0~3 已通关星级（0 星也算通关）。
    /// 规则：
    /// 1) 章节解锁：第 1 章永远解锁；第 C 章需第 C-1 章全部关卡（3 难度×10 关）3 星通关；
    /// 2) 难度解锁：Hard/ExtremelyHard 需上一难度同关卡 3 星通关；
    /// 3) 关卡解锁：同难度第 L 关需第 L-1 关已通关（无论几星）。
    /// </summary>
    public int GetLevelState(int chapter, LevelDifficulty difficulty, int level)
    {
        // 章节解锁
        if (chapter > 1 && !IsChapterAllThreeStars(chapter - 1))
        {
            return -2;
        }

        // 难度解锁：上一难度同关卡需 3 星
        if (difficulty > LevelDifficulty.Normal)
        {
            LevelDifficulty prevDifficulty = (LevelDifficulty)((int)difficulty - 1);
            if (DataManager.Instance == null || DataManager.Instance.GetStars(chapter, prevDifficulty, level) < 3)
            {
                return -2;
            }
        }

        // 关卡解锁：同难度前一关需已通关（无论几星）
        if (level > 1)
        {
            if (DataManager.Instance == null || DataManager.Instance.GetStars(chapter, difficulty, level - 1) < 0)
            {
                Debug.Log($"普通关卡第一关{DataManager.Instance.GetStars(1,LevelDifficulty.Normal,1)}");
                return -2;
            }
        }

        // 已解锁：返回本关星级（-1 表示尚未通关）
        if (DataManager.Instance != null)
        {
            int stars = DataManager.Instance.GetStars(chapter, difficulty, level);
            return stars >= 0 ? stars : -1;
        }
        return -1;
    }

    /// <summary>某章节是否全部关卡（3 难度×10 关）均已 3 星通关。</summary>
    public bool IsChapterAllThreeStars(int chapter)
    {
        if (DataManager.Instance == null)
        {
            return false;
        }
        for (int d = 0; d < 3; d++)
        {
            for (int l = 1; l <= 10; l++)
            {
                if (DataManager.Instance.GetStars(chapter, (LevelDifficulty)d, l) < 3)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 某章节的默认难度：当前闯到的最高的关卡所在的难度。
    /// 从最高难度（极难）往下找第一个**该难度第 1 关已通关（星级 ≥ 0）**的难度——
    /// "已通关"才算闯到该难度；难度解锁（上一难度同关 3 星）但该难度第 1 关尚未通关的，
    /// 不算闯到（例：只在普通通关 4 关 → 默认普通，不应跳到已解锁未玩的困难）。
    /// 全部未通关回退 Normal（第 1 关永远可玩）。
    /// </summary>
    public LevelDifficulty GetDefaultDifficulty(int chapter)
    {
        for (int d = 2; d >= 0; d--)
        {
            if (DataManager.Instance != null && DataManager.Instance.GetStars(chapter, (LevelDifficulty)d, 1) >= 0)
            {
                return (LevelDifficulty)d;
            }
        }
        return LevelDifficulty.Normal;
    }
}

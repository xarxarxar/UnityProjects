using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡选择 Panel：开始界面选好章节后，点击"开始游戏"按钮进入本 Panel。
/// 每个关卡用一个 SingleLevelUI 组件（含锁/星星/关卡按钮），按三维解锁规则显示状态：
/// - 章节解锁：第 1 章永远解锁；后续章节需前一章节全部关卡 3 星通关；
/// - 难度解锁：困难/极难需上一难度同关卡 3 星通关；
/// - 关卡解锁：同难度前一关已通关。
/// 三个难度按钮切换当前难度（普通/困难/极难），切换后按新难度刷新关卡列表。
///
/// 通关记录存 DataManager（唯一存档入口），每章每难度独立解锁。
///
/// 用法：
/// 1. 本脚本挂在 LevelChoose Panel 根物体上（Panel 初始隐藏）；
/// 2. 开始界面根物体拖入 mainPanel；章节选择器（PageSwitcher 组件）拖入 pageSwitcher；
/// 3. 【重要】"开始游戏"按钮不要拖任何字段——本 Panel 初始隐藏，脚本 Awake 不会执行，
///    无法自动绑定。请在"开始游戏"按钮的 Button.onClick 里手动添加
///    LevelChoosePanel.OpenLevelChoose()（public 方法，序列化事件在隐藏时也能调用）；
/// 4. 关卡按钮的 SingleLevelUI 组件按顺序拖入 levelButtons（首次打开本 Panel 时脚本自动绑定点击回调，带关卡序号）；
/// 5. 三个难度按钮按顺序（普通/困难/极难）拖入 difficultyButtons，自动绑定难度切换；
/// 6. 运行：点"开始游戏" → 进入关卡选择；未解锁关卡显示锁并不可点击。
/// </summary>
public class LevelChoosePanel : MonoBehaviour
{
    [Header("开始界面")]
    [Tooltip("开始界面根物体（打开本 Panel 时隐藏）")]
    public GameObject mainPanel;

    [Tooltip("章节选择器（拖入 PageSwitcher 组件，自动读取当前章节）")]
    public PageSwitcher pageSwitcher;

    [Header("切换管理（可选）")]
    [Tooltip("PanelManager 组件（拖入后，点击关卡会自动切换到 GamePanel）")]
    public PanelManager panelManager;

    [Tooltip("目标 Panel：游戏界面（拖入后点击关卡切换过去）")]
    public GameObject gamePanel;

    [Header("关卡按钮")]
    [Tooltip("关卡按钮的 SingleLevelUI 组件，按顺序拖入（第 1 个 = 第 1 关）")]
    public SingleLevelUI[] levelButtons;

    [Tooltip("关卡总数（默认 10）")]
    public int levelCount = 10;

    [Header("难度切换")]
    [Tooltip("3 个难度按钮，按顺序拖入：普通/困难/极难（Normal/Hard/ExtremelyHard）")]
    public Button[] difficultyButtons;

    [Header("调试")]
    [Tooltip("调试用：手动指定已通关关卡数（>0 时覆盖存档用于预览解锁效果，0 则读取存档）")]
    public int debugClearedLevels = 0;

    /// <summary>当前章节（1 起）。</summary>
    public int CurrentChapter => _currentChapter;

    /// <summary>当前难度。</summary>
    public LevelDifficulty CurrentDifficulty => _currentDifficulty;

    private int _currentChapter = 1;
    private LevelDifficulty _currentDifficulty = LevelDifficulty.Normal;

    private void Awake()
    {
        if (levelCount <= 0)
        {
            levelCount = 10;
        }

        // 注意：本 Panel 初始隐藏时 Awake 不会执行；startGameButton 的绑定不在这里做，
        // "开始游戏"按钮请在 Button.onClick 中手动绑定 OpenLevelChoose()。

        // 首次打开本 Panel（SetActive(true)）时 Awake 会执行，这里自动给每个关卡按钮
        // 绑定点击回调（带关卡序号 0 起）
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i; // 闭包捕获
            SingleLevelUI ui = levelButtons[i];
            if (ui != null && ui.levelButton != null)
            {
                ui.levelButton.onClick.AddListener(() => OnLevelButtonClicked(index));
            }
        }

        // 三个难度按钮自动绑定：按顺序 普通/困难/极难 → Normal/Hard/ExtremelyHard
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            int index = i; // 闭包捕获
            Button b = difficultyButtons[i];
            if (b != null)
            {
                b.onClick.AddListener(() => SetDifficulty((LevelDifficulty)index));
            }
        }
    }

    /// <summary>
    /// 每次本 Panel 变为可见（SetActive(true)）时执行：统一完成"打开初始化"——
    /// 当前章节（PageSwitcher 优先）→ 默认难度（该章节闯到的最高的关卡所在难度）→ 刷新关卡列表。
    /// 这样无论从"开始游戏"按钮进入，还是从 GamePanel/结果面板返回，都会重新计算默认难度与解锁状态。
    /// </summary>
    private void OnEnable()
    {
        // 章节：以章节选择器当前章节为准（无则沿用上次章节）
        if (pageSwitcher != null)
        {
            _currentChapter = Mathf.Max(1, pageSwitcher.CurrentIndex + 1);
        }
        // 默认难度：该章节闯到的最高的关卡所在的难度（如已闯到极难第 1 关则默认极难）
        _currentDifficulty = GameManager.Instance != null
            ? GameManager.Instance.GetDefaultDifficulty(_currentChapter)
            : LevelDifficulty.Normal;
        RefreshLevelButtons();
    }

    /// <summary>打开关卡选择（自动取 PageSwitcher 当前章节，无则用上次章节）。</summary>
    public void OpenLevelChoose()
    {
        int chapter = pageSwitcher != null ? pageSwitcher.CurrentIndex + 1 : _currentChapter;
        OpenLevelChoose(chapter);
    }

    /// <summary>打开指定章节的关卡选择（显示后由 OnEnable 自动完成默认难度设置与刷新）。</summary>
    public void OpenLevelChoose(int chapter)
    {
        _currentChapter = Mathf.Max(1, chapter);
        // 优先走 PanelManager 通用切换（不关心谁消失）；无 PanelManager 时回退直接隐藏 mainPanel
        if (panelManager != null)
        {
            panelManager.SwitchTo(gameObject);
        }
        else
        {
            if (mainPanel != null)
            {
                mainPanel.SetActive(false);
            }
            gameObject.SetActive(true);
        }
    }

    /// <summary>返回开始界面（MainPanel）。</summary>
    public void BackToMainPanel()
    {
        if (panelManager != null && mainPanel != null)
        {
            panelManager.SwitchTo(mainPanel);
        }
        else if (mainPanel != null)
        {
            mainPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    /// <summary>按当前章节与难度刷新所有关卡按钮的解锁状态（SingleLevelUI 全参 Init）。</summary>
    public void RefreshLevelButtons()
    {
        int count = Mathf.Min(levelButtons.Length, levelCount);
        for (int i = 0; i < count; i++)
        {
            
            SingleLevelUI ui = levelButtons[i];
            if (ui == null)
            {
                continue;
            }

            int state;
            if (debugClearedLevels > 0)
            {
                // 调试预览：前 debugClearedLevels 关视为已通关（3 星），当前关已解锁未通关，其余未解锁
                state = i < debugClearedLevels ? 3 : (i == debugClearedLevels ? -1 : -2);
            }
            else if (GameManager.Instance != null)
            {
                state = GameManager.Instance.GetLevelState(_currentChapter, _currentDifficulty, i + 1);
            }
            else
            {
                // 无 GameManager（编辑器裸跑调试）：仅第 1 关可玩
                state = i == 0 ? -1 : -2;
            }
            Debug.Log($"第{i}个,state is {state}");
            ui.Init(_currentChapter, _currentDifficulty, i + 1, state);
        }
    }

    /// <summary>切换当前难度并刷新关卡列表（难度按钮点击调用）。</summary>
    public void SetDifficulty(LevelDifficulty difficulty)
    {
        _currentDifficulty = difficulty;
        RefreshLevelButtons();
        Debug.Log("切换难度：" + _currentDifficulty);
    }

    /// <summary>读取当前章节已通关关卡数（调试字段 > 0 时优先）。</summary>
    public int GetClearedLevels()
    {
        if (debugClearedLevels > 0)
        {
            return debugClearedLevels;
        }
        return DataManager.Instance != null ? DataManager.Instance.GetClearedLevels(_currentChapter) : 0;
    }

    /// <summary>写入当前章节已通关关卡数并刷新按钮（供通关流程/调试调用）。</summary>
    public void SetClearedLevels(int cleared)
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetClearedLevels(_currentChapter, cleared);
        }
        RefreshLevelButtons();
    }

    /// <summary>
    /// 记录指定章节的关卡已通关（取与旧存档的最大值，向前推进解锁）。
    /// 由 GamePanel 在关卡成功时调用；不刷新按钮（下次打开本 Panel 时自动刷新）。
    /// </summary>
    public void RecordCleared(int chapter, int level)
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.RecordCleared(chapter, level);
        }
    }

    /// <summary>点击某个关卡按钮（index 0 起，第 1 关 = 0）。</summary>
    public void OnLevelButtonClicked(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelCount)
        {
            return;
        }
        // 防手滑：未解锁关卡理论上已被禁用，这里再兜底校验一次
        bool unlocked;
        if (debugClearedLevels > 0)
        {
            unlocked = levelIndex <= debugClearedLevels;
        }
        else if (GameManager.Instance != null)
        {
            unlocked = GameManager.Instance.GetLevelState(_currentChapter, _currentDifficulty, levelIndex + 1) > -2;
        }
        else
        {
            unlocked = levelIndex == 0;
        }
        if (!unlocked)
        {
            return;
        }
        Debug.Log("开始游戏：第" + _currentChapter + "章 第" + (levelIndex + 1) + "关（难度：" + _currentDifficulty + "）");
        // 若配置了切换管理，点击关卡后切到 GamePanel，并把章节/难度/关卡信息传给 GamePanel
        if (panelManager != null && gamePanel != null)
        {
            panelManager.SwitchTo(gamePanel);
            GamePanel gp = gamePanel.GetComponent<GamePanel>();
            if (gp != null)
            {
                gp.StartLevel(_currentChapter, _currentDifficulty, levelIndex + 1);
            }
        }
    }
}

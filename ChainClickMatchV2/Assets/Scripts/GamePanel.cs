using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GamePanel：关卡正式玩的时候的 UI 管理器。
/// - 导航：提供返回关卡选择（BackToLevelChoose），配合 PanelManager 通用切换；
/// - 关卡信息：StartLevel(chapter, level) 记录当前章节/关卡，并刷新标题"第X章 第Y关"（可选）。
///
/// 注意：GamePanel 初始隐藏时 Awake 不会执行，返回按钮请在 Button.onClick 中手动绑定
/// BackToLevelChoose()（public 方法）；backButton 字段仅当 GamePanel 初始激活时才自动绑定。
///
/// 用法：
/// 1. 本脚本挂在 GamePanel 根物体上；
/// 2. PanelManager 拖入 panelManager、关卡选择 Panel 拖入 levelChoosePanel；
/// 3. 返回按钮（可选字段）或手动在 onClick 绑定 BackToLevelChoose()；
/// 4. 标题 Text（可选）拖入 titleText，由关卡选择调用 StartLevel 刷新。
/// </summary>
public class GamePanel : MonoBehaviour
{
    [Header("导航")]
    [Tooltip("PanelManager 组件（拖入）")]
    public PanelManager panelManager;

    [Tooltip("返回目标：关卡选择 Panel（拖入）")]
    public GameObject levelChoosePanel;

    [Tooltip("返回按钮（GamePanel 初始激活时自动绑定；隐藏时请在 onClick 手动绑定 BackToLevelChoose）")]
    public Button backButton;

    [Header("关卡信息")]
    [Tooltip("标题文本（显示“第X章 第Y关”，可选）")]
    public Text titleText;

    [Header("游戏")]
    [Tooltip("CardGridLayout 组件（CardRoot 上，游戏正式开始/重置/结算都走它；拖入）")]
    public CardGridLayout cardGrid;

    [Tooltip("通关 Panel（初始隐藏；关卡成功时显示）")]
    public GameObject winPanel;

    [Tooltip("失败 Panel（初始隐藏；关卡失败时显示）")]
    public GameObject losePanel;

    /// <summary>最近一次通关获得的星级（0~3；未通关为 0）。</summary>
    public int LastStars { get; private set; }

    /// <summary>当前章节（1 起）。</summary>
    public int CurrentChapter => _currentChapter;

    /// <summary>当前关卡（1 起）。</summary>
    public int CurrentLevel => _currentLevel;

    /// <summary>当前难度。</summary>
    public LevelDifficulty CurrentDifficulty => _currentDifficulty;

    private int _currentChapter = 1;
    private int _currentLevel = 1;
    private LevelDifficulty _currentDifficulty = LevelDifficulty.Normal;

    private void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToLevelChoose);
        }

        // 订阅棋盘关卡结算事件：成功/失败时显示对应结果面板
        if (cardGrid != null)
        {
            cardGrid.OnLevelCleared += HandleLevelCleared;
            cardGrid.OnLevelFailed += HandleLevelFailed;
        }
    }

    private void OnDestroy()
    {
        if (cardGrid != null)
        {
            cardGrid.OnLevelCleared -= HandleLevelCleared;
            cardGrid.OnLevelFailed -= HandleLevelFailed;
        }
    }

    /// <summary>开始某章节某难度某关卡：记录章节/难度/关卡、隐藏结果面板、重置并启动棋盘游戏。</summary>
    public void StartLevel(int chapter, LevelDifficulty difficulty, int level)
    {
        _currentChapter = Mathf.Max(1, chapter);
        _currentDifficulty = difficulty;
        _currentLevel = Mathf.Max(1, level);
        if (titleText != null)
        {
            titleText.text = "第" + _currentChapter + "章 第" + _currentLevel + "关";
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentLevel(_currentChapter, _currentDifficulty, _currentLevel);
        }

        // 开始新一局：隐藏上一次的结果面板，棋盘全量重置（分数/步数/卡牌等）
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
        if (cardGrid != null)
        {
            cardGrid.StartLevel(_currentChapter, _currentLevel);
        }
    }

    /// <summary>下一关（通关面板"下一关"按钮 → ResultPanel.OnNextLevelClick 调用；同难度下一关）。</summary>
    public void StartNextLevel()
    {
        StartLevel(_currentChapter, _currentDifficulty, _currentLevel + 1);
    }

    /// <summary>重玩此关（失败面板"重玩"按钮 → ResultPanel.OnReplayClick 调用）。</summary>
    public void ReplayLevel()
    {
        StartLevel(_currentChapter, _currentDifficulty, _currentLevel);
    }

    /// <summary>关卡成功（达到分数立即触发）：记录星级、记录通关（解锁下一关）并显示通关 Panel。</summary>
    private void HandleLevelCleared(int stars)
    {
        LastStars = stars;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordWin(_currentChapter, _currentDifficulty, _currentLevel, stars);
        }
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    /// <summary>关卡失败：显示失败 Panel。</summary>
    private void HandleLevelFailed()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }

    /// <summary>返回关卡选择 Panel（退出时清理当前局现场，避免残留影响下一局）。</summary>
    public void BackToLevelChoose()
    {
        if (cardGrid != null)
        {
            cardGrid.CleanupLevel();
        }
        if (panelManager != null && levelChoosePanel != null)
        {
            panelManager.SwitchTo(levelChoosePanel);
        }
        else if (levelChoosePanel != null)
        {
            levelChoosePanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}

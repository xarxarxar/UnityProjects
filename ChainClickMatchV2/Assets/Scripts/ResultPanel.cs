using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡结果面板（通关 Panel / 失败 Panel 共用）：
/// - 通关 Panel：下一关（OnNextLevelClick）+ 返回（OnBackClick）；
/// - 失败 Panel：重玩此关（OnReplayClick）+ 返回（OnBackClick）。
///
/// 注意：结果 Panel 初始隐藏，Awake 不会执行，请在按钮的 Button.onClick 中手动绑定以下 public 方法：
/// - 下一关按钮 → OnNextLevelClick
/// - 重玩按钮   → OnReplayClick
/// - 返回按钮   → OnBackClick
///
/// 用法：两个 Panel 各挂一个本脚本；panelManager / levelChoosePanel / gamePanel 拖入引用即可。
/// </summary>
public class ResultPanel : MonoBehaviour
{
    [Header("导航")]
    [Tooltip("PanelManager 组件（拖入，返回关卡选择用）")]
    public PanelManager panelManager;

    [Tooltip("返回目标：关卡选择 Panel（拖入）")]
    public GameObject levelChoosePanel;

    [Header("游戏")]
    [Tooltip("GamePanel 组件（拖入，下一关/重玩需要它）")]
    public GamePanel gamePanel;

    /// <summary>下一关：GamePanel.StartNextLevel()（通关 Panel 的"下一关"按钮绑定）。</summary>
    public void OnNextLevelClick()
    {
        if (gamePanel != null)
        {
            gamePanel.StartNextLevel();
        }
    }

    /// <summary>重玩此关：GamePanel.ReplayLevel()（失败 Panel 的"重玩"按钮绑定）。</summary>
    public void OnReplayClick()
    {
        if (gamePanel != null)
        {
            gamePanel.ReplayLevel();
        }
    }

    /// <summary>返回关卡选择：先经 GamePanel.BackToLevelChoose 清理当前局现场（棋盘回池、停止倒计时/呼吸、复位状态）再切换 Panel（两个 Panel 的"返回"按钮绑定）。</summary>
    public void OnBackClick()
    {
        if (gamePanel != null)
        {
            gamePanel.BackToLevelChoose();
        }
        else if (panelManager != null && levelChoosePanel != null)
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

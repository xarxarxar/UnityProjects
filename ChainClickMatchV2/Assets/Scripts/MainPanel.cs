using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主界面（Play 时默认显示的界面）脚本：
/// 在 MainPanel 中绑定"开始游戏"按钮，点击后通过 PanelManager 切换到关卡选择 Panel。
///
/// 注意：MainPanel 是 Play 默认显示的界面（激活状态），所以本脚本的 Awake 一定会执行，
/// "开始游戏"按钮可以在这里自动绑定（与隐藏的 LevelChoosePanel 不同，那里必须手动 onClick 绑定）。
///
/// 用法：
/// 1. 本脚本挂在 MainPanel 根物体上；
/// 2. "开始游戏"按钮拖入 playButton；
/// 3. PanelManager 组件拖入 panelManager，关卡选择 Panel 拖入 levelChoosePanel；
/// 4. 运行点"开始游戏" → 自动切到关卡选择。
/// </summary>
public class MainPanel : MonoBehaviour
{
    [Header("开始游戏按钮（MainPanel 激活，Awake 自动绑定）")]
    public Button playButton;

    [Header("切换管理")]
    [Tooltip("PanelManager 组件（拖入）")]
    public PanelManager panelManager;

    [Tooltip("目标 Panel：关卡选择（拖入）")]
    public GameObject levelChoosePanel;

    private void Awake()
    {
        if (playButton != null && panelManager != null && levelChoosePanel != null)
        {
            playButton.onClick.AddListener(() => panelManager.SwitchTo(levelChoosePanel));
        }
    }
}

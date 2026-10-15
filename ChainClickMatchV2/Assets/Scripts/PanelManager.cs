using UnityEngine;

/// <summary>
/// 通用 Panel 切换管理器：维护一组 Panel（如 MainPanel / LevelChoosePanel / GamePanel），
/// 调用 SwitchTo(目标Panel) 时自动隐藏列表中其他所有 Panel、只显示目标 Panel——
/// 调用方不关心"当前是哪个 Panel、哪个需要消失"，只需指定"要显示哪个"。
///
/// 用法：
/// 1. 挂在一个常驻物体上（如 Canvas 或空物体）；
/// 2. 把要管理的 Panel 全部拖入 panels[]；
/// 3. 任意脚本/按钮调用 panelManager.SwitchTo(目标Panel) 即可切换。
/// </summary>
public class PanelManager : MonoBehaviour
{
    [Header("要管理的所有 Panel（切换时只显示目标，其余全部隐藏）")]
    public GameObject[] panels;

    /// <summary>当前显示的 Panel。</summary>
    public GameObject CurrentPanel => _currentPanel;

    private GameObject _currentPanel;

    private void Awake()
    {
        // 初始状态尊重场景预设：记录已激活的那个 Panel；
        // 若都未激活则默认激活第一个
        foreach (GameObject p in panels)
        {
            if (p != null && p.activeSelf)
            {
                _currentPanel = p;
                break;
            }
        }
        if (_currentPanel == null && panels.Length > 0 && panels[0] != null)
        {
            _currentPanel = panels[0];
            _currentPanel.SetActive(true);
        }
    }

    /// <summary>切换到目标 Panel：隐藏列表中其他所有 Panel，只显示目标。</summary>
    public void SwitchTo(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            return;
        }
        foreach (GameObject p in panels)
        {
            if (p == null)
            {
                continue;
            }
            bool show = p == targetPanel;
            if (p.activeSelf != show)
            {
                p.SetActive(show);
            }
        }
        _currentPanel = targetPanel;
    }

    /// <summary>按索引切换到 panels[] 中的某个 Panel。</summary>
    public void SwitchTo(int index)
    {
        if (index < 0 || index >= panels.Length)
        {
            return;
        }
        SwitchTo(panels[index]);
    }
}

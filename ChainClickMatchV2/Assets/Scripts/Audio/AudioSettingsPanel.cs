using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音频设置面板：三个 Toggle 分别控制 **音乐开关 / 音效开关 / 震动开关**。
/// - **数据驱动**：面板每次显示（OnEnable）时从 AudioManager（内部来自 DataManager 存档）读取状态，
///   设置 Toggle.isOn——音乐/音效 Toggle 与"静音"取反（isOn=true 表示开启声音），震动 Toggle 直读开关；
/// - **点击写入**：Toggle.onValueChanged 触发时把新状态写回 AudioManager，由其持久化到 DataManager。
///
/// 用法：把三个 Toggle 分别拖入 musicToggle / sfxToggle / vibrateToggle 字段即可，
/// 无需在 Inspector 手动绑定 onValueChanged（本脚本自动绑定）。
/// </summary>
public class AudioSettingsPanel : MonoBehaviour
{
    [Tooltip("音乐开关 Toggle（isOn=true 表示音乐开启）")]
    public Toggle musicToggle;

    [Tooltip("音效开关 Toggle（isOn=true 表示音效开启）")]
    public Toggle sfxToggle;

    [Tooltip("震动开关 Toggle（isOn=true 表示震动开启）")]
    public Toggle vibrateToggle;

    /// <summary>防止刷新时反向触发 onValueChanged 导致多余写回。</summary>
    private bool _refreshing;

    private void Start()
    {
        // 首次激活（OnEnable 在 Start 之前）：若 AudioManager 尚未就绪，Start 再兜底同步一次
        RefreshFromData();

        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnMusicToggled);
        }
        if (sfxToggle != null)
        {
            sfxToggle.onValueChanged.AddListener(OnSfxToggled);
        }
        if (vibrateToggle != null)
        {
            vibrateToggle.onValueChanged.AddListener(OnVibrateToggled);
        }
    }

    private void OnEnable()
    {
        // 每次面板打开都从数据同步一次（设置可能在其他界面被修改）
        RefreshFromData();
    }

    /// <summary>从 AudioManager（数据）读取状态并刷新三个 Toggle。</summary>
    private void RefreshFromData()
    {
        AudioManager am = AudioManager.Instance;
        if (am == null)
        {
            return;
        }
        _refreshing = true;
        if (musicToggle != null)
        {
            musicToggle.isOn = !am.MusicMuted;
        }
        if (sfxToggle != null)
        {
            sfxToggle.isOn = !am.SfxMuted;
        }
        if (vibrateToggle != null)
        {
            vibrateToggle.isOn = am.VibrateEnabled;
        }
        _refreshing = false;
    }

    private void OnMusicToggled(bool isOn)
    {
        if (_refreshing)
        {
            return;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicMuted(!isOn);
        }
    }

    private void OnSfxToggled(bool isOn)
    {
        if (_refreshing)
        {
            return;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxMuted(!isOn);
        }
    }

    private void OnVibrateToggled(bool isOn)
    {
        if (_refreshing)
        {
            return;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVibrateEnabled(isOn);
        }
    }
}

using UnityEngine;

/// <summary>
/// AudioManager：全局音频管理（单例，DontDestroyOnLoad 常驻）。
/// - 通过**字符串**播放音频，字符串与 AudioClip 的映射由 AudioLibrary（ScriptableObject）预先配置；
/// - BGM 与 SFX 分离：音乐（循环）与音效（一次性）独立播放；
/// - **音乐音量 / 音乐静音、音效音量 / 音效静音独立控制**（无全局静音），
///   持久化到 DataManager（随存档保存，适配微信小游戏）。
///
/// 用法：
/// 1. 挂到常驻物体（与 DataManager/GameManager 同一物体即可），拖入 library（AudioLibrary 资产）；
/// 2. 代码中直接调用：
///    AudioManager.Instance.PlayBGM("BGM_关卡");        // 同曲不重播，循环
///    AudioManager.Instance.StopBGM();
///    AudioManager.Instance.PlaySFX("卡牌点击");
///    AudioManager.Instance.PlaySFX("消除", 1f, 1.2f);   // 可选 音量倍率 / 音调
///    AudioManager.Instance.SetMusicVolume(0.8f);
///    AudioManager.Instance.SetMusicMuted(true);
///    AudioManager.Instance.SetSfxVolume(1f);
///    AudioManager.Instance.SetSfxMuted(false);
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音频表")]
    [Tooltip("音频映射表（字符串 ↔ AudioClip），必填")]
    public AudioLibrary library;

    [Tooltip("默认 BGM key（可选，进入游戏自动播放该曲）")]
    public string defaultBgmKey;

    [Header("播放器")]
    [Tooltip("音乐播放器（循环播放；不拖则自动创建）")]
    public AudioSource musicSource;

    [Tooltip("音效播放器数量（轮转复用，避免常见音效互相打断；默认 4）")]
    public int sfxSourceCount = 4;

    /// <summary>当前音乐音量（0~1）。</summary>
    public float MusicVolume => _musicVolume;
    /// <summary>当前音效音量（0~1）。</summary>
    public float SfxVolume => _sfxVolume;
    /// <summary>音乐是否静音。</summary>
    public bool MusicMuted => _musicMuted;
    /// <summary>音效是否静音。</summary>
    public bool SfxMuted => _sfxMuted;
    /// <summary>震动总开关是否开启。</summary>
    public bool VibrateEnabled => _vibrateEnabled;

    private AudioSource[] _sfxSources;
    private int _sfxIndex;

    private float _musicVolume = 0.8f;
    private float _sfxVolume = 1f;
    private bool _musicMuted;
    private bool _sfxMuted;
    private bool _vibrateEnabled = true;

    private string _currentBgmKey;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAudioSettings();   // 从 DataManager 读取音量/静音
        EnsureSources();       // 创建/补齐播放器

        // 应用音量与静音（音乐静音用 AudioSource.mute，保留播放进度）
        if (musicSource != null)
        {
            musicSource.volume = _musicVolume;
            musicSource.mute = _musicMuted;
        }

        if (!string.IsNullOrEmpty(defaultBgmKey))
        {
            PlayBGM(defaultBgmKey);
        }
    }

    /// <summary>从 DataManager 读取音量/静音/震动设置（无存档时用默认值）。</summary>
    private void LoadAudioSettings()
    {
        if (DataManager.Instance != null)
        {
            _musicVolume = DataManager.Instance.MusicVolume;
            _sfxVolume = DataManager.Instance.SfxVolume;
            _musicMuted = DataManager.Instance.MusicMuted;
            _sfxMuted = DataManager.Instance.SfxMuted;
            _vibrateEnabled = DataManager.Instance.VibrateEnabled;
        }
    }

    /// <summary>写回 DataManager 并落盘（所有存档读写统一走 DataManager）。</summary>
    private void SaveAudioSettings()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetAudioSettings(_musicVolume, _sfxVolume, _musicMuted, _sfxMuted, _vibrateEnabled);
        }
    }

    private void EnsureSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        int count = Mathf.Max(1, sfxSourceCount);
        if (_sfxSources == null || _sfxSources.Length != count)
        {
            _sfxSources = new AudioSource[count];
        }
        for (int i = 0; i < count; i++)
        {
            if (_sfxSources[i] == null)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.loop = false;
                src.playOnAwake = false;
                _sfxSources[i] = src;
            }
        }
    }

    // ---------- BGM ----------

    /// <summary>播放 BGM（按 key 从音频表取曲；同曲已播放则忽略）。</summary>
    public void PlayBGM(string key)
    {
        if (musicSource == null)
        {
            return;
        }
        if (key == _currentBgmKey && musicSource.isPlaying)
        {
            return;
        }
        AudioClip clip = library != null ? library.GetClip(key) : null;
        if (clip == null)
        {
            return;
        }
        _currentBgmKey = key;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.mute = _musicMuted;
        musicSource.Play();
    }

    /// <summary>停止当前 BGM。</summary>
    public void StopBGM()
    {
        _currentBgmKey = null;
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // ---------- SFX ----------

    /// <summary>播放音效（按 key 从音频表取；静音时不播放）。</summary>
    public void PlaySFX(string key)
    {
        PlaySFX(key, 1f, 1f);
    }

    /// <summary>播放音效，可指定音量倍率（0~1）与音调（pitch）。</summary>
    public void PlaySFX(string key, float volumeScale, float pitch)
    {
        if (_sfxMuted || _sfxSources == null || _sfxSources.Length == 0)
        {
            return;
        }
        AudioClip clip = library != null ? library.GetClip(key) : null;
        if (clip == null)
        {
            return;
        }
        AudioSource src = _sfxSources[_sfxIndex];
        _sfxIndex = (_sfxIndex + 1) % _sfxSources.Length;
        src.clip = clip;
        src.volume = _sfxVolume * Mathf.Clamp01(volumeScale);
        src.pitch = Mathf.Max(0.1f, pitch);
        src.Play();
    }

    // ---------- 音量 / 静音（音乐、音效独立） ----------

    /// <summary>设置音乐音量（0~1）并持久化。</summary>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = _musicVolume;
        }
        SaveAudioSettings();
    }

    /// <summary>设置音乐静音（保留播放进度，取消静音后继续）。</summary>
    public void SetMusicMuted(bool muted)
    {
        _musicMuted = muted;
        if (musicSource != null)
        {
            musicSource.mute = muted;
        }
        SaveAudioSettings();
    }

    /// <summary>设置音效音量（0~1）并持久化。</summary>
    public void SetSfxVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        SaveAudioSettings();
    }

    /// <summary>设置音效静音（静音期间 PlaySFX 直接不播放）。</summary>
    public void SetSfxMuted(bool muted)
    {
        _sfxMuted = muted;
        SaveAudioSettings();
    }

    /// <summary>设置震动总开关（关闭后 VibrateLight/Medium/Strong 均不震动）并持久化。</summary>
    public void SetVibrateEnabled(bool enabled)
    {
        _vibrateEnabled = enabled;
        SaveAudioSettings();
    }

    // ---------- 震动（微信小游戏） ----------
    // 轻/中/强三档震动：
    // - 轻：wx.vibrateShort({ type: "light" })
    // - 中：wx.vibrateShort({ type: "medium" })
    // - 强：wx.vibrateLong()（400ms 长震，震感最强、全机型支持好）
    // 实现说明：SDK 的 VibrateShortOption 等类型在插件 dll 中，为避免工程脚本对插件 dll 的
    // 编译期依赖，统一在微信小游戏平台（UNITY_WEBGL 且非编辑器）运行时用反射调用；
    // 编辑器/其他平台仅打日志模拟，不报错。

    /// <summary>轻震动（wx.vibrateShort type=light；震动开关关闭时不震动）。</summary>
    public void VibrateLight()
    {
        if (!_vibrateEnabled)
        {
            return;
        }
        VibrateShort("light");
    }

    /// <summary>中震动（wx.vibrateShort type=medium；震动开关关闭时不震动）。</summary>
    public void VibrateMedium()
    {
        if (!_vibrateEnabled)
        {
            return;
        }
        VibrateShort("medium");
    }

    /// <summary>强震动（wx.vibrateLong，400ms 长震；震动开关关闭时不震动）。</summary>
    public void VibrateStrong()
    {
        if (!_vibrateEnabled)
        {
            return;
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        VibrateLongInternal();
#else
        Debug.Log("[AudioManager] 强震动(vibrateLong)：仅微信小游戏平台生效（当前为编辑器/其他平台，不执行）。");
#endif
    }

    private void VibrateShort(string type)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        VibrateShortInternal(type);
#else
        Debug.Log($"[AudioManager] 震动({type})：仅微信小游戏平台生效（当前为编辑器/其他平台，不执行）。");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    /// <summary>在已加载程序集中查找 SDK 类型（WX / VibrateShortOption / VibrateLongOption 等）。</summary>
    private static System.Type FindSdkType(string fullName)
    {
        foreach (System.Reflection.Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            System.Type t = asm.GetType(fullName);
            if (t != null)
            {
                return t;
            }
        }
        return null;
    }

    private static void VibrateShortInternal(string type)
    {
        try
        {
            System.Type optType = FindSdkType("WeChatWASM.VibrateShortOption");
            System.Type wxType = FindSdkType("WeChatWASM.WX");
            if (optType == null || wxType == null)
            {
                Debug.LogWarning("[AudioManager] 未找到微信小游戏 SDK 震动类型（VibrateShortOption/WX），无法执行短震动。");
                return;
            }
            object opt = System.Activator.CreateInstance(optType);
            System.Reflection.FieldInfo typeField = optType.GetField("type");
            if (typeField != null)
            {
                typeField.SetValue(opt, type);
            }
            System.Reflection.MethodInfo method = wxType.GetMethod("VibrateShort", new[] { optType });
            if (method != null)
            {
                method.Invoke(null, new[] { opt });
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[AudioManager] 调用 wx.vibrateShort({type}) 失败：{e.Message}");
        }
    }

    private static void VibrateLongInternal()
    {
        try
        {
            System.Type optType = FindSdkType("WeChatWASM.VibrateLongOption");
            System.Type wxType = FindSdkType("WeChatWASM.WX");
            if (optType == null || wxType == null)
            {
                Debug.LogWarning("[AudioManager] 未找到微信小游戏 SDK 震动类型（VibrateLongOption/WX），无法执行长震动。");
                return;
            }
            object opt = System.Activator.CreateInstance(optType);
            System.Reflection.MethodInfo method = wxType.GetMethod("VibrateLong", new[] { optType });
            if (method != null)
            {
                method.Invoke(null, new[] { opt });
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[AudioManager] 调用 wx.vibrateLong 失败：{e.Message}");
        }
    }
#endif
}

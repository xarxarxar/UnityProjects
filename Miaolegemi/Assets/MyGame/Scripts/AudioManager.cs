using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;

    [Header("Audio Clips")]
    public AudioClip[] backgroundMusicClips;
    public AudioClip[] soundEffectClips;

    // 引用 GameSettings 类
    private GameSettings gameSettings;

    private void Awake()
    {
        // 确保只有一个实例存在
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 在加载新场景时不销毁此对象

            // 初始化 GameSettings
            gameSettings = new GameSettings();
            gameSettings.LoadSettings(); // 从某种存储中加载设置

            // 设置初始音量
            SetVolume(gameSettings.MusicVolume, gameSettings.SFXVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clipIndex">背景音乐剪辑的索引</param>
    public void PlayBackgroundMusic(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= backgroundMusicClips.Length)
        {
            Debug.LogWarning("无效的背景音乐索引！");
            return;
        }

        backgroundMusicSource.clip = backgroundMusicClips[clipIndex];
        backgroundMusicSource.Play();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clipIndex">音效剪辑的索引</param>
    public void PlaySoundEffect(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= soundEffectClips.Length)
        {
            Debug.LogWarning("无效的音效索引！");
            return;
        }

        soundEffectSource.PlayOneShot(soundEffectClips[clipIndex]);
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="musicVolume">背景音乐音量</param>
    /// <param name="sfxVolume">音效音量</param>
    public void SetVolume(float musicVolume, float sfxVolume)
    {
        musicVolume = Mathf.Clamp(musicVolume, 0.0f, 1.0f);
        sfxVolume = Mathf.Clamp(sfxVolume, 0.0f, 1.0f);

        backgroundMusicSource.volume = musicVolume;
        soundEffectSource.volume = sfxVolume;

        // 更新 GameSettings 中的音量
        gameSettings.MusicVolume = musicVolume;
        gameSettings.SFXVolume = sfxVolume;

        // 保存更新后的设置
        gameSettings.SaveSettings();
    }
}

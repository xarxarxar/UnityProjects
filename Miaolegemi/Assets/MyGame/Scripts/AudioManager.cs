using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    private AudioSource backgroundMusicSource;
    private AudioSource soundEffectSource;

    [Header("Audio Clips")]
    public List<AudioClip> backgroundMusicClips=new List<AudioClip>();
    public List<AudioClip> soundEffectClips=new List<AudioClip>();

    // 用于存储音频剪辑的字典
    private Dictionary<BackgroundMusicType, AudioClip> backgroundMusicMap;
    private Dictionary<SoundEffectType, AudioClip> soundEffectMap;

    // 游戏设置引用
    private GameSettings gameSettings;

    private void Awake()
    {
        // 确保只有一个实例存在
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 在加载新场景时不销毁此对象


            // 获取 Main Camera 上的所有 AudioSource 组件
            AudioSource[] audioSources = Camera.main.GetComponents<AudioSource>();

            Debug.Log($"AudioSource length is {audioSources.Length}");
            if (audioSources.Length >= 2)
            {
                // 将第一个 AudioSource 设置为 backgroundMusicSource
                backgroundMusicSource = audioSources[0];

                // 将第二个 AudioSource 设置为 soundEffectSource
                soundEffectSource = audioSources[1];
            }
            else
            {
                Debug.LogError("Main Camera 上没有足够的 AudioSource 组件。");
            }



            // 初始化游戏设置
            gameSettings = new GameSettings();
            gameSettings.LoadSettings(); // 从某种存储中加载设置

            // 初始化音频剪辑映射
            InitializeAudioMaps();

            // 设置初始音量
            SetVolume(gameSettings.MusicVolume, gameSettings.SFXVolume);
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void InitializeAudioMaps()
    {
        backgroundMusicMap = new Dictionary<BackgroundMusicType, AudioClip>
        {
            { BackgroundMusicType.MainMenu, backgroundMusicClips[0] },
            { BackgroundMusicType.Gameplay, backgroundMusicClips[1] },
            { BackgroundMusicType.Victory, backgroundMusicClips[2] },
            { BackgroundMusicType.Defeat, backgroundMusicClips[3] }
        };

        soundEffectMap = new Dictionary<SoundEffectType, AudioClip>
        {
            { SoundEffectType.ClickButton, soundEffectClips[0] },
            { SoundEffectType.ClickCard, soundEffectClips[1] },
            { SoundEffectType.ComposeCard, soundEffectClips[2] },
            { SoundEffectType.SwitchPage, soundEffectClips[3] }
        };
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="musicType">背景音乐的类型</param>
    public void PlayBackgroundMusic(BackgroundMusicType musicType)
    {
        if (backgroundMusicMap.TryGetValue(musicType, out AudioClip clip))
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("无效的背景音乐类型！");
        }
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
    /// <param name="sfxType">音效的类型</param>
    public void PlaySoundEffect(SoundEffectType sfxType)
    {
        if (soundEffectMap.TryGetValue(sfxType, out AudioClip clip))
        {
            soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("无效的音效类型！");
        }
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

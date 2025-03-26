using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSettings audioSettings; // 引用 AudioSettings ScriptableObject

    [SerializeField]private AudioSource musicSource;  // 用于播放背景音乐
    [SerializeField]private AudioSource effectsSource;  // 用于播放音效

    private float musicVolume;
    public float MusicVolume 
    { 
        get => musicVolume;
        set
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                SetBackgroundMusicVolume(value/10);
                SettingManager.Instance.musicSlider.value = value;
            }
        } 
    }

    private float soundVolume;
    public float SoundVolume 
    { 
        get => soundVolume; 
        set 
        {
            if (soundVolume != value)
            {
                soundVolume = value;
                SetSoundEffectsVolume(value / 2);
                SettingManager.Instance.soundSlider.value = value;
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }


    // Start 用于初始化
    private void Start()
    {
        // 初始化音源设置
        InitializeAudio();
    }

    // 初始化音源
    private void InitializeAudio()
    {
        if (audioSettings != null)
        {
            // 设置背景音乐音量
            musicSource.volume = MusicVolume;

            // 设置音效音量
            effectsSource.volume = SoundVolume;

            // 设置背景音乐循环播放
            musicSource.loop = true;

            // 判断是否启用背景音乐
            if (audioSettings.enableBackgroundMusic && audioSettings.backgroundMusic != null)
            {
                musicSource.clip = audioSettings.backgroundMusic;
                musicSource.Play(); // 播放背景音乐
            }
        }
    }

    // 播放音效，使用音效的名字作为键来查找对应的音频
    public void PlaySoundEffect(string soundName)
    {
        if (audioSettings != null && audioSettings.enableSoundEffects)
        {
            if (audioSettings.soundEffects.ContainsKey(soundName))
            {
                AudioClip clip = audioSettings.soundEffects.Dictionary[soundName];
                effectsSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"Sound effect '{soundName}' not found in the dictionary!");
            }
        }
    }

    // 设置背景音乐音量
    public void SetBackgroundMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    // 设置音效音量
    public void SetSoundEffectsVolume(float volume)
    {
        if (effectsSource != null)
        {
            effectsSource.volume = volume;
        }
    }

    // 停止背景音乐
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // 播放背景音乐
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && audioSettings != null && audioSettings.enableBackgroundMusic)
        {
            musicSource.clip = audioSettings.backgroundMusic;
            musicSource.Play();
        }
    }
}

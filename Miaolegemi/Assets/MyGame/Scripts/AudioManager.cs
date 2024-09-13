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

    // ���ڴ洢��Ƶ�������ֵ�
    private Dictionary<BackgroundMusicType, AudioClip> backgroundMusicMap;
    private Dictionary<SoundEffectType, AudioClip> soundEffectMap;

    // ��Ϸ��������
    private GameSettings gameSettings;

    private void Awake()
    {
        // ȷ��ֻ��һ��ʵ������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �ڼ����³���ʱ�����ٴ˶���


            // ��ȡ Main Camera �ϵ����� AudioSource ���
            AudioSource[] audioSources = Camera.main.GetComponents<AudioSource>();

            Debug.Log($"AudioSource length is {audioSources.Length}");
            if (audioSources.Length >= 2)
            {
                // ����һ�� AudioSource ����Ϊ backgroundMusicSource
                backgroundMusicSource = audioSources[0];

                // ���ڶ��� AudioSource ����Ϊ soundEffectSource
                soundEffectSource = audioSources[1];
            }
            else
            {
                Debug.LogError("Main Camera ��û���㹻�� AudioSource �����");
            }



            // ��ʼ����Ϸ����
            gameSettings = new GameSettings();
            gameSettings.LoadSettings(); // ��ĳ�ִ洢�м�������

            // ��ʼ����Ƶ����ӳ��
            InitializeAudioMaps();

            // ���ó�ʼ����
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
    /// ���ű�������
    /// </summary>
    /// <param name="musicType">�������ֵ�����</param>
    public void PlayBackgroundMusic(BackgroundMusicType musicType)
    {
        if (backgroundMusicMap.TryGetValue(musicType, out AudioClip clip))
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("��Ч�ı����������ͣ�");
        }
    }

    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopBackgroundMusic()
    {
        backgroundMusicSource.Stop();
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="sfxType">��Ч������</param>
    public void PlaySoundEffect(SoundEffectType sfxType)
    {
        if (soundEffectMap.TryGetValue(sfxType, out AudioClip clip))
        {
            soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("��Ч����Ч���ͣ�");
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="musicVolume">������������</param>
    /// <param name="sfxVolume">��Ч����</param>
    public void SetVolume(float musicVolume, float sfxVolume)
    {
        musicVolume = Mathf.Clamp(musicVolume, 0.0f, 1.0f);
        sfxVolume = Mathf.Clamp(sfxVolume, 0.0f, 1.0f);

        backgroundMusicSource.volume = musicVolume;
        soundEffectSource.volume = sfxVolume;

        // ���� GameSettings �е�����
        gameSettings.MusicVolume = musicVolume;
        gameSettings.SFXVolume = sfxVolume;

        // ������º������
        gameSettings.SaveSettings();
    }
}

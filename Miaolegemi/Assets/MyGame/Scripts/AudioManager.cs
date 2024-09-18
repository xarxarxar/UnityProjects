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
                backgroundMusicSource.loop = true;//����һֱѭ��
                backgroundMusicSource.playOnAwake = false;//��Ҫһ��ʼ�Ͳ���

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
        };

        soundEffectMap = new Dictionary<SoundEffectType, AudioClip>
        {
            { SoundEffectType.ClickButton, soundEffectClips[0] },
            { SoundEffectType.ClickCard, soundEffectClips[1] },
            { SoundEffectType.ComposeCard, soundEffectClips[2] },
            { SoundEffectType.SwitchPage, soundEffectClips[3] },
            { SoundEffectType.Victory, soundEffectClips[4] },
            { SoundEffectType.Defeat, soundEffectClips[5] }
        };
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="musicType">�������ֵ�����</param>
    /// <param name="volum">�������ֵ�������Ĭ��ֵΪ1��</param>
    public void PlayBackgroundMusic(BackgroundMusicType musicType,float volum=1)
    {
        if (backgroundMusicMap.TryGetValue(musicType, out AudioClip clip))
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.volume = volum;  // ��������
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("��Ч�ı����������ͣ�");
        }
    }

    // <summary>
    /// ����ǰ��bgm��������Ϊvolum��С
    /// </summary>
    /// <param name="volum">������С����ΧΪ0��1</param>
    public void SetBgmVolum(float volum)
    {
        // ȷ�������ں���Χ��
        volum = Mathf.Clamp(volum, 0f, 1f);

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volum;
        }
        else
        {
            Debug.LogWarning("��������Դδ���ã�");
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
    /// ����һ��audioclip
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public void ReverseAudioClip(SoundEffectType sfxType)
    {
        if (soundEffectMap.TryGetValue(sfxType, out AudioClip clip))
        {
            // ��ȡԭʼ��Ƶ����
            float[] data = new float[clip.samples * clip.channels];
            clip.GetData(data, 0);

            // ������Ƶ����
            float[] reversedData = new float[data.Length];
            int dataLength = data.Length;
            for (int i = 0; i < dataLength; i++)
            {
                reversedData[i] = data[dataLength - i - 1];
            }

            // �����µ� AudioClip
            AudioClip reversedClip = AudioClip.Create("ReversedClip", clip.samples, clip.channels, clip.frequency, false);
            reversedClip.SetData(reversedData, 0);
            soundEffectSource.PlayOneShot(reversedClip);
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

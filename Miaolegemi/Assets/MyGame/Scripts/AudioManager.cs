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

    // ���� GameSettings ��
    private GameSettings gameSettings;

    private void Awake()
    {
        // ȷ��ֻ��һ��ʵ������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �ڼ����³���ʱ�����ٴ˶���

            // ��ʼ�� GameSettings
            gameSettings = new GameSettings();
            gameSettings.LoadSettings(); // ��ĳ�ִ洢�м�������

            // ���ó�ʼ����
            SetVolume(gameSettings.MusicVolume, gameSettings.SFXVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="clipIndex">�������ּ���������</param>
    public void PlayBackgroundMusic(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= backgroundMusicClips.Length)
        {
            Debug.LogWarning("��Ч�ı�������������");
            return;
        }

        backgroundMusicSource.clip = backgroundMusicClips[clipIndex];
        backgroundMusicSource.Play();
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
    /// <param name="clipIndex">��Ч����������</param>
    public void PlaySoundEffect(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= soundEffectClips.Length)
        {
            Debug.LogWarning("��Ч����Ч������");
            return;
        }

        soundEffectSource.PlayOneShot(soundEffectClips[clipIndex]);
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

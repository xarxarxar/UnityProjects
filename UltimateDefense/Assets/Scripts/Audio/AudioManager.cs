using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Library")]
    public AudioLibrary audioLibrary;

    private ObjectPool<AudioSource> sfxPool;

    private static AudioManager _instance;//单例
    /// <summary>
    /// GameManger单例
    /// </summary>
    public static AudioManager Instance { get => _instance; }


    protected void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        if (audioLibrary != null)
            audioLibrary.Init();

        if (sfxPool == null)
        {
            sfxPool = new ObjectPool<AudioSource>(sfxSource,10,transform);
        }
    }

    // 播放背景音乐（通过 key）
    public void PlayBGM(string key, bool loop = true)
    {
        var clip = audioLibrary.GetClip(key);
        if (clip != null)
        {
            PlayBGM(clip, loop);
        }
    }

    // 播放背景音乐（直接播放 clip）
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySFX(string key)
    {
        var clip = audioLibrary.GetClip(key);
        if (clip != null)
        {
            PlaySFX(clip);
        }
    }

    public void PlaySFX(string key, float pitch = 1f)
    {
        var clip = audioLibrary.GetClip(key);
        if (clip != null)
        {
            PlaySFX(clip, pitch);
        }
    }

    // 播放音效（可指定 Pitch）
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        AudioSource sfx = sfxPool.Get();
        sfx.clip = clip;
        sfx.volume = sfxVolume;
        sfx.pitch = pitch;
        sfx.Play();
        DOVirtual.DelayedCall(clip.length / pitch, () =>
        {
            sfx.Stop();
            sfx.clip = null;
            sfx.gameObject.SetActive(false);
            sfxPool.Return(sfx);
        }); // 考虑 pitch 对播放时长的影响
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmSource != null) bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    /// <summary>
    /// 获取audioclip
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public AudioClip GetAudioClip(string key)
    {
        var clip = audioLibrary.GetClip(key);
        if (clip != null)
        {
            return clip;
        }
        return null;
    }
}

[System.Serializable]
public class AudioData
{
    public string key;
    public AudioClip clip;
}
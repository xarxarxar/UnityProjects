using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bgmSourcePrefab;
    public AudioSource sfxSourcePrefab;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Library")]
    public AudioLibrary audioLibrary;

    private AudioSource currentBgm;
    private readonly Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    public static AudioManager Instance;

    protected void Awake()
    {
        Instance=this;


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
        if (currentBgm != null)
        {
            Destroy(currentBgm);
        }

        currentBgm = Instantiate(bgmSourcePrefab, transform);
        currentBgm.clip = clip;
        currentBgm.loop = loop;
        currentBgm.volume = bgmVolume;
        currentBgm.Play();
    }

    public void StopBGM()
    {
        if (currentBgm != null)
        {
            currentBgm.Stop();
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

    /// <summary>
    /// 播放一次音效。
    /// </summary>
    /// <param name="clip">要播放的音效片段。</param>
    /// <param name="pitch">播放音高，会影响回收等待时间。</param>
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        AudioSource sfx = GetSfxSource();
        sfx.clip = clip;
        sfx.volume = sfxVolume;
        sfx.pitch = pitch;
        sfx.Play();
        StartCoroutine(ReturnSfxSourceAfterPlaying(sfx, clip.length / pitch)); // 保持原来的播放时长公式
    }

    /// <summary>
    /// 获取一个可用的音效 AudioSource。
    /// </summary>
    /// <returns>可用于播放本次音效的 AudioSource。</returns>
    private AudioSource GetSfxSource()
    {
        if (sfxPool.Count > 0)
        {
            AudioSource pooledSource = sfxPool.Dequeue();
            pooledSource.gameObject.SetActive(true);
            return pooledSource;
        }

        return Instantiate(sfxSourcePrefab, transform);
    }

    /// <summary>
    /// 音效播放结束后回收到池中。
    /// </summary>
    /// <param name="sfx">需要回收的音效源。</param>
    /// <param name="delay">等待回收的时间。</param>
    private IEnumerator ReturnSfxSourceAfterPlaying(AudioSource sfx, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (sfx == null)
        {
            yield break;
        }

        sfx.Stop();
        sfx.clip = null;
        sfx.gameObject.SetActive(false);
        sfxPool.Enqueue(sfx);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (currentBgm != null) currentBgm.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
}

[System.Serializable]
public class AudioData
{
    public string key;
    public AudioClip clip;
}

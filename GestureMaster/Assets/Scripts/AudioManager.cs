using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSourcePrefab;
    public AudioSource sfxSourcePrefab;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Library")]
    public AudioLibrary audioLibrary;

    private AudioSource currentBgm;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioLibrary != null)
            audioLibrary.Init();
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

    // 播放音效（可指定 Pitch）
    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        AudioSource sfx = Instantiate(sfxSourcePrefab, transform);
        sfx.clip = clip;
        sfx.volume = sfxVolume;
        sfx.pitch = pitch;
        sfx.Play();
        Destroy(sfx.gameObject, clip.length / pitch); // 考虑 pitch 对播放时长的影响
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


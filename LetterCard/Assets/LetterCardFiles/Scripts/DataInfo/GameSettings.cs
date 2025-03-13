using System;
using UnityEngine;

[Serializable]
public class GameSettings
{
    // 音乐音量
    [Range(0f, 1f)]
    public float musicVolume;

    // 音效音量
    [Range(0f, 1f)]
    public float soundEffectVolume;

    public GameSettings()
    {
        musicVolume = 0.5f;
        soundEffectVolume = 0.5f;
    }

    // 构造函数
    public GameSettings(float musicVolume, float soundEffectVolume)
    {
        this.musicVolume = musicVolume;
        this.soundEffectVolume = soundEffectVolume;
    }
}

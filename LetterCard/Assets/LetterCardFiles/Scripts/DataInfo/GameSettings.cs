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

    // 游戏语言
    public string language;

    public GameSettings()
    {

    }

    // 构造函数
    public GameSettings(float musicVolume, float soundEffectVolume, string language)
    {
        this.musicVolume = musicVolume;
        this.soundEffectVolume = soundEffectVolume;
        this.language = language;
    }
}

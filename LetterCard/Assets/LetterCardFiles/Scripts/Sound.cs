using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Sound 类定义了一个音效或音乐的基本属性，
// 主要包含音频名称、音频文件（AudioClip）以及音量等信息。
[System.Serializable]
public class Sound
{
    // 音频的名称，用于在播放音效或背景音乐时查找对应的音频。
    public string name;

    // 音频文件，存储音效或背景音乐的音频剪辑。
    public AudioClip clip;

    // 音量，设置播放音频的音量。默认值为 1，表示最大音量。
    // 使用 [Range] 特性将音量限制在 0 到 1 之间，便于在编辑器中调整。
    [Range(0f, 1f)] public float volume = 1;
}

// AudioManager 类用于管理所有的音频播放。
// 它包含了背景音乐和音效的播放、初始化和控制等功能。
public class AudioManager : MonoBehaviour
{
    // 存储所有背景音乐的数组。每一项都是一个 Sound 类型的音频信息。
    public Sound[] musicTracks;

    // 存储所有音效的数组。每一项也是一个 Sound 类型的音频信息。
    public Sound[] soundEffects;

    // 一个字典，用于存储通过名称查找到的音频源（AudioSource）。
    // 字典的键是音频的名称，值是对应的音频源（AudioSource）。
    private Dictionary<string, AudioSource> soundSources = new();

    // Awake 方法在脚本初始化时调用，负责初始化音频源。
    void Awake()
    {
        // 将背景音乐和音效数组合并，遍历所有音频项来初始化音频源。
        foreach (var s in musicTracks.Concat(soundEffects))
        {
            // 为每个音频项创建一个 AudioSource 组件
            var source = gameObject.AddComponent<AudioSource>();

            // 设置音频源的音频剪辑为当前音效或背景音乐的剪辑
            source.clip = s.clip;

            // 设置音频源的音量为当前音效或背景音乐的音量
            source.volume = s.volume;

            // 将音频源添加到字典中，使用音频的名称作为键，音频源作为值
            soundSources.Add(s.name, source);
        }
    }

    // Play 方法用于播放指定名称的音频。
    // 输入参数 name 是音频的名称，用来查找字典中的音频源。
    public void Play(string name)
    {
        // 如果字典中包含指定名称的音频源，则播放该音频
        if (soundSources.ContainsKey(name))
            soundSources[name].Play();
    }
}


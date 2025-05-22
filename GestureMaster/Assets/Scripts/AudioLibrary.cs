using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    public AudioData[] audioClips;

    private Dictionary<string, AudioClip> clipDict;

    public void Init()
    {
        clipDict = new Dictionary<string, AudioClip>();
        foreach (var data in audioClips)
        {
            if (!clipDict.ContainsKey(data.key))
                clipDict.Add(data.key, data.clip);
        }
    }

    public AudioClip GetClip(string key)
    {
        if (clipDict == null) Init();
        return clipDict.TryGetValue(key, out var clip) ? clip : null;
    }
}
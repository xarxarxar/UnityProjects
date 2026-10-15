using UnityEngine;

/// <summary>
/// 音频映射表（ScriptableObject）：预先配置"字符串 ↔ AudioClip"的对应关系。
/// AudioManager 通过字符串播放音频，实际 clip 从本表查找。
///
/// 用法：
/// 1. 在 Project 窗口右键 → Create → ChainClick → Audio Library，创建资产（如 Assets/Audio/AudioLibrary.asset）；
/// 2. 在 Inspector 中配置 entries：key（播放用的字符串，如 "卡牌点击"/"BGM_关卡"）+ clip（对应音频）；
/// 3. 把该资产拖到场景中 AudioManager 的 library 字段。
/// </summary>
[CreateAssetMenu(fileName = "AudioLibrary", menuName = "ChainClick/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        [Tooltip("播放用的字符串名，与代码中 PlayBGM/PlaySFX 传入的 key 完全一致")]
        public string key;

        [Tooltip("对应的音频资源")]
        public AudioClip clip;
    }

    [Tooltip("字符串 ↔ AudioClip 映射表")]
    public AudioEntry[] entries;

    /// <summary>按 key 查找 AudioClip；key 为空或未配置时返回 null 并打印警告（便于排查）。</summary>
    public AudioClip GetClip(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[AudioLibrary] key 为空，无法查找 AudioClip。");
            return null;
        }
        if (entries != null)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] != null && entries[i].key == key)
                {
                    return entries[i].clip;
                }
            }
        }
        Debug.LogWarning($"[AudioLibrary] 未找到 key=\"{key}\" 对应的 AudioClip，请在 AudioLibrary 资产中配置该条目。");
        return null;
    }
}

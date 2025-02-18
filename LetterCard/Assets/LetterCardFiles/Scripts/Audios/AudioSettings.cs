using System.Collections.Generic;
using UnityEngine;
using SerializableDictionary.Scripts;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Game Config/AudioSettings", order = 1)]
public class AudioSettings : ScriptableObject
{
    // ±≥æ∞“Ù¿÷“Ù∆µ∆¨∂Œ
    public AudioClip backgroundMusic;

    // “Ù–ß“Ù∆µ∆¨∂Œ
    [SerializeField]
    public SerializableDictionary<string,AudioClip> soundEffects =new SerializableDictionary<string,AudioClip>();

    // “Ù–ß“Ù¡ø
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

    // “Ù¿÷“Ù¡ø
    [Range(0f, 1f)]
    public float backgroundMusicVolume = 1f;

    //  «∑Ò∆Ù”√±≥æ∞“Ù¿÷
    public bool enableBackgroundMusic = true;

    //  «∑Ò∆Ù”√“Ù–ß
    public bool enableSoundEffects = true;
}

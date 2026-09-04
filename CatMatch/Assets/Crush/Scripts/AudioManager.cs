using System;
using System.Collections;
using System.Collections.Generic;
using MergeBeast;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public Sound[] sounds;

    private IEnumerator randomIE;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        foreach (var s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void StartCrush()
    {
        if (SoundManager.Instance != null && !SoundManager.Instance.MusicIsOn) return;
        Play("bgm_battle");
    }

    public void StartBattle()
    {
        if (SoundManager.Instance != null && !SoundManager.Instance.MusicIsOn) return;

        // if (randomIE != null) StopCoroutine(randomIE);
        // randomIE = Random();
        // StartCoroutine(randomIE);
    }

    public void EndBattle()
    {
        if (SoundManager.Instance != null && !SoundManager.Instance.MusicIsOn) return;
        // if (randomIE != null) StopCoroutine(randomIE);
    }

    private void Play(string name, bool isMusic = false)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        sound?.source.Play();
    }

    private void Stop(string name)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        sound?.source.Stop();
    }

    private IEnumerator Random()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            var val = UnityEngine.Random.Range(0.5f, 1.5f);

            yield return new WaitForSeconds(val);

            Play("explosion");
        }
    }

    public void MonsterDie()
    {
        if (SoundManager.Instance != null && !SoundManager.Instance.SoundIsOn) return;
        Play("die");
    }

    public void StopAll()
    {
        // if (randomIE != null)
        //     StopCoroutine(randomIE);
        foreach (var s in sounds)
        {
            s.source.Stop();
        }
    }
}

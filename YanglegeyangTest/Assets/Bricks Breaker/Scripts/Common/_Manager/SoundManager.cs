using System;
using UnityEngine;
using DG.Tweening;

public class SoundManager : Singleton<SoundManager>
{
	public AudioSource activeEffectLoop;
	public AudioSource activeEffect;
	public AudioSource activeBGM;

	private void Awake()
	{
		//get audiosource component
		activeEffectLoop = gameObject.AddComponent<AudioSource>();
		activeEffect = gameObject.AddComponent<AudioSource>();
		activeBGM = gameObject.AddComponent<AudioSource>();
	}


	private void Start()
	{
		activeBGM.volume = Data.VolumeMusic;
		activeEffect.volume = Data.VolumeEffect;
	}
	
	/// <summary>
	/// Repeat sound effect - Play
	/// </summary>
	public void PlayEffectLoop(string soundName)
	{
		activeEffectLoop.loop = true;
		activeEffectLoop.clip = Resources.Load(string.Format("{0}{1}", Data.path_sound, soundName)) as AudioClip;
		activeEffectLoop.Play();
	}

	/// <summary>
	/// Repeat sound effect - Stop
	/// </summary>
	public void StopEffectLoop()
	{
		if (activeEffectLoop == null) return;
		activeEffectLoop.Stop();
	}


	/// <summary>
	/// Sound effect - Play
	/// </summary>
	public void PlayEffect(string effect)
	{
		activeEffect.loop = false;
		activeEffect.playOnAwake = false;
		AudioClip clip = Resources.Load(string.Format("{0}{1}", Data.path_sound, effect)) as AudioClip;
		activeEffect.PlayOneShot(clip);
	}


	/// <summary>
	/// Sound BGM - Play
	/// </summary>
	public void PlayBGM(string music, bool isLoop = true)
	{
		if (activeBGM.clip)
		{
			if (activeBGM.clip.name == music) return;

			DOTween.To(() => activeBGM.volume, x => activeBGM.volume = x, 0f, 0.1f).SetEase(Ease.Linear).OnComplete(
				() =>
				{
					activeBGM.Stop();
					activeBGM.clip = null;
					PlayBGM(music, isLoop);
				}).SetUpdate(true);
		}
		else
		{
			activeBGM.loop = isLoop;
			activeBGM.volume = 1f;
			activeBGM.clip = Resources.Load(string.Format("{0}{1}", Data.path_sound, music)) as AudioClip;
			activeBGM.Play();
		}
	}

	/// <summary>
	/// Sound BGM - Pause
	/// </summary>
	public void PauseBGM()
	{
		if (activeBGM.clip == null) return;

		DOTween.To(() => activeBGM.volume, x => activeBGM.volume = x, 0f, 0.1f).SetEase(Ease.Linear)
			.SetUpdate(true);
		activeBGM.Pause();
	}


	/// <summary>
	/// Sound BGM - Resume
	/// </summary>
	public void ResumeBGM()
	{
		if (activeBGM.clip == null) return;

		activeBGM.Play();
		DOTween.To(() => activeBGM.volume, x => activeBGM.volume = x, 1f, 0.1f).SetEase(Ease.Linear)
			.SetUpdate(true);
	}


	/// <summary>
	/// Sound BGM - Stop
	/// </summary>
	public void StopBGM()
	{
		if (activeBGM.clip == null) return;

		DOTween.To(() => activeBGM.volume, x => activeBGM.volume = x, 0f, 0.1f).SetEase(Ease.Linear).OnComplete(
			() =>
			{
				activeBGM.Stop();
				Destroy(activeBGM.gameObject);
				activeBGM = null;
			}).SetUpdate(true);
	}

	/// <summary>
	/// BGMVolume Control
	/// </summary>
	public void SetBgmVolume()
	{
		activeBGM.volume = Data.VolumeMusic;
	}
	
	/// <summary>
	/// EffectVolume Control
	/// </summary>
	public void SetEffectVolume()
	{
		activeEffect.volume = Data.VolumeEffect;
	}
}
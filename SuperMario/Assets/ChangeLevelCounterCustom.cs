using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevelCounterCustom : MonoBehaviour {
	AudioSource DieMusicAudio;
	public string LevelToLoad;
	public float TimeToLoad;

	// Use this for initialization
	void Start () {

		Invoke("LoadLevel", TimeToLoad);
	}

	void LoadLevel() {
		SceneManager.LoadScene (LevelToLoad);

	}
}

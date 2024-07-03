using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeCounter : MonoBehaviour {

	AudioSource DieMusicAudio;
	public string LevelToLoad;

	// Use this for initialization
	void Start () {
		DieMusicAudio = GetComponent<AudioSource> ();

		Invoke("LoadLevel", DieMusicAudio.clip.length);
	}
	
	void LoadLevel() {
		SceneManager.LoadScene (LevelToLoad);

	}
}

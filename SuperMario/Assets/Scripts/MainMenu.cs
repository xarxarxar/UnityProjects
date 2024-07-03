using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public string firstLevel;
	public string levelSelect;
	public string[] levelNames;

	public int startingLives;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void NewGame() {
		SceneManager.LoadScene (firstLevel);

		foreach (var levelName in levelNames) {
			PlayerPrefs.SetInt (levelName, 0);
		}

		PlayerPrefs.SetInt ("CoinCount", 0);
		PlayerPrefs.SetInt ("PlayerLives", startingLives);
	}
	public void Continue() {
		SceneManager.LoadScene (levelSelect);

	}
	public void QuitGame() {
		Application.Quit();
	}

}

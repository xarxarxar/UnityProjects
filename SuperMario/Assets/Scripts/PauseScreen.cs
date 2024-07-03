using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour {

	public string levelSelect;
	public string mainMenu;
	public GameObject thePauseScreen;

	private LevelManager theLevelManager;

	private PlayerController thePlayer;

	// Use this for initialization
	void Start () {
		theLevelManager = FindObjectOfType<LevelManager> ();
		thePlayer = FindObjectOfType<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Pause")) {
			if (Time.timeScale == 0) {
				ResumeGame ();
			} else {
				PauseGame ();
			}
		}
	}
	public void PauseGame() {
		Time.timeScale = 0;

		thePauseScreen.SetActive (true);
		thePlayer.canMove = false;
		theLevelManager.levelMusic.Pause ();
	}
	public void ResumeGame() {
		thePauseScreen.SetActive (false);

		thePlayer.canMove = true;

		theLevelManager.levelMusic.Play();

		Time.timeScale = 1f;

	}
	public void LevelSelect() {
		PlayerPrefs.SetInt ("PlayerLives", theLevelManager.currentLives);
		PlayerPrefs.SetInt ("CoinCount", theLevelManager.coinCount);

		Time.timeScale = 1f;

		SceneManager.LoadScene (levelSelect);
	}
	public void QuitToMainMenu() {
		Time.timeScale = 1f;

		SceneManager.LoadScene (mainMenu);
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public float waitToRespawn;
	public PlayerController thePlayer;
	public GameObject deathExplosion;
	public int coinCount;
	private int coinBonusLifeCount;
	public int bonusLifeThreshold;

	public Text coinText;

	public int maxHealth;
	public int healthCount;

	private bool respawning;

	private ResetOnRespawn[] objectsToReset;

	public bool invincible;

	public int startingLives;
	public int currentLives;

	public GameObject gameOverScreen;

	public AudioSource levelMusic;
	public AudioSource gameOverMusic;

	public AudioSource coinAudioSource;

	// Use this for initialization
	void Start () {
		thePlayer = FindObjectOfType<PlayerController> ();

		healthCount = maxHealth;

		objectsToReset = FindObjectsOfType<ResetOnRespawn> ();

//		if (PlayerPrefs.HasKey ("CoinCount")) {
//			coinCount = PlayerPrefs.GetInt ("CoinCount");
//		}

		coinText.text = "00" /*+ coinCount*/;

		if (PlayerPrefs.HasKey ("PlayerLives")) {
			currentLives = PlayerPrefs.GetInt ("PlayerLives");

		} else {
			currentLives = startingLives;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (healthCount <= 0 && !respawning) {
			Respawn ();
			respawning = true;
		}
		if (coinBonusLifeCount >= bonusLifeThreshold) {
			currentLives++;
			coinBonusLifeCount -= bonusLifeThreshold;
		}
	}

	public void Respawn()
	{
		currentLives--;
		if (currentLives > 0) {
			StartCoroutine (RespawnCo ());	
		} else {
			thePlayer.gameObject.SetActive (false);
			gameOverScreen.SetActive (true);
			levelMusic.Stop ();
			gameOverMusic.Play ();
			//levelMusic.volume /= 2f;
		}
	}

	public IEnumerator RespawnCo() {
		thePlayer.gameObject.SetActive (false);

		Instantiate (deathExplosion, thePlayer.transform.position, thePlayer.transform.rotation);

		yield return new WaitForSeconds (waitToRespawn);

		healthCount = maxHealth;
		respawning = false;

		coinCount = 0;
		coinBonusLifeCount = 0;
		coinText.text = "Coins: " + coinCount;

		thePlayer.transform.position = thePlayer.respawnPosition;
		thePlayer.gameObject.SetActive (true);

		for (int i = 0; i < objectsToReset.Length; ++i) {		
			objectsToReset [i].gameObject.SetActive (true);	
			objectsToReset [i].ResetObject ();
		}
	}
	public void AddCoins(int coinsToAdd) {
		coinCount += coinsToAdd;
		coinBonusLifeCount += coinsToAdd;

		coinText.text = "Coins: " + coinCount;
		coinAudioSource.Play ();
	}

	public void HurtPlayer(int damageToTake) {
		if (!invincible) {
			//healthCount -= damageToTake;

			thePlayer.PlayerEnemyCollision ();
			//thePlayer.HurtAudioSource.Play ();
		}
	}
	public void GiveHealth(int healthToGive) {
		healthCount += healthToGive;
		if (healthCount < maxHealth) {
			healthCount = maxHealth;
		}
		coinAudioSource.Play ();

	}

	public void AddLives(int livesToAdd) {
		currentLives += livesToAdd;

		coinAudioSource.Play ();
	}
}

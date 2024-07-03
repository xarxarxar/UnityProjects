using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour {

	Animator theAnimator;
	public Sprite MetalFace;
	public SpriteRenderer Block;
	public AudioSource BumpAudioSource; 
	public AudioSource CoinSound;
	public AudioSource BreakBlockAudioSource;
	public int coinAmount;
	public bool isMetalFace;
	bool triggered = false;

	// Use this for initialization
	void Start () {
		theAnimator = GetComponent<Animator> ();
		isMetalFace = false;

		if (coinAmount > 0) {
			theAnimator.SetBool ("HadCoins",true);

		}
	}
	
	// Update is called once per frame
	void Update () {

//		if (Block.sprite != MetalFace && theAnimator.GetBool ("HadCoins")==true) {
//			isMetalFace = true;
//			Block.sprite = MetalFace;
//			Debug.Log ("coinAmountZero!");
//		}
	}
	void OnTriggerEnter2D(Collider2D other) {
		if (coinAmount > 0) {
			CoinSound.Play ();

		}
		BumpAudioSource.Play ();
		if (other.tag == "Player" && triggered == false) {
			theAnimator.SetBool ("HitAnimation",true);
			if (PlayerController.PlayerState != PlayerController.PlayerStates.Small) {
				theAnimator.SetBool ("MarioBig", true);
			} else {
				theAnimator.SetBool ("MarioBig", false);
			}

			triggered = true;
			if (!(theAnimator.GetBool ("HadCoins") == true && coinAmount <= 0)) {				
				theAnimator.Play ("Idle");
				theAnimator.SetBool ("HitAnimation", true);
			}

		}

	}
	void OnTriggerExit2D(Collider2D other) {
		if (coinAmount > 0 && triggered) {
			triggered = false;
			coinAmount--;
			theAnimator.SetInteger ("Coins", coinAmount);
			theAnimator.SetBool ("HadCoins", true);
		}
	}
	public void HitNoCoinDone() {
		theAnimator.SetBool ("HitAnimation",false);
		triggered = false;

	}
	public void HitLastCoinDone() {
		theAnimator.SetBool ("HitAnimation", false);


	}
	public void HitCoinsDone() {
		theAnimator.SetBool ("HitAnimation", false);
		triggered = false;
	}
	public void OnBrickBreak() {
		BreakBlockAudioSource.Play ();
		BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
		foreach (BoxCollider2D c in colliders) {
			c.enabled = false;
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour {

	public float fadeTime;

	private Image blackScreen;

	// Use this for initialization
	void Start () {
		blackScreen = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		blackScreen.CrossFadeAlpha (0f, fadeTime, false);

		if (blackScreen.color.a == 0f) {
			gameObject.SetActive (false);


		}
	}
}

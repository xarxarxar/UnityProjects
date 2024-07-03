using UnityEngine;
using System.Collections;

public class SpiderController : MonoBehaviour {

	public float moveSpeed;
	public bool canMove;
	public Animator myAnimator;

	private Rigidbody2D myRigidbody;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		myAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (canMove && PlayerController.PauseAllAnimations == false) {
			if (myAnimator.enabled == false) {
				myAnimator.enabled = true;
			}
			myRigidbody.velocity = new Vector3 (-moveSpeed, myRigidbody.velocity.y, 0f);
		}
		if (PlayerController.PauseAllAnimations == true) {
			myRigidbody.velocity = Vector2.zero;
			myAnimator.enabled = false;
		}
	}
	void OnBecameVisible() {
		canMove = true;

	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "KillPlane") {
			//Destroy (gameObject);
			gameObject.SetActive (false);

		} else if (other.tag != "Flower" || other.tag != "Mushroom") {
			moveSpeed *= -1;
		}

	}
	void OnEnable() {
		canMove = false;

	}
}

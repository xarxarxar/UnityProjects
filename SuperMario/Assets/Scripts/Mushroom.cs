using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour {
	public float moveSpeed;
	public bool canMove;

	private Rigidbody2D myRigidbody;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		myRigidbody.velocity = Vector2.zero;
	}

	// Update is called once per frame
	void Update () {
		//if (canMove) {

			myRigidbody.velocity = new Vector3 (moveSpeed, myRigidbody.velocity.y, 0f);

		//}
	}
//	void OnBecameVisible() {
//		canMove = true;
//
//	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "KillPlane") {
			//Destroy (gameObject);
			gameObject.SetActive(false);

		}
		//if (canMove)
			moveSpeed *= -1f;
	}
	void OnEnable() {
		canMove = false;

	}
}

using UnityEngine;
using System.Collections;

public class StompEnemy : MonoBehaviour {

	private Rigidbody2D playerRigidbody;

	public float bounceForce;

	public GameObject deathExplosion;
	public GameObject goombaSplat;

	public AudioSource goombaStompSound;

	// Use this for initialization
	void Start () {
		playerRigidbody = transform.parent.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy") {
			//Destroy (other.gameObject);
			other.gameObject.SetActive(false);

			Instantiate (deathExplosion,  other.transform.position, other.transform.rotation);

			playerRigidbody.velocity = new Vector3 (playerRigidbody.velocity.x, bounceForce, 0f);
		}
		if (other.tag == "Goomba") {
			other.gameObject.SetActive(false);

			Vector3 vec = other.transform.position;
			vec.y += -.2f;

			Instantiate (goombaSplat, vec, other.transform.rotation);

			playerRigidbody.velocity = new Vector3 (playerRigidbody.velocity.x, bounceForce, 0f);

			goombaStompSound.Play ();
		}
	}
}

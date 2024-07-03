using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject target;
	public float followAhead;

	public float smoothing;

	private Vector3 targetPosition;

	public bool followTarget;

	// Use this for initialization
	void Start () {
		followTarget = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (followTarget && target.transform.position.x >= transform.position.x) {
			targetPosition = new Vector3 (target.transform.position.x, transform.position.y, transform.position.z);

			if (target.GetComponent<SpriteRenderer> ().flipX == false) {
				targetPosition = new Vector3 (targetPosition.x + followAhead, targetPosition.y, targetPosition.z);

				transform.position = Vector3.Lerp (transform.position, targetPosition, smoothing * Time.deltaTime);
			} 

			//transform.position = targetPosition;

			//transform.position = Vector3.Lerp (transform.position, targetPosition, smoothing * Time.deltaTime);
		}
	}

}

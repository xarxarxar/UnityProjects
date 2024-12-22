using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TiltCamera : MonoBehaviour {
    private bool isShaking = false;
    private float shakePower = 0.02f;
    private float time = 0.2f;
    private Vector3 initialPosition;

    private void Start () {
        initialPosition = transform.localPosition;
    }
  
    public void Shaking (float power) {
        shakePower = power;
        StartCoroutine(ShakingCo());
    }

    IEnumerator ShakingCo () {
        isShaking = true;
        yield return new WaitForSeconds(time);
        isShaking = false;
    }

    void Update () {
        if(isShaking) {
            Vector3 pos = initialPosition + Random.insideUnitSphere * shakePower;
            pos.z = initialPosition.z;
            transform.localPosition = pos;
        } else {
            transform.localPosition = initialPosition;
        }
    }
}

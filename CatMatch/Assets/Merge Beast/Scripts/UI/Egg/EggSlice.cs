using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSlice : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        BackToNormal();
    }


    public void Break() {
        rb.bodyType = RigidbodyType2D.Dynamic;
        //float x = Random.Range(-100, 100);
        //float y = Random.Range(-100, 200);
        //rb.AddForce(new Vector2(x, y));
        float x = Random.Range(-2f, 2f);
        float y = Random.Range(1.5f, 2f);
        transform.localPosition = new Vector2(x, y);
    }

    public void BackToNormal() {
        transform.localPosition = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }
}

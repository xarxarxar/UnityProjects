using System.Collections;
using UnityEngine;

public class Roket : MonoBehaviour
{

    Rigidbody2D rb;
    bool isDestory = false;

    public void SetRoket()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        isDestory = false;

        rb.AddRelativeForce(Vector2.up * 18f, ForceMode2D.Impulse);

        StartCoroutine(CheckTimerCo());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Block"))
        {
            collision.GetComponent<BlockBase>().Destory();
        }
    }

    IEnumerator CheckTimerCo()
    {
        yield return new WaitForSeconds(3f);

        if (!isDestory)
        {
            isDestory = true;
            PoolManager.Despawn(this.gameObject);
        }
    }
}

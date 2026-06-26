using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CircleBullet : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 初始化子弹的方向和力度
    /// </summary>
    /// <param name="dir">发射方向，必须是归一化的方向</param>
    /// <param name="force">力的大小</param>
    public void Init(Vector2 dir, float force)
    {
        // 确保方向是二维的
        Vector2 direction2D = dir.normalized;

        // 使用 AddForce，模拟“冲击”发射
        rb.AddForce(direction2D * force, ForceMode2D.Impulse);

        // 可选：N 秒后自动销毁子弹
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Role"))
        {
            collision.GetComponent<BaseRole>().TakeDamage(10);
        }
        if (collision.CompareTag("wall"))
        {
            Destroy(gameObject);
        }
    }
}

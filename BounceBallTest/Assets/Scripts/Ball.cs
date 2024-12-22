using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // 获取球的刚体组件
    }

    private void Start()
    {
        if (rb != null)
        {
            //rb.velocity = Vector2.zero;  // 初始速度设置为零，防止球一开始就有运动
        }
    }
}

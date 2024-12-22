using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject ballPrefab;  // 小球的预制体
    public float ballSpeed = 5f;  // 球的发射速度

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))  // 按下射击按钮（比如鼠标点击或空格键）
        {
            ShootBall();
        }
    }

    private void ShootBall()
    {
        // 获取鼠标位置并转换为世界坐标
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;  // 设置 z 为 0，保持平面上的发射方向

        // 获取当前球的位置（发射点）
        Vector3 ballPosition = ballPrefab. transform.position;

        // 计算从小球当前位置到鼠标位置的方向
        Vector2 direction = (mousePosition - ballPosition).normalized;

        // 创建一个球
        GameObject ball =ballPrefab;

        // 获取球的刚体组件
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 使用 AddRelativeForce 向小球施加一个相对的力
            rb.AddRelativeForce(direction * ballSpeed, ForceMode2D.Impulse);
        }
    }
}

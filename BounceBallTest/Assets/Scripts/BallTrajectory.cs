using UnityEngine;

public class BallTrajectory : MonoBehaviour
{
    public LineRenderer lineRenderer; // 用于绘制轨迹的LineRenderer
    public Rigidbody2D rb;           // 小球的Rigidbody2D
    public float speed = 10f;        // 发射速度
    public float angle = 45f;        // 发射角度
    public int numPoints = 50;       // 轨迹的点数
    public float gravity = -9.8f;    // 重力（根据实际情况调整）

    private void Start()
    {
        // 禁用物理模拟，不让小球真正运动
        rb.simulated = false;
        DrawTrajectory();
    }

    private void DrawTrajectory()
    {
        // 将发射角度转换为弧度
        float angleInRadians = angle * Mathf.Deg2Rad;

        // 初始速度的水平和垂直分量
        float initialVelocityX = speed * Mathf.Cos(angleInRadians);
        float initialVelocityY = speed * Mathf.Sin(angleInRadians);

        // 轨迹的点数组
        Vector3[] trajectoryPoints = new Vector3[numPoints];

        // 计算每个轨迹点的位置
        for (int i = 0; i < numPoints; i++)
        {
            float time = i * 0.1f; // 时间步长，可以调整这个值来控制轨迹的细致程度

            // 计算小球在每个时间点的x和y坐标
            float posX = initialVelocityX * time;
            float posY = initialVelocityY * time + 0.5f * gravity * time * time; // 计算y轴的运动，考虑重力

            // 将轨迹点存储到数组中
            trajectoryPoints[i] = new Vector3(posX, posY, 0);

            // 如果轨迹超过了地面（y < 0），可以停止绘制
            if (posY < 0)
            {
                break;
            }
        }

        // 使用LineRenderer绘制轨迹
        lineRenderer.positionCount = trajectoryPoints.Length;
        lineRenderer.SetPositions(trajectoryPoints);
    }
}

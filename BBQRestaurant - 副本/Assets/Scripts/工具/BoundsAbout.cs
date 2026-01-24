using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 和Bounds相关的工具
/// </summary>
public class BoundsAbout : MonoBehaviour
{
    public static BoundsAbout Instance;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 获取两个 Collider2D 之间的最短边界距离
    /// </summary>
    public static float DistanceBetweenColliders(Collider2D a, Collider2D b)
    {
        if (a == null || b == null) return -1f;

        // b 上离 a 最近的点
        Vector2 closestOnB = b.ClosestPoint(a.bounds.center);

        // a 上离 closestOnB 最近的点
        Vector2 closestOnA = a.ClosestPoint(closestOnB);

        // 计算距离
        float distance = Vector2.Distance(closestOnA, closestOnB);

        return distance;
    }

    /// <summary>
    /// 计算点和collider2D之间的最短距离
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static float DistanceBetweenColliderAndPoint(Collider2D collider, Vector2 point)
    {
        // 获取 Collider 上离 point 最近的点
        Vector2 closestPoint = collider.ClosestPoint(point);
        // 计算距离
        float distance = Vector2.Distance(closestPoint, point);
        return distance;
    }

    /// <summary>
    /// collider2D在某个方向上的宽度
    /// </summary>
    /// <param name="col"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static float GetLengthAlongComplex(Collider2D col, Vector2 dir)
    {
        dir.Normalize();

        Vector2[] points;

        if (col is PolygonCollider2D poly)
        {
            points = poly.points; // 注意 points 是相对本地坐标，需要 col.transform.position 或 TransformPoint
            for (int i = 0; i < points.Length; i++)
                points[i] = col.transform.TransformPoint(points[i]);
        }
        else if (col is CircleCollider2D circle)
        {
            float radius = circle.radius * Mathf.Max(col.transform.lossyScale.x, col.transform.lossyScale.y);
            return 2f * radius; // 圆形投影长度 = 直径，无论方向
        }
        else
        {
            // fallback
            Bounds b = col.bounds;
            points = new Vector2[]
            {
            new Vector2(b.min.x, b.min.y),
            new Vector2(b.min.x, b.max.y),
            new Vector2(b.max.x, b.min.y),
            new Vector2(b.max.x, b.max.y)
            };
        }

        float minProj = Vector2.Dot(points[0], dir);
        float maxProj = minProj;
        for (int i = 1; i < points.Length; i++)
        {
            float proj = Vector2.Dot(points[i], dir);
            if (proj < minProj) minProj = proj;
            if (proj > maxProj) maxProj = proj;
        }
        return maxProj - minProj;
    }
}

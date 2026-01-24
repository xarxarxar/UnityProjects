using UnityEngine;

public static class CollisionHelper2D
{
    /// <summary>
    /// 检查一个碰撞器是否与指定图层的其他碰撞器重叠
    /// </summary>
    public static bool IsOverlapping(Collider2D self, LayerMask targetLayer)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = false;

        Collider2D[] results = new Collider2D[10];
        int count = self.OverlapCollider(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i] != self)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 判断 colliderA 是否完全被 colliderB 包围
    /// </summary>
    public static bool IsCompletelyInside(Collider2D inner, Collider2D outer)
    {
        Bounds innerBounds = inner.bounds;
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(innerBounds.min.x, innerBounds.min.y);
        corners[1] = new Vector3(innerBounds.min.x, innerBounds.max.y);
        corners[2] = new Vector3(innerBounds.max.x, innerBounds.min.y);
        corners[3] = new Vector3(innerBounds.max.x, innerBounds.max.y);

        foreach (var corner in corners)
        {
            if (!outer.OverlapPoint(corner))
                return false;
        }

        return true;
    }
}

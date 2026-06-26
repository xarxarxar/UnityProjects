using UnityEngine;

/// <summary>
/// 游戏内通用瞄准计算工具。
/// </summary>
public static class VideoGameAimUtility
{
    /// <summary>
    /// 当前武器美术朝向与目标方向相反时使用的角度偏移。
    /// </summary>
    public const float ReverseAngleOffset = 180f;

    /// <summary>
    /// 计算从起点指向目标点的单位方向。
    /// </summary>
    /// <param name="startPosition">起点世界坐标。</param>
    /// <param name="targetPosition">目标世界坐标。</param>
    /// <returns>从起点指向目标点的单位方向。</returns>
    public static Vector3 GetDirection(Vector3 startPosition, Vector3 targetPosition)
    {
        return (targetPosition - startPosition).normalized;
    }

    /// <summary>
    /// 让物体绕 Z 轴朝向目标点。
    /// </summary>
    /// <param name="root">需要旋转的物体。</param>
    /// <param name="startPosition">瞄准起点世界坐标。</param>
    /// <param name="targetPosition">目标世界坐标。</param>
    /// <param name="angleOffset">最终角度偏移，用于适配武器美术朝向。</param>
    public static void AimZAxisAtTarget(Transform root, Vector3 startPosition, Vector3 targetPosition, float angleOffset)
    {
        Vector3 direction = GetDirection(startPosition, targetPosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        root.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }

    /// <summary>
    /// 根据当前 Z 轴旋转计算武器上下翻转后的缩放。
    /// </summary>
    /// <param name="root">用于读取 Z 轴旋转的物体。</param>
    /// <param name="xScale">原始 X 轴缩放。</param>
    /// <param name="yScale">原始 Y 轴缩放。</param>
    /// <param name="zScale">原始 Z 轴缩放。</param>
    /// <returns>根据朝向决定是否翻转 Y 轴后的缩放。</returns>
    public static Vector3 GetVerticalFlipScale(Transform root, float xScale, float yScale, float zScale = 1f)
    {
        float z = root.rotation.eulerAngles.z;
        z = (z > 180f) ? z - 360f : z;

        if (z < 90f && z > -90f)
        {
            return new Vector3(xScale, yScale, zScale);
        }

        return new Vector3(xScale, -yScale, zScale);
    }
}

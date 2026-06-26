using UnityEngine;

/// <summary>
/// 视频玩法武器通用 Transform 工具，只处理挂载、瞄准和隐藏根物体等机械动作。
/// </summary>
public static class VideoGameWeaponUtility
{
    /// <summary>
    /// 将武器根物体移动到角色位置，并挂到角色下面。
    /// </summary>
    /// <param name="weaponRoot">武器根物体。</param>
    /// <param name="roleTransform">持有武器的角色 Transform。</param>
    public static void AttachRootToRole(Transform weaponRoot, Transform roleTransform)
    {
        weaponRoot.position = roleTransform.position;
        weaponRoot.SetParent(roleTransform);
    }

    /// <summary>
    /// 让武器朝向目标，并返回从射击点指向目标的方向。
    /// </summary>
    /// <param name="weaponRoot">需要旋转的武器根物体。</param>
    /// <param name="weaponBody">需要根据朝向翻转缩放的武器表现物体。</param>
    /// <param name="shootPos">射击点。</param>
    /// <param name="rolePosition">持有者当前世界坐标。</param>
    /// <param name="targetPosition">目标当前世界坐标。</param>
    /// <param name="scaleX">武器原始 X 缩放。</param>
    /// <param name="scaleY">武器原始 Y 缩放。</param>
    /// <param name="scaleZ">武器原始 Z 缩放。</param>
    /// <returns>从射击点指向目标的单位方向。</returns>
    public static Vector3 AimAndGetShootDirection(
        Transform weaponRoot,
        Transform weaponBody,
        Transform shootPos,
        Vector3 rolePosition,
        Vector3 targetPosition,
        float scaleX,
        float scaleY,
        float scaleZ = 1f)
    {
        VideoGameAimUtility.AimZAxisAtTarget(
            weaponRoot,
            rolePosition,
            targetPosition,
            VideoGameAimUtility.ReverseAngleOffset);

        weaponBody.localScale = VideoGameAimUtility.GetVerticalFlipScale(
            weaponRoot,
            scaleX,
            scaleY,
            scaleZ);

        return VideoGameAimUtility.GetDirection(shootPos.position, targetPosition);
    }

    /// <summary>
    /// 隐藏武器根物体，并重置根物体旋转。
    /// </summary>
    /// <param name="weaponRoot">武器根物体。</param>
    public static void HideRootAndResetRotation(Transform weaponRoot)
    {
        weaponRoot.gameObject.SetActive(false);
        weaponRoot.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// 隐藏通用子弹的根物体。
    /// </summary>
    /// <param name="bullet">需要隐藏根物体的子弹脚本。</param>
    public static void HideBulletRoot(VideoGameBulletBase bullet)
    {
        bullet.transform.parent.gameObject.SetActive(false);
    }
}

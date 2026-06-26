using UnityEngine;

/// <summary>
/// 游戏内通用战斗结果处理工具。
/// </summary>
public static class VideoGameCombatUtility
{
    /// <summary>
    /// 先播报战报，再对目标造成伤害。
    /// </summary>
    /// <param name="targetRole">承受伤害的目标角色。</param>
    /// <param name="damage">造成的伤害数值。</param>
    /// <param name="news">需要播报的战报内容。</param>
    /// <param name="newsColor">战报显示颜色。</param>
    public static void BroadcastThenDamage(BaseRole targetRole, int damage, string news, Color32 newsColor)
    {
        Broadcast.instance.BroadCastNews(news, newsColor);
        targetRole.TakeDamage(damage);
    }

    /// <summary>
    /// 先对目标造成伤害，再播报战报。
    /// </summary>
    /// <param name="targetRole">承受伤害的目标角色。</param>
    /// <param name="damage">造成的伤害数值。</param>
    /// <param name="news">需要播报的战报内容。</param>
    /// <param name="newsColor">战报显示颜色。</param>
    public static void DamageThenBroadcast(BaseRole targetRole, int damage, string news, Color32 newsColor)
    {
        targetRole.TakeDamage(damage);
        Broadcast.instance.BroadCastNews(news, newsColor);
    }

    /// <summary>
    /// 先播报战报，再对目标造成伤害并改变移动速度。
    /// </summary>
    /// <param name="targetRole">承受效果的目标角色。</param>
    /// <param name="damage">造成的伤害数值。</param>
    /// <param name="speedPercent">速度百分比。</param>
    /// <param name="speedDuration">速度效果持续时间。</param>
    /// <param name="news">需要播报的战报内容。</param>
    /// <param name="newsColor">战报显示颜色。</param>
    public static void BroadcastThenDamageAndSetSpeed(
        BaseRole targetRole,
        int damage,
        float speedPercent,
        float speedDuration,
        string news,
        Color32 newsColor)
    {
        Broadcast.instance.BroadCastNews(news, newsColor);
        targetRole.TakeDamage(damage);
        targetRole.SetSpeed(speedPercent, speedDuration);
    }

    /// <summary>
    /// 先设置目标速度，再播报战报。
    /// </summary>
    /// <param name="targetRole">承受速度效果的目标角色。</param>
    /// <param name="speedPercent">速度百分比。</param>
    /// <param name="speedDuration">速度效果持续时间。</param>
    /// <param name="news">需要播报的战报内容。</param>
    /// <param name="newsColor">战报显示颜色。</param>
    public static void SetSpeedThenBroadcast(
        BaseRole targetRole,
        float speedPercent,
        float speedDuration,
        string news,
        Color32 newsColor)
    {
        targetRole.SetSpeed(speedPercent, speedDuration);
        Broadcast.instance.BroadCastNews(news, newsColor);
    }

    /// <summary>
    /// 先设置目标是否可以拾取武器，再播报战报。
    /// </summary>
    /// <param name="targetRole">承受状态效果的目标角色。</param>
    /// <param name="canGetWeapon">是否可以拾取武器。</param>
    /// <param name="duration">状态持续时间。</param>
    /// <param name="news">需要播报的战报内容。</param>
    /// <param name="newsColor">战报显示颜色。</param>
    public static void SetCanGetWeaponThenBroadcast(
        BaseRole targetRole,
        bool canGetWeapon,
        float duration,
        string news,
        Color32 newsColor)
    {
        targetRole.SetCanGetWeapon(canGetWeapon, duration);
        Broadcast.instance.BroadCastNews(news, newsColor);
    }

    /// <summary>
    /// 先设置目标是否可以使用大招，再播报战报。
    /// </summary>
    /// <param name="targetRole">承受状态效果的目标角色。</param>
    /// <param name="canUseBig">是否可以使用大招。</param>
    /// <param name="duration">状态持续时间。</param>
    /// <param name="news">需要播报的战报内容。</param>
    /// <param name="newsColor">战报显示颜色。</param>
    public static void SetCanUseBigThenBroadcast(
        BaseRole targetRole,
        bool canUseBig,
        float duration,
        string news,
        Color32 newsColor)
    {
        targetRole.SetCanUseBig(canUseBig, duration);
        Broadcast.instance.BroadCastNews(news, newsColor);
    }
}

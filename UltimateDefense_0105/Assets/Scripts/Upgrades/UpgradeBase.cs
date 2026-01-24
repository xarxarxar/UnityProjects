using UnityEngine;

/// <summary>
/// 所有升级属性的基类，定义基本字段与 Apply 方法签名
/// </summary>
public abstract class UpgradeBase
{
    #region 公有属性

    /// <summary>
    /// 升级唯一标识，例如 "IncreaseTowerAttack"
    /// </summary>
    public string UpgradeID { get; protected set; }

    /// <summary>
    /// 购买该升级消耗的金币数
    /// </summary>
    public int Cost { get; set; }

    /// <summary>
    /// 升级UI上显示的描述文案，例如 "所有炮塔攻击力 +10"
    /// </summary>
    public string Description { get; protected set; }

    #endregion

    #region 公有方法

    // 判断该升级是否可以在当前情况下出现
    public virtual bool IsAvailable()
    {
        return true; // 默认允许
    }

    /// <summary>
    /// 当该升级被玩家购买后调用，使升级效果立即生效
    /// </summary>
    public abstract void Apply();

    #endregion
}

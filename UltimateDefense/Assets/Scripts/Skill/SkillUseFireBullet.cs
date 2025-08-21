using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 切换为使用火焰子弹，持续一段时间
/// </summary>
public class SkillUseFireBullet : PlayerSkillBase
{
    public override void SkillEffect()
    {
        TowerManager.Instance.ChangeBullet(BulletKind.Fire, 60);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 切换为使用冰子弹，持续一段时间
/// </summary>
public class SkillUseIceBullet : PlayerSkillBase
{
    public override void SkillEffect()
    {
        TowerManager.Instance.ChangeBullet(BulletKind.Ice,60);
    }

    
}

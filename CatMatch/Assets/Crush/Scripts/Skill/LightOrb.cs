using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightOrb : SkillBase
{
    public override void Passive(BeastBase attacker)
    {
        var enemies = GameManager.instance.GetEntityInTeam(attacker.currentTeam,true);
        var myBeasts = enemies.Where(en => en.curIndex != attacker.curIndex).ToList();

        Debug.Log("my team:" + myBeasts.Count());

        if (myBeasts.Count() == 0) return;

        foreach (var en in myBeasts)
        {
            var beast = en as BeastBase;
            beast.OnShield(ShieldType.ShieldMerlinus, bulletPref, 30);
        }
    }
}

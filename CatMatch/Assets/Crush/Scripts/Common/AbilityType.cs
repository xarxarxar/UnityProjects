using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    None,
    AntiPoison,
    AutoShield,
    CriticalDamageFlyingTargets,
    ExtraLoot,
    Evade,
    Silence,
    AntiStun,
    Poison,
    AntiFreeze,
    Revive,
    KnockBack,
    AntiBurn,
    Stun,
    Burn,
    BulletEvade,
    Slowdown,
    LifeSteal,
    Piercing,
    Crusher,// x3 chỉ số đánh boss
    Disarm,
    Freeze,
    Splash,
    AntiKnockBack,
    SummonOnDeath,
    HitCombo,// combo damage 1.05^ số lần attack
    BlockDamage,
    FreezeExplode,
    SnowmanCurse,
    Wounded,
    DarkAndLight,
    EvadeSplashDamage,// né AOE,
    
    // Tự định nghĩa
    Root,// đứng im và trừ máu
    AtkPerSec, // % atk giảm trên giây
}
